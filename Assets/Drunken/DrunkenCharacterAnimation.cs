using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Advanced drunken animation controller that integrates with TopDown Engine
    /// </summary>
    public class DrunkenCharacterAnimation : CharacterAbility
    {
        [Header("Drunken Animation Settings")]
        [Tooltip("The animator controller for drunken animations")]
        public RuntimeAnimatorController drunkenAnimatorController;
        
        [Tooltip("The animator controller for normal animations")]
        public RuntimeAnimatorController normalAnimatorController;
        
        [Tooltip("Minimum movement speed to trigger drunken animations")]
        public float drunkenMovementThreshold = 0.1f;
        
        [Tooltip("Animation transition duration")]
        public float animationTransitionDuration = 0.2f;
        
        [Tooltip("Drunken animation speed multiplier")]
        public float drunkenAnimationSpeed = 1.2f;
        
        [Header("Animation States")]
        [Tooltip("Name of the Move parameter in the animator")]
        public string moveParameterName = "Move";
        
        [Tooltip("Name of the Drunken parameter for special drunken states")]
        public string drunkenParameterName = "Drunken";
        
        [Tooltip("Name of the Speed parameter for animation speed")]
        public string speedParameterName = "Speed";
        
        // Private variables
        private Animator _koalaModelAnimator;
        private bool _isDrunkenMode = false;
        private bool _isMoving = false;
        private float _currentSpeed = 0f;
        
        // Animation parameter hashes
        private int _moveParameterHash;
        private int _drunkenParameterHash;
        private int _speedParameterHash;
        
        /// <summary>
        /// Initializes the drunken animation system
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            InitializeDrunkenAnimation();
        }
        
        /// <summary>
        /// Updates the animation system each frame
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            UpdateDrunkenAnimation();
        }
        
        /// <summary>
        /// Initializes the drunken animation system
        /// </summary>
        private void InitializeDrunkenAnimation()
        {
            // Find the koala model animator
            FindKoalaModelAnimator();
            
            if (_koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenCharacterAnimation: No Animator found on KoalaModel child!");
                return;
            }
            
            // Cache animation parameter hashes
            CacheAnimationHashes();
            
            // Set up initial animation state
            SetupInitialAnimationState();
            
            Debug.Log("DrunkenCharacterAnimation: Successfully initialized drunken animation system.");
        }
        
        /// <summary>
        /// Finds the koala model animator component
        /// </summary>
        private void FindKoalaModelAnimator()
        {
            // Look for a child named "KoalaModel" with an Animator component
            Transform koalaModel = _character.transform.Find("KoalaModel");
            if (koalaModel != null)
            {
                _koalaModelAnimator = koalaModel.GetComponent<Animator>();
            }
            
            // If still not found, try to find any Animator in children
            if (_koalaModelAnimator == null)
            {
                _koalaModelAnimator = _character.GetComponentInChildren<Animator>();
            }
        }
        
        /// <summary>
        /// Caches animation parameter hashes for performance
        /// </summary>
        private void CacheAnimationHashes()
        {
            if (_koalaModelAnimator == null) return;
            
            _moveParameterHash = Animator.StringToHash(moveParameterName);
            _drunkenParameterHash = Animator.StringToHash(drunkenParameterName);
            _speedParameterHash = Animator.StringToHash(speedParameterName);
        }
        
        /// <summary>
        /// Sets up the initial animation state
        /// </summary>
        private void SetupInitialAnimationState()
        {
            if (_koalaModelAnimator == null) return;
            
            // Set initial parameters
            _koalaModelAnimator.SetBool(_moveParameterHash, false);
            _koalaModelAnimator.SetBool(_drunkenParameterHash, _isDrunkenMode);
            _koalaModelAnimator.SetFloat(_speedParameterHash, 0f);
        }
        
        /// <summary>
        /// Updates the drunken animation based on character state
        /// </summary>
        private void UpdateDrunkenAnimation()
        {
            if (_koalaModelAnimator == null) return;
            
            // Check if character is moving
            bool isCurrentlyMoving = _controller.CurrentMovement.magnitude > drunkenMovementThreshold;
            
            // Update movement state
            if (isCurrentlyMoving != _isMoving)
            {
                _isMoving = isCurrentlyMoving;
                _koalaModelAnimator.SetBool(_moveParameterHash, _isMoving);
            }
            
            // Update speed parameter
            _currentSpeed = _controller.CurrentMovement.magnitude;
            _koalaModelAnimator.SetFloat(_speedParameterHash, _currentSpeed);
            
            // Update drunken mode
            _koalaModelAnimator.SetBool(_drunkenParameterHash, _isDrunkenMode);
        }
        
        /// <summary>
        /// Enables drunken animation mode
        /// </summary>
        public void EnableDrunkenMode()
        {
            _isDrunkenMode = true;
            
            if (_koalaModelAnimator != null && drunkenAnimatorController != null)
            {
                _koalaModelAnimator.runtimeAnimatorController = drunkenAnimatorController;
                _koalaModelAnimator.speed = drunkenAnimationSpeed;
            }
            
            Debug.Log("DrunkenCharacterAnimation: Enabled drunken mode");
        }
        
        /// <summary>
        /// Disables drunken animation mode
        /// </summary>
        public void DisableDrunkenMode()
        {
            _isDrunkenMode = false;
            
            if (_koalaModelAnimator != null && normalAnimatorController != null)
            {
                _koalaModelAnimator.runtimeAnimatorController = normalAnimatorController;
                _koalaModelAnimator.speed = 1.0f;
            }
            
            Debug.Log("DrunkenCharacterAnimation: Disabled drunken mode");
        }
        
        /// <summary>
        /// Toggles drunken animation mode
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
        /// Manually triggers the drunken run animation
        /// </summary>
        public void TriggerDrunkenRun()
        {
            if (_koalaModelAnimator == null) return;
            
            _isMoving = true;
            _koalaModelAnimator.SetBool(_moveParameterHash, true);
            Debug.Log("DrunkenCharacterAnimation: Manually triggered drunken run animation");
        }
        
        /// <summary>
        /// Manually triggers the idle animation
        /// </summary>
        public void TriggerIdle()
        {
            if (_koalaModelAnimator == null) return;
            
            _isMoving = false;
            _koalaModelAnimator.SetBool(_moveParameterHash, false);
            Debug.Log("DrunkenCharacterAnimation: Manually triggered idle animation");
        }
        
        /// <summary>
        /// Sets the animation speed multiplier
        /// </summary>
        /// <param name="speed">Speed multiplier</param>
        public void SetAnimationSpeed(float speed)
        {
            if (_koalaModelAnimator == null) return;
            
            _koalaModelAnimator.speed = speed;
            Debug.Log($"DrunkenCharacterAnimation: Set animation speed to {speed}");
        }
        
        /// <summary>
        /// Gets the current animation state information
        /// </summary>
        /// <returns>Current animation state info</returns>
        public AnimatorStateInfo GetCurrentAnimationState()
        {
            if (_koalaModelAnimator == null)
            {
                return new AnimatorStateInfo();
            }
            
            return _koalaModelAnimator.GetCurrentAnimatorStateInfo(0);
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
            if (_koalaModelAnimator == null) return;
            
            _isMoving = false;
            _isDrunkenMode = false;
            _koalaModelAnimator.SetBool(_moveParameterHash, false);
            _koalaModelAnimator.SetBool(_drunkenParameterHash, false);
            _koalaModelAnimator.SetFloat(_speedParameterHash, 0f);
            _koalaModelAnimator.speed = 1.0f;
            
            Debug.Log("DrunkenCharacterAnimation: Reset animation to default state");
        }
    }
}
