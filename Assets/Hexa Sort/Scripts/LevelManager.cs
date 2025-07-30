using System.Collections;
using UnityEngine;
[System.Serializable]
public class LevelData
{
    public GameObject levelPrefab;
    public int targetScore;
    public string levelName;
    //public int gridSize;
}
public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform levelParent;
    [SerializeField] private LevelData[] levels;
    [Header("Elements")]
    [SerializeField] private GameObject completePanel;

    private GameObject currentLevelInstance;
    private int currentLevelIndex = 0;
    public static LevelManager Instance { get; private set; }
    

    private void Awake()
    {
        Debug.Log("LevelManager Awake called");

        if (Instance == null)
        {
            Instance = this;
            Debug.Log("LevelManager: I am the first instance, setting as singleton");
        }
        else
        {
            Debug.Log($"LevelManager: Another instance already exists ({Instance.name}), destroying myself ({gameObject.name})");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel(int levelIndex)
    {
        Debug.Log($"=== LoadLevel called with index: {levelIndex} ===");
    
        // Check if we have levels
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No levels assigned to LevelManager!");
            return;
        }
    
        // Check if levelParent exists
        if (levelParent == null)
        {
            Debug.LogError("LevelParent is not assigned!");
            return;
        }

        // Destroy current level if exists
        if (currentLevelInstance != null)
        {
            Debug.Log("Destroying previous level instance");
            DestroyImmediate(currentLevelInstance);          
        }

        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    private IEnumerator LoadLevelCoroutine(int levelIndex)
    {
        // Destroy level if exist
        if (currentLevelInstance != null)
        {
            DestroyImmediate(currentLevelInstance);
            yield return null;
        }
        // Reset StackSpawner
        StackSpawner stackSpawner = FindFirstObjectByType<StackSpawner>();
        if (stackSpawner != null)
        {
            stackSpawner.ResetForNewLevel();
        }

        yield return null; // wait anathor frame

        // check if level exist
        if (levelIndex >= levels.Length)
        {
            Debug.Log("All Levels Completed");
            levelIndex = 0;
        }

        currentLevelIndex = levelIndex;
        LevelData levelData = levels[currentLevelIndex];

        // Check if prefab exists
        if (levelData.levelPrefab == null)
        {
            Debug.LogError($"Level prefab is null for level {levelIndex}!");
            yield break;
        }
    
        Debug.Log($"Attempting to instantiate: {levelData.levelPrefab.name}");
    
        // Spawn new level
        currentLevelInstance = Instantiate(levelData.levelPrefab, levelParent);
        currentLevelInstance.SetActive(true);

        Debug.Log($"Successfully instantiated: {currentLevelInstance.name}");

        // Wait gor GridGenerator to initialize
        yield return new WaitForEndOfFrame();

        // Update ScoreManager with target score only

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetLevelData(currentLevelIndex + 1, levelData.targetScore);
            Debug.Log($"ScoreManager updated: Level {currentLevelIndex + 1}, Target: {levelData.targetScore}");
        }

        // Force generate stacks after level is fully loaded
        if (stackSpawner != null)
        {
            stackSpawner.ForceGenerateStacks();
        }

        // Make sure StackController is enabled
        StackContoller stackController = FindFirstObjectByType<StackContoller>();
        if (stackController != null)
        {
            stackController.enabled = true;
            Debug.Log("StackController re-enabled");
        }
        // Verify grid cells exist
        yield return null;
        GridCell[] gridCells = FindObjectsByType<GridCell>(FindObjectsSortMode.None);
        Debug.Log($"Found {gridCells.Length} grid cells after level load");

        Debug.Log($"=== Level {levelData.levelName} loaded successfully ===");
    
    }

    public void LoadNextLevel()
    {
        // Hide the complete panel first
        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }

        // Re-enable StackController immediately
        StackContoller stackController = FindFirstObjectByType<StackContoller>();
        if (stackController != null)
        {
            stackController.enabled = true;
        }

        // Load the next level
        LoadLevel(currentLevelIndex + 1);
    }

    public LevelData GetCurrentLevelData()
    {
        if (currentLevelIndex < levels.Length)
            return levels[currentLevelIndex];

        return null;
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}

