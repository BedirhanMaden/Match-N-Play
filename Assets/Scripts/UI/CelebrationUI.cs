using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CelebrationUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;            // Star Icon
        [SerializeField] private Animator _textAnimator;        // NEW: Celebration Text
        [SerializeField] private Animator _backgroundAnimator;  // Background Darken
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _backgroundButton; 

        [Header("Effects")]
        [SerializeField] private ParticleSystem[] _confettiParticles;

        private Action _onCompleteCallback;
        private static readonly int PlayTrigger = Animator.StringToHash("Play");

        private void Awake()
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            if (_canvas == null) _canvas = GetComponent<Canvas>();
            
            // Default to hidden
            if (_canvas != null) _canvas.enabled = false;
        }

        public void Show(Action onComplete)
        {
            _onCompleteCallback = onComplete;
            if (_canvas != null) _canvas.enabled = true;
            gameObject.SetActive(true);

            // Trigger Star Animation
            if (_animator != null)
            {
                _animator.SetTrigger(PlayTrigger);
            }

            // Trigger Text Animation
            if (_textAnimator != null)
            {
                 // Ensure it starts from clean state if reusing the same object
                _textAnimator.ResetTrigger("Hide");
                _textAnimator.SetTrigger(PlayTrigger); // Using "Play" trigger for Enter as well
            }

            // Trigger Background Show Key
            if (_backgroundAnimator != null)
            {
                _backgroundAnimator.ResetTrigger("Hide");
                _backgroundAnimator.SetTrigger("Show");
            }

            if (_confettiParticles != null)
            {
                foreach (var p in _confettiParticles)
                {
                    if (p != null) p.Play();
                }
            }

            // SAFETY: Start a fallback timer in case Animation Event isn't set up
            StopAllCoroutines(); 
            StartCoroutine(SafetyFallbackRoutine());
        }

        private IEnumerator SafetyFallbackRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            if (_onCompleteCallback != null)
            {
                Debug.LogWarning("[CelebrationUI] Animation Event missed! Using safety fallback.");
                OnAnimationComplete();
            }
        }

        public void OnAnimationComplete()
        {
            if (_backgroundAnimator != null || _textAnimator != null)
            {
                StartCoroutine(HideBackgroundAndFinish());
            }
            else
            {
                _onCompleteCallback?.Invoke();
            }
        }

        private IEnumerator HideBackgroundAndFinish()
        {
             // Trigger Hide for Background, Star, and Text
             if (_backgroundAnimator != null) _backgroundAnimator.SetTrigger("Hide");
             if (_animator != null) _animator.SetTrigger("Hide");
             if (_textAnimator != null) _textAnimator.SetTrigger("Hide");
             
             yield return new WaitForSeconds(1);
             _onCompleteCallback?.Invoke();
        }
        
        // Alternative: Setup a button to manually continue if animation events aren't preferred
        public void OnContinueClicked()
        {
             OnAnimationComplete();
        }
    }
}
