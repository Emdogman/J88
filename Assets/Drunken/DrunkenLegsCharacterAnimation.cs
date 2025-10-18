using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Advanced drunken legs animation controller that integrates with TopDown Engine
    /// </summary>
    public class DrunkenLegsCharacterAnimation : CharacterAbility
    {
        [Header("Drunken Legs Animation Settings")]
        [Tooltip("The animator controller for drunken legs animations")]
        public RuntimeAnimatorController drunkenLegsAnimatorController;
        
        [Tooltip("The animator controller for normal legs animations")]
        public RuntimeAnimatorController normalLegsAnimatorController;
        
        [Tooltip("Minimum movement speed to trigger drunken legs animations")]
        public float drunkenMovementThreshold = 0.1f;
        
        [Tooltip("Animation transition duration")]
        public float animationTransitionDuration = 0.2f;
        
        [Tooltip("Drunken legs animation speed multiplier")]
        public float drunkenAnimationSpeed = 1.2f;
        
        [Header("Animation States")]
        [Tooltip("Name of the Move parameter in the animator")]
        public string moveParameterName = "Move";
        
        [Tooltip("Name of the Drunken parameter for special drunken states")]
        public string drunkenParameterName = "Drunken";
        
        [Tooltip("Name of the Speed parameter for animation speed")]
        public string speedParameterName = "Speed";
        
        [Header("Sync Settings")]
        [Tooltip("Sync with main character animation")]
        public bool syncWithMainAnimation = true;
        
        [Tooltip("Sync animation speed with main animation")]
        public bool syncAnimationSpeed = true;
        
        // Private variables
        private Animator _legsAnimator;
        private bool _isDrunkenMode = false;
        private bool _isMoving = false;
        private float _currentSpeed = 0f;
        
        // Animation parameter hashes
        private int _moveParameterHash;
        private int _drunkenParameterHash;
        private int _speedParameterHash;
        
        // Reference to main animation controllers
        private DrunkenCharacterAnimation _mainCharacterAnimation;
        private DrunkenAnimationController _mainAnimationController;
        
        /// <summary>
        /// Initializes the drunken legs animation system
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            InitializeDrunkenLegsAnimation();
        }
        
        /// <summary>
        /// Updates the legs animation system each frame
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            UpdateDrunkenLegsAnimation();
        }
        
        /// <summary>
        /// Initializes the drunken legs animation system
        /// </summary>
        private void InitializeDrunkenLegsAnimation()
        {
            // Find the legs animator
            FindLegsAnimator();
            
            if (_legsAnimator == null)
            {
                Debug.LogError("DrunkenLegsCharacterAnimation: No Animator found on Legs child!");
                return;
            }
            
            // Cache animation parameter hashes
            CacheAnimationHashes();
            
            // Set up initial animation state
            SetupInitialAnimationState();
            
            // Find main animation controllers for syncing
            FindMainAnimationControllers();
            
            Debug.Log("DrunkenLegsCharacterAnimation: Successfully initialized drunken legs animation system.");
        }
        
        /// <summary>
        /// Finds the legs animator component
        /// </summary>
        private void FindLegsAnimator()
        {
            // Look for a child named "Legs" with an Animator component
            Transform legs = _character.transform.Find("Legs");
            if (legs != null)
            {
                _legsAnimator = legs.GetComponent<Animator>();
            }
            
            // If still not found, try to find any Animator in children
            if (_legsAnimator == null)
            {
                _legsAnimator = _character.GetComponentInChildren<Animator>();
            }
        }
        
        /// <summary>
        /// Finds the main animation controllers for syncing
        /// </summary>
        private void FindMainAnimationControllers()
        {
            _mainCharacterAnimation = _character.GetComponent<DrunkenCharacterAnimation>();
            _mainAnimationController = _character.GetComponent<DrunkenAnimationController>();
        }
        
        /// <summary>
        /// Caches animation parameter hashes for performance
        /// </summary>
        private void CacheAnimationHashes()
        {
            if (_legsAnimator == null) return;
            
            _moveParameterHash = Animator.StringToHash(moveParameterName);
            _drunkenParameterHash = Animator.StringToHash(drunkenParameterName);
            _speedParameterHash = Animator.StringToHash(speedParameterName);
        }
        
        /// <summary>
        /// Sets up the initial animation state
        /// </summary>
        private void SetupInitialAnimationState()
        {
            if (_legsAnimator == null) return;
            
            // Set initial parameters
            _legsAnimator.SetBool(_moveParameterHash, false);
            _legsAnimator.SetBool(_drunkenParameterHash, _isDrunkenMode);
            _legsAnimator.SetFloat(_speedParameterHash, 0f);
        }
        
        /// <summary>
        /// Updates the drunken legs animation based on character state
        /// </summary>
        private void UpdateDrunkenLegsAnimation()
        {
            if (_legsAnimator == null) return;
            
            bool isCurrentlyMoving = false;
            float currentSpeed = 0f;
            bool isDrunkenMode = false;
            
            if (syncWithMainAnimation)
            {
                // Sync with main animation controllers
                if (_mainCharacterAnimation != null)
                {
                    isCurrentlyMoving = _mainCharacterAnimation.IsMoving();
                    isDrunkenMode = _mainCharacterAnimation.IsDrunkenMode();
                    currentSpeed = _mainCharacterAnimation.GetCurrentAnimationState().speed;
                }
                else if (_mainAnimationController != null)
                {
                    isCurrentlyMoving = _mainAnimationController.IsMoving();
                    currentSpeed = _controller.CurrentMovement.magnitude;
                }
                else
                {
                    // Fallback to direct movement detection
                    isCurrentlyMoving = _controller.CurrentMovement.magnitude > drunkenMovementThreshold;
                    currentSpeed = _controller.CurrentMovement.magnitude;
                }
            }
            else
            {
                // Direct movement detection
                isCurrentlyMoving = _controller.CurrentMovement.magnitude > drunkenMovementThreshold;
                currentSpeed = _controller.CurrentMovement.magnitude;
            }
            
            // Update movement state
            if (isCurrentlyMoving != _isMoving)
            {
                _isMoving = isCurrentlyMoving;
                _legsAnimator.SetBool(_moveParameterHash, _isMoving);
            }
            
            // Update speed parameter
            if (syncAnimationSpeed)
            {
                _currentSpeed = currentSpeed;
            }
            else
            {
                _currentSpeed = _controller.CurrentMovement.magnitude;
            }
            _legsAnimator.SetFloat(_speedParameterHash, _currentSpeed);
            
            // Update drunken mode
            _legsAnimator.SetBool(_drunkenParameterHash, isDrunkenMode);
        }
        
        /// <summary>
        /// Enables drunken legs animation mode
        /// </summary>
        public void EnableDrunkenMode()
        {
            _isDrunkenMode = true;
            
            if (_legsAnimator != null && drunkenLegsAnimatorController != null)
            {
                _legsAnimator.runtimeAnimatorController = drunkenLegsAnimatorController;
                _legsAnimator.speed = drunkenAnimationSpeed;
            }
            
            Debug.Log("DrunkenLegsCharacterAnimation: Enabled drunken legs mode");
        }
        
        /// <summary>
        /// Disables drunken legs animation mode
        /// </summary>
        public void DisableDrunkenMode()
        {
            _isDrunkenMode = false;
            
            if (_legsAnimator != null && normalLegsAnimatorController != null)
            {
                _legsAnimator.runtimeAnimatorController = normalLegsAnimatorController;
                _legsAnimator.speed = 1.0f;
            }
            
            Debug.Log("DrunkenLegsCharacterAnimation: Disabled drunken legs mode");
        }
        
        /// <summary>
        /// Toggles drunken legs animation mode
        /// </summary>
        public void ToggleDrunkenMode()
        {
            if (_isDrunkenMode)
            {
                DisableDrunkenMode();
            }
            else
            {
                EnableDrunkenMode();
            }
        }
        
        /// <summary>
        /// Manually triggers the drunken feet animation
        /// </summary>
        public void TriggerDrunkenFeet()
        {
            if (_legsAnimator == null) return;
            
            _isMoving = true;
            _legsAnimator.SetBool(_moveParameterHash, true);
            Debug.Log("DrunkenLegsCharacterAnimation: Manually triggered drunken feet animation");
        }
        
        /// <summary>
        /// Manually triggers the idle animation
        /// </summary>
        public void TriggerIdle()
        {
            if (_legsAnimator == null) return;
            
            _isMoving = false;
            _legsAnimator.SetBool(_moveParameterHash, false);
            Debug.Log("DrunkenLegsCharacterAnimation: Manually triggered idle animation");
        }
        
        /// <summary>
        /// Sets the animation speed
        /// </summary>
        /// <param name="speed">Speed multiplier</param>
        public void SetAnimationSpeed(float speed)
        {
            if (_legsAnimator == null) return;
            
            _legsAnimator.speed = speed;
            Debug.Log($"DrunkenLegsCharacterAnimation: Set animation speed to {speed}");
        }
        
        /// <summary>
        /// Gets the current animation state information
        /// </summary>
        /// <returns>Current animation state info</returns>
        public AnimatorStateInfo GetCurrentAnimationState()
        {
            if (_legsAnimator == null)
            {
                return new AnimatorStateInfo();
            }
            
            return _legsAnimator.GetCurrentAnimatorStateInfo(0);
        }
        
        /// <summary>
        /// Checks if the character is currently in drunken mode
        /// </summary>
        /// <returns>True if in drunken mode</returns>
        public bool IsDrunkenMode()
        {
            return _isDrunkenMode;
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
        /// Resets the animation system to default state
        /// </summary>
        public void ResetAnimation()
        {
            if (_legsAnimator == null) return;
            
            _isMoving = false;
            _isDrunkenMode = false;
            _legsAnimator.SetBool(_moveParameterHash, false);
            _legsAnimator.SetBool(_drunkenParameterHash, false);
            _legsAnimator.SetFloat(_speedParameterHash, 0f);
            _legsAnimator.speed = 1.0f;
            
            Debug.Log("DrunkenLegsCharacterAnimation: Reset animation to default state");
        }
        
        /// <summary>
        /// Sets whether to sync with main animation controllers
        /// </summary>
        /// <param name="sync">Whether to sync with main animation</param>
        public void SetSyncWithMainAnimation(bool sync)
        {
            syncWithMainAnimation = sync;
            Debug.Log($"DrunkenLegsCharacterAnimation: Set sync with main animation to {sync}");
        }
        
        /// <summary>
        /// Sets whether to sync animation speed with main animation
        /// </summary>
        /// <param name="sync">Whether to sync animation speed</param>
        public void SetSyncAnimationSpeed(bool sync)
        {
            syncAnimationSpeed = sync;
            Debug.Log($"DrunkenLegsCharacterAnimation: Set sync animation speed to {sync}");
        }
    }
}
