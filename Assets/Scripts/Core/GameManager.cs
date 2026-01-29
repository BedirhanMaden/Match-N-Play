using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager. Uses Singleton pattern.
/// Responsibilities: Game state management and scene transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    public static GameManager Instance { get; private set; }
    
    /// <summary>Game states</summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Win,
        Lose
    }
    
    /// <summary>Current game state</summary>
    public GameState CurrentState { get; private set; }
    
    private void Awake()
    {
        // Apply Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Only root GameObjects can use DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject); // Persists across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        CurrentState = GameState.MainMenu;
    }
    
    /// <summary>
    /// Changes the game state.
    /// </summary>
    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }
    
    /// <summary>
    /// Transitions to MainScene.
    /// </summary>
    public void LoadMainScene()
    {
        SetState(GameState.MainMenu);
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("MainScene");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }
    
    /// <summary>
    /// Transitions to LevelScene and starts the game.
    /// </summary>
    public void LoadLevelScene()
    {
        SetState(GameState.Playing);
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("LevelScene");
        }
        else
        {
            SceneManager.LoadScene("LevelScene");
        }
    }
    
    /// <summary>
    /// Reloads the current level.
    /// </summary>
    public void ReloadCurrentLevel()
    {
        SetState(GameState.Playing);
        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(currentScene);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
    }
}
