using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_
{
    private int width, hight, lengh;
    private float cellSize;
    private Vector3 originPosition;
    private Cell_[,,] gridArray;
    private Cell_Debug[,,] debugArray;

    //Debug Options ///////////////////////////////////////////////////////////////////////////////////////
    private bool debug = true;
    private GameObject GeneratorObject;

    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public Grid_(int width, int lengh, int hight, float cellSize, Vector3 originPosition)//start up grid with no debug object
    {
        GridSetUp(width, lengh, hight, cellSize, originPosition);
    }
    public Grid_(int width, int lengh, int hight, float cellSize, Vector3 originPosition, GameObject DebugGenObject)//start up grid with debug object
    {
        if(DebugGenObject != null)//if debug object is not null, set the generator object to the debug object
        {
            GeneratorObject = DebugGenObject;
        }
        GridSetUp(width, lengh, hight, cellSize, originPosition);
    }
    private void GridSetUp(int width, int lengh, int hight, float cellSize, Vector3 originPosition)//sets up the grid with the given parameters
    {
        this.width = width; this.lengh = lengh; this.hight = hight; //sets the width, length, and height of the grid

        this.cellSize = cellSize; //sets the size of each cell in the grid
        this.originPosition = originPosition; //sets the origin position of the grid

        gridArray = new Cell_[width, hight, lengh]; //creates a new 3D array of cells with the given width (x), height (y), and length (z)

        // cicles through the grid and creates a new cell for each position in the grid
        for (int y = 0; y < gridArray.GetLength(1); y++) //
        {
            for (int z = 0; z < gridArray.GetLength(2); z++)
            {
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    //Nodes Generation
                    gridArray[x, y, z] = new Cell_(x, y, z, 0, 0, null);
                    IsCellWalkable(x, y, z);
                    //GridClassDebug
                }
            }
        }
        if(debug) GenerateDebugObjects();
    }

    public void SetWalkableLoop() // Loops For Cells verifying if they are walkable or not, and sets the walkable value accordingly (can be used to update the walkable status of all cells after grid generation)
    {
        for (int y = 0; y < gridArray.GetLength(1); y++)
        {
            for (int z = 0; z < gridArray.GetLength(2); z++)
            {
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    IsCellWalkable(x, y, z);
                    if (debug)
                    {
                        debugArray[x, y, z].UpdateDebugInfo(ReturnCell(x, y, z));
                    }
                }
            }
        }
    }
    private void IsCellWalkable(int xpos, int ypos, int zpos) //Upon Cell Position, Check if the Cell is Walkable or not
    {
        Cell_ N = ReturnCell(xpos, ypos, zpos);
        if(fivePointRaycast(xpos, ypos, zpos)) // RayCast To check the spot
        {
            N.walkable = false;
        }
        else
        {
            N.walkable = true;
        }
    }
    private bool fivePointRaycast(int xpos, int ypos, int zpos) // Cell 5point RayCast Return False if space is walkable
    {
        bool isWalkable = false;
        Vector3 CellPos = GetWorldPosition(xpos, ypos, zpos);
        Vector3[] DetectorSystem = new Vector3[5];
        // pos Rays in form of cube pointed down (additional ray at center)
        DetectorSystem[0] = new Vector3(CellPos.x + (cellSize / 2), CellPos.y + cellSize, CellPos.z + (cellSize / 2)); //RayStartPosCenter
        DetectorSystem[1] = new Vector3(CellPos.x, CellPos.y + cellSize, CellPos.z);                                   //RayStartPosCloseTo0
        DetectorSystem[2] = new Vector3(CellPos.x, CellPos.y + cellSize, CellPos.z + cellSize);                        //RayStartPosLowX
        DetectorSystem[3] = new Vector3(CellPos.x + cellSize, CellPos.y + cellSize, CellPos.z);                        //RayStartPosLowZ
        DetectorSystem[4] = new Vector3(CellPos.x + cellSize, CellPos.y + cellSize, CellPos.z + cellSize);             //RayStartPosFarFrom0

        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(DetectorSystem[i], Vector3.down, out RaycastHit hitInfo, cellSize))
            {
                isWalkable = true;
            }
        }
        return isWalkable;
    }


    public Vector3 GetWorldPosition(int x, int y, int z) //returns the world position of the given cell coordinates
    {
        return new Vector3(x, y, z) * cellSize + originPosition; 
    }
    public void GetXYZ(Vector3 worldPosition, out int x,out int y,out int z) // returns the cell coordinates of the given world position
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public Cell_ ReturnCell(int x, int y, int z)
    {
        if (x >= 0 && y >= 0 && z>=0 && x< width && y < hight && z < lengh)
        {
            return gridArray[x, y, z];
        }
        else
        {
            if (debug){
                Debug.Log($"Out of Grid Bownds x = {x} y = {y} z = {z}");
            }
            return null;
        }
    }

    public void returnWidthLenghHeight(out int width, out int hight, out int lengh)
    {
        width = this.width;
        hight = this.hight;
        lengh = this.lengh;
    }

    public void SetNodeTrueFalse(Vector3 NodePos)
    {
        GetXYZ(NodePos, out int x, out int y,out int z);
        SetNodeTrueFalse(x, y, z);
    }

    public void SetNodeTrueFalse(int x, int y, int z)
    {
        SetNodeTrueFalse(ReturnCell(x, y, z));
    }

    public void SetNodeTrueFalse(Cell_ N)
    {
        bool changed = false;
        if(N.walkable == true && changed == false)
        {
            N.walkable = false;
            changed = true;
        }
        if(N.walkable == false && changed == false)
        {
            N.walkable = true;
            changed = true;
        }
    }
    public void GenerateDebugObjects()
    {
        debugArray = new Cell_Debug[width, hight, lengh];
        for (int y = 0; y < gridArray.GetLength(1); y++)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    GameObject debugObj = new GameObject($"DebugCell_ {x}, {y}, {z}");
                    debugObj.transform.position = GetWorldPosition(x, y, z) + new Vector3(cellSize / 2, cellSize / 2, cellSize / 2);
                    debugObj.transform.parent = GeneratorObject.transform;
                    debugArray[x, y, z] = debugObj.AddComponent<Cell_Debug>();
                    debugArray[x, y, z].UpdateDebugInfo(gridArray[x, y, z]);
                }
            }
        }
    }
}
