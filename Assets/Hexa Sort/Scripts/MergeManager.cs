using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MergeManager : MonoBehaviour
{
    private void Awake()
    {
        StackContoller.onStackPlaced += StackPlacedCallback;
    }
    private void OnDestroy()
    {
        StackContoller.onStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        // Does this cell has neighbors ?
        LayerMask gridCellMask = 1 << gridCell.gameObject.layer; // we create a layer mask with this grid cells layer. This will be helpful to detect any other grid cell

        List<GridCell> neighborGridCells = new List<GridCell>();

        Collider[] neighborGridCellColliders = Physics.OverlapSphere(gridCell.transform.position, 2, gridCellMask);

        //At this point, we have the grid cell collider neighbors.
        foreach (Collider gridCellCollider in neighborGridCellColliders)
        {
            GridCell neighborGridCell = gridCellCollider.GetComponent<GridCell>(); // to store it

            if (!neighborGridCell.IsOccupied)
                continue;

            if (neighborGridCell == gridCell) // if the gridCell is the current cell we are checking
                continue;

            neighborGridCells.Add(neighborGridCell);
        }

        if (neighborGridCells.Count <= 0)
        {
            Debug.Log("No neighbor for this cell");
            return;
        }


        //At this point, whe have a list of neighbor grid cells, that are occupied
        Color gridCellTopHexagonColor = gridCell.Stack.GetTopHexagonColor();

        // Do these neighbours have the same top hex color ?
        List<GridCell> similarNeighborGridCells = new List<GridCell>();

        foreach (GridCell neighborGridCell in neighborGridCells)
        {
            Color neighborGridCellTopHexagonColor = neighborGridCell.Stack.GetTopHexagonColor();
            if (gridCellTopHexagonColor == neighborGridCellTopHexagonColor)
            {
                similarNeighborGridCells.Add(neighborGridCell);
            }
        }

        if (similarNeighborGridCells.Count <= 0)
        {
            Debug.Log("No similar neighbor for this cell");
            return;
        }

        // At this point, we have a list of similar neighbors
        List<Hexagon> hexagonsToAdd = new List<Hexagon>();

        foreach (GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack neighborCellHexStack = neighborCell.Stack;
            for (int i = neighborCellHexStack.Hexagons.Count - 1; i >= 0; i--)
            {
                Hexagon hexagon = neighborCellHexStack.Hexagons[i];
                if (hexagon.color != gridCellTopHexagonColor)
                {
                    break;
                }

                hexagonsToAdd.Add(hexagon);
                hexagon.SetParent(null);
            }

        }

        //Remove the hexagons from their stacks
        foreach (GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack stack = neighborCell.Stack;
            foreach (Hexagon hexagon in hexagonsToAdd)
            {
                if (stack.Contains(hexagon))
                {
                    stack.Remove(hexagon);
                }
            }
        }

        // At this point, we have removed the stacks we dont need anymore
        // We have some free grid cells

        float initialY = gridCell.Stack.Hexagons.Count * .2f;

        for (int i = 0; i < hexagonsToAdd.Count; i++)
        {
            Hexagon hexagon = hexagonsToAdd[i]; //first we need to cache

            float targetY = initialY * i * .2f;
            Vector3 targetLocalPosition = Vector3.up * targetY;

            gridCell.Stack.Add(hexagon);
            hexagon.transform.localPosition = targetLocalPosition;
        }

        // We need the merge!

        //Merge everything inside of this cell

        //Is the stack on this cell complete ?
        //Does it have 10 or more similar hexagons

        //Check the updated cells
        //Repeat
    }
}

//--------  notes -----------------
// when we merge. \
// first case is when we drop a stack on a grid cell. We already have a action for that! Lets subscribe it