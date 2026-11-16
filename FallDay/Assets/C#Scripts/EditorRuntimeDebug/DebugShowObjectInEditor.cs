using UnityEngine;

public class DebugShowObjectInEditor : MonoBehaviour
{
    [ExecuteInEditMode]

    public bool showObjectInEditor = true;
    public float SciseOfCube = 1.0f; // Size of the cube to draw
    public Color OuterColler = new Color(0.5f, 0.5f, 0, 0.5f); // Color of the outer cube
    public Color InnerColler = Color.red; // Color of the inner cube

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = OuterColler; //new Color(0.5f, 0.5f, 0, 0.5f); /*Green*/
        //Gizmos.color = new Color(1, 0, 0, 0.1f); /*Red*/
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3( SciseOfCube, SciseOfCube, SciseOfCube)); //Draw Command

        //Gizmos.color = Color.blue; /*Blue*/
        Gizmos.color = InnerColler; /*Red*/
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(0.05f, 0.05f, 0.05f)); //Draw Command
    }
}
