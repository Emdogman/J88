using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Makes an object move in a sinusoidal (wave) pattern after landing
    /// Perfect for KoalaCoinPicker beer items
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/Sinus Movement")]
    public class SinusMovement : MonoBehaviour
    {
        [Header("Sinus Movement Settings")]
        [Tooltip("Enable sinus movement")]
        public bool enableMovement = true;
        
        [Tooltip("Delay before sinus movement starts (wait for landing animation)")]
        public float startDelay = 0f;
        
        [Tooltip("Amplitude of horizontal movement (how far left/right)")]
        public float horizontalAmplitude = 0.3f;
        
        [Tooltip("Amplitude of vertical movement (how far up/down)")]
        public float verticalAmplitude = 0.2f;
        
        [Tooltip("Speed of horizontal wave")]
        public float horizontalSpeed = 2f;
        
        [Tooltip("Speed of vertical wave")]
        public float verticalSpeed = 3f;
        
        [Tooltip("Use figure-8 pattern instead of simple wave")]
        public bool useFigure8Pattern = false;
        
        [Header("Bobbing Settings (Optional)")]
        [Tooltip("Enable gentle bobbing up and down")]
        public bool enableBobbing = true;
        
        [Tooltip("How much to bob up and down")]
        public float bobbingAmplitude = 0.1f;
        
        [Tooltip("Speed of bobbing")]
        public float bobbingSpeed = 1.5f;
        
        [Header("Debug")]
        [Tooltip("Show debug visualization")]
        public bool showDebugInfo = false;

        private Vector3 _startPosition;
        private float _elapsedTime = 0f;
        private bool _hasStarted = false;
        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
        }

        private void OnEnable()
        {
            // Reset when object is enabled (like when spawned)
            _hasStarted = false;
            _elapsedTime = 0f;
            _startTime = Time.time;
        }

        private void Update()
        {
            // Wait for start delay
            if (!_hasStarted)
            {
                if (Time.time - _startTime >= startDelay)
                {
                    _hasStarted = true;
                    _startPosition = transform.position;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"{gameObject.name}: Sinus movement started at position {_startPosition}");
                    }
                }
                else
                {
                    return;
                }
            }

            if (!enableMovement) return;

            _elapsedTime += Time.deltaTime;

            // Calculate sinus movement
            Vector3 offset = CalculateSinusOffset();
            
            // Apply movement
            transform.position = _startPosition + offset;
        }

        /// <summary>
        /// Calculates the sinus wave offset
        /// </summary>
        private Vector3 CalculateSinusOffset()
        {
            float xOffset, yOffset;
            
            if (useFigure8Pattern)
            {
                // Figure-8 pattern (Lissajous curve)
                xOffset = Mathf.Sin(_elapsedTime * horizontalSpeed) * horizontalAmplitude;
                yOffset = Mathf.Sin(_elapsedTime * verticalSpeed * 2f) * verticalAmplitude; // 2x speed for figure-8
            }
            else
            {
                // Simple wave pattern
                xOffset = Mathf.Sin(_elapsedTime * horizontalSpeed) * horizontalAmplitude;
                yOffset = Mathf.Sin(_elapsedTime * verticalSpeed) * verticalAmplitude;
            }
            
            // Add gentle bobbing if enabled
            if (enableBobbing)
            {
                float bobOffset = Mathf.Sin(_elapsedTime * bobbingSpeed) * bobbingAmplitude;
                yOffset += bobOffset;
            }
            
            return new Vector3(xOffset, yOffset, 0f);
        }

        /// <summary>
        /// Reset to start position
        /// </summary>
        [ContextMenu("Reset Position")]
        public void ResetPosition()
        {
            if (_hasStarted)
            {
                transform.position = _startPosition;
                _elapsedTime = 0f;
            }
        }

        /// <summary>
        /// Set the landed position (call this after drop animation completes)
        /// </summary>
        public void SetLandedPosition(Vector3 landedPosition)
        {
            _startPosition = landedPosition;
            _hasStarted = true;
            _elapsedTime = 0f;
            transform.position = landedPosition;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Landed position set to {landedPosition}, sinus movement starting");
            }
        }

        /// <summary>
        /// Enable/disable movement
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            enableMovement = enabled;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugInfo || !_hasStarted) return;

            // Draw movement path preview
            Gizmos.color = Color.yellow;
            
            // Draw center point
            Gizmos.DrawWireSphere(_startPosition, 0.05f);
            
            // Draw movement bounds
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_startPosition, new Vector3(horizontalAmplitude * 2f, verticalAmplitude * 2f, 0.1f));
        }
    }
}

