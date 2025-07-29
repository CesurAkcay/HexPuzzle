using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MergeManager : MonoBehaviour
{

    [Header("Elements")]
    private List<GridCell> updatedCells = new List<GridCell>();
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
        StartCoroutine(StackPlacedCoroutine(gridCell));
    }

    IEnumerator StackPlacedCoroutine(GridCell gridCell)
    {
        updatedCells.Add(gridCell);

        while (updatedCells.Count > 0)
            yield return CheckForMerge(updatedCells[0]); // this will make sure that we wait for CheckForMerge coroutine to complete before doing anything else after      
    }

    IEnumerator CheckForMerge(GridCell gridCell)
    {
        updatedCells.Remove(gridCell);

        if (!gridCell.IsOccupied)
            yield break;
        //Does this cell neighbors ?
        List<GridCell> neighborGridCells = GetNeighborGridCells(gridCell);

        if (neighborGridCells.Count <= 0)
        {
            Debug.Log("No neighbors for this cell");
            yield break;
        }


        //At this point, whe have a list of neighbor grid cells, that are occupied
        Color gridCellTopHexagonColor = gridCell.Stack.GetTopHexagonColor();

        // Do these neighbours have the same top hex color ?
        List<GridCell> similarNeighborGridCells = GetSimilarNeighborGridCells(gridCellTopHexagonColor, neighborGridCells.ToArray());

        if (similarNeighborGridCells.Count <= 0)
        {
            Debug.Log("No similar neighbor for this cell");
            yield break;
        }

        updatedCells.AddRange(similarNeighborGridCells);

        // At this point, we have a list of similar neighbors
        List<Hexagon> hexagonsToAdd = GetHexagonsToAdd(gridCellTopHexagonColor, similarNeighborGridCells.ToArray());

        //Remove the hexagons from their stacks
        RemoveHexagonsFromStacks(hexagonsToAdd, similarNeighborGridCells.ToArray());

        // At this point, we have removed the stacks we dont need anymore
        // We have some free grid cells

        MoveHexagons(gridCell, hexagonsToAdd);

        yield return new WaitForSeconds(.2f + (hexagonsToAdd.Count + 1) * .01f);

        // We need the merge!

        //Merge everything inside of this cell

        //Is the stack on this cell complete ?
        //Does it have 10 or more similar hexagons

        yield return CheckForCompleteStacks(gridCell, gridCellTopHexagonColor);

        //Check the updated cells
        //Repeat
    }
    public List<GridCell> GetNeighborGridCells(GridCell gridCell)
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

        return neighborGridCells;
    }
    private List<GridCell> GetSimilarNeighborGridCells(Color gridCellTopHexagonColor, GridCell[] neighborGridCells)
    {
        List<GridCell> similarNeighborGridCells = new List<GridCell>();

        foreach (GridCell neighborGridCell in neighborGridCells)
        {
            Color neighborGridCellTopHexagonColor = neighborGridCell.Stack.GetTopHexagonColor();
            if (gridCellTopHexagonColor == neighborGridCellTopHexagonColor)
            {
                similarNeighborGridCells.Add(neighborGridCell);
            }
        }
        return similarNeighborGridCells;
    }

    private List<Hexagon> GetHexagonsToAdd(Color gridCellTopHexagonColor, GridCell[] similarNeighborGridCells)

    {

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

        return hexagonsToAdd;
    }

    private void RemoveHexagonsFromStacks(List<Hexagon> hexagonsToAdd, GridCell[] similarNeighborGridCells)
    {
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
    }

    private void MoveHexagons(GridCell gridCell, List<Hexagon> hexagonsToAdd)

    {
        float initialY = gridCell.Stack.Hexagons.Count * .2f;

        for (int i = 0; i < hexagonsToAdd.Count; i++)
        {
            Hexagon hexagon = hexagonsToAdd[i]; //first we need to cache

            float targetY = initialY + (i * .2f);
            Vector3 targetLocalPosition = Vector3.up * targetY;

            gridCell.Stack.Add(hexagon);
            hexagon.MoveToLocal(targetLocalPosition);
            //hexagon.transform.localPosition = targetLocalPosition;
        }
    }


    private IEnumerator CheckForCompleteStacks(GridCell gridCell, Color topColor)
    {
        if (gridCell.Stack.Hexagons.Count < 10)
            yield break;

        List<Hexagon> similarHexagons = new List<Hexagon>();
        for (int i = gridCell.Stack.Hexagons.Count - 1; i >= 0; i--)
        {
            Hexagon hexagon = gridCell.Stack.Hexagons[i];
            if (hexagon.color != topColor)
                break;

            similarHexagons.Add(hexagon);
        }

        // At this point, we have a list of similar hexagons
        //How Many?

        int similarHexagonCount = similarHexagons.Count;

        if (similarHexagons.Count < 10)
            yield break;

        float delay = 0;

        while (similarHexagons.Count > 0)
        {
            similarHexagons[0].SetParent(null);
            similarHexagons[0].Vanish(delay);
            //DestroyImmediate(similarHexagons[0].gameObject);

            delay += .01f;

            gridCell.Stack.Remove(similarHexagons[0]);
            similarHexagons.RemoveAt(0);
        }
        updatedCells.Add(gridCell);

        yield return new WaitForSeconds(.2f + (similarHexagonCount + 1) * .01f);
    }

   
}

//--------  notes -----------------
// when we merge. \
// first case is when we drop a stack on a grid cell. We already have a action for that! Lets subscribe it

// to animate our hexagons, we are going to turn some stuff into coroutines. Its going to make it way easier to animate and add delays