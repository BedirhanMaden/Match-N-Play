using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UI component managing the Goal panel in TopBar.
/// Spawns GoalItemUI instances based on obstacles found in grid.
/// Goals are automatically parsed from the level grid data.
/// </summary>
public class GoalUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Container for goal items (should have HorizontalLayoutGroup)")]
    public RectTransform goalItemsContainer;
    
    [Tooltip("Prefab for individual goal items")]
    public GameObject goalItemPrefab;
    
    [Header("Goal Sprites")]
    [Tooltip("Box obstacle sprite")]
    public Sprite boxSprite;
    
    [Tooltip("Stone obstacle sprite")]
    public Sprite stoneSprite;
    
    [Tooltip("Vase obstacle sprite")]
    public Sprite vaseSprite;
    
    /// <summary>Active goal item UI instances</summary>
    private Dictionary<string, GoalItemUI> goalItems = new Dictionary<string, GoalItemUI>();
    
    private void Start()
    {
        // Subscribe to LevelManager events if needed
        if (LevelManager.Instance != null)
        {
            // Initialize goals when level loads
            InitializeGoals();
        }
    }
    
    /// <summary>
    /// Initializes goals by parsing obstacles from grid data.
    /// Counts all obstacles (bo, s, v) in the grid and creates goal items.
    /// </summary>
    public void InitializeGoals()
    {
        ClearGoals();
        
        LevelData levelData = LevelManager.Instance?.CurrentLevelData;
        if (levelData == null || levelData.grid == null)
        {
            Debug.LogWarning("GoalUI: No level data or grid found.");
            return;
        }
        
        // Count obstacles from grid
        Dictionary<string, int> obstacleCounts = CountObstaclesInGrid(levelData.grid);
        
        // Create goal items for each obstacle type found
        foreach (var kvp in obstacleCounts)
        {
            if (kvp.Value > 0)
            {
                CreateGoalItem(kvp.Key, kvp.Value);
            }
        }
    }
    
    /// <summary>
    /// Counts obstacles in the grid data.
    /// </summary>
    /// <param name="grid">Grid data from level</param>
    /// <returns>Dictionary of obstacle type to count</returns>
    private Dictionary<string, int> CountObstaclesInGrid(List<string> grid)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>()
        {
            { "bo", 0 },  // Box
            { "s", 0 },   // Stone
            { "v", 0 }    // Vase
        };
        
        foreach (string cell in grid)
        {
            string cellLower = cell.ToLower().Trim();
            
            if (counts.ContainsKey(cellLower))
            {
                counts[cellLower]++;
            }
        }
        
        return counts;
    }
    
    /// <summary>
    /// Creates a goal item UI for the given type and count.
    /// </summary>
    /// <param name="type">Obstacle type (bo, s, v)</param>
    /// <param name="count">Total count to destroy</param>
    private void CreateGoalItem(string type, int count)
    {
        if (goalItemPrefab == null || goalItemsContainer == null)
        {
            Debug.LogError("GoalUI: Missing prefab or container reference.");
            return;
        }
        
        // Instantiate prefab
        GameObject itemObj = Instantiate(goalItemPrefab, goalItemsContainer);
        GoalItemUI itemUI = itemObj.GetComponent<GoalItemUI>();
        
        if (itemUI == null)
        {
            Debug.LogError("GoalUI: GoalItemPrefab missing GoalItemUI component.");
            Destroy(itemObj);
            return;
        }
        
        // Get sprite for goal type
        Sprite sprite = GetSpriteForType(type);
        
        // Initialize the item
        itemUI.Initialize(type, sprite, count);
        
        // Store reference
        goalItems[type] = itemUI;
        
        // Debug log removed

    }
    
    /// <summary>
    /// Gets the sprite for a goal type.
    /// </summary>
    /// <param name="type">Goal type identifier</param>
    /// <returns>Corresponding sprite</returns>
    private Sprite GetSpriteForType(string type)
    {
        switch (type.ToLower())
        {
            case "bo":
            case "box":
                return boxSprite;
            case "s":
            case "stone":
                return stoneSprite;
            case "v":
            case "vase":
                return vaseSprite;
            default:
                Debug.LogWarning($"GoalUI: Unknown goal type: {type}");
                return null;
        }
    }
    
    /// <summary>
    /// Called when an obstacle is destroyed.
    /// Updates the corresponding goal.
    /// </summary>
    /// <param name="obstacleType">Type of destroyed obstacle</param>
    public void OnObstacleDestroyed(string obstacleType)
    {
        if (goalItems.TryGetValue(obstacleType, out GoalItemUI itemUI))
        {
            itemUI.DecrementCount(1);
            
            // Check if all goals completed
            CheckAllGoalsCompleted();
        }
    }
    
    /// <summary>
    /// Checks if all goals are completed.
    /// </summary>
    private void CheckAllGoalsCompleted()
    {
        bool allCompleted = true;
        
        foreach (var item in goalItems.Values)
        {
            if (!item.IsCompleted())
            {
                allCompleted = false;
                break;
            }
        }
        
        if (allCompleted)
        {
            // Debug log removed

            // LevelManager will handle win condition via CheckWinCondition
        }
    }
    
    /// <summary>
    /// Clears all goal items from the container.
    /// </summary>
    public void ClearGoals()
    {
        foreach (Transform child in goalItemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        goalItems.Clear();
    }
    
    /// <summary>
    /// Gets remaining count for a specific goal type.
    /// </summary>
    /// <param name="type">Goal type</param>
    /// <returns>Remaining count, or -1 if not found</returns>
    public int GetRemainingCount(string type)
    {
        if (goalItems.TryGetValue(type, out GoalItemUI itemUI))
        {
            return itemUI.RemainingCount;
        }
        return -1;
    }
    
    /// <summary>
    /// Gets total remaining obstacles across all goals.
    /// </summary>
    public int GetTotalRemainingObstacles()
    {
        int total = 0;
        foreach (var item in goalItems.Values)
        {
            total += item.RemainingCount;
        }
        return total;
    }
}
