using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Example script showing how to use the drunken animation system
    /// </summary>
    public class DrunkenAnimationExample : MonoBehaviour
    {
        [Header("Animation Controls")]
        [Tooltip("The drunken animation controller")]
        public DrunkenCharacterAnimation drunkenAnimation;
        
        [Tooltip("The basic animation controller")]
        public DrunkenAnimationController animationController;
        
        [Tooltip("The legs animation controller")]
        public DrunkenLegsAnimationController legsAnimationController;
        
        [Tooltip("The advanced legs animation controller")]
        public DrunkenLegsCharacterAnimation legsCharacterAnimation;
        
        [Header("Input Settings")]
        [Tooltip("Key to toggle drunken mode")]
        public KeyCode toggleDrunkenKey = KeyCode.D;
        
        [Tooltip("Key to trigger drunken run")]
        public KeyCode triggerDrunkenRunKey = KeyCode.R;
        
        [Tooltip("Key to trigger idle")]
        public KeyCode triggerIdleKey = KeyCode.I;
        
        [Tooltip("Key to reset animation")]
        public KeyCode resetAnimationKey = KeyCode.T;
        
        [Tooltip("Key to toggle legs animation sync")]
        public KeyCode toggleLegsSyncKey = KeyCode.L;
        
        [Header("Animation Settings")]
        [Tooltip("Animation speed multiplier")]
        [Range(0.1f, 3.0f)]
        public float animationSpeed = 1.0f;
        
        /// <summary>
        /// Initializes the example
        /// </summary>
        void Start()
        {
            // Try to find the animation components if not assigned
            if (drunkenAnimation == null)
            {
                drunkenAnimation = GetComponent<DrunkenCharacterAnimation>();
            }
            
            if (animationController == null)
            {
                animationController = GetComponent<DrunkenAnimationController>();
            }
            
            if (legsAnimationController == null)
            {
                legsAnimationController = GetComponent<DrunkenLegsAnimationController>();
            }
            
            if (legsCharacterAnimation == null)
            {
                legsCharacterAnimation = GetComponent<DrunkenLegsCharacterAnimation>();
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
            // Toggle drunken mode
            if (Input.GetKeyDown(toggleDrunkenKey))
            {
                ToggleDrunkenMode();
            }
            
            // Trigger drunken run
            if (Input.GetKeyDown(triggerDrunkenRunKey))
            {
                TriggerDrunkenRun();
            }
            
            // Trigger idle
            if (Input.GetKeyDown(triggerIdleKey))
            {
                TriggerIdle();
            }
            
            // Reset animation
            if (Input.GetKeyDown(resetAnimationKey))
            {
                ResetAnimation();
            }
            
            // Toggle legs animation sync
            if (Input.GetKeyDown(toggleLegsSyncKey))
            {
                ToggleLegsSync();
            }
        }
        
        /// <summary>
        /// Toggles drunken animation mode
        /// </summary>
        public void ToggleDrunkenMode()
        {
            if (drunkenAnimation != null)
            {
                drunkenAnimation.ToggleDrunkenMode();
                Debug.Log($"DrunkenAnimationExample: Toggled drunken mode. Current state: {drunkenAnimation.IsDrunkenMode()}");
            }
            else
            {
                Debug.LogWarning("DrunkenAnimationExample: No DrunkenCharacterAnimation component found!");
            }
        }
        
        /// <summary>
        /// Triggers the drunken run animation
        /// </summary>
        public void TriggerDrunkenRun()
        {
            if (drunkenAnimation != null)
            {
                drunkenAnimation.TriggerDrunkenRun();
                Debug.Log("DrunkenAnimationExample: Triggered drunken run animation");
            }
            else if (animationController != null)
            {
                animationController.TriggerDrunkenRun();
                Debug.Log("DrunkenAnimationExample: Triggered drunken run animation (basic controller)");
            }
            else
            {
                Debug.LogWarning("DrunkenAnimationExample: No animation controller found!");
            }
        }
        
        /// <summary>
        /// Triggers the idle animation
        /// </summary>
        public void TriggerIdle()
        {
            if (drunkenAnimation != null)
            {
                drunkenAnimation.TriggerIdle();
                Debug.Log("DrunkenAnimationExample: Triggered idle animation");
            }
            else if (animationController != null)
            {
                animationController.TriggerIdle();
                Debug.Log("DrunkenAnimationExample: Triggered idle animation (basic controller)");
            }
            else
            {
                Debug.LogWarning("DrunkenAnimationExample: No animation controller found!");
            }
        }
        
        /// <summary>
        /// Resets the animation to default state
        /// </summary>
        public void ResetAnimation()
        {
            if (drunkenAnimation != null)
            {
                drunkenAnimation.ResetAnimation();
                Debug.Log("DrunkenAnimationExample: Reset animation (advanced controller)");
            }
            else if (animationController != null)
            {
                animationController.ResetAnimation();
                Debug.Log("DrunkenAnimationExample: Reset animation (basic controller)");
            }
            else
            {
                Debug.LogWarning("DrunkenAnimationExample: No animation controller found!");
            }
        }
        
        /// <summary>
        /// Sets the animation speed
        /// </summary>
        /// <param name="speed">Animation speed multiplier</param>
        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
            
            if (drunkenAnimation != null)
            {
                drunkenAnimation.SetAnimationSpeed(speed);
                Debug.Log($"DrunkenAnimationExample: Set animation speed to {speed}");
            }
            else if (animationController != null)
            {
                animationController.SetAnimationSpeed(speed);
                Debug.Log($"DrunkenAnimationExample: Set animation speed to {speed} (basic controller)");
            }
        }
        
        /// <summary>
        /// Logs the current animation state
        /// </summary>
        public void LogCurrentAnimationState()
        {
            if (drunkenAnimation != null)
            {
                AnimatorStateInfo stateInfo = drunkenAnimation.GetCurrentAnimationState();
                Debug.Log($"DrunkenAnimationExample: Current animation state - Drunken: {drunkenAnimation.IsDrunkenMode()}, Moving: {drunkenAnimation.IsMoving()}, Speed: {stateInfo.speed}");
            }
            else if (animationController != null)
            {
                string currentState = animationController.GetCurrentAnimationState();
                Debug.Log($"DrunkenAnimationExample: Current animation state: {currentState}");
            }
        }
        
        /// <summary>
        /// Toggles legs animation sync
        /// </summary>
        public void ToggleLegsSync()
        {
            if (legsAnimationController != null)
            {
                bool currentSync = legsAnimationController.syncWithMainAnimation;
                legsAnimationController.SetSyncWithMainAnimation(!currentSync);
                Debug.Log($"DrunkenAnimationExample: Toggled legs sync to {!currentSync}");
            }
            else if (legsCharacterAnimation != null)
            {
                bool currentSync = legsCharacterAnimation.syncWithMainAnimation;
                legsCharacterAnimation.SetSyncWithMainAnimation(!currentSync);
                Debug.Log($"DrunkenAnimationExample: Toggled legs sync to {!currentSync}");
            }
            else
            {
                Debug.LogWarning("DrunkenAnimationExample: No legs animation controller found!");
            }
        }
        
        /// <summary>
        /// Gets the current animation state as a string
        /// </summary>
        /// <returns>Current animation state description</returns>
        public string GetAnimationStateDescription()
        {
            string mainState = "";
            string legsState = "";
            
            if (drunkenAnimation != null)
            {
                mainState = $"Drunken: {drunkenAnimation.IsDrunkenMode()}, Moving: {drunkenAnimation.IsMoving()}";
            }
            else if (animationController != null)
            {
                mainState = $"State: {animationController.GetCurrentAnimationState()}";
            }
            else
            {
                mainState = "No main animation controller found";
            }
            
            if (legsAnimationController != null)
            {
                legsState = $"Legs: {legsAnimationController.GetCurrentAnimationState()}, Moving: {legsAnimationController.IsMoving()}";
            }
            else if (legsCharacterAnimation != null)
            {
                legsState = $"Legs: Drunken: {legsCharacterAnimation.IsDrunkenMode()}, Moving: {legsCharacterAnimation.IsMoving()}";
            }
            else
            {
                legsState = "No legs animation controller found";
            }
            
            return $"{mainState} | {legsState}";
        }
        
        /// <summary>
        /// OnGUI for displaying current state
        /// </summary>
        void OnGUI()
        {
            // Display current animation state
            GUI.Label(new Rect(10, 10, 300, 20), $"Animation State: {GetAnimationStateDescription()}");
            
            // Display controls
            GUI.Label(new Rect(10, 30, 300, 20), $"Controls:");
            GUI.Label(new Rect(10, 50, 300, 20), $"{toggleDrunkenKey} - Toggle Drunken Mode");
            GUI.Label(new Rect(10, 70, 300, 20), $"{triggerDrunkenRunKey} - Trigger Drunken Run");
            GUI.Label(new Rect(10, 90, 300, 20), $"{triggerIdleKey} - Trigger Idle");
            GUI.Label(new Rect(10, 110, 300, 20), $"{resetAnimationKey} - Reset Animation");
            GUI.Label(new Rect(10, 130, 300, 20), $"{toggleLegsSyncKey} - Toggle Legs Sync");
        }
    }
}
