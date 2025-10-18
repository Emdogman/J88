using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Setup script for configuring drunken legs animations on the koala character
    /// </summary>
    public class DrunkenLegsAnimationSetup : MonoBehaviour
    {
        [Header("Animation Controllers")]
        [Tooltip("The drunken animator controller for the main model")]
        public RuntimeAnimatorController drunkenAnimatorController;
        
        [Tooltip("The normal animator controller (optional)")]
        public RuntimeAnimatorController normalAnimatorController;
        
        [Tooltip("The drunken legs animator controller")]
        public RuntimeAnimatorController drunkenLegsAnimatorController;
        
        [Tooltip("The normal legs animator controller (optional)")]
        public RuntimeAnimatorController normalLegsAnimatorController;
        
        [Header("Animation Clips")]
        [Tooltip("The idle animation clip")]
        public AnimationClip idleClip;
        
        [Tooltip("The drunken run animation clip")]
        public AnimationClip drunkenRunClip;
        
        [Tooltip("The drunken feet animation clip")]
        public AnimationClip drunkenFeetClip;
        
        [Header("Setup Options")]
        [Tooltip("Automatically setup animations on start")]
        public bool autoSetupOnStart = true;
        
        [Tooltip("Apply drunken controller to koala model on start")]
        public bool applyDrunkenControllerOnStart = true;
        
        [Tooltip("Apply drunken controller to legs on start")]
        public bool applyDrunkenLegsControllerOnStart = true;
        
        // Private variables
        private Animator _koalaModelAnimator;
        private Animator _legsAnimator;
        private DrunkenCharacterAnimation _drunkenAnimation;
        private DrunkenAnimationController _animationController;
        private DrunkenLegsCharacterAnimation _drunkenLegsAnimation;
        private DrunkenLegsAnimationController _legsAnimationController;
        
        /// <summary>
        /// Initializes the animation setup
        /// </summary>
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupAnimations();
            }
        }
        
        /// <summary>
        /// Sets up the drunken animations for both model and legs
        /// </summary>
        [ContextMenu("Setup Drunken Animations")]
        public void SetupAnimations()
        {
            Debug.Log("DrunkenLegsAnimationSetup: Starting animation setup...");
            
            // Find the animators
            FindAnimators();
            
            if (_koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenLegsAnimationSetup: No Animator found on KoalaModel child!");
                return;
            }
            
            if (_legsAnimator == null)
            {
                Debug.LogWarning("DrunkenLegsAnimationSetup: No Animator found on Legs child!");
            }
            
            // Apply the animator controllers if specified
            if (applyDrunkenControllerOnStart && drunkenAnimatorController != null)
            {
                ApplyDrunkenController();
            }
            
            if (applyDrunkenLegsControllerOnStart && drunkenLegsAnimatorController != null && _legsAnimator != null)
            {
                ApplyDrunkenLegsController();
            }
            
            // Add animation components
            AddAnimationComponents();
            
            // Configure animation parameters
            ConfigureAnimationParameters();
            
            Debug.Log("DrunkenLegsAnimationSetup: Animation setup completed successfully!");
        }
        
        /// <summary>
        /// Finds all animator components
        /// </summary>
        private void FindAnimators()
        {
            // Find koala model animator
            Transform koalaModel = transform.Find("KoalaModel");
            if (koalaModel != null)
            {
                _koalaModelAnimator = koalaModel.GetComponent<Animator>();
                Debug.Log("DrunkenLegsAnimationSetup: Found KoalaModel animator");
            }
            
            // Find legs animator
            Transform legs = transform.Find("Legs");
            if (legs != null)
            {
                _legsAnimator = legs.GetComponent<Animator>();
                Debug.Log("DrunkenLegsAnimationSetup: Found Legs animator");
            }
            
            // If still not found, try to find any Animator in children
            if (_koalaModelAnimator == null)
            {
                _koalaModelAnimator = GetComponentInChildren<Animator>();
                Debug.Log("DrunkenLegsAnimationSetup: Found animator in children");
            }
        }
        
        /// <summary>
        /// Applies the drunken animator controller
        /// </summary>
        private void ApplyDrunkenController()
        {
            if (_koalaModelAnimator != null && drunkenAnimatorController != null)
            {
                _koalaModelAnimator.runtimeAnimatorController = drunkenAnimatorController;
                Debug.Log("DrunkenLegsAnimationSetup: Applied drunken animator controller");
            }
        }
        
        /// <summary>
        /// Applies the drunken legs animator controller
        /// </summary>
        private void ApplyDrunkenLegsController()
        {
            if (_legsAnimator != null && drunkenLegsAnimatorController != null)
            {
                _legsAnimator.runtimeAnimatorController = drunkenLegsAnimatorController;
                Debug.Log("DrunkenLegsAnimationSetup: Applied drunken legs animator controller");
            }
        }
        
        /// <summary>
        /// Adds the necessary animation components
        /// </summary>
        private void AddAnimationComponents()
        {
            // Add DrunkenCharacterAnimation component if not present
            _drunkenAnimation = GetComponent<DrunkenCharacterAnimation>();
            if (_drunkenAnimation == null)
            {
                _drunkenAnimation = gameObject.AddComponent<DrunkenCharacterAnimation>();
                Debug.Log("DrunkenLegsAnimationSetup: Added DrunkenCharacterAnimation component");
            }
            
            // Add DrunkenAnimationController component if not present
            _animationController = GetComponent<DrunkenAnimationController>();
            if (_animationController == null)
            {
                _animationController = gameObject.AddComponent<DrunkenAnimationController>();
                Debug.Log("DrunkenLegsAnimationSetup: Added DrunkenAnimationController component");
            }
            
            // Add DrunkenLegsCharacterAnimation component if not present
            _drunkenLegsAnimation = GetComponent<DrunkenLegsCharacterAnimation>();
            if (_drunkenLegsAnimation == null)
            {
                _drunkenLegsAnimation = gameObject.AddComponent<DrunkenLegsCharacterAnimation>();
                Debug.Log("DrunkenLegsAnimationSetup: Added DrunkenLegsCharacterAnimation component");
            }
            
            // Add DrunkenLegsAnimationController component if not present
            _legsAnimationController = GetComponent<DrunkenLegsAnimationController>();
            if (_legsAnimationController == null)
            {
                _legsAnimationController = gameObject.AddComponent<DrunkenLegsAnimationController>();
                Debug.Log("DrunkenLegsAnimationSetup: Added DrunkenLegsAnimationController component");
            }
        }
        
        /// <summary>
        /// Configures animation parameters
        /// </summary>
        private void ConfigureAnimationParameters()
        {
            if (_drunkenAnimation != null)
            {
                // Configure the drunken animation component
                _drunkenAnimation.drunkenAnimatorController = drunkenAnimatorController;
                _drunkenAnimation.normalAnimatorController = normalAnimatorController;
                _drunkenAnimation.drunkenMovementThreshold = 0.1f;
                _drunkenAnimation.animationTransitionDuration = 0.2f;
                _drunkenAnimation.drunkenAnimationSpeed = 1.2f;
            }
            
            if (_animationController != null)
            {
                // Configure the animation controller component
                _animationController.koalaModelAnimator = _koalaModelAnimator;
                _animationController.movementThreshold = 0.1f;
                _animationController.transitionDuration = 0.1f;
            }
            
            if (_drunkenLegsAnimation != null)
            {
                // Configure the drunken legs animation component
                _drunkenLegsAnimation.drunkenLegsAnimatorController = drunkenLegsAnimatorController;
                _drunkenLegsAnimation.normalLegsAnimatorController = normalLegsAnimatorController;
                _drunkenLegsAnimation.drunkenMovementThreshold = 0.1f;
                _drunkenLegsAnimation.animationTransitionDuration = 0.2f;
                _drunkenLegsAnimation.drunkenAnimationSpeed = 1.2f;
            }
            
            if (_legsAnimationController != null)
            {
                // Configure the legs animation controller component
                _legsAnimationController.legsAnimator = _legsAnimator;
                _legsAnimationController.movementThreshold = 0.1f;
                _legsAnimationController.transitionDuration = 0.1f;
                _legsAnimationController.syncWithMainAnimation = true;
            }
        }
        
        /// <summary>
        /// Validates the animation setup
        /// </summary>
        [ContextMenu("Validate Animation Setup")]
        public void ValidateSetup()
        {
            Debug.Log("DrunkenLegsAnimationSetup: Validating animation setup...");
            
            bool isValid = true;
            
            // Check for koala model animator
            if (_koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenLegsAnimationSetup: No KoalaModel animator found!");
                isValid = false;
            }
            
            // Check for legs animator
            if (_legsAnimator == null)
            {
                Debug.LogWarning("DrunkenLegsAnimationSetup: No Legs animator found!");
            }
            
            // Check for drunken animator controller
            if (drunkenAnimatorController == null)
            {
                Debug.LogWarning("DrunkenLegsAnimationSetup: No drunken animator controller assigned!");
            }
            
            // Check for drunken legs animator controller
            if (drunkenLegsAnimatorController == null)
            {
                Debug.LogWarning("DrunkenLegsAnimationSetup: No drunken legs animator controller assigned!");
            }
            
            // Check for character component
            Character character = GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("DrunkenLegsAnimationSetup: No Character component found!");
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log("DrunkenLegsAnimationSetup: Animation setup validation passed!");
            }
            else
            {
                Debug.LogError("DrunkenLegsAnimationSetup: Animation setup validation failed!");
            }
        }
        
        /// <summary>
        /// Resets the animation setup
        /// </summary>
        [ContextMenu("Reset Animation Setup")]
        public void ResetSetup()
        {
            Debug.Log("DrunkenLegsAnimationSetup: Resetting animation setup...");
            
            // Remove animation components
            if (_drunkenAnimation != null)
            {
                DestroyImmediate(_drunkenAnimation);
            }
            
            if (_animationController != null)
            {
                DestroyImmediate(_animationController);
            }
            
            if (_drunkenLegsAnimation != null)
            {
                DestroyImmediate(_drunkenLegsAnimation);
            }
            
            if (_legsAnimationController != null)
            {
                DestroyImmediate(_legsAnimationController);
            }
            
            // Reset animator controllers
            if (_koalaModelAnimator != null && normalAnimatorController != null)
            {
                _koalaModelAnimator.runtimeAnimatorController = normalAnimatorController;
            }
            
            if (_legsAnimator != null && normalLegsAnimatorController != null)
            {
                _legsAnimator.runtimeAnimatorController = normalLegsAnimatorController;
            }
            
            Debug.Log("DrunkenLegsAnimationSetup: Animation setup reset completed!");
        }
        
        /// <summary>
        /// Gets the current animation state of both model and legs
        /// </summary>
        /// <returns>Current animation state description</returns>
        public string GetAnimationState()
        {
            string modelState = "No Model Animator";
            string legsState = "No Legs Animator";
            
            if (_koalaModelAnimator != null)
            {
                AnimatorStateInfo stateInfo = _koalaModelAnimator.GetCurrentAnimatorStateInfo(0);
                modelState = $"Model: {stateInfo.shortNameHash}";
            }
            
            if (_legsAnimator != null)
            {
                AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
                legsState = $"Legs: {stateInfo.shortNameHash}";
            }
            
            return $"{modelState} | {legsState}";
        }
        
        /// <summary>
        /// Checks if the setup is valid
        /// </summary>
        /// <returns>True if setup is valid</returns>
        public bool IsSetupValid()
        {
            return _koalaModelAnimator != null && 
                   GetComponent<Character>() != null && 
                   drunkenAnimatorController != null;
        }
    }
}
