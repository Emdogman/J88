using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Automatically disables UI Images or GameObjects after a set delay
    /// Perfect for temporary UI elements, notifications, or splash screens
    /// </summary>
    [AddComponentMenu("TopDown Engine/GUI/Auto Disable UI After Delay")]
    public class AutoDisableUIAfterDelay : MonoBehaviour
    {
        [Header("Disable Settings")]
        [Tooltip("Time to wait before disabling (seconds)")]
        public float disableDelay = 4f;
        
        [Tooltip("Disable this GameObject (if unchecked, only disables specific targets)")]
        public bool disableSelf = true;
        
        [Tooltip("Additional GameObjects to disable (optional)")]
        public GameObject[] additionalTargets;
        
        [Header("Auto Start")]
        [Tooltip("Start timer automatically on Enable")]
        public bool autoStartOnEnable = true;
        
        [Header("Fade Out (Optional)")]
        [Tooltip("Fade out before disabling")]
        public bool fadeBeforeDisable = false;
        
        [Tooltip("Duration of fade out (seconds)")]
        public float fadeDuration = 1f;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private float _startTime;
        private bool _timerStarted = false;
        private bool _hasDisabled = false;
        private Image[] _images;
        private Color[] _originalColors;
        private bool _isFading = false;
        private float _fadeStartTime;

        private void OnEnable()
        {
            // Reset state when enabled
            _hasDisabled = false;
            _isFading = false;
            
            if (autoStartOnEnable)
            {
                StartTimer();
            }
        }

        private void Start()
        {
            if (fadeBeforeDisable)
            {
                // Get all images for fading
                _images = GetComponentsInChildren<Image>();
                _originalColors = new Color[_images.Length];
                
                for (int i = 0; i < _images.Length; i++)
                {
                    _originalColors[i] = _images[i].color;
                }
            }
        }

        private void Update()
        {
            if (!_timerStarted || _hasDisabled) return;

            float elapsedTime = Time.time - _startTime;
            
            // Check if we should start fading
            if (fadeBeforeDisable && !_isFading && elapsedTime >= (disableDelay - fadeDuration))
            {
                StartFadeOut();
            }
            
            // Check if we should disable
            if (elapsedTime >= disableDelay)
            {
                DisableTargets();
            }
            
            // Update fade
            if (_isFading)
            {
                UpdateFade();
            }
        }

        /// <summary>
        /// Starts the countdown timer
        /// </summary>
        public void StartTimer()
        {
            _startTime = Time.time;
            _timerStarted = true;
            _hasDisabled = false;
            _isFading = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoDisableUIAfterDelay: Timer started, will disable in {disableDelay} seconds");
            }
        }

        /// <summary>
        /// Starts fade out effect
        /// </summary>
        private void StartFadeOut()
        {
            _isFading = true;
            _fadeStartTime = Time.time;
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoDisableUIAfterDelay: Fade out started");
            }
        }

        /// <summary>
        /// Updates fade effect
        /// </summary>
        private void UpdateFade()
        {
            float fadeElapsed = Time.time - _fadeStartTime;
            float fadeProgress = Mathf.Clamp01(fadeElapsed / fadeDuration);
            
            // Fade from original to transparent
            for (int i = 0; i < _images.Length; i++)
            {
                if (_images[i] != null)
                {
                    Color newColor = _originalColors[i];
                    newColor.a = Mathf.Lerp(_originalColors[i].a, 0f, fadeProgress);
                    _images[i].color = newColor;
                }
            }
        }

        /// <summary>
        /// Disables the target GameObjects
        /// </summary>
        private void DisableTargets()
        {
            if (_hasDisabled) return;
            _hasDisabled = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoDisableUIAfterDelay: Disabling targets after {disableDelay} seconds");
            }
            
            // Disable additional targets
            if (additionalTargets != null)
            {
                foreach (GameObject target in additionalTargets)
                {
                    if (target != null)
                    {
                        target.SetActive(false);
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"AutoDisableUIAfterDelay: Disabled {target.name}");
                        }
                    }
                }
            }
            
            // Disable self
            if (disableSelf)
            {
                gameObject.SetActive(false);
                
                if (showDebugInfo)
                {
                    Debug.Log($"AutoDisableUIAfterDelay: Disabled {gameObject.name}");
                }
            }
        }

        /// <summary>
        /// Reset timer and re-enable
        /// </summary>
        [ContextMenu("Reset Timer")]
        public void ResetTimer()
        {
            _hasDisabled = false;
            _timerStarted = false;
            _isFading = false;
            
            // Restore colors if fading
            if (fadeBeforeDisable && _images != null)
            {
                for (int i = 0; i < _images.Length; i++)
                {
                    if (_images[i] != null)
                    {
                        _images[i].color = _originalColors[i];
                    }
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoDisableUIAfterDelay: Timer reset");
            }
        }

        /// <summary>
        /// Manually trigger disable
        /// </summary>
        [ContextMenu("Disable Now")]
        public void DisableNow()
        {
            DisableTargets();
        }

        /// <summary>
        /// Cancel timer
        /// </summary>
        [ContextMenu("Cancel Timer")]
        public void CancelTimer()
        {
            _timerStarted = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoDisableUIAfterDelay: Timer cancelled");
            }
        }
    }
}

