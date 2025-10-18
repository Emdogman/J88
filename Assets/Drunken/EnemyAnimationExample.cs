using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Example script showing how to use the enemy animation system
    /// </summary>
    public class EnemyAnimationExample : MonoBehaviour
    {
        [Header("Animation Controls")]
        [Tooltip("The enemy animation controller")]
        public EnemyAnimationController enemyAnimationController;
        
        [Tooltip("The enemy animation setup")]
        public EnemyAnimationSetup enemyAnimationSetup;
        
        [Header("Input Settings")]
        [Tooltip("Key to trigger enemy run")]
        public KeyCode triggerEnemyRunKey = KeyCode.R;
        
        [Tooltip("Key to trigger enemy idle")]
        public KeyCode triggerEnemyIdleKey = KeyCode.I;
        
        [Tooltip("Key to trigger enemy attack")]
        public KeyCode triggerEnemyAttackKey = KeyCode.A;
        
        [Tooltip("Key to reset animation")]
        public KeyCode resetAnimationKey = KeyCode.T;
        
        [Tooltip("Key to test animation setup")]
        public KeyCode testSetupKey = KeyCode.Y;
        
        /// <summary>
        /// Initializes the example
        /// </summary>
        void Start()
        {
            // Try to find the animation components if not assigned
            if (enemyAnimationController == null)
            {
                enemyAnimationController = GetComponent<EnemyAnimationController>();
            }
            
            if (enemyAnimationSetup == null)
            {
                enemyAnimationSetup = GetComponent<EnemyAnimationSetup>();
            }
            
            // Log current animation state
            LogCurrentAnimationState();
        }
        
        /// <summary>
        /// Updates the example each frame
        /// </summary>
        void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// Handles input for animation controls
        /// </summary>
        private void HandleInput()
        {
            // Trigger enemy run
            if (Input.GetKeyDown(triggerEnemyRunKey))
            {
                TriggerEnemyRun();
            }
            
            // Trigger enemy idle
            if (Input.GetKeyDown(triggerEnemyIdleKey))
            {
                TriggerEnemyIdle();
            }
            
            // Trigger enemy attack
            if (Input.GetKeyDown(triggerEnemyAttackKey))
            {
                TriggerEnemyAttack();
            }
            
            // Reset animation
            if (Input.GetKeyDown(resetAnimationKey))
            {
                ResetAnimation();
            }
            
            // Test animation setup
            if (Input.GetKeyDown(testSetupKey))
            {
                TestAnimationSetup();
            }
        }
        
        /// <summary>
        /// Triggers the enemy run animation
        /// </summary>
        public void TriggerEnemyRun()
        {
            if (enemyAnimationController != null)
            {
                enemyAnimationController.TriggerEnemyRun();
                Debug.Log("EnemyAnimationExample: Triggered enemy run animation");
            }
            else
            {
                Debug.LogWarning("EnemyAnimationExample: EnemyAnimationController not found!");
            }
        }
        
        /// <summary>
        /// Triggers the enemy idle animation
        /// </summary>
        public void TriggerEnemyIdle()
        {
            if (enemyAnimationController != null)
            {
                enemyAnimationController.TriggerEnemyIdle();
                Debug.Log("EnemyAnimationExample: Triggered enemy idle animation");
            }
            else
            {
                Debug.LogWarning("EnemyAnimationExample: EnemyAnimationController not found!");
            }
        }
        
        /// <summary>
        /// Triggers the enemy attack animation
        /// </summary>
        public void TriggerEnemyAttack()
        {
            if (enemyAnimationController != null)
            {
                enemyAnimationController.TriggerEnemyAttack();
                Debug.Log("EnemyAnimationExample: Triggered enemy attack animation");
            }
            else
            {
                Debug.LogWarning("EnemyAnimationExample: EnemyAnimationController not found!");
            }
        }
        
        /// <summary>
        /// Resets the animation to default state
        /// </summary>
        public void ResetAnimation()
        {
            if (enemyAnimationController != null)
            {
                enemyAnimationController.ResetAnimation();
                Debug.Log("EnemyAnimationExample: Reset enemy animation");
            }
            else
            {
                Debug.LogWarning("EnemyAnimationExample: EnemyAnimationController not found!");
            }
        }
        
        /// <summary>
        /// Tests the animation setup
        /// </summary>
        public void TestAnimationSetup()
        {
            if (enemyAnimationSetup != null)
            {
                enemyAnimationSetup.TestEnemyAnimation();
                Debug.Log("EnemyAnimationExample: Testing enemy animation setup");
            }
            else
            {
                Debug.LogWarning("EnemyAnimationExample: EnemyAnimationSetup not found!");
            }
        }
        
        /// <summary>
        /// Logs the current animation state
        /// </summary>
        public void LogCurrentAnimationState()
        {
            string animationState = "";
            
            if (enemyAnimationController != null)
            {
                animationState = $"Enemy: {enemyAnimationController.GetCurrentAnimationState()}, Moving: {enemyAnimationController.IsMoving()}, Attacking: {enemyAnimationController.IsAttacking()}, Movement: {enemyAnimationController.GetMovementMagnitude():F2}";
            }
            else
            {
                animationState = "Enemy: No animation controller found";
            }
            
            Debug.Log($"EnemyAnimationExample: {animationState}");
        }
        
        /// <summary>
        /// Gets the current animation state as a string
        /// </summary>
        /// <returns>Current animation state description</returns>
        public string GetAnimationStateDescription()
        {
            if (enemyAnimationController != null)
            {
                return $"Enemy: {enemyAnimationController.GetCurrentAnimationState()}, Moving: {enemyAnimationController.IsMoving()}, Attacking: {enemyAnimationController.IsAttacking()}, Movement: {enemyAnimationController.GetMovementMagnitude():F2}";
            }
            else
            {
                return "Enemy: No animation controller found";
            }
        }
        
        /// <summary>
        /// OnGUI for displaying current state and controls
        /// </summary>
        void OnGUI()
        {
            // Display current animation state
            GUI.Label(new Rect(10, 10, 800, 20), $"Animation State: {GetAnimationStateDescription()}");
            
            // Display controls
            GUI.Label(new Rect(10, 30, 300, 20), $"Controls:");
            GUI.Label(new Rect(10, 50, 300, 20), $"{triggerEnemyRunKey} - Trigger Enemy Run");
            GUI.Label(new Rect(10, 70, 300, 20), $"{triggerEnemyIdleKey} - Trigger Enemy Idle");
            GUI.Label(new Rect(10, 90, 300, 20), $"{triggerEnemyAttackKey} - Trigger Enemy Attack");
            GUI.Label(new Rect(10, 110, 300, 20), $"{resetAnimationKey} - Reset Animation");
            GUI.Label(new Rect(10, 130, 300, 20), $"{testSetupKey} - Test Animation Setup");
        }
    }
}
