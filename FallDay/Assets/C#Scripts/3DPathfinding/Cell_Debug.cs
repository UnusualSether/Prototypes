using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell_Debug : MonoBehaviour // Revise Monobehavior
{
    public bool walkable = false;
    private void Start()
    {
        
    }
    // grid Refrences
    private Cell_ cellRef;
    public void UpdateDebugInfo(Cell_ cellRef)
    {
        this.cellRef = cellRef;
        walkable = cellRef.walkable;
    }

    private void DrawCubeRed() //Draw function base AlterLater
    {
        Gizmos.color = new Color(1, 0, 0, 0.1f); //Red
        // Gizmos matrix If Necessary
        
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
    //Gizmos.color = new Color(1, 0, 0, 0.1f); //Red
    //Gizmos.color = new Color(0, 1, 0, 0.1f); //Green
    //Gizmos.color = new Color(0, 0, 1, 0.1f); //Blue
    //Gizmos.color = new Color(1, 1, 0, 0.1f); //Yellow
}
