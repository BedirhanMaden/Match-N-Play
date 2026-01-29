using UnityEngine;
using System;

/// <summary>
/// Active level management. Move count, win/lose control.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    public static LevelManager Instance { get; private set; }
    
    /// <summary>Current level data</summary>
    public LevelData CurrentLevelData { get; private set; }
    
    /// <summary>Remaining move count</summary>
    public int RemainingMoves { get; private set; }
    
    /// <summary>Active level number</summary>
    public int CurrentLevelNumber { get; private set; }
    
    // Events
    public event Action<int> OnMoveCountChanged;
    public event Action OnLevelWin;
    public event Action OnLevelLose;
    
    private LevelLoader levelLoader;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        levelLoader = FindAnyObjectByType<LevelLoader>();
        if (levelLoader == null)
        {
            levelLoader = gameObject.AddComponent<LevelLoader>();
        }
    }
    
    private void Start()
    {
        // Get last played level from SaveManager
        CurrentLevelNumber = SaveManager.Instance != null 
            ? SaveManager.Instance.GetLastLevel() 
            : 1;
            
        LoadCurrentLevel();
    }
    
    /// <summary>
    /// Loads the current level.
    /// </summary>
    public void LoadCurrentLevel()
    {
        CurrentLevelData = levelLoader.LoadLevel(CurrentLevelNumber);
        
        if (CurrentLevelData != null)
        {
            RemainingMoves = CurrentLevelData.move_count;
            OnMoveCountChanged?.Invoke(RemainingMoves);
        }
    }
    
    /// <summary>
    /// Called when a move is used. After valid blast/tap.
    /// </summary>
    public void UseMove()
    {
        if (RemainingMoves <= 0) return;
        
        RemainingMoves--;
        OnMoveCountChanged?.Invoke(RemainingMoves);
        

        
        // Lose check happens after GridManager obstacle count check
    }
    
    /// <summary>
    /// Checks win condition. Win if all obstacles are cleared.
    /// </summary>
    public void CheckWinCondition(int remainingObstacles)
    {
        // Safety: If already Won, do not check for Lose (happens during Celebration when moves -> 0)
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Win)
        {
            return;
        }

        if (remainingObstacles <= 0)
        {
            TriggerWin();
        }
        else if (RemainingMoves <= 0)
        {
            TriggerLose();
        }
    }
    
    /// <summary>
    /// Called when level is won.
    /// </summary>
    private void TriggerWin()
    {

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameManager.GameState.Win);
        }
        
        // Save next level
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetLastLevel(CurrentLevelNumber + 1);
        }
        
        OnLevelWin?.Invoke();
        
        // Show celebration particles
        ShowCelebration();
        
        // Load MainScene after a short delay for celebration
        // Invoke(nameof(LoadMainSceneAfterWin), 1.5f); // Handled in CelebrationRoutine
    }
    
    /// <summary>
    /// Shows celebration particles and effects.
    /// New Logic: Converts remaining moves to rockets -> Explodes them.
    /// </summary>
    private void ShowCelebration()
    {

        StartCoroutine(CelebrationRoutine());
    }

    private System.Collections.IEnumerator CelebrationRoutine()
    {
        // 1. Convert Moves to Rockets
        while (RemainingMoves > 0)
        {
            yield return new WaitForSeconds(0.2f); // Delay between conversions
            
            RemainingMoves--;
            OnMoveCountChanged?.Invoke(RemainingMoves);
            
            if (GridManager.Instance != null)
            {
                GridManager.Instance.ReplaceRandomCubeWithRocket();
            }
        }
        
        yield return new WaitForSeconds(0.5f); // Waiting before explosion
        
        // 2. Explode All Special Items
        if (GridManager.Instance != null)
        {
            float waitTime = GridManager.Instance.ExplodeAllSpecialItems();
            yield return new WaitForSeconds(waitTime + 1.0f); // Wait for explosions + buffer
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        // 3. Finish Level - Show Celebration UI
        if (celebrationUI != null)
        {

            celebrationUI.Show(LoadMainSceneAfterWin);
        }
        else
        {
            Debug.LogWarning("[LevelManager] CelebrationUI not assigned! Falling back to direct load.");
            Invoke(nameof(LoadMainSceneAfterWin), 0.5f);
        }
    }
    
    /// <summary>
    /// Loads MainScene after win celebration.
    /// </summary>
    private void LoadMainSceneAfterWin()
    {

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
    }
    
    [Header("UI References")]
    [Tooltip("Celebration UI reference")]
    public Assets.Scripts.UI.CelebrationUI celebrationUI; // Updated namespace if needed, or just CelebrationUI if using namespace

    [Tooltip("Fail popup reference")]
    public FailPopup failPopup;
    
    /// <summary>
    /// Registers a new FailPopup instance.
    /// Called by FailPopup on Awake/Start.
    /// </summary>
    public void RegisterFailPopup(FailPopup popup)
    {
        failPopup = popup;

    }

    /// <summary>
    /// Registers a new CelebrationUI instance.
    /// </summary>
    public void RegisterCelebrationUI(Assets.Scripts.UI.CelebrationUI ui)
    {
        celebrationUI = ui;

    }
    
    /// <summary>
    /// Called when level is lost.
    /// </summary>
    private void TriggerLose()
    {

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameManager.GameState.Lose);
        }
        
        OnLevelLose?.Invoke();
        
        // Show Fail popup via direct reference if possible, otherwise find it
        if (failPopup != null)
        {
            failPopup.Show();

        }
        else
        {
            // Fallback search
            FailPopup foundPopup = FindAnyObjectByType<FailPopup>(FindObjectsInactive.Include);
            if (foundPopup != null)
            {
                foundPopup.Show();

            }
            else
            {
                Debug.LogWarning("[LevelManager] FailPopup not found in scene!");
            }
        }
    }

    /// <summary>
    /// CHEAT: Instantly wins the level.
    /// Can be linked to a UI Button.
    /// </summary>
    public void CheatWin()
    {

        
        // 1. Force clear obstacles if GridManager exists (optional but good for consistency)
        if (GridManager.Instance != null)
        {
            // Just satisfy the condition logically
            CheckWinCondition(0); 
        }
        else
        {
            // Fallback direct trigger
            TriggerWin();
        }
    }
}
