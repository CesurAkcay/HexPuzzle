using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text gameOverText;
    
    [Header("Game Controllers")]
    [SerializeField] private StackContoller stackController;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    private void Awake()
    {
        StackContoller.onStackPlaced += CheckForGameOver;
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }
    
    private void OnDestroy()
    {
        StackContoller.onStackPlaced -= CheckForGameOver;
    }
    
    private void CheckForGameOver(GridCell gridCell)
    {
        if (debugMode)
        {
            Debug.Log("Checking for game over...");
        }
        
        if (IsGameOver())
        {
            ShowGameOver();
        }
        else if (debugMode)
        {
            Debug.Log("Game continues - moves still available");
        }
    }
    
    private bool IsGameOver()
    {
        // Check if all grid cells are occupied
        GridCell[] allGridCells = FindObjectsByType<GridCell>(FindObjectsSortMode.None);
        int occupiedCells = 0;
        
        foreach (GridCell cell in allGridCells)
        {
            if (cell.IsOccupied)
            {
                occupiedCells++;
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Occupied cells: {occupiedCells}/{allGridCells.Length}");
        }
        
        // If not all cells are occupied, game continues
        if (occupiedCells < allGridCells.Length)
        {
            return false;
        }
        
        // All cells are occupied, check if any actual merges are possible
        bool mergesPossible = AnyActualMergesPossible(allGridCells);
        
        if (debugMode)
        {
            Debug.Log($"Actual merges possible: {mergesPossible}");
        }
        
        return !mergesPossible;
    }
    
    private bool AnyActualMergesPossible(GridCell[] allGridCells)
    {
        // Use the same logic as MergeManager to detect actual valid merges
        LayerMask gridCellMask = 1 << allGridCells[0].gameObject.layer;
        
        foreach (GridCell cell in allGridCells)
        {
            if (!cell.IsOccupied) continue;
            
            // Get neighbors using the same method as MergeManager
            List<GridCell> neighborGridCells = GetNeighborGridCells(cell, gridCellMask);
            
            if (neighborGridCells.Count <= 0)
                continue;
            
            Color gridCellTopHexagonColor = cell.Stack.GetTopHexagonColor();
            
            // Check for similar neighbors using the same logic as MergeManager
            List<GridCell> similarNeighborGridCells = GetSimilarNeighborGridCells(gridCellTopHexagonColor, neighborGridCells.ToArray());
            
            if (similarNeighborGridCells.Count > 0)
            {
                // Check if there are actually hexagons to merge
                List<Hexagon> hexagonsToAdd = GetHexagonsToAdd(gridCellTopHexagonColor, similarNeighborGridCells.ToArray());
                
                if (hexagonsToAdd.Count > 0)
                {
                    if (debugMode)
                    {
                        Debug.Log($"Found valid merge at {cell.transform.position} with {hexagonsToAdd.Count} hexagons to merge");
                    }
                    return true; // Found a valid merge
                }
            }
        }
        
        if (debugMode)
        {
            Debug.Log("No valid merges found - Game Over!");
        }
        
        return false; // No valid merges possible
    }
    
    // Copy the exact same methods from MergeManager
    private List<GridCell> GetNeighborGridCells(GridCell gridCell, LayerMask gridCellMask)
    {
        List<GridCell> neighborGridCells = new List<GridCell>();
        Collider[] neighborGridCellColliders = Physics.OverlapSphere(gridCell.transform.position, 2, gridCellMask);

        foreach (Collider gridCellCollider in neighborGridCellColliders)
        {
            GridCell neighborGridCell = gridCellCollider.GetComponent<GridCell>();

            if (!neighborGridCell.IsOccupied)
                continue;

            if (neighborGridCell == gridCell)
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
            if (ColorEquals(gridCellTopHexagonColor, neighborGridCellTopHexagonColor))
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
                if (!ColorEquals(hexagon.color, gridCellTopHexagonColor))
                {
                    break;
                }

                hexagonsToAdd.Add(hexagon);
            }
        }

        return hexagonsToAdd;
    }
    
    private bool ColorEquals(Color color1, Color color2)
    {
        return Mathf.Approximately(color1.r, color2.r) &&
               Mathf.Approximately(color1.g, color2.g) &&
               Mathf.Approximately(color1.b, color2.b) &&
               Mathf.Approximately(color1.a, color2.a);
    }
    
    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        stackController.enabled = false;
        Time.timeScale = 0f;
        
        Debug.Log("Game Over! No more moves possible.");
    }
    
    private void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}