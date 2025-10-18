using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Controls the drunken animation states for the koala character
    /// </summary>
    public class DrunkenAnimationController : CharacterAbility
    {
        [Header("Animation References")]
        [Tooltip("The animator component on the koala model child")]
        public Animator koalaModelAnimator;
        
        // Character is inherited from CharacterAbility base class
        
        [Header("Animation Parameters")]
        [Tooltip("Name of the Move parameter in the animator")]
        public string moveParameterName = "Move";
        
        [Tooltip("Name of the Idle animation state")]
        public string idleStateName = "Idle";
        
        [Tooltip("Name of the Drunken run animation state")]
        public string drunkenRunStateName = "Drunken run";
        
        [Header("Animation Settings")]
        [Tooltip("Minimum movement speed to trigger drunken run animation")]
        public float movementThreshold = 0.1f;
        
        [Tooltip("Animation transition duration")]
        public float transitionDuration = 0.1f;
        
        // Private variables
        private bool _isMoving = false;
        private int _moveParameterHash;
        private int _idleStateHash;
        private int _drunkenRunStateHash;
        
        /// <summary>
        /// Initializes the animation controller
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            InitializeAnimator();
            CacheAnimationHashes();
        }
        
        /// <summary>
        /// Updates the animation state based on character movement
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (koalaModelAnimator == null) return;
            
            UpdateAnimationState();
        }
        
        /// <summary>
        /// Initializes the animator component
        /// </summary>
        private void InitializeAnimator()
        {
            // Try to find the koala model animator if not assigned
            if (koalaModelAnimator == null)
            {
                // Look for a child named "KoalaModel" with an Animator component
                Transform koalaModel = transform.Find("KoalaModel");
                if (koalaModel != null)
                {
                    koalaModelAnimator = koalaModel.GetComponent<Animator>();
                }
                
                // If still not found, try to find any Animator in children
                if (koalaModelAnimator == null)
                {
                    koalaModelAnimator = GetComponentInChildren<Animator>();
                }
            }
            
            // Character is already available through CharacterAbility
            
            // Validate components
            if (koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenAnimationController: No Animator found on KoalaModel child!");
                return;
            }
            
            // Character is available through CharacterAbility base class
            
            Debug.Log("DrunkenAnimationController: Successfully initialized with animator and character components.");
        }
        
        /// <summary>
        /// Caches animation parameter and state hashes for performance
        /// </summary>
        private void CacheAnimationHashes()
        {
            if (koalaModelAnimator == null) return;
            
            _moveParameterHash = Animator.StringToHash(moveParameterName);
            _idleStateHash = Animator.StringToHash(idleStateName);
            _drunkenRunStateHash = Animator.StringToHash(drunkenRunStateName);
        }
        
        /// <summary>
        /// Updates the animation state based on character movement
        /// </summary>
        private void UpdateAnimationState()
        {
            // Check if character is moving
            bool isCurrentlyMoving = _controller.CurrentMovement.magnitude > movementThreshold;
            
            // Only update if movement state has changed
            if (isCurrentlyMoving != _isMoving)
            {
                _isMoving = isCurrentlyMoving;
                UpdateAnimatorParameter();
            }
        }
        
        /// <summary>
        /// Updates the animator parameter based on current movement state
        /// </summary>
        private void UpdateAnimatorParameter()
        {
            if (koalaModelAnimator == null) return;
            
            // Set the Move parameter
            koalaModelAnimator.SetBool(_moveParameterHash, _isMoving);
            
            Debug.Log($"DrunkenAnimationController: Set Move parameter to {_isMoving}");
        }
        
        /// <summary>
        /// Manually triggers the drunken run animation
        /// </summary>
        public void TriggerDrunkenRun()
        {
            if (koalaModelAnimator == null) return;
            
            _isMoving = true;
            koalaModelAnimator.SetBool(_moveParameterHash, true);
            Debug.Log("DrunkenAnimationController: Manually triggered drunken run animation");
        }
        
        /// <summary>
        /// Manually triggers the idle animation
        /// </summary>
        public void TriggerIdle()
        {
            if (koalaModelAnimator == null) return;
            
            _isMoving = false;
            koalaModelAnimator.SetBool(_moveParameterHash, false);
            Debug.Log("DrunkenAnimationController: Manually triggered idle animation");
        }
        
        /// <summary>
        /// Gets the current animation state name
        /// </summary>
        /// <returns>Current animation state name</returns>
        public string GetCurrentAnimationState()
        {
            if (koalaModelAnimator == null) return "No Animator";
            
            AnimatorStateInfo stateInfo = koalaModelAnimator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName(idleStateName))
                return idleStateName;
            else if (stateInfo.IsName(drunkenRunStateName))
                return drunkenRunStateName;
            else
                return "Unknown State";
        }
        
        /// <summary>
        /// Checks if the character is currently in the drunken run state
        /// </summary>
        /// <returns>True if in drunken run state</returns>
        public bool IsInDrunkenRunState()
        {
            if (koalaModelAnimator == null) return false;
            
            AnimatorStateInfo stateInfo = koalaModelAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(drunkenRunStateName);
        }
        
        /// <summary>
        /// Sets the animation speed multiplier
        /// </summary>
        /// <param name="speed">Speed multiplier (1.0 = normal speed)</param>
        public void SetAnimationSpeed(float speed)
        {
            if (koalaModelAnimator == null) return;
            
            koalaModelAnimator.speed = speed;
            Debug.Log($"DrunkenAnimationController: Set animation speed to {speed}");
        }
        
        /// <summary>
        /// Checks if the character is currently moving
        /// </summary>
        /// <returns>True if moving</returns>
        public bool IsMoving()
        {
            return _isMoving;
        }
        
        /// <summary>
        /// Resets animation to default state
        /// </summary>
        public void ResetAnimation()
        {
            if (koalaModelAnimator == null) return;
            
            _isMoving = false;
            koalaModelAnimator.SetBool(_moveParameterHash, false);
            koalaModelAnimator.speed = 1.0f;
            Debug.Log("DrunkenAnimationController: Reset animation to idle state");
        }
    }
}
