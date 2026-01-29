using UnityEngine;

/// <summary>
/// Saves and loads game progress using PlayerPrefs.
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance
                _instance = FindAnyObjectByType<SaveManager>();
                
                // If not found, create one
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    _instance = go.AddComponent<SaveManager>();
                    // Debug log removed

                }
            }
            return _instance;
        }
    }
    
    // PlayerPrefs keys
    private const string LAST_LEVEL_KEY = "LastLevel";
    private const int MAX_LEVEL = 10; // Total level count
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Ensure root for DDOL
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Returns the last played/reached level number.
    /// Returns 1 if first time opening.
    /// </summary>
    public int GetLastLevel()
    {
        return PlayerPrefs.GetInt(LAST_LEVEL_KEY, 1);
    }
    
    /// <summary>
    /// Saves the last played level number.
    /// </summary>
    /// <param name="level">Level number to save</param>
    public void SetLastLevel(int level)
    {
        // Don't exceed maximum level limit
        int clampedLevel = Mathf.Clamp(level, 1, MAX_LEVEL + 1);
        PlayerPrefs.SetInt(LAST_LEVEL_KEY, clampedLevel);
        PlayerPrefs.Save();
        
        // Debug log removed

    }
    
    /// <summary>
    /// Checks if all levels are completed.
    /// </summary>
    public bool IsGameFinished()
    {
        return GetLastLevel() > MAX_LEVEL;
    }
    
    /// <summary>
    /// Clears all saved data. For Debug/Test purposes.
    /// </summary>
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // Debug log removed

    }
}
