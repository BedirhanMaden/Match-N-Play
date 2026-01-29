using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for a single goal item in the TopBar.
/// Displays the goal icon and remaining count.
/// </summary>
public class GoalItemUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Image showing the goal item icon")]
    public Image goalIcon;
    
    [Tooltip("TextMeshPro showing remaining count")]
    public TextMeshProUGUI countText;
    
    [Header("Visual Settings")]
    [Tooltip("Color when goal is completed")]
    public Color completedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Completion Feedback")]
    [Tooltip("Checkmark object to show when goal is completed")]
    public GameObject checkMarkIcon;
    
    /// <summary>Goal type identifier</summary>
    public string GoalType { get; private set; }
    
    /// <summary>Remaining count to complete goal</summary>
    public int RemainingCount { get; private set; }
    
    /// <summary>
    /// Initializes the goal item with icon and count.
    /// </summary>
    /// <param name="type">Goal type (bo, s, v)</param>
    /// <param name="sprite">Icon sprite</param>
    /// <param name="initialCount">Initial count</param>
    public void Initialize(string type, Sprite sprite, int initialCount)
    {
        GoalType = type;
        RemainingCount = initialCount;
        
        if (goalIcon != null && sprite != null)
        {
            goalIcon.sprite = sprite;
        }

        // Reset state
        if (checkMarkIcon != null) checkMarkIcon.SetActive(false);
        if (countText != null) countText.gameObject.SetActive(true);
        
        UpdateCountDisplay();
    }
    
    /// <summary>
    /// Updates the remaining count and display.
    /// </summary>
    /// <param name="newCount">New remaining count</param>
    public void UpdateCount(int newCount)
    {
        RemainingCount = Mathf.Max(0, newCount);
        UpdateCountDisplay();
        
        // Visual feedback when completed
        if (RemainingCount <= 0)
        {
            OnGoalCompleted();
        }
    }
    
    /// <summary>
    /// Decrements the count by specified amount.
    /// </summary>
    /// <param name="amount">Amount to decrement</param>
    public void DecrementCount(int amount = 1)
    {
        UpdateCount(RemainingCount - amount);
    }
    
    /// <summary>
    /// Updates the count text display.
    /// </summary>
    private void UpdateCountDisplay()
    {
        string displayText = RemainingCount.ToString();
        
        if (countText != null)
        {
            countText.text = displayText;
        }
    }
    
    /// <summary>
    /// Called when goal is completed (count reaches 0).
    /// </summary>
    private void OnGoalCompleted()
    {
        // Show checkmark
        if (checkMarkIcon != null)
        {
            checkMarkIcon.SetActive(true);
        }

        // Hide text
        if (countText != null) countText.gameObject.SetActive(false);

        // Debug log removed

    }
    
    /// <summary>
    /// Checks if this goal is completed.
    /// </summary>
    public bool IsCompleted()
    {
        return RemainingCount <= 0;
    }
}
