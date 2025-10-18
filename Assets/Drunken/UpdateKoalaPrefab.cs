using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Utility script to update the koala prefab with drunken animation components
    /// </summary>
    public class UpdateKoalaPrefab : MonoBehaviour
    {
        [Header("Prefab Update Settings")]
        [Tooltip("The koala prefab to update")]
        public GameObject koalaPrefab;
        
        [Tooltip("The drunken animator controller")]
        public RuntimeAnimatorController drunkenAnimatorController;
        
        [Tooltip("The normal animator controller")]
        public RuntimeAnimatorController normalAnimatorController;
        
        [Tooltip("Apply changes immediately")]
        public bool applyChangesImmediately = true;
        
        /// <summary>
        /// Updates the koala prefab with drunken animation components
        /// </summary>
        [ContextMenu("Update Koala Prefab")]
        public void UpdateKoalaPrefabWithAnimations()
        {
            if (koalaPrefab == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No koala prefab assigned!");
                return;
            }
            
            Debug.Log("UpdateKoalaPrefab: Starting koala prefab update...");
            
            // Find the koala model child
            Transform koalaModel = koalaPrefab.transform.Find("KoalaModel");
            if (koalaModel == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No KoalaModel child found in prefab!");
                return;
            }
            
            // Get or add animator component
            Animator animator = koalaModel.GetComponent<Animator>();
            if (animator == null)
            {
                animator = koalaModel.gameObject.AddComponent<Animator>();
                Debug.Log("UpdateKoalaPrefab: Added Animator component to KoalaModel");
            }
            
            // Set the drunken animator controller
            if (drunkenAnimatorController != null)
            {
                animator.runtimeAnimatorController = drunkenAnimatorController;
                Debug.Log("UpdateKoalaPrefab: Set drunken animator controller");
            }
            
            // Add animation setup component
            DrunkenAnimationSetup setup = koalaPrefab.GetComponent<DrunkenAnimationSetup>();
            if (setup == null)
            {
                setup = koalaPrefab.AddComponent<DrunkenAnimationSetup>();
                Debug.Log("UpdateKoalaPrefab: Added DrunkenAnimationSetup component");
            }
            
            // Configure the setup component
            setup.drunkenAnimatorController = drunkenAnimatorController;
            setup.normalAnimatorController = normalAnimatorController;
            setup.autoSetupOnStart = true;
            setup.applyDrunkenControllerOnStart = true;
            
            // Add example component for testing
            DrunkenAnimationExample example = koalaPrefab.GetComponent<DrunkenAnimationExample>();
            if (example == null)
            {
                example = koalaPrefab.AddComponent<DrunkenAnimationExample>();
                Debug.Log("UpdateKoalaPrefab: Added DrunkenAnimationExample component");
            }
            
            // Configure the example component
            example.toggleDrunkenKey = KeyCode.D;
            example.triggerDrunkenRunKey = KeyCode.R;
            example.triggerIdleKey = KeyCode.I;
            example.resetAnimationKey = KeyCode.T;
            example.animationSpeed = 1.0f;
            
            // Apply changes if requested
            if (applyChangesImmediately)
            {
                setup.SetupAnimations();
                Debug.Log("UpdateKoalaPrefab: Applied animation setup immediately");
            }
            
            Debug.Log("UpdateKoalaPrefab: Koala prefab update completed successfully!");
        }
        
        /// <summary>
        /// Validates the koala prefab setup
        /// </summary>
        [ContextMenu("Validate Koala Prefab")]
        public void ValidateKoalaPrefab()
        {
            if (koalaPrefab == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No koala prefab assigned!");
                return;
            }
            
            Debug.Log("UpdateKoalaPrefab: Validating koala prefab setup...");
            
            bool isValid = true;
            
            // Check for KoalaModel child
            Transform koalaModel = koalaPrefab.transform.Find("KoalaModel");
            if (koalaModel == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No KoalaModel child found!");
                isValid = false;
            }
            else
            {
                // Check for animator component
                Animator animator = koalaModel.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("UpdateKoalaPrefab: No Animator component on KoalaModel!");
                    isValid = false;
                }
                else
                {
                    Debug.Log("UpdateKoalaPrefab: Animator component found");
                }
            }
            
            // Check for character component
            Character character = koalaPrefab.GetComponent<Character>();
            if (character == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No Character component found!");
                isValid = false;
            }
            else
            {
                Debug.Log("UpdateKoalaPrefab: Character component found");
            }
            
            // Check for animation setup component
            DrunkenAnimationSetup setup = koalaPrefab.GetComponent<DrunkenAnimationSetup>();
            if (setup == null)
            {
                Debug.LogWarning("UpdateKoalaPrefab: No DrunkenAnimationSetup component found!");
            }
            else
            {
                Debug.Log("UpdateKoalaPrefab: DrunkenAnimationSetup component found");
            }
            
            // Check for example component
            DrunkenAnimationExample example = koalaPrefab.GetComponent<DrunkenAnimationExample>();
            if (example == null)
            {
                Debug.LogWarning("UpdateKoalaPrefab: No DrunkenAnimationExample component found!");
            }
            else
            {
                Debug.Log("UpdateKoalaPrefab: DrunkenAnimationExample component found");
            }
            
            if (isValid)
            {
                Debug.Log("UpdateKoalaPrefab: Koala prefab validation passed!");
            }
            else
            {
                Debug.LogError("UpdateKoalaPrefab: Koala prefab validation failed!");
            }
        }
        
        /// <summary>
        /// Resets the koala prefab to default state
        /// </summary>
        [ContextMenu("Reset Koala Prefab")]
        public void ResetKoalaPrefab()
        {
            if (koalaPrefab == null)
            {
                Debug.LogError("UpdateKoalaPrefab: No koala prefab assigned!");
                return;
            }
            
            Debug.Log("UpdateKoalaPrefab: Resetting koala prefab...");
            
            // Remove animation components
            DrunkenAnimationSetup setup = koalaPrefab.GetComponent<DrunkenAnimationSetup>();
            if (setup != null)
            {
                setup.ResetSetup();
                Debug.Log("UpdateKoalaPrefab: Reset DrunkenAnimationSetup");
            }
            
            DrunkenAnimationExample example = koalaPrefab.GetComponent<DrunkenAnimationExample>();
            if (example != null)
            {
                DestroyImmediate(example);
                Debug.Log("UpdateKoalaPrefab: Removed DrunkenAnimationExample");
            }
            
            // Reset animator controller
            Transform koalaModel = koalaPrefab.transform.Find("KoalaModel");
            if (koalaModel != null)
            {
                Animator animator = koalaModel.GetComponent<Animator>();
                if (animator != null && normalAnimatorController != null)
                {
                    animator.runtimeAnimatorController = normalAnimatorController;
                    Debug.Log("UpdateKoalaPrefab: Reset animator controller to normal");
                }
            }
            
            Debug.Log("UpdateKoalaPrefab: Koala prefab reset completed!");
        }
        
        /// <summary>
        /// Gets the current animation state of the koala prefab
        /// </summary>
        /// <returns>Current animation state description</returns>
        public string GetKoalaAnimationState()
        {
            if (koalaPrefab == null)
            {
                return "No koala prefab assigned";
            }
            
            Transform koalaModel = koalaPrefab.transform.Find("KoalaModel");
            if (koalaModel == null)
            {
                return "No KoalaModel child found";
            }
            
            Animator animator = koalaModel.GetComponent<Animator>();
            if (animator == null)
            {
                return "No Animator component found";
            }
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return $"Animation: {stateInfo.shortNameHash}, Speed: {stateInfo.speed}";
        }
        
        /// <summary>
        /// Checks if the koala prefab is properly set up
        /// </summary>
        /// <returns>True if properly set up</returns>
        public bool IsKoalaPrefabSetup()
        {
            if (koalaPrefab == null) return false;
            
            Transform koalaModel = koalaPrefab.transform.Find("KoalaModel");
            if (koalaModel == null) return false;
            
            Animator animator = koalaModel.GetComponent<Animator>();
            if (animator == null) return false;
            
            Character character = koalaPrefab.GetComponent<Character>();
            if (character == null) return false;
            
            return true;
        }
    }
}
