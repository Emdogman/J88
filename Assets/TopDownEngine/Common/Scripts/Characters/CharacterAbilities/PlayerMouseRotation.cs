using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Makes the player character rotate to face the mouse cursor position
    /// Works alongside TopDownEngine's existing weapon aim system
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Player Mouse Rotation")]
    public class PlayerMouseRotation : CharacterAbility
    {
        [Header("Rotation Settings")]
        [Tooltip("Whether to rotate the entire character or just the sprite")]
        public bool rotateEntireCharacter = true;
        
        [Tooltip("Speed of rotation (0 = instant, higher = smoother)")]
        [Range(0f, 20f)]
        public float rotationSpeed = 15f;
        
        [Tooltip("Minimum angle difference to trigger rotation (prevents micro-jitter)")]
        [Range(0f, 10f)]
        public float rotationDeadZone = 2f;
        
        [Tooltip("Rotation offset to adjust sprite facing direction (0° = right, 90° = up, -90° = down, 180° = left)")]
        [Range(-180f, 180f)]
        public float rotationOffset = 0f;
        
        [Tooltip("Use sprite flipping instead of rotation (for 2D sprites)")]
        public bool useFlipping = false;
        
        [Header("Debug")]
        [Tooltip("Show debug line from character to mouse")]
        public bool showDebugLine = false;
        
        [Tooltip("Show debug information in console")]
        public bool showDebugInfo = false;

        // Private variables
        private Camera _camera;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _lastMousePosition;
        private bool _isMouseMoving = false;

        protected override void Initialization()
        {
            base.Initialization();
            
            // Get camera reference
            _camera = Camera.main;
            if (_camera == null)
            {
                _camera = FindObjectOfType<Camera>();
            }
            
            if (_camera == null)
            {
                Debug.LogError("PlayerMouseRotation: No camera found! Make sure there's a camera in the scene.");
                return;
            }
            
            // Get sprite renderer if not rotating entire character
            if (!rotateEntireCharacter)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                }
                
                if (_spriteRenderer == null)
                {
                    Debug.LogWarning("PlayerMouseRotation: No SpriteRenderer found. Falling back to rotating entire character.");
                    rotateEntireCharacter = true;
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"PlayerMouseRotation: Initialized - Rotate Entire Character: {rotateEntireCharacter}, Use Flipping: {useFlipping}");
            }
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            
            if (!AbilityAuthorized || _camera == null) return;
            
            // Only track mouse movement for debug purposes
            // Rotation is now handled in LateUpdate to sync with physics
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            _isMouseMoving = Vector3.Distance(mouseWorldPosition, _lastMousePosition) > 0.1f;
            _lastMousePosition = mouseWorldPosition;
            
            if (showDebugInfo && _isMouseMoving)
            {
                float targetAngle = CalculateRotationAngle(mouseWorldPosition);
                Debug.Log($"PlayerMouseRotation: Mouse at {mouseWorldPosition}, Target Angle: {targetAngle:F1}°");
            }
        }
        
        /// <summary>
        /// Apply rotation in FixedUpdate to fully sync with physics and eliminate jitter
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (!AbilityAuthorized || _camera == null) return;
            
            // Get mouse position and apply rotation in sync with physics
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            float targetAngle = CalculateRotationAngle(mouseWorldPosition);
            ApplyRotation(targetAngle);
        }

        /// <summary>
        /// Get mouse position in world space
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = _camera.transform.position.z - transform.position.z;
            return _camera.ScreenToWorldPoint(mouseScreenPos);
        }

        /// <summary>
        /// Calculate rotation angle to face mouse position
        /// </summary>
        private float CalculateRotationAngle(Vector3 mouseWorldPosition)
        {
            Vector2 direction = (mouseWorldPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Apply rotation offset
            angle += rotationOffset;
            
            return angle;
        }

        /// <summary>
        /// Apply rotation to character or sprite
        /// </summary>
        private void ApplyRotation(float targetAngle)
        {
            if (useFlipping)
            {
                ApplySpriteFlipping(targetAngle);
            }
            else if (rotateEntireCharacter)
            {
                ApplyCharacterRotation(targetAngle);
            }
            else
            {
                ApplySpriteRotation(targetAngle);
            }
        }

        /// <summary>
        /// Rotate entire character GameObject
        /// </summary>
        private void ApplyCharacterRotation(float targetAngle)
        {
            // Check dead zone to prevent micro-jitter
            float currentAngle = transform.eulerAngles.z;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
            
            if (angleDifference < rotationDeadZone)
            {
                return; // Don't rotate if within dead zone
            }
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            
            if (rotationSpeed <= 0f)
            {
                // Instant rotation - always update to ensure continuous tracking
                transform.rotation = targetRotation;
            }
            else
            {
                // Smooth rotation with fixed delta time for consistency
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Rotate only the sprite renderer
        /// </summary>
        private void ApplySpriteRotation(float targetAngle)
        {
            if (_spriteRenderer == null) return;
            
            // Check dead zone to prevent micro-jitter
            float currentAngle = _spriteRenderer.transform.eulerAngles.z;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
            
            if (angleDifference < rotationDeadZone)
            {
                return; // Don't rotate if within dead zone
            }
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            
            if (rotationSpeed <= 0f)
            {
                // Instant rotation - always update to ensure continuous tracking
                _spriteRenderer.transform.rotation = targetRotation;
            }
            else
            {
                // Smooth rotation with fixed delta time for consistency
                _spriteRenderer.transform.rotation = Quaternion.Lerp(_spriteRenderer.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Flip sprite horizontally instead of rotating
        /// </summary>
        private void ApplySpriteFlipping(float targetAngle)
        {
            if (_spriteRenderer == null) return;
            
            // Determine if mouse is to the left or right of character
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            bool mouseIsLeft = mouseWorldPosition.x < transform.position.x;
            
            _spriteRenderer.flipX = mouseIsLeft;
        }

        /// <summary>
        /// Test rotation to a specific angle (for debugging)
        /// </summary>
        [ContextMenu("Test Rotation 0°")]
        public void TestRotation0()
        {
            ApplyRotation(0f);
        }

        /// <summary>
        /// Test rotation to a specific angle (for debugging)
        /// </summary>
        [ContextMenu("Test Rotation 90°")]
        public void TestRotation90()
        {
            ApplyRotation(90f);
        }

        /// <summary>
        /// Test rotation to a specific angle (for debugging)
        /// </summary>
        [ContextMenu("Test Rotation 180°")]
        public void TestRotation180()
        {
            ApplyRotation(180f);
        }

        /// <summary>
        /// Test rotation to a specific angle (for debugging)
        /// </summary>
        [ContextMenu("Test Rotation -90°")]
        public void TestRotationMinus90()
        {
            ApplyRotation(-90f);
        }

        /// <summary>
        /// Reset rotation to default
        /// </summary>
        [ContextMenu("Reset Rotation")]
        public void ResetRotation()
        {
            if (rotateEntireCharacter)
            {
                transform.rotation = Quaternion.identity;
            }
            else if (_spriteRenderer != null)
            {
                _spriteRenderer.transform.rotation = Quaternion.identity;
                _spriteRenderer.flipX = false;
            }
        }

        /// <summary>
        /// Show debug information
        /// </summary>
        [ContextMenu("Show Debug Info")]
        public void ShowDebugInfo()
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            float angle = CalculateRotationAngle(mouseWorldPos);
            
            Debug.Log($"PlayerMouseRotation Debug Info:");
            Debug.Log($"  - Mouse World Position: {mouseWorldPos}");
            Debug.Log($"  - Character Position: {transform.position}");
            Debug.Log($"  - Target Angle: {angle:F1}°");
            Debug.Log($"  - Current Rotation: {transform.eulerAngles.z:F1}°");
            Debug.Log($"  - Rotate Entire Character: {rotateEntireCharacter}");
            Debug.Log($"  - Use Flipping: {useFlipping}");
            Debug.Log($"  - Rotation Speed: {rotationSpeed}");
            Debug.Log($"  - Rotation Offset: {rotationOffset}");
        }

        private void OnDrawGizmos()
        {
            if (!showDebugLine || !Application.isPlaying) return;
            
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            
            // Draw line from character to mouse
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, mouseWorldPos);
            
            // Draw mouse position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(mouseWorldPos, 0.2f);
        }

    }
}
