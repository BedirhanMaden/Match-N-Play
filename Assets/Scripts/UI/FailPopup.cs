using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Popup shown when level is lost.
/// Responsibilities:
/// - Show fail state
/// - Close: Return to MainScene
/// - Try Again: Reload same level
/// </summary>
public class FailPopup : MonoBehaviour
{
    [Header("Text References")]
    [Tooltip("Text showing level number (e.g. 'Level 1')")]
    public TextMeshProUGUI levelText;
    
    [Tooltip("Text showing result (e.g. 'Out of Moves')")]
    public TextMeshProUGUI resultText;

    [Header("Buttons")]
    [Tooltip("Button to return to main menu")]
    public Button closeButton;
    
    [Tooltip("Button to try again")]
    public Button tryAgainButton;
    
    [Header("Animation")]
    [Tooltip("Animator for popup open animation (optional)")]
    public Animator popupAnimator;

    [Tooltip("Animator for the background fade (optional)")]
    public Animator backgroundAnimator;
    
    private void Start()
    {
        // Subscribe to LevelManager lose event
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLose += Show;
            LevelManager.Instance.RegisterFailPopup(this); // Register new instance
            // Debug log removed

        }
        else
        {
            Debug.LogWarning("[FailPopup] LevelManager.Instance is null in Start!");
        }
        
        // Hide at start (but allows Awake/Start to run first)
        gameObject.SetActive(false);
        
        // Connect button events
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClick);
        }
        
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(OnTryAgainClick);
        }
    }
    
    private void OnEnable()
    {
        // Also try subscribing on enable in case LevelManager wasn't ready in Awake
        if (LevelManager.Instance != null && !isSubscribed)
        {
            LevelManager.Instance.OnLevelLose += Show;
            isSubscribed = true;
        }
    }
    
    private bool isSubscribed = false;
    
    private void OnDestroy()
    {
        // Unsubscribe from event
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLose -= Show;
        }
    }
    
    /// <summary>
    /// Shows the popup.
    /// </summary>
    /// <summary>
    /// Shows the popup.
    /// </summary>
    /// <summary>
    /// Shows the popup.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // Ensure it's on top
        transform.localScale = Vector3.one; // Ensure scale is 1
        
        // Ensure all children are active
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
        // Set Texts
        if (levelText != null && LevelManager.Instance != null)
        {
            levelText.text = $"Level {LevelManager.Instance.CurrentLevelNumber}";
        }
        
        if (resultText != null)
        {
            resultText.text = "Out of Moves"; // Hardcoded for now, or could make configurable
        }

        // Play animation if exists
        if (popupAnimator != null)
        {
            popupAnimator.Rebind();
            popupAnimator.Update(0f);
            popupAnimator.SetTrigger("Show");
        }

        // Trigger Background Animation
        if (backgroundAnimator != null)
        {
            backgroundAnimator.ResetTrigger("Hide");
            backgroundAnimator.SetTrigger("Show");
        }
        
        // Debug log removed

    }
    
    /// <summary>
    /// Hides the popup and executes a callback after animation.
    /// </summary>
    public void Hide(System.Action onComplete = null)
    {
        bool hasAnimation = false;

        // Trigger Content Animation
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("Hide");
            hasAnimation = true;
        }

        // Trigger Background Animation
        if (backgroundAnimator != null)
        {
            backgroundAnimator.SetTrigger("Hide");
            hasAnimation = true;
        }

        if (hasAnimation)
        {
            StartCoroutine(WaitAndDeactivate(0.4f, onComplete)); // 0.4s safety buffer (anim usually ~0.3s)
        }
        else
        {
            Deactivate();
            onComplete?.Invoke();
        }
    }
    
    private System.Collections.IEnumerator WaitAndDeactivate(float delay, System.Action onComplete)
    {
        yield return new WaitForSecondsRealtime(delay); // Use Realtime incase game is paused
        
        Deactivate();
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Deactivates the GameObject.
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// When Close button is clicked. Returns to MainScene.
    /// </summary>
    public void OnCloseClick()
    {
        // Debug log removed

        
        // Pass scene loading as callback
        Hide(() => 
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadMainScene();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
            }
        });
    }
    
    /// <summary>
    /// When Try Again button is clicked. Reloads same level.
    /// </summary>
    public void OnTryAgainClick()
    {
        // Debug log removed

        
        // Pass reload logic as callback
        Hide(() => 
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReloadCurrentLevel();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        });
    }
}
