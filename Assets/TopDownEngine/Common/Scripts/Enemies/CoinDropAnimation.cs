using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Animates a coin drop from enemy position to final position with scale bump
    /// </summary>
    public class CoinDropAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Duration of the lerp animation")]
        public float lerpDuration = 0.5f;
        
        [Tooltip("Scale multiplier at the peak of the bump (e.g., 1.5 = 150% size)")]
        public float scaleBumpMultiplier = 1.5f;
        
        [Tooltip("Animation curve for movement (optional)")]
        public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Tooltip("Animation curve for scale bump (optional)")]
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Vector3 _originalScale;
        private float _elapsedTime = 0f;
        private bool _isAnimating = false;
        
        /// <summary>
        /// Initialize and start the animation
        /// </summary>
        public void StartAnimation(Vector3 startPos, Vector3 targetPos)
        {
            _startPosition = startPos;
            _targetPosition = targetPos;
            _originalScale = transform.localScale;
            _elapsedTime = 0f;
            _isAnimating = true;
            
            // Start at the start position
            transform.position = _startPosition;
        }
        
        private void Update()
        {
            if (!_isAnimating) return;
            
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / lerpDuration);
            
            // Apply movement curve
            float movementT = movementCurve.Evaluate(t);
            
            // Lerp position
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, movementT);
            
            // Scale bump effect
            float scaleT = scaleCurve.Evaluate(t);
            float currentScaleMultiplier = 1f + (scaleBumpMultiplier - 1f) * scaleT;
            transform.localScale = _originalScale * currentScaleMultiplier;
            
            // Animation complete
            if (t >= 1f)
            {
                _isAnimating = false;
                transform.position = _targetPosition;
                transform.localScale = _originalScale;
                
                // Destroy this component after animation completes
                Destroy(this);
            }
        }
    }
}
