using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component showing remaining move count.
/// Gets data from LevelManager and updates.
/// </summary>
public class MoveCounterUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("TextMeshPro showing move count")]
    public TextMeshProUGUI moveText;
    
    [Header("Settings")]
    [Tooltip("Text format. {0} = remaining moves")]
    public string textFormat = "{0}";
    
    private void Start()
    {
        // Subscribe to LevelManager event
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnMoveCountChanged += UpdateMoveCount;
            
            // Show initial value
            UpdateMoveCount(LevelManager.Instance.RemainingMoves);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from event (prevent memory leak)
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnMoveCountChanged -= UpdateMoveCount;
        }
    }
    
    /// <summary>
    /// Updates move count display.
    /// </summary>
    /// <param name="remainingMoves">Remaining move count</param>
    public void UpdateMoveCount(int remainingMoves)
    {
        string displayText = string.Format(textFormat, remainingMoves);
        
        // If using TextMeshPro
        if (moveText != null)
        {
            moveText.text = displayText;
        }
    }
}
