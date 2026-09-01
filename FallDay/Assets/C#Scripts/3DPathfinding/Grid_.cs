using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_
{
    private int width, hight, lengh;
    private float cellSize;
    private float cellHeightRayOverrite = 0f; //AdditionOverrite. Only Overides Raycast height, not the actual cell size, this is to allow for a more accurate walkable check for uneven terrain
    private Vector3 originPosition;
    private Cell_[,,] gridArray;
    private Cell_Debug[,,] debugArray;

    //Debug Options ///////////////////////////////////////////////////////////////////////////////////////
    private bool debug = true;
    private GameObject GeneratorObject;

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    ///Driffrent ways to start up the grid, with or without debug object, with Vector3 or int values for width, hight, and length

    /// <Vector3Sise>
    public Grid_(Vector3 gridSize, float cellSize, Vector3 originPosition)//start up grid with debug object
    {
        GridSetUp((int)gridSize.x, (int)gridSize.y, (int)gridSize.z, cellSize, originPosition);
    }
    public Grid_(Vector3 gridSize, float cellSize, Vector3 originPosition, GameObject DebugGenObject)//start up grid with debug object
    {
        if (DebugGenObject != null)//if debug object is not null, set the generator object to the debug object
        {
            GeneratorObject = DebugGenObject;
        }
        GridSetUp((int)gridSize.x, (int)gridSize.y, (int)gridSize.z, cellSize, originPosition);
    }

    /// <IntSise>
    public Grid_(int width, int hight, int lengh, float cellSize, Vector3 originPosition)//start up grid with no debug object
    {
        GridSetUp(width, hight, lengh, cellSize, originPosition);
    }
    public Grid_(int width, int hight, int lengh, float cellSize, Vector3 originPosition, GameObject DebugGenObject)//start up grid with debug object
    {
        if(DebugGenObject != null)//if debug object is not null, set the generator object to the debug object
        {
            GeneratorObject = DebugGenObject;
        }
        GridSetUp(width, hight, lengh, cellSize, originPosition);
    }

    /// <Vector3SiseWithHeightOverrite>
    public Grid_(Vector3 gridSize, float cellSize, float cellHeightRayOverrite, Vector3 originPosition)//start up grid with debug object
    {
        GridSetUp((int)gridSize.x, (int)gridSize.y, (int)gridSize.z, cellSize, cellHeightRayOverrite, originPosition);
    }
    public Grid_(Vector3 gridSize, float cellSize, float cellHeightRayOverrite, Vector3 originPosition, GameObject DebugGenObject)//start up grid with debug object
    {
        if (DebugGenObject != null)//if debug object is not null, set the generator object to the debug object
        {
            GeneratorObject = DebugGenObject;
        }
        GridSetUp((int)gridSize.x, (int)gridSize.y, (int)gridSize.z, cellSize, cellHeightRayOverrite, originPosition);
    }
    /// </IntSiseWithHeightOverrite>
    public Grid_(int width, int hight, int lengh, float cellSize, float cellHeightRayOverrite, Vector3 originPosition)//start up grid with no debug object
    {
        GridSetUp(width, hight, lengh, cellSize, cellHeightRayOverrite, originPosition);
    }
    public Grid_(int width, int hight, int lengh, float cellSize, float cellHeightRayOverrite, Vector3 originPosition, GameObject DebugGenObject)//start up grid with debug object
    {
        if (DebugGenObject != null)//if debug object is not null, set the generator object to the debug object
        {
            GeneratorObject = DebugGenObject;
        }
        GridSetUp(width, hight, lengh, cellSize, cellHeightRayOverrite, originPosition);
    }

    /// End of start Up grid variations methods ///////////////////////////////////////////////////////////////////////////////////////
    private void GridSetUp(int width, int hight, int lengh, float cellSize, Vector3 originPosition)//sets up the grid with the given parameters
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
                    gridArray[x, y, z] = new Cell_(x, y, z, cellSize, 0, 0, null);
                    IsCellWalkable(x, y, z);
                    //GridClassDebug
                }
            }
        }
        if(debug) GenerateDebugObjects();
    }
    private void GridSetUp(int width, int hight, int lengh, float cellSize, float cellHeightRayOverrite, Vector3 originPosition)//sets up the grid with the given parameters
    {
        this.width = width; this.lengh = lengh; this.hight = hight; //sets the width, length, and height of the grid

        this.cellSize = cellSize; //sets the size of each cell in the grid
        this.cellHeightRayOverrite = cellHeightRayOverrite; //sets the height override of each cell in the grid
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
                    gridArray[x, y, z] = new Cell_(x, y, z, cellSize, 0, 0, null);
                    IsCellWalkable(x, y, z);
                    //GridClassDebug
                }
            }
        }
        if (debug) GenerateDebugObjects();
    }
    // set Up Methods End ///////////////////////////////////////////////////////////////////////////////////////////////////////


    public void checkWalkableAll() // Loops For Cells verifying if they are walkable or not, and sets the walkable value accordingly (can be used to update the walkable status of all cells after grid generation)
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
        if(fivePointRaycast(xpos, ypos, zpos) != true) // RayCast To check the spot
        {
            N.walkable = false;
        }
        else
        {
            N.walkable = true;
        }
    }
    private bool fivePointRaycast(int xpos, int ypos, int zpos) 
    {
        int rayCount = 4;
        bool isWalkable = true;
        Vector3 CellPos = GetWorldPosition(xpos, ypos, zpos);
        List<Vector3> DetectorSystem = new List<Vector3>(rayCount);

        float devide = 8f;
        float cellDevide = cellSize / devide;

        // pos Rays in form of cube pointed down (additional ray at center)
        //DetectorSystem[0] = new Vector3(CellPos.x + (cellSize / 2), CellPos.y + cellSize, CellPos.z + (cellSize / 2)); //RayStartPosCenter
        DetectorSystem.Add(new Vector3(CellPos.x + cellDevide, CellPos.y + cellSize, CellPos.z + cellDevide));                                   //RayStartPosCloseTo0
        DetectorSystem.Add(new Vector3(CellPos.x + cellDevide, CellPos.y + cellSize, CellPos.z + cellSize - cellDevide));                        //RayStartPosLowX
        DetectorSystem.Add(new Vector3(CellPos.x + cellSize - cellDevide, CellPos.y + cellSize, CellPos.z + cellDevide));                        //RayStartPosLowZ
        DetectorSystem.Add(new Vector3((CellPos.x + cellSize) - cellDevide, CellPos.y + cellSize, (CellPos.z + cellSize) - cellDevide));             //RayStartPosFarFrom0

        for (int i = 0; i < rayCount; i++)
        {
            //Physics.Raycast(DetectorSystem[i], Vector3.down, out RaycastHit hitInfo, cellSize)
            if (DetectorSystem[i] != null)
            {
                RaycastHit[] hits = Physics.RaycastAll(DetectorSystem[i], Vector3.down, cellSize + cellHeightRayOverrite, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
                foreach (RaycastHit hitInfo in hits)
                {
                    if (hitInfo.collider != null)
                    {
                        if(hitInfo.collider.gameObject.tag == "obstacle")
                        {
                            if (debug) { Debug.DrawRay(DetectorSystem[i], Vector3.down * (cellSize + cellHeightRayOverrite), Color.red, 1f); }
                            if (debug) { Debug.Log($"Cell at {xpos}, {ypos}, {zpos} is not walkable due to obstacle: {hitInfo.collider.gameObject.name} - RayTrigger {i}"); }
                            isWalkable = false;
                        }
                    }
                }
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
