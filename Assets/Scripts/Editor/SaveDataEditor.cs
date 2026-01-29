using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor window to manage Save Data (PlayerPrefs).
/// Accessible via Tools -> Dream Games -> Save Data Manager
/// </summary>
public class SaveDataEditor : EditorWindow
{
    private int levelToSet = 1;

    [MenuItem("Tools/Dream Games/Save Data Manager")]
    public static void ShowWindow()
    {
        GetWindow<SaveDataEditor>("Save Data");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save Data Management", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Display current value (read-only for info)
        int currentSavedLevel = PlayerPrefs.GetInt("LastLevel", 1);
        EditorGUILayout.LabelField("Current Saved Level:", currentSavedLevel.ToString());

        GUILayout.Space(10);
        
        // Input for new level
        levelToSet = EditorGUILayout.IntField("New Level:", levelToSet);

        if (GUILayout.Button("Set Last Level"))
        {
            SetLevel(levelToSet);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Clear All Save Data"))
        {
            if (EditorUtility.DisplayDialog("Clear Data", 
                "Are you sure you want to delete ALL PlayerPrefs data? This cannot be undone.", "Yes", "No"))
            {
                ClearData();
            }
        }
    }

    private void SetLevel(int level)
    {
        // Direct PlayerPrefs modification to work even if SaveManager isn't running in scene
        PlayerPrefs.SetInt("LastLevel", level);
        PlayerPrefs.Save();
        // Debug log removed

        
        // Force repaint to show new value immediately
        Repaint(); 
    }

    private void ClearData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // Debug log removed

        Repaint();
    }
}
