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

        StackSpawner stackSpawner = FindFirstObjectByType<StackSpawner>();
        if (stackSpawner != null)
        {
            stackSpawner.ResetForNewLevel();
        }
    
        // Check if level exists
        if (levelIndex >= levels.Length)
        {
            Debug.Log("All levels completed!");
            levelIndex = 0;
        }
    
        currentLevelIndex = levelIndex;
        LevelData levelData = levels[currentLevelIndex];
    
        // Check if prefab exists
        if (levelData.levelPrefab == null)
        {
            Debug.LogError($"Level prefab is null for level {levelIndex}!");
            return;
        }
    
        Debug.Log($"Attempting to instantiate: {levelData.levelPrefab.name}");
    
        // Spawn new level
        currentLevelInstance = Instantiate(levelData.levelPrefab, levelParent);
    
        Debug.Log($"Successfully instantiated: {currentLevelInstance.name}");
        Debug.Log($"Position: {currentLevelInstance.transform.position}");
        Debug.Log($"Parent: {currentLevelInstance.transform.parent.name}");
        Debug.Log($"Active: {currentLevelInstance.activeInHierarchy}");

        // Update ScoreManager with target score only

        if (stackSpawner != null)
        {
            stackSpawner.ForceGenerateStacks();
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetLevelData(currentLevelIndex + 1, levelData.targetScore);
        }
    
        Debug.Log($"=== LoadLevel completed ===");
    }

    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
        completePanel.gameObject.SetActive(false);
    }

    public LevelData GetCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}

