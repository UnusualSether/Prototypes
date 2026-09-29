using UnityEngine;

public class Cell_Debug : MonoBehaviour // Revise Monobehavior
{
    public bool walkable = false; //sets the walkable state of debugObject in 
    // grid Refrences
    private Cell_ cellRef;
    public Grid_ grid { private get; set; }
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
        Gizmos.color = new Color(cellDivide, cellDivide, cellDivide);
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(cellRef.cellSize - cellDivide, cellRef.cellSize - cellDivide, cellRef.cellSize - cellDivide)); //Draw Command
    }

    // add a Control for path in serch pathfinding//

    private void DrawPreveousCellVector()
    {
        Debug.DrawLine(grid.CellWorldPosition(cellRef.x, cellRef.y, cellRef.z), grid.CellWorldPosition(cellRef.PreveousCell.x  , cellRef.PreveousCell.y, cellRef.PreveousCell.z));
    }
}
