using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls level button in MainScene.
/// Responsibilities:
/// - Display current level number
/// - Show "FINISHED" if all levels completed
/// - Transition to LevelScene when clicked
/// </summary>
public class LevelButtonUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Text showing level number")]
    public TextMeshProUGUI levelText;
    
    [Header("Settings")]
    [Tooltip("Level text format. {0} = level number")]
    public string levelFormat = "LEVEL {0}";
    
    [Tooltip("Text shown when all levels completed")]
    public string finishedText = "FINISHED";
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    
    private void Start()
    {
        UpdateButtonText();
    }
    
    /// <summary>
    /// Updates button text.
    /// </summary>
    public void UpdateButtonText()
    {
        string displayText;
        
        // Get last level from SaveManager
        if (SaveManager.Instance != null && SaveManager.Instance.IsGameFinished())
        {
            displayText = finishedText;
        }
        else
        {
            int currentLevel = SaveManager.Instance != null 
                ? SaveManager.Instance.GetLastLevel() 
                : 1;
            displayText = string.Format(levelFormat, currentLevel);
        }
        
        // If using TextMeshPro
        if (levelText != null)
        {
            levelText.text = displayText;
        }
    }
    
    /// <summary>
    /// Called when button is clicked.
    /// </summary>
    public void OnClick()
    {
        // Debug.Log("Level Button Clicked!");

        
        // If game finished, click is invalid
        if (SaveManager.Instance != null && SaveManager.Instance.IsGameFinished())
        {
            // Debug.Log("All levels completed!");

            return;
        }
        
        // Transition to LevelScene
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevelScene();
        }
        else if (SceneTransitionManager.Instance != null)
        {
            // Fallback: Use Transition Manager directly if GameManager is missing
            SceneTransitionManager.Instance.LoadScene("LevelScene");
        }
        else
        {
            // If neither exists, load scene directly
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
        }
    }
}
