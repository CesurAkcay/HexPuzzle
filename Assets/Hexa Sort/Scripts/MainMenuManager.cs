using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    
    [Header("Tower Animation")]
    [SerializeField] private Tower tower;
    [SerializeField] private float fillSpeed = 0.02f;
    
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "SampleScene";
    
    private bool isStarting = false;
    
    private void Start()
    {
        Debug.Log("MainMenuManager Start - Setting up button");
        
        if (startButton != null)
        {
            // Clear any existing listeners
            startButton.onClick.RemoveAllListeners();
            // Add the listener
            startButton.onClick.AddListener(StartGame);
            
            Debug.Log("Start button listener added");
        }
        else
        {
            Debug.LogError("Start button is null!");
        }
        
        Time.timeScale = 1f;
    }
    
    public void StartGame()
    {
        Debug.Log("StartGame called!");
        
        if (isStarting) 
        {
            Debug.Log("Already starting, ignoring");
            return;
        }
        
        isStarting = true;
        
        if (startButton != null)
            startButton.interactable = false;
        
        StartCoroutine(FillTowerAndStartGame());
    }
    
    private IEnumerator FillTowerAndStartGame()
    {
        Debug.Log("Starting tower fill animation");
        
        if (tower != null)
        {
            while (tower.GetFillPercent() < 1f)
            {
                tower.Fill(fillSpeed);
                yield return null;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("Loading game scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Alternative method you can call for testing
    [ContextMenu("Test Start Game")]
    public void TestStartGame()
    {
        StartGame();
    }
}