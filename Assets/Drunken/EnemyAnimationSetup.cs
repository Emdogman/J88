using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Setup script for enemy animations
    /// </summary>
    public class EnemyAnimationSetup : MonoBehaviour
    {
        [Header("Animation Controllers")]
        [Tooltip("The enemy animator controller")]
        public RuntimeAnimatorController enemyAnimatorController;
        
        [Tooltip("The enemy idle animation clip")]
        public AnimationClip enemyIdleClip;
        
        [Tooltip("The enemy run animation clip")]
        public AnimationClip enemyRunClip;
        
        [Tooltip("The enemy attack animation clip")]
        public AnimationClip enemyAttackClip;
        
        [Header("Setup Options")]
        [Tooltip("Automatically setup animations on start")]
        public bool autoSetupOnStart = true;
        
        [Tooltip("Apply enemy controller on start")]
        public bool applyEnemyControllerOnStart = true;
        
        // Private variables
        private Animator _enemyAnimator;
        private EnemyAnimationController _enemyAnimationController;
        private ChaserEnemy _chaserEnemy;
        
        /// <summary>
        /// Initializes the enemy animation setup
        /// </summary>
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupEnemyAnimations();
            }
        }
        
        /// <summary>
        /// Sets up the enemy animations
        /// </summary>
        [ContextMenu("Setup Enemy Animations")]
        public void SetupEnemyAnimations()
        {
            Debug.Log("EnemyAnimationSetup: Starting enemy animation setup...");
            
            // Find the enemy animator
            FindEnemyAnimator();
            
            if (_enemyAnimator == null)
            {
                Debug.LogError("EnemyAnimationSetup: No Animator found on enemy!");
                return;
            }
            
            // Apply the animator controller if specified
            if (applyEnemyControllerOnStart && enemyAnimatorController != null)
            {
                ApplyEnemyController();
            }
            
            // Add animation controller
            AddAnimationController();
            
            // Configure animation parameters
            ConfigureAnimationParameters();
            
            Debug.Log("EnemyAnimationSetup: Enemy animation setup completed successfully!");
        }
        
        /// <summary>
        /// Finds the enemy animator component
        /// </summary>
        private void FindEnemyAnimator()
        {
            // Find enemy animator
            _enemyAnimator = GetComponent<Animator>();
            if (_enemyAnimator == null)
            {
                _enemyAnimator = GetComponentInChildren<Animator>();
            }
            
            if (_enemyAnimator != null)
            {
                Debug.Log("EnemyAnimationSetup: Found enemy animator");
            }
            else
            {
                Debug.LogError("EnemyAnimationSetup: No Animator found on enemy!");
            }
        }
        
        /// <summary>
        /// Applies the enemy animator controller
        /// </summary>
        private void ApplyEnemyController()
        {
            if (_enemyAnimator != null && enemyAnimatorController != null)
            {
                _enemyAnimator.runtimeAnimatorController = enemyAnimatorController;
                Debug.Log("EnemyAnimationSetup: Applied enemy animator controller");
            }
        }
        
        /// <summary>
        /// Adds the enemy animation controller component
        /// </summary>
        private void AddAnimationController()
        {
            _enemyAnimationController = GetComponent<EnemyAnimationController>();
            if (_enemyAnimationController == null)
            {
                _enemyAnimationController = gameObject.AddComponent<EnemyAnimationController>();
                Debug.Log("EnemyAnimationSetup: Added EnemyAnimationController component");
            }
            else
            {
                Debug.Log("EnemyAnimationSetup: EnemyAnimationController already exists");
            }
        }
        
        /// <summary>
        /// Configures animation parameters
        /// </summary>
        private void ConfigureAnimationParameters()
        {
            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.enemyAnimator = _enemyAnimator;
                _enemyAnimationController.movementThreshold = 0.1f;
                _enemyAnimationController.transitionDuration = 0.1f;
                _enemyAnimationController.moveParameterName = "Move";
                _enemyAnimationController.attackTriggerName = "Attack";
                _enemyAnimationController.idleStateName = "Idle";
                _enemyAnimationController.enemyRunStateName = "Enemy run";
                _enemyAnimationController.attackStateName = "Attack";
                
                Debug.Log("EnemyAnimationSetup: Configured enemy animation parameters");
            }
        }
        
        /// <summary>
        /// Validates the enemy animation setup
        /// </summary>
        [ContextMenu("Validate Enemy Animation Setup")]
        public void ValidateSetup()
        {
            Debug.Log("EnemyAnimationSetup: Validating enemy animation setup...");
            
            bool isValid = true;
            
            // Check for enemy animator
            if (_enemyAnimator == null)
            {
                Debug.LogError("EnemyAnimationSetup: No enemy animator found!");
                isValid = false;
            }
            
            // Check for enemy animation controller
            if (_enemyAnimationController == null)
            {
                Debug.LogError("EnemyAnimationSetup: No EnemyAnimationController found!");
                isValid = false;
            }
            
            // Check for ChaserEnemy component
            _chaserEnemy = GetComponent<ChaserEnemy>();
            if (_chaserEnemy == null)
            {
                Debug.LogWarning("EnemyAnimationSetup: No ChaserEnemy component found!");
            }
            
            // Check for Rigidbody2D
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogWarning("EnemyAnimationSetup: No Rigidbody2D found!");
            }
            
            if (isValid)
            {
                Debug.Log("EnemyAnimationSetup: Enemy animation setup validation passed!");
            }
            else
            {
                Debug.LogError("EnemyAnimationSetup: Enemy animation setup validation failed!");
            }
        }
        
        /// <summary>
        /// Tests the enemy animation
        /// </summary>
        [ContextMenu("Test Enemy Animation")]
        public void TestEnemyAnimation()
        {
            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.TriggerEnemyRun();
                Debug.Log("EnemyAnimationSetup: Triggered enemy run animation for testing");
                
                // Auto-reset after 2 seconds
                StartCoroutine(ResetTestAfterDelay());
            }
            else
            {
                Debug.LogError("EnemyAnimationSetup: No EnemyAnimationController found! Run setup first.");
            }
        }
        
        private System.Collections.IEnumerator ResetTestAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.TriggerEnemyIdle();
                Debug.Log("EnemyAnimationSetup: Reset enemy animation to idle");
            }
        }
        
        /// <summary>
        /// Gets the current animation state
        /// </summary>
        /// <returns>Current animation state description</returns>
        public string GetAnimationState()
        {
            if (_enemyAnimationController != null)
            {
                return _enemyAnimationController.GetCurrentAnimationState();
            }
            return "No Animation Controller";
        }
    }
}
