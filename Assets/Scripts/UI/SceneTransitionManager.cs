using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles smooth scene transitions with Fade In/Out effects.
/// Persistent Singleton.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Components")]
    [Tooltip("Animator for transition effects (Must have 'FadeOut' and 'FadeIn' triggers or states)")]
    public Animator transitionAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads a scene with a transition.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Trigger Fade Out (Animation)
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start"); // Starts transition (e.g., Fade to Black)
            
            // Wait for animation to finish (approx 1 sec or use animation event)
            yield return new WaitForSeconds(1.0f); 
        }

        // 2. Load Scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
        yield return new WaitForEndOfFrame();

        // 3. Trigger Fade In (Animation)
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("End"); // Ends transition (e.g., Fade from Black)
        }
    }
}
