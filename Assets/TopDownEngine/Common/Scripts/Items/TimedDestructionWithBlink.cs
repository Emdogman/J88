using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Destroys the GameObject after a set time, with blinking effect before destruction
    /// Perfect for KoalaCoinPicker (beer) items that should disappear
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/Timed Destruction With Blink")]
    public class TimedDestructionWithBlink : MonoBehaviour
    {
        [Header("Destruction Settings")]
        [Tooltip("Total lifetime before destruction (seconds)")]
        public float lifetime = 6f;
        
        [Tooltip("When to start blinking before destruction (seconds before death)")]
        public float blinkStartTime = 3f;
        
        [Header("Blink Settings")]
        [Tooltip("How fast to blink (times per second)")]
        public float blinkSpeed = 8f;
        
        [Tooltip("Minimum alpha during blink (0 = invisible, 1 = fully visible)")]
        [Range(0f, 1f)]
        public float minAlpha = 0f;
        
        [Tooltip("Maximum alpha during blink")]
        [Range(0f, 1f)]
        public float maxAlpha = 1f;
        
        [Header("Delay")]
        [Tooltip("Delay before timer starts (wait for drop animation)")]
        public float startDelay = 0.5f;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private float _elapsedTime = 0f;
        private bool _isBlinking = false;
        private bool _hasStarted = false;
        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
            
            // Get sprite renderer
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
            else
            {
                // Try to find in children
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (_spriteRenderer != null)
                {
                    _originalColor = _spriteRenderer.color;
                }
                else
                {
                    Debug.LogWarning($"TimedDestructionWithBlink: No SpriteRenderer found on {gameObject.name}!");
                }
            }
        }

        private void Update()
        {
            // Wait for start delay
            if (!_hasStarted)
            {
                if (Time.time - _startTime >= startDelay)
                {
                    _hasStarted = true;
                    _elapsedTime = 0f;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"{gameObject.name}: Timed destruction started - will destroy in {lifetime}s");
                    }
                }
                else
                {
                    return;
                }
            }

            _elapsedTime += Time.deltaTime;
            
            // Check if we should start blinking
            float timeRemaining = lifetime - _elapsedTime;
            
            if (timeRemaining <= blinkStartTime && !_isBlinking)
            {
                _isBlinking = true;
                
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name}: Blinking started - {timeRemaining:F1}s remaining");
                }
            }
            
            // Apply blinking effect
            if (_isBlinking && _spriteRenderer != null)
            {
                ApplyBlinkEffect();
            }
            
            // Destroy when time is up
            if (_elapsedTime >= lifetime)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name}: Lifetime expired, destroying");
                }
                
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Applies the blinking transparency effect
        /// </summary>
        private void ApplyBlinkEffect()
        {
            // Calculate blink using sine wave for smooth transition
            float blinkValue = Mathf.Sin(Time.time * blinkSpeed * Mathf.PI * 2f);
            
            // Remap from [-1, 1] to [minAlpha, maxAlpha]
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (blinkValue + 1f) / 2f);
            
            Color newColor = _originalColor;
            newColor.a = alpha;
            _spriteRenderer.color = newColor;
        }

        /// <summary>
        /// Set custom lifetime
        /// </summary>
        public void SetLifetime(float newLifetime)
        {
            lifetime = newLifetime;
        }

        /// <summary>
        /// Reset timer
        /// </summary>
        [ContextMenu("Reset Timer")]
        public void ResetTimer()
        {
            _elapsedTime = 0f;
            _isBlinking = false;
            _hasStarted = false;
            _startTime = Time.time;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Timer reset");
            }
        }
    }
}

