
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;



public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject hexagon;

    [Header("Settings")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private int gridSize;

    [Header("Level Design")]
    [SerializeField] private bool useCustomDesign = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // We want generate the grid every time we update the grid size 

    private void Start()
    {
        if (!useCustomDesign || transform.childCount == 0)
        {
            GenerateGrid();
        }
        else
        {
            InitializeExistingGridCells();
        }

    }
    private void GenerateGrid()
    {
        transform.Clear();

        int hexCount = 0;
        for (int x = -gridSize; x <= gridSize; x++)
        {
            for (int y = -gridSize; y <= gridSize; y++)
            {
                Vector3 spawnPos = grid.CellToWorld(new Vector3Int(x, y, 0));

                if (spawnPos.magnitude > grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * gridSize)
                    continue;

                GameObject gridHex = Instantiate(hexagon, spawnPos, Quaternion.identity, transform);
                hexCount++;
            }
        }

    }

    private void InitializeExistingGridCells()
    {
        // Initialize any existing GridCells that might have HexStacks
        GridCell[] existingCells = GetComponentsInChildren<GridCell>();

        foreach (GridCell cell in existingCells)
        {
            // if the cell has pre-designed stacks, initalize them
            if (cell.transform.childCount > 1 && cell.Stack == null)
            {
                HexStack existingStack = cell.transform.GetChild(1).GetComponent<HexStack>();
                if (existingStack != null)
                {
                    existingStack.Initialize();
                    cell.AssignStack(existingStack);
                    Debug.Log($"Initialized existing stack in cell: {cell.name}");
                }
            }
        }  
    }
}

