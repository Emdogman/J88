using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Controls enemy animations based on movement and attack states
    /// </summary>
    public class EnemyAnimationController : MonoBehaviour
    {
        [Header("Animation References")]
        [Tooltip("The animator component for the enemy")]
        public Animator enemyAnimator;
        
        [Tooltip("The enemy character component")]
        public Character enemyCharacter;
        
        [Header("Animation Parameters")]
        [Tooltip("Name of the Move parameter in the animator")]
        public string moveParameterName = "Move";
        
        [Tooltip("Name of the Attack trigger in the animator")]
        public string attackTriggerName = "Attack";
        
        [Tooltip("Name of the Idle animation state")]
        public string idleStateName = "Idle";
        
        [Tooltip("Name of the Enemy run animation state")]
        public string enemyRunStateName = "Enemy run";
        
        [Tooltip("Name of the Attack animation state")]
        public string attackStateName = "Attack";
        
        [Header("Movement Settings")]
        [Tooltip("Movement threshold to trigger run animation")]
        public float movementThreshold = 0.1f;
        
        [Tooltip("Duration of animation transitions")]
        public float transitionDuration = 0.1f;
        
        // Private variables
        private bool _isMoving = false;
        private int _moveParameterHash;
        private int _attackTriggerHash;
        private int _idleStateHash;
        private int _enemyRunStateHash;
        private int _attackStateHash;
        
        // Reference to enemy movement
        private ChaserEnemy _chaserEnemy;
        private Rigidbody2D _rigidbody;
        private Vector2 _lastPosition;
        private float _movementMagnitude;
        
        /// <summary>
        /// Initializes the enemy animation controller
        /// </summary>
        void Start()
        {
            InitializeAnimator();
            CacheAnimationHashes();
            FindEnemyComponents();
        }
        
        /// <summary>
        /// Updates the enemy animation each frame
        /// </summary>
        void Update()
        {
            if (enemyAnimator == null) return;
            
            UpdateMovementAnimation();
        }
        
        /// <summary>
        /// Initializes the enemy animator
        /// </summary>
        private void InitializeAnimator()
        {
            // Try to find the animator if not assigned
            if (enemyAnimator == null)
            {
                enemyAnimator = GetComponent<Animator>();
            }
            
            if (enemyAnimator == null)
            {
                Debug.LogError("EnemyAnimationController: No Animator found!");
                return;
            }
            
            Debug.Log("EnemyAnimationController: Successfully initialized with animator");
        }
        
        /// <summary>
        /// Caches animation parameter hashes for performance
        /// </summary>
        private void CacheAnimationHashes()
        {
            if (enemyAnimator == null) return;
            
            _moveParameterHash = Animator.StringToHash(moveParameterName);
            _attackTriggerHash = Animator.StringToHash(attackTriggerName);
            _idleStateHash = Animator.StringToHash(idleStateName);
            _enemyRunStateHash = Animator.StringToHash(enemyRunStateName);
            _attackStateHash = Animator.StringToHash(attackStateName);
        }
        
        /// <summary>
        /// Finds enemy components for movement detection
        /// </summary>
        private void FindEnemyComponents()
        {
            // Find ChaserEnemy component
            _chaserEnemy = GetComponent<ChaserEnemy>();
            if (_chaserEnemy == null)
            {
                Debug.LogWarning("EnemyAnimationController: No ChaserEnemy component found!");
            }
            
            // Find Rigidbody2D for movement detection
            _rigidbody = GetComponent<Rigidbody2D>();
            if (_rigidbody == null)
            {
                Debug.LogWarning("EnemyAnimationController: No Rigidbody2D found!");
            }
            
            // Find Character component
            if (enemyCharacter == null)
            {
                enemyCharacter = GetComponent<Character>();
            }
            
            // Initialize last position for movement detection
            _lastPosition = transform.position;
        }
        
        /// <summary>
        /// Updates the movement animation based on enemy movement
        /// </summary>
        private void UpdateMovementAnimation()
        {
            bool isCurrentlyMoving = false;
            
            // Method 1: Check Rigidbody2D velocity
            if (_rigidbody != null)
            {
                _movementMagnitude = _rigidbody.linearVelocity.magnitude;
                isCurrentlyMoving = _movementMagnitude > movementThreshold;
            }
            // Method 2: Check position change
            else
            {
                Vector2 currentPosition = transform.position;
                float distanceMoved = Vector2.Distance(currentPosition, _lastPosition);
                isCurrentlyMoving = distanceMoved > movementThreshold;
                _lastPosition = currentPosition;
            }
            
            // Method 3: Check ChaserEnemy movement state
            if (_chaserEnemy != null)
            {
                // Use reflection to access private movement field if available
                var movementField = typeof(ChaserEnemy).GetField("_movement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (movementField != null)
                {
                    Vector2 movement = (Vector2)movementField.GetValue(_chaserEnemy);
                    isCurrentlyMoving = movement.magnitude > movementThreshold;
                }
            }
            
            // Update animation if movement state changed
            if (isCurrentlyMoving != _isMoving)
            {
                _isMoving = isCurrentlyMoving;
                enemyAnimator.SetBool(_moveParameterHash, _isMoving);
                
                if (_isMoving)
                {
                    Debug.Log("EnemyAnimationController: Enemy started moving - Set Move parameter to true");
                }
                else
                {
                    Debug.Log("EnemyAnimationController: Enemy stopped moving - Set Move parameter to false");
                }
            }
        }
        
        /// <summary>
        /// Triggers the attack animation
        /// </summary>
        public void TriggerAttackAnimation()
        {
            if (enemyAnimator != null)
            {
                try
                {
                    enemyAnimator.SetTrigger(_attackTriggerHash);
                    Debug.Log("EnemyAnimationController: Triggered Attack animation");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"EnemyAnimationController: Could not trigger Attack: {e.Message}");
                }
            }
        }
        
        /// <summary>
        /// Gets the current animation state name
        /// </summary>
        /// <returns>Current animation state name</returns>
        public string GetCurrentAnimationState()
        {
            if (enemyAnimator == null) return "No Animator";
            
            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName(idleStateName))
                return idleStateName;
            else if (stateInfo.IsName(enemyRunStateName))
                return enemyRunStateName;
            else if (stateInfo.IsName(attackStateName))
                return attackStateName;
            else
                return "Unknown State";
        }
        
        /// <summary>
        /// Checks if the enemy is currently moving
        /// </summary>
        /// <returns>True if moving</returns>
        public bool IsMoving()
        {
            return _isMoving;
        }
        
        /// <summary>
        /// Checks if the enemy is currently in attack animation state
        /// </summary>
        /// <returns>True if in attack state</returns>
        public bool IsAttacking()
        {
            if (enemyAnimator == null) return false;
            
            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(attackStateName);
        }
        
        /// <summary>
        /// Manually triggers the enemy run animation
        /// </summary>
        public void TriggerEnemyRun()
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool(_moveParameterHash, true);
                _isMoving = true;
                Debug.Log("EnemyAnimationController: Manually triggered enemy run animation");
            }
        }
        
        /// <summary>
        /// Manually triggers the enemy idle animation
        /// </summary>
        public void TriggerEnemyIdle()
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool(_moveParameterHash, false);
                _isMoving = false;
                Debug.Log("EnemyAnimationController: Manually triggered enemy idle animation");
            }
        }
        
        /// <summary>
        /// Manually triggers the enemy attack animation
        /// </summary>
        public void TriggerEnemyAttack()
        {
            TriggerAttackAnimation();
        }
        
        /// <summary>
        /// Resets the enemy animation to idle
        /// </summary>
        public void ResetAnimation()
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool(_moveParameterHash, false);
                _isMoving = false;
                Debug.Log("EnemyAnimationController: Reset enemy animation to idle");
            }
        }
        
        /// <summary>
        /// Gets the current movement magnitude
        /// </summary>
        /// <returns>Current movement magnitude</returns>
        public float GetMovementMagnitude()
        {
            return _movementMagnitude;
        }
    }
}
