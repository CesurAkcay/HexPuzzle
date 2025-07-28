using System;
using System.Linq;
using UnityEngine;

public class StackContoller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask hexagonLayerMask;
    [SerializeField] private LayerMask gridHexagonLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    private HexStack currentStack;
    private Vector3 currentStackInitialPos; //Sürüklemeye başladığında o grubun ilk pozisyonunu saklar


    [Header("Data")]
    private GridCell targetCell;

    [Header("Actions")]
    public static Action<GridCell> onStackPlaced; // whenever we drop a a stack on a grid cell, we are going to pass om the grid cell along with the action


    void Start()
    {

    }

 
    void Update()
    {
        ManageControl();
    }

    // 1- Whenever we click, 2-whenever we drag the mouse, 3-whenever we mouse up
    private void ManageControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ManageMouseDown();
        }
        else if (Input.GetMouseButton(0) && currentStack != null)
        {
            ManageMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0) && currentStack != null)
        {
            ManageMouseUp();
        }
    }

    private void ManageMouseDown()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, hexagonLayerMask);

        if (hit.collider == null)
        {
            Debug.Log("Did not clicked any hexagon");
        }

        currentStack = hit.collider.GetComponent<Hexagon>().HexStack; // if we hit something we grab the hexagon component and grab the HexStack attached that hexagon
        currentStackInitialPos = currentStack.transform.position;
    }

    private void ManageMouseDrag()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, gridHexagonLayerMask);

        if (hit.collider == null)
        {
            DraggingAboveGround();
        }
        else
        {
            DraggingAroundAboveGridCell(hit);
        }
    }

    private void DraggingAboveGround()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, groundLayerMask);

        if (hit.collider == null)
        {
            Debug.Log("No ground detected, this is problem...");
        }

        Vector3 currentStackTargetPos = hit.point.With(y: 2);
        currentStack.transform.position = Vector3.MoveTowards(
            currentStack.transform.position,
            currentStackTargetPos, Time.deltaTime * 30);

        targetCell = null; //so now we are only going to have a target cell whenever we are above one that is not occupied
    }

    private void DraggingAroundAboveGridCell(RaycastHit hit)
    {
        GridCell gridCell = hit.collider.GetComponent<GridCell>();

        if (gridCell.IsOccupied)
        {
            DraggingAboveGround();
        }
        else
        {
            DraggingAboveNonOccupiedGridCell(gridCell);
  
        }
    }
    private void DraggingAboveNonOccupiedGridCell(GridCell gridCell)
    {
        Vector3 currentStackTargetPos = gridCell.transform.position.With(y: 2);

        currentStack.transform.position = Vector3.MoveTowards(
            currentStack.transform.position,
            currentStackTargetPos, Time.deltaTime * 30);

        targetCell = gridCell; // cache the current grid cell we are above
    }


    private void ManageMouseUp()
    {
        if (targetCell == null) // check if we got a target cell or not
        {
            currentStack.transform.position = currentStackInitialPos;
            currentStack = null;
            return;
        }

        currentStack.transform.position = targetCell.transform.position.With(y: .2f); //move our current stack
        currentStack.transform.SetParent(targetCell.transform); // drop the stack as a child of the target cell
        currentStack.Place();

        targetCell.AssignStack(currentStack);

        onStackPlaced?.Invoke(targetCell); // dont call or trigger the event after setting the target cell to null

        targetCell = null;
        currentStack = null;
    }

    private Ray GetClickedRay() //sent a ray to scene from mouse location
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
