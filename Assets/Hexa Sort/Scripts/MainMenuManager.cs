using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Button startButton;
    [Header("Tower Animation")]
    [SerializeField] private Tower tower;
    [SerializeField] private float fillSpeed = 0.03f;
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "SampleScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator FillTowerAndStartGame()
    {
        if (tower != null)
        {
            // Fill the tower to 100%
            while (tower.GetFillPercent() < 1f)
            {
                tower.Fill(fillSpeed);
                yield return null; // Wait one frame
            }
            
            // Wait a moment after filling completes
            yield return new WaitForSeconds(0.5f);
        }
        
        // Load the game scene directly
        SceneManager.LoadScene(gameSceneName);
    }
}
