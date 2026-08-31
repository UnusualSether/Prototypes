using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid_ grid_;
    private List<Cell_> openList;
    private List<Cell_> closeList;

    private List<Cell_> NaborList;

    private bool debug = false;

    public Pathfinding(Grid_ grid)
    {
        grid_ = grid;
    }
    public Pathfinding(int width, int lengh, int height)
    {
        grid_ = new Grid_(width + 1, lengh + 1, height, 1f, /*new Vector3(-10,-5, 0)*/ Vector3.zero);
    }
    public Pathfinding(int width, int lengh, int height, GameObject DebugGenObject)
    {
        grid_ = new Grid_(width + 1, lengh + 1, height, 1f, /*new Vector3(-10,-5, 0)*/ DebugGenObject.transform.position, DebugGenObject);
    }
    public Grid_ GetGridA_()
    {
        return grid_;
    }

    /// <summary>  Main Pathfinding Algorithm - A* _3D (A Star) Implementation (Resets uppon each call to FindPath)
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    private bool EmergencyBreak = false;
    public List<Cell_> FindPath(int startX, int startY, int startZ, int endX, int endY, int endZ) {
        Cell_ startCell = grid_.ReturnCell(startX, startY, startZ);//Get Start Cell
        Cell_ endCell = grid_.ReturnCell(endX, endY, endZ);//Get End Cell

        // verify if the start and end cells are valid
        if (startCell == null || endCell == null)
        {
            // Invalid Path
            Debug.LogWarning("null Invalid Path");
            return null;
        }
        if (startCell.walkable == false || endCell.walkable == false)
        {
            // Invalid Path
            Debug.LogWarning("walkable Invalid Path");
            return null;
        }

        // Initialize the open and closed lists (organizes search)
        openList = new List<Cell_> { startCell };
        closeList = new List<Cell_>();

        grid_.returnWidthLenghHeight(out int width, out int height, out int lengh); 

        // Reset all Cells gCost, hCost, fCost and PreveousCell to initial values before starting the pathfinding algorithm
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < lengh; z++) {
                    Cell_ cell = grid_.ReturnCell(x, y, z);
                    cell.g = int.MaxValue;
                    cell.CalcFCost();
                    cell.PreveousCell = null;
                }
            }
        }

        // Set the gCost, hCost, and fCost for the start cell
        startCell.g = 0;
        startCell.h = CalculateDistanceCost(startCell, endCell);
        startCell.CalcFCost();

        // Start the pathfinding loop
        while (openList.Count > 0) {
            Cell_ currentCell = GetLowestFCostNode(openList);

            if (debug) Debug.Log("currentNode = " + currentCell + " endNode = " + endCell);

            // Check if the current cell is the end cell
            if (currentCell == endCell) {
                return CalculatePath(endCell);
            }

            // Move the current cell from the open list to the closed list
            openList.Remove(currentCell);
            closeList.Add(currentCell);

            NaborList = GetNeighbourList(currentCell);

            foreach (Cell_ neighbourCell in NaborList)
            {
                if (closeList.Contains(neighbourCell))
                {
                    continue;
                }

                int tentativeGCost = currentCell.g + CalculateDistanceCost(currentCell, neighbourCell);
                if (tentativeGCost < neighbourCell.g) {
                    neighbourCell.PreveousCell = currentCell;
                    neighbourCell.g = tentativeGCost;
                    neighbourCell.h = CalculateDistanceCost(neighbourCell, endCell);
                    neighbourCell.CalcFCost();

                    if (!openList.Contains(neighbourCell))
                    {
                        openList.Add(neighbourCell);
                    }
                }
            }
            // Emergency Break for Debugging (allows to stop the pathfinding loop)
            if (EmergencyBreak)
            {
                break;
            }
        }
        //Out Of Nodes on openList
        return null;
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    // Get the list of valid neighboring cells around the current cell
    private List<Cell_> GetNeighbourList(Cell_ currentCell)
    {
        List<Cell_> ValidNeighbourList = new List<Cell_>();
        List<Cell_> Nabors = new List<Cell_>();
        //grid_.returnWidthLenghHeight(out int width, out int height, out int lengh);

        for (int varZ = -1; varZ <= 1; ++varZ) //Pega Todos os Visinhos Mesmo Que sejam Null e nao andavel
        {
            for (int varY = -1; varY <= 1; ++varY)
            {
                for (int varX = -1; varX <= 1; ++varX)
                {
                    // All Nodes in a 3x3x3 grid around the current node, including diagonals and the current node itself
                    Nabors.Add(grid_.ReturnCell(currentCell.x + varX, currentCell.y + varY, currentCell.z + varZ)); //
                }
            }
        }

        // Remover cells visinhas em casos de obstrução diagonal (diagonal obstruction) - This is a common technique in pathfinding to prevent moving diagonally through corners where there are obstacles.
        for (int varY = -1; varY <= 1; ++varY) // -1, 0, 1 | y
        {
            for (int varZ = -1; varZ <= 1; ++varZ) // -1, 0, 1 | z
            {
                for (int varX = -1; varX <= 1; ++varX) // -1, 0, 1 | x
                {
                    NaborSelectshift(Nabors, currentCell, varX, varY, varZ);
                }
            }
        }

        foreach (Cell_ cell in Nabors) //Barra Membros que Nao sao andaveis
        {
            if (cell != null && cell.walkable)
            {
                ValidNeighbourList.Add(cell);
            }
        }

        //DebugNodeA_List(neighbourList);
        return ValidNeighbourList;
    }
    private List<Cell_> NaborSelectshift(List<Cell_> Nabors, Cell_ currentCell, int xpos, int ypos, int zpos)
    {
        // This method is intended to filter out neighboring cells based on their relative positions (xpos, ypos, zpos)
        switch ((xpos, ypos, zpos))
        {
            default:
                // none (leve this section empty)
                break;

            case (-1, 0, 0): case (1, 0, 0): //right and left neighbors | x
                if(currentCell.walkable == false) //Condition to remove the 9 cells in a 3x3 section around the current cell based on the specified position offsets (xpos, ypos, zpos)
                {
                    nineCellSectionRemover(Nabors, currentCell, 0, xpos);
                }
                break;

            case (0, -1, 0): case (0, 1, 0): //up and down neighbors | y
                if (currentCell.walkable == false) //Condition to remove the 9 cells in a 3x3 section around the current cell based on the specified position offsets (xpos, ypos, zpos)
                {
                    nineCellSectionRemover(Nabors, currentCell, 1, ypos);
                }
                break;

            case (0, 0, -1): case (0, 0, 1): //front and back neighbors | z
                if (currentCell.walkable == false) //Condition to remove the 9 cells in a 3x3 section around the current cell based on the specified position offsets (xpos, ypos, zpos)
                {
                    nineCellSectionRemover(Nabors, currentCell, 2, zpos);
                }
                break;
        }
        return Nabors;
    }

    // Removes the 9 cells in a 3x3 section around the current cell based on the specified position offsets (xpos, ypos, zpos)
    private List<Cell_> nineCellSectionRemover(List<Cell_> Nabors, Cell_ currentCell, int switchSelect, int nVar) //switchSelect selects which axis to remove cells from (0 = x-axis, 1 = y-axis, 2 = z-axis), nVar is the offset for the selected axis
    {
        // Loop through the 3x3 section around the current cell
        for (int varA = -1; varA <= 1; ++varA) // -1, 0, 1 |
        {
            for (int varB = -1; varB <= 1; ++varB) // -1, 0, 1 |
            {
                // Determine which axis to remove cells from based on switchSelect
                switch (switchSelect)
                {
                    case 0: // Remove cells along the x-axis
                        Nabors.Remove(grid_.ReturnCell(currentCell.x + nVar, currentCell.y + varA, currentCell.z + varB));
                        break;
                    case 1: // Remove cells along the y-axis
                        Nabors.Remove(grid_.ReturnCell(currentCell.x + varA, currentCell.y + nVar, currentCell.z + varB));
                        break;
                    case 2: // Remove cells along the z-axis
                        Nabors.Remove(grid_.ReturnCell(currentCell.x + varA, currentCell.y + varB, currentCell.z + nVar));
                        break;
                }
            }
        }
        return Nabors;
    }

    // Calculate the path from the end cell to the start cell by following the PreveousCell references
    private List<Cell_> CalculatePath(Cell_ endCell){
        List<Cell_> path = new List<Cell_>();
        path.Add(endCell);
        Cell_ currentCell = endCell;
        while (currentCell.PreveousCell != null) { 
            path.Add(currentCell.PreveousCell);
            currentCell = currentCell.PreveousCell;
        }
        path.Reverse();
        return path;
    }
    // Calculate the distance cost between two cells using Manhattan distance with diagonal movement
    private int CalculateDistanceCost(Cell_ a, Cell_ b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs (a.y - b.y);
        int nOfStraitCell = Mathf.Abs(xDistance - yDistance);
        int nOfDiagonalCell = Mathf.Min(xDistance, yDistance);
        int ToReturn = MOVE_DIAGONAL_COST * nOfDiagonalCell + MOVE_STRAIGHT_COST * nOfStraitCell;
        return ToReturn;
    }
    // Get the cell with the lowest fCost from a list of cells | Useful for selecting the next cell to evaluate in the A* algorithm
    private Cell_ GetLowestFCostNode(List<Cell_> CellList) { 
        Cell_ lowestFCostNode = CellList[0];
        for (int i = 1; i< CellList.Count; i++){
            if (CellList[i].f < lowestFCostNode.f){
                lowestFCostNode = CellList[i];
            }
        }
        return lowestFCostNode;
    }

    // Debugging method to print the contents of a list of cells to the console | Can Be Implemented in the future for debugging purposes
    private void DebugCell_List(List<Cell_> CellList){
        for(int i = 0; i < CellList.Count; i++)
        {
            Debug.Log($"Cell_ = {CellList[i]} CellNumber = {i}");
        }
    }
}
