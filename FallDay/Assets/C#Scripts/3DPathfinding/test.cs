using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    private Pathfinding pathfinding;
    private Grid_ grid;

    [SerializeField]
    private GameObject Selector1, Selector2;

    private int StartX = 0; private int StartY = 0; private int StartZ = 0;
    private int EndX = 5; private int EndY = 0; private int EndZ = 5;

    void Start()
    {
        pathfinding = new Pathfinding(5,5,1, this.gameObject);
        grid = pathfinding.GetGridA_();
        //grid.GenerateDebugObjects();
    }
    private void Update()
    {
        grid.SetWalkableLoop();
    }
    public void SetStartNode(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = Selector1.transform.position;
        int NewX, NewY, NewZ;
        grid.GetXYZ(mouseWorldPosition, out NewX, out NewY, out NewZ);
        Debug.Log("PosNew " +  NewX + "," + NewY + "," + NewZ + " PosOld " + StartX + "," + StartY + "," + StartZ);
        if (NewX != StartX || NewY != StartY || NewZ != StartZ)
        {
            StartX = NewX; StartY = NewY;
        }
    }
    public void SetEndNode(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = Selector2.transform.position;
        int NewX, NewY, NewZ;
        grid.GetXYZ(mouseWorldPosition, out NewX, out NewY, out NewZ);
        if (NewX != EndX || NewY != EndY || NewZ != EndZ)
        {
            EndX = NewX; EndY = NewY;
        }
    }
    public void SetObstacle(InputAction.CallbackContext context)
    {
        
    }
    public void StartPath()
    {
        List<Cell_> path = pathfinding.FindPath(StartX, StartY, StartZ, EndX, EndY, EndZ);
        if (path != null)
        {
            for (int i = 0; i < path.Count -1; i++) {
                Vector3 pos1 = grid.GetWorldPosition(path[i].x, path[i].y, path[i].z);
                Vector3 pos2 = grid.GetWorldPosition(path[i + 1].x, path[i + 1].y, path[i + 1].z);
                Debug.DrawLine(pos1 + new Vector3(1f, 1f, 1f) * .5f, pos2 + new Vector3(1f, 1f, 1f) * .5f, Color.green, 100f);
            }
        }
        DebugPathDysplay(path);
    }

    public void DebugPathDysplay(List<Cell_> path)
    {
        if(path != null)
        {
            int i = 0;
            foreach(Cell_ cell in path)
            {
                i++;
                Debug.Log(cell + " node " + i);
            }
        }
    }
    /*
    private void Update()
    {

        if (Input.GetMouseButtonDown(0)){
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mouseWorldPosition, out int x, out int y);
            List<NodeA_> path = pathfinding.FindPath(2,3,x,y);
            if (path != null){
                for (int i = 0; i < path.Count -1; i++) {
                    Vector3 pos1 = grid.GetWorldPosition(path[i].x, path[i].y);
                    Vector3 pos2 = grid.GetWorldPosition(path[i + 1].x, path[i + 1].y);
                    Debug.DrawLine( pos1 + new Vector3(1f, 1f) * .5f, pos2 + new Vector3(1f, 1f) * .5f, Color.green, 100f);
                }
            }
            grid.UpdateLines();
        }
        if (Input.GetMouseButtonDown(1)){
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            grid.SetNodeTrueFalse(mouseWorldPosition);
            grid.UpdateLines();
        }
    }
    */
}
