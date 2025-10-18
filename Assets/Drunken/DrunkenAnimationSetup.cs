using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Setup script for configuring drunken animations on the koala character
    /// </summary>
    public class DrunkenAnimationSetup : MonoBehaviour
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
        
        [Tooltip("The drunken legs animation clip")]
        public AnimationClip drunkenLegsClip;
        
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
        /// Sets up the drunken animations
        /// </summary>
        [ContextMenu("Setup Drunken Animations")]
        public void SetupAnimations()
        {
            Debug.Log("DrunkenAnimationSetup: Starting animation setup...");
            
            // Find the koala model animator
            FindKoalaModelAnimator();
            
            // Find the legs animator
            FindLegsAnimator();
            
            if (_koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenAnimationSetup: No Animator found on KoalaModel child!");
                return;
            }
            
            if (_legsAnimator == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No Animator found on Legs child!");
            }
            
            // Apply the drunken animator controller if specified
            if (applyDrunkenControllerOnStart && drunkenAnimatorController != null)
            {
                ApplyDrunkenController();
            }
            
            // Apply the drunken legs animator controller if specified
            if (applyDrunkenLegsControllerOnStart && drunkenLegsAnimatorController != null && _legsAnimator != null)
            {
                ApplyDrunkenLegsController();
            }
            
            // Add animation components
            AddAnimationComponents();
            
            // Configure animation parameters
            ConfigureAnimationParameters();
            
            Debug.Log("DrunkenAnimationSetup: Animation setup completed successfully!");
        }
        
        /// <summary>
        /// Finds the koala model animator component
        /// </summary>
        private void FindKoalaModelAnimator()
        {
            // Look for a child named "KoalaModel" with an Animator component
            Transform koalaModel = transform.Find("KoalaModel");
            if (koalaModel != null)
            {
                _koalaModelAnimator = koalaModel.GetComponent<Animator>();
                Debug.Log("DrunkenAnimationSetup: Found KoalaModel animator");
            }
            
            // If still not found, try to find any Animator in children
            if (_koalaModelAnimator == null)
            {
                _koalaModelAnimator = GetComponentInChildren<Animator>();
                Debug.Log("DrunkenAnimationSetup: Found animator in children");
            }
        }
        
        /// <summary>
        /// Finds the legs animator component
        /// </summary>
        private void FindLegsAnimator()
        {
            // Look for a child named "Legs" with an Animator component
            Transform legs = transform.Find("Legs");
            if (legs != null)
            {
                _legsAnimator = legs.GetComponent<Animator>();
                Debug.Log("DrunkenAnimationSetup: Found Legs animator");
            }
            
            // If still not found, try to find any Animator in children (excluding KoalaModel)
            if (_legsAnimator == null)
            {
                Animator[] animators = GetComponentsInChildren<Animator>();
                foreach (Animator animator in animators)
                {
                    if (animator != _koalaModelAnimator)
                    {
                        _legsAnimator = animator;
                        Debug.Log("DrunkenAnimationSetup: Found legs animator in children");
                        break;
                    }
                }
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
                Debug.Log("DrunkenAnimationSetup: Applied drunken animator controller");
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
                Debug.Log("DrunkenAnimationSetup: Applied drunken legs animator controller");
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
                Debug.Log("DrunkenAnimationSetup: Added DrunkenCharacterAnimation component");
            }
            
            // Add DrunkenAnimationController component if not present
            _animationController = GetComponent<DrunkenAnimationController>();
            if (_animationController == null)
            {
                _animationController = gameObject.AddComponent<DrunkenAnimationController>();
                Debug.Log("DrunkenAnimationSetup: Added DrunkenAnimationController component");
            }
            
            // Add DrunkenLegsCharacterAnimation component if not present
            _drunkenLegsAnimation = GetComponent<DrunkenLegsCharacterAnimation>();
            if (_drunkenLegsAnimation == null)
            {
                _drunkenLegsAnimation = gameObject.AddComponent<DrunkenLegsCharacterAnimation>();
                Debug.Log("DrunkenAnimationSetup: Added DrunkenLegsCharacterAnimation component");
            }
            
            // Add DrunkenLegsAnimationController component if not present
            _legsAnimationController = GetComponent<DrunkenLegsAnimationController>();
            if (_legsAnimationController == null)
            {
                _legsAnimationController = gameObject.AddComponent<DrunkenLegsAnimationController>();
                Debug.Log("DrunkenAnimationSetup: Added DrunkenLegsAnimationController component");
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
            Debug.Log("DrunkenAnimationSetup: Validating animation setup...");
            
            bool isValid = true;
            
            // Check for koala model animator
            if (_koalaModelAnimator == null)
            {
                Debug.LogError("DrunkenAnimationSetup: No KoalaModel animator found!");
                isValid = false;
            }
            
            // Check for drunken animator controller
            if (drunkenAnimatorController == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No drunken animator controller assigned!");
            }
            
            // Check for animation clips
            if (idleClip == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No idle animation clip assigned!");
            }
            
            if (drunkenRunClip == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No drunken run animation clip assigned!");
            }
            
            // Check for legs animator
            if (_legsAnimator == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No Legs animator found!");
            }
            
            // Check for drunken legs animator controller
            if (drunkenLegsAnimatorController == null)
            {
                Debug.LogWarning("DrunkenAnimationSetup: No drunken legs animator controller assigned!");
            }
            
            // Check for character component
            Character character = GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("DrunkenAnimationSetup: No Character component found!");
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log("DrunkenAnimationSetup: Animation setup validation passed!");
            }
            else
            {
                Debug.LogError("DrunkenAnimationSetup: Animation setup validation failed!");
            }
        }
        
        /// <summary>
        /// Resets the animation setup
        /// </summary>
        [ContextMenu("Reset Animation Setup")]
        public void ResetSetup()
        {
            Debug.Log("DrunkenAnimationSetup: Resetting animation setup...");
            
            // Remove animation components
            if (_drunkenAnimation != null)
            {
                DestroyImmediate(_drunkenAnimation);
            }
            
            if (_animationController != null)
            {
                DestroyImmediate(_animationController);
            }
            
            // Reset animator controller
            if (_koalaModelAnimator != null && normalAnimatorController != null)
            {
                _koalaModelAnimator.runtimeAnimatorController = normalAnimatorController;
            }
            
            Debug.Log("DrunkenAnimationSetup: Animation setup reset completed!");
        }
        
        /// <summary>
        /// Gets the current animation state
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
