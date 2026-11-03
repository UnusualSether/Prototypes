using UnityEngine;

public class Cell_Debug : MonoBehaviour // Revise Monobehavior
{
    public bool walkable = false; //sets the walkable state of debugObject in 
    // grid Refrences
    private Cell_ cellRef;
    public Grid_ grid;
    
    public Vector3 previousCellPos;
    public void UpdateDebugInfo(Cell_ cellRef)
    {
        this.cellRef = cellRef;
        walkable = cellRef.walkable;
    }
    private void OnDrawGizmos()
    {
        float cellDivide = cellRef.cellSize * 0.05f;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (walkable)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f); /*Green*/
        }
        else
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f); /*Red*/
        }
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(cellRef.cellSize - cellDivide, cellRef.cellSize - cellDivide, cellRef.cellSize - cellDivide)); //Draw Command
        
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(cellRef.cellSize * 0.05f, cellRef.cellSize * 0.05f, cellRef.cellSize * 0.05f)); //Draw Command
        DrawPreveousCellVector();
    }

    // add a Control for path in serch pathfinding//

    private void DrawPreveousCellVector()
    {
        if(cellRef.PreveousCell != null && grid != null)
        {
            previousCellPos = new Vector3(cellRef.PreveousCell.x, cellRef.PreveousCell.y, cellRef.PreveousCell.z);
            Debug.DrawLine(grid.CellWorldPosition(cellRef.x, cellRef.y, cellRef.z), grid.CellWorldPosition(cellRef.PreveousCell.x, cellRef.PreveousCell.y, cellRef.PreveousCell.z), Color.orange);
        }
    }
}
