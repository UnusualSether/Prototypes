using UnityEngine;

public class Cell_Debug : MonoBehaviour // Revise Monobehavior
{
    public bool walkable = false;
    // grid Refrences
    private Cell_ cellRef;
    public void UpdateDebugInfo(Cell_ cellRef)
    {
        this.cellRef = cellRef;
        walkable = cellRef.walkable;
    }
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        if (walkable)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f); /*Green*/
        }
        else 
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f); /*Red*/
        }
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(1, 1, 1)); //Draw Command
    }
}
