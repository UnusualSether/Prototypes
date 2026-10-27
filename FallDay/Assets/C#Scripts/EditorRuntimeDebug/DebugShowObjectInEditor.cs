using UnityEngine;

public class DebugShowObjectInEditor : MonoBehaviour
{
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        float cellDivide = 1 * 0.05f;
        Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 1, 0, 0.1f); /*Green*/
            //Gizmos.color = new Color(1, 0, 0, 0.1f); /*Red*/
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(1 - cellDivide, 1 - cellDivide, 1 - cellDivide)); //Draw Command

        Gizmos.color = Color.blue; /*Blue*/
        Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(0.05f, 0.05f, 0.05f)); //Draw Command
    }
}
