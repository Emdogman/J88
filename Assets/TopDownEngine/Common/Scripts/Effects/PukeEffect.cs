using UnityEngine;
using System.Collections;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Controls the visual behavior of the puke effect
    /// Handles fade in/out animation and auto-destruction
    /// </summary>
    public class PukeEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        [Tooltip("Total duration of the effect")]
        [Range(1f, 10f)]
        public float effectDuration = 4f;
        
        [Tooltip("Time to fade in")]
        [Range(0.1f, 2f)]
        public float fadeInTime = 0.3f;
        
        [Tooltip("Time to fade out")]
        [Range(0.1f, 2f)]
        public float fadeOutTime = 1f;
        
        [Tooltip("Final scale of the effect")]
        [Range(0.5f, 3f)]
        public float finalScale = 1.5f;
        
        [Tooltip("Maximum alpha value")]
        [Range(0.1f, 1f)]
        public float maxAlpha = 0.7f;

        [Header("Animation")]
        [Tooltip("Whether to animate the scale")]
        public bool animateScale = true;
        
        [Tooltip("Whether to animate the alpha")]
        public bool animateAlpha = true;
        
        [Tooltip("Whether to add subtle rotation")]
        public bool animateRotation = true;
        
        [Tooltip("Rotation speed")]
        [Range(0f, 360f)]
        public float rotationSpeed = 10f;

        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        // Private variables
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Vector3 _originalScale;
        private float _startTime;
        private bool _isAnimating = false;

        private void Start()
        {
            // Get sprite renderer
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            if (_spriteRenderer == null)
            {
                Debug.LogWarning("PukeEffect: No SpriteRenderer found! Effect may not be visible.");
                return;
            }
            
            // Store original values
            _originalColor = _spriteRenderer.color;
            _originalScale = transform.localScale;
            _startTime = Time.time;
            
            // Start animation
            StartCoroutine(EffectSequence());
            
            if (showDebugInfo)
            {
                Debug.Log($"PukeEffect: Started - Duration: {effectDuration}s, Final Scale: {finalScale}");
            }
        }

        /// <summary>
        /// Main effect animation sequence
        /// </summary>
        private IEnumerator EffectSequence()
        {
            _isAnimating = true;
            
            // Phase 1: Fade in and scale up
            yield return StartCoroutine(FadeInAndScaleUp());
            
            // Phase 2: Stay visible (subtle animation)
            yield return StartCoroutine(StayVisible());
            
            // Phase 3: Fade out
            yield return StartCoroutine(FadeOut());
            
            _isAnimating = false;
            
            // Destroy the effect
            Destroy(gameObject);
        }

        /// <summary>
        /// Fade in and scale up animation
        /// </summary>
        private IEnumerator FadeInAndScaleUp()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeInTime)
            {
                float t = elapsedTime / fadeInTime;
                
                // Animate scale
                if (animateScale)
                {
                    float scale = Mathf.Lerp(0f, finalScale, t);
                    transform.localScale = _originalScale * scale;
                }
                
                // Animate alpha
                if (animateAlpha)
                {
                    float alpha = Mathf.Lerp(0f, maxAlpha, t);
                    SetAlpha(alpha);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Ensure final values
            if (animateScale)
            {
                transform.localScale = _originalScale * finalScale;
            }
            if (animateAlpha)
            {
                SetAlpha(maxAlpha);
            }
        }

        /// <summary>
        /// Stay visible with subtle animation
        /// </summary>
        private IEnumerator StayVisible()
        {
            float stayDuration = effectDuration - fadeInTime - fadeOutTime;
            float elapsedTime = 0f;
            
            while (elapsedTime < stayDuration)
            {
                // Subtle rotation
                if (animateRotation)
                {
                    transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Fade out animation
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeOutTime)
            {
                float t = elapsedTime / fadeOutTime;
                
                // Animate alpha
                if (animateAlpha)
                {
                    float alpha = Mathf.Lerp(maxAlpha, 0f, t);
                    SetAlpha(alpha);
                }
                
                // Optional: Scale down slightly
                if (animateScale)
                {
                    float scale = Mathf.Lerp(finalScale, finalScale * 0.8f, t);
                    transform.localScale = _originalScale * scale;
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Ensure final values
            if (animateAlpha)
            {
                SetAlpha(0f);
            }
        }

        /// <summary>
        /// Set the alpha value of the sprite
        /// </summary>
        private void SetAlpha(float alpha)
        {
            if (_spriteRenderer != null)
            {
                Color color = _spriteRenderer.color;
                color.a = alpha;
                _spriteRenderer.color = color;
            }
        }

        /// <summary>
        /// Test the effect animation (for debugging)
        /// </summary>
        [ContextMenu("Test Effect Animation")]
        public void TestEffectAnimation()
        {
            if (_isAnimating)
            {
                Debug.Log("PukeEffect: Already animating, cannot test");
                return;
            }
            
            Debug.Log("PukeEffect: Testing effect animation");
            StartCoroutine(EffectSequence());
        }

        /// <summary>
        /// Show current effect status (for debugging)
        /// </summary>
        [ContextMenu("Show Effect Status")]
        public void ShowEffectStatus()
        {
            Debug.Log($"PukeEffect Status:");
            Debug.Log($"  - Effect Duration: {effectDuration}s");
            Debug.Log($"  - Fade In Time: {fadeInTime}s");
            Debug.Log($"  - Fade Out Time: {fadeOutTime}s");
            Debug.Log($"  - Final Scale: {finalScale}");
            Debug.Log($"  - Max Alpha: {maxAlpha}");
            Debug.Log($"  - Is Animating: {_isAnimating}");
            Debug.Log($"  - Sprite Renderer: {(_spriteRenderer != null ? "Found" : "Not found")}");
        }

        private void OnGUI()
        {
            if (!showDebugInfo || !Application.isPlaying) return;
            
            GUI.Box(new Rect(10, 120, 300, 80), "Puke Effect Debug");
            
            GUI.Label(new Rect(20, 145, 280, 20), $"Duration: {effectDuration}s");
            GUI.Label(new Rect(20, 165, 280, 20), $"Animating: {(_isAnimating ? "Yes" : "No")}");
            GUI.Label(new Rect(20, 185, 280, 20), $"Scale: {transform.localScale.x:F2}");
            
            if (GUI.Button(new Rect(20, 205, 100, 20), "Test Animation"))
            {
                TestEffectAnimation();
            }
        }
    }
}
