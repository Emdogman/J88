using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Controls the drunken feet animation for the koala character's legs
    /// </summary>
    public class DrunkenLegsAnimationController : CharacterAbility
    {
        [Header("Animation References")]
        [Tooltip("The animator component on the legs child")]
        public Animator legsAnimator;
        
        [Header("Animation Parameters")]
        [Tooltip("Name of the Move parameter in the animator")]
        public string moveParameterName = "Move";
        
        [Tooltip("Name of the Idle animation state")]
        public string idleStateName = "New State";
        
        [Tooltip("Name of the Drunken feet animation state")]
        public string drunkenFeetStateName = "Drunken feet";
        
        [Header("Animation Settings")]
        [Tooltip("Minimum movement speed to trigger drunken feet animation")]
        public float movementThreshold = 0.1f;
        
        [Tooltip("Animation transition duration")]
        public float transitionDuration = 0.1f;
        
        [Tooltip("Sync with main character animation")]
        public bool syncWithMainAnimation = true;
        
        // Private variables
        private bool _isMoving = false;
        private int _moveParameterHash;
        private int _idleStateHash;
        private int _drunkenFeetStateHash;
        
        // Reference to main animation controller for syncing
        private DrunkenAnimationController _mainAnimationController;
        private DrunkenCharacterAnimation _mainCharacterAnimation;
        
        /// <summary>
        /// Initializes the legs animation controller
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            InitializeAnimator();
            CacheAnimationHashes();
            FindMainAnimationControllers();
        }
        
        /// <summary>
        /// Updates the legs animation state based on character movement
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (legsAnimator == null) return;
            
            UpdateAnimationState();
        }
        
        /// <summary>
        /// Initializes the legs animator component
        /// </summary>
        private void InitializeAnimator()
        {
            // Try to find the legs animator if not assigned
            if (legsAnimator == null)
            {
                // Look for a child named "Legs" with an Animator component
                Transform legs = _character.transform.Find("Legs");
                if (legs != null)
                {
                    legsAnimator = legs.GetComponent<Animator>();
                }
                
                // If still not found, try to find any Animator in children
                if (legsAnimator == null)
                {
                    legsAnimator = GetComponentInChildren<Animator>();
                }
            }
            
            // Validate components
            if (legsAnimator == null)
            {
                Debug.LogError("DrunkenLegsAnimationController: No Animator found on Legs child!");
                return;
            }
            
            Debug.Log("DrunkenLegsAnimationController: Successfully initialized with legs animator.");
        }
        
        /// <summary>
        /// Finds the main animation controllers for syncing
        /// </summary>
        private void FindMainAnimationControllers()
        {
            _mainAnimationController = _character.GetComponent<DrunkenAnimationController>();
            _mainCharacterAnimation = _character.GetComponent<DrunkenCharacterAnimation>();
            
            if (_mainAnimationController == null && _mainCharacterAnimation == null)
            {
                Debug.LogWarning("DrunkenLegsAnimationController: No main animation controllers found for syncing.");
            }
        }
        
        /// <summary>
        /// Caches animation parameter hashes for performance
        /// </summary>
        private void CacheAnimationHashes()
        {
            if (legsAnimator == null) return;
            
            _moveParameterHash = Animator.StringToHash(moveParameterName);
            _idleStateHash = Animator.StringToHash(idleStateName);
            _drunkenFeetStateHash = Animator.StringToHash(drunkenFeetStateName);
        }
        
        /// <summary>
        /// Updates the legs animation state based on character movement
        /// </summary>
        private void UpdateAnimationState()
        {
            bool isCurrentlyMoving = false;
            
            if (syncWithMainAnimation)
            {
                // Sync with main animation controllers
                if (_mainAnimationController != null)
                {
                    isCurrentlyMoving = _mainAnimationController.IsMoving();
                }
                else if (_mainCharacterAnimation != null)
                {
                    isCurrentlyMoving = _mainCharacterAnimation.IsMoving();
                }
                else
                {
                    // Fallback to direct movement detection
                    isCurrentlyMoving = _controller.CurrentMovement.magnitude > movementThreshold;
                }
            }
            else
            {
                // Direct movement detection
                isCurrentlyMoving = _controller.CurrentMovement.magnitude > movementThreshold;
            }
            
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
            if (legsAnimator == null) return;
            
            // Set the Move parameter
            legsAnimator.SetBool(_moveParameterHash, _isMoving);
            
            Debug.Log($"DrunkenLegsAnimationController: Set Move parameter to {_isMoving}");
        }
        
        /// <summary>
        /// Manually triggers the drunken feet animation
        /// </summary>
        public void TriggerDrunkenFeet()
        {
            if (legsAnimator == null) return;
            
            _isMoving = true;
            legsAnimator.SetBool(_moveParameterHash, true);
            Debug.Log("DrunkenLegsAnimationController: Manually triggered drunken feet animation");
        }
        
        /// <summary>
        /// Manually triggers the idle animation
        /// </summary>
        public void TriggerIdle()
        {
            if (legsAnimator == null) return;
            
            _isMoving = false;
            legsAnimator.SetBool(_moveParameterHash, false);
            Debug.Log("DrunkenLegsAnimationController: Manually triggered idle animation");
        }
        
        /// <summary>
        /// Gets the current animation state name
        /// </summary>
        /// <returns>Current animation state name</returns>
        public string GetCurrentAnimationState()
        {
            if (legsAnimator == null) return "No Animator";
            
            AnimatorStateInfo stateInfo = legsAnimator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName(idleStateName))
                return idleStateName;
            else if (stateInfo.IsName(drunkenFeetStateName))
                return drunkenFeetStateName;
            else
                return "Unknown State";
        }
        
        /// <summary>
        /// Checks if the character is currently in the drunken feet state
        /// </summary>
        /// <returns>True if in drunken feet state</returns>
        public bool IsInDrunkenFeetState()
        {
            if (legsAnimator == null) return false;
            
            AnimatorStateInfo stateInfo = legsAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(drunkenFeetStateName);
        }
        
        /// <summary>
        /// Sets the animation speed multiplier
        /// </summary>
        /// <param name="speed">Speed multiplier (1.0 = normal speed)</param>
        public void SetAnimationSpeed(float speed)
        {
            if (legsAnimator == null) return;
            
            legsAnimator.speed = speed;
            Debug.Log($"DrunkenLegsAnimationController: Set animation speed to {speed}");
        }
        
        /// <summary>
        /// Resets animation to default state
        /// </summary>
        public void ResetAnimation()
        {
            if (legsAnimator == null) return;
            
            _isMoving = false;
            legsAnimator.SetBool(_moveParameterHash, false);
            legsAnimator.speed = 1.0f;
            Debug.Log("DrunkenLegsAnimationController: Reset animation to idle state");
        }
        
        /// <summary>
        /// Checks if the legs are currently moving
        /// </summary>
        /// <returns>True if moving</returns>
        public bool IsMoving()
        {
            return _isMoving;
        }
        
        /// <summary>
        /// Sets whether to sync with main animation controllers
        /// </summary>
        /// <param name="sync">Whether to sync with main animation</param>
        public void SetSyncWithMainAnimation(bool sync)
        {
            syncWithMainAnimation = sync;
            Debug.Log($"DrunkenLegsAnimationController: Set sync with main animation to {sync}");
        }
    }
}
