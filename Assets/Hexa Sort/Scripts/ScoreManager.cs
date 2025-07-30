using System.Collections;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetScoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Button nextLevelButton;
    [Header("Settings")]
    [SerializeField] private int scorePerHexagon = 10;
    [SerializeField] private int completionBonusBase = 100;

    [SerializeField] private float comboMultiplier = 1.5f;

    private int currentScore = 0;
    private int currentCombo = 0;
    private int targetScore;
    private bool isComboActive = false;

    public static ScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            MergeManager.OnMergeCompleted += OnMergeCompleted;
            MergeManager.OnStackCompleted += OnStackCompleted;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
         MergeManager.OnMergeCompleted -= OnMergeCompleted;
         MergeManager.OnStackCompleted -= OnStackCompleted;
    }
    private void OnStackCompleted(int hexagonCount, Color color)
    {
        int completionBonus = Mathf.RoundToInt(completionBonusBase * (hexagonCount / 10f));

        if (isComboActive)
        {
            completionBonus = Mathf.RoundToInt(completionBonus * (1 + (currentCombo * 0.5f)));
        }

        AddScore(completionBonus);

        Debug.Log($"Stack Completed{completionBonus} points!");
    }
    void Start()
    {
        InitializeLevel();
    }

    private void OnMergeCompleted(int hexaagonCount)
    {
        currentCombo++;
        isComboActive = true;

        //calculate score with combo multiplier

        int baseScore = hexaagonCount * scorePerHexagon;
        int comboBonus = Mathf.RoundToInt(baseScore * (comboMultiplier * (currentCombo - 1)));
        int totalScore = baseScore + comboBonus;

        AddScore(totalScore);
        ShowCombo();

        // Reset combo after delay if no more merges to happen
        StartCoroutine(ResetComboAfterDelay());
    }

    private void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();

        if (currentScore >= targetScore)
        {
            LevelComplete();
        }
    }

    private void ShowCombo()
    {
        if (comboText != null && currentCombo > 1)
        {
            comboText.text = $"COMBO : {currentCombo}!";
            comboText.gameObject.SetActive(true);
        }
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (isComboActive)
        {
            currentCombo = 0;
            isComboActive = false;

            if (comboText != null)
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";

        if (targetScoreText != null)
            targetScoreText.text = $"Target Score: {targetScore}";

    }

    private void LevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        //Disable game contorls temporarily
        FindFirstObjectByType<StackContoller>().enabled = false;

        Debug.Log($"Level Complete!  Score : {currentScore}/{targetScore}");
    }

    private void InitializeLevel()
    {
        targetScore = 500;

        UpdateUI();
        Debug.Log($"Level initialized with target score: {targetScore}");

    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
    public int GetCurrentCombo()
    {
        return currentCombo;
    }
}
