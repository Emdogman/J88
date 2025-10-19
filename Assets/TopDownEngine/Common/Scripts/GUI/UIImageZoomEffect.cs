using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Gradually zooms a UI Image from the center when activated
    /// Perfect for splash screens, overlays, or UI transitions
    /// </summary>
    [AddComponentMenu("TopDown Engine/GUI/UI Image Zoom Effect")]
    [RequireComponent(typeof(Image))]
    public class UIImageZoomEffect : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [Tooltip("Starting scale (0 = invisible, 1 = normal size)")]
        [Range(0f, 2f)]
        public float startScale = 0f;
        
        [Tooltip("Target scale to zoom to (1 = normal size, >1 = larger)")]
        [Range(0f, 5f)]
        public float targetScale = 1f;
        
        [Tooltip("Duration of the zoom animation (seconds)")]
        public float zoomDuration = 1f;
        
        [Tooltip("Animation curve for zoom progression")]
        public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Auto Start")]
        [Tooltip("Start zooming automatically when enabled")]
        public bool autoStartOnEnable = true;
        
        [Tooltip("Delay before auto-start (seconds)")]
        public float autoStartDelay = 0f;
        
        [Header("Fade Options")]
        [Tooltip("Also fade in alpha during zoom")]
        public bool fadeWhileZooming = false;
        
        [Tooltip("Starting alpha (0 = invisible, 1 = visible)")]
        [Range(0f, 1f)]
        public float startAlpha = 0f;
        
        [Tooltip("Target alpha")]
        [Range(0f, 1f)]
        public float targetAlpha = 1f;
        
        [Header("Loop Settings")]
        [Tooltip("Loop the zoom animation continuously")]
        public bool loopAnimation = false;
        
        [Tooltip("Reverse back to start scale when loop completes")]
        public bool reverseOnLoop = false;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private Image _image;
        private RectTransform _rectTransform;
        private float _elapsedTime = 0f;
        private bool _isZooming = false;
        private Vector3 _originalScale;
        private Color _originalColor;
        private bool _isReversing = false;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            
            // Store original values
            _originalScale = _rectTransform.localScale;
            _originalColor = _image.color;
        }

        private void OnEnable()
        {
            if (autoStartOnEnable)
            {
                if (autoStartDelay > 0f)
                {
                    Invoke(nameof(StartZoom), autoStartDelay);
                }
                else
                {
                    StartZoom();
                }
            }
        }

        private void Update()
        {
            if (!_isZooming) return;

            _elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_elapsedTime / zoomDuration);
            
            // Apply curve
            float curveValue = zoomCurve.Evaluate(normalizedTime);
            
            // Calculate current scale
            float currentScale;
            if (_isReversing)
            {
                currentScale = Mathf.Lerp(targetScale, startScale, curveValue);
            }
            else
            {
                currentScale = Mathf.Lerp(startScale, targetScale, curveValue);
            }
            
            // Apply scale
            _rectTransform.localScale = _originalScale * currentScale;
            
            // Apply fade if enabled
            if (fadeWhileZooming)
            {
                float currentAlpha;
                if (_isReversing)
                {
                    currentAlpha = Mathf.Lerp(targetAlpha, startAlpha, curveValue);
                }
                else
                {
                    currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                }
                
                Color newColor = _originalColor;
                newColor.a = currentAlpha;
                _image.color = newColor;
            }
            
            // Check if animation complete
            if (normalizedTime >= 1f)
            {
                if (loopAnimation)
                {
                    if (reverseOnLoop)
                    {
                        _isReversing = !_isReversing;
                    }
                    
                    _elapsedTime = 0f;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"UIImageZoomEffect: Loop iteration complete, {(_isReversing ? "reversing" : "forward")}");
                    }
                }
                else
                {
                    _isZooming = false;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log("UIImageZoomEffect: Zoom complete");
                    }
                }
            }
        }

        /// <summary>
        /// Starts the zoom animation
        /// </summary>
        [ContextMenu("Start Zoom")]
        public void StartZoom()
        {
            _elapsedTime = 0f;
            _isZooming = true;
            _isReversing = false;
            
            // Set initial scale
            _rectTransform.localScale = _originalScale * startScale;
            
            // Set initial alpha if fading
            if (fadeWhileZooming)
            {
                Color newColor = _originalColor;
                newColor.a = startAlpha;
                _image.color = newColor;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"UIImageZoomEffect: Zoom started from {startScale} to {targetScale}");
            }
        }

        /// <summary>
        /// Stops the zoom animation
        /// </summary>
        [ContextMenu("Stop Zoom")]
        public void StopZoom()
        {
            _isZooming = false;
            
            if (showDebugInfo)
            {
                Debug.Log("UIImageZoomEffect: Zoom stopped");
            }
        }

        /// <summary>
        /// Resets to original scale and color
        /// </summary>
        [ContextMenu("Reset")]
        public void ResetToOriginal()
        {
            _isZooming = false;
            _elapsedTime = 0f;
            _rectTransform.localScale = _originalScale;
            _image.color = _originalColor;
            
            if (showDebugInfo)
            {
                Debug.Log("UIImageZoomEffect: Reset to original");
            }
        }

        /// <summary>
        /// Set to start scale immediately
        /// </summary>
        [ContextMenu("Set Start Scale")]
        public void SetToStartScale()
        {
            _rectTransform.localScale = _originalScale * startScale;
            
            if (fadeWhileZooming)
            {
                Color newColor = _originalColor;
                newColor.a = startAlpha;
                _image.color = newColor;
            }
        }

        /// <summary>
        /// Set to target scale immediately
        /// </summary>
        [ContextMenu("Set Target Scale")]
        public void SetToTargetScale()
        {
            _rectTransform.localScale = _originalScale * targetScale;
            
            if (fadeWhileZooming)
            {
                Color newColor = _originalColor;
                newColor.a = targetAlpha;
                _image.color = newColor;
            }
        }
    }
}

