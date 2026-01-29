using UnityEngine;

/// <summary>
/// Loads and parses JSON files from Resources/Levels folder.
/// </summary>
public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// Loads the JSON file for the specified level number.
    /// </summary>
    /// <param name="levelIndex">Level number to load (1, 2, 3...)</param>
    /// <returns>Parsed LevelData object, null if not found</returns>
    public LevelData LoadLevel(int levelIndex)
    {
        // Read level file from Resources/Levels folder (format: level_01, level_02, etc.)
        string path = $"Levels/level_{levelIndex:D2}";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        
        if (jsonFile == null)
        {
            Debug.LogError($"Level file not found: {path}");
            return null;
        }
        
        // Convert JSON to LevelData object
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        
        if (levelData == null)
        {
            Debug.LogError($"Failed to parse level JSON: {path}");
            return null;
        }
        
        return levelData;
    }
}
