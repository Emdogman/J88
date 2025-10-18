using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Quick fix script for common drunken feet animation problems
    /// </summary>
    public class DrunkenFeetQuickFix : MonoBehaviour
    {
        [Header("Quick Fix Settings")]
        [Tooltip("The drunken feet animator controller")]
        public RuntimeAnimatorController drunkenFeetController;
        
        [Tooltip("Apply fixes automatically on start")]
        public bool autoFixOnStart = true;
        
        private Animator _legsAnimator;
        private DrunkenLegsAnimationController _legsController;
        
        void Start()
        {
            if (autoFixOnStart)
            {
                ApplyQuickFixes();
            }
        }
        
        /// <summary>
        /// Applies quick fixes for common problems
        /// </summary>
        [ContextMenu("Apply Quick Fixes")]
        public void ApplyQuickFixes()
        {
            Debug.Log("🔧 Applying quick fixes for drunken feet animation...");
            
            // Find legs animator
            FindLegsAnimator();
            
            // Fix 1: Ensure legs animator exists
            if (_legsAnimator == null)
            {
                Debug.LogError("❌ No legs animator found! Please ensure the 'Legs' child has an Animator component.");
                return;
            }
            
            // Fix 2: Assign the correct controller
            if (drunkenFeetController != null)
            {
                _legsAnimator.runtimeAnimatorController = drunkenFeetController;
                Debug.Log("✅ Assigned drunken feet controller to legs animator");
            }
            else
            {
                Debug.LogWarning("⚠️ No drunken feet controller assigned in inspector");
            }
            
            // Fix 3: Ensure animator is enabled
            if (!_legsAnimator.enabled)
            {
                _legsAnimator.enabled = true;
                Debug.Log("✅ Enabled legs animator");
            }
            
            // Fix 4: Reset animator speed
            _legsAnimator.speed = 1.0f;
            Debug.Log("✅ Reset legs animator speed to 1.0");
            
            // Fix 5: Ensure legs controller exists
            FindLegsController();
            if (_legsController == null)
            {
                _legsController = gameObject.AddComponent<DrunkenLegsAnimationController>();
                Debug.Log("✅ Added DrunkenLegsAnimationController component");
            }
            
            // Fix 6: Configure legs controller
            if (_legsController != null)
            {
                _legsController.legsAnimator = _legsAnimator;
                _legsController.syncWithMainAnimation = true;
                _legsController.movementThreshold = 0.1f;
                Debug.Log("✅ Configured legs animation controller");
            }
            
            // Fix 7: Test the animation
            TestAnimation();
            
            Debug.Log("🎉 Quick fixes applied! Check if the feet animation is working now.");
        }
        
        private void FindLegsAnimator()
        {
            // Look for a child named "Legs" with an Animator component
            Transform legs = transform.Find("Legs");
            if (legs != null)
            {
                _legsAnimator = legs.GetComponent<Animator>();
            }
            
            // If still not found, try to find any Animator in children
            if (_legsAnimator == null)
            {
                Animator[] animators = GetComponentsInChildren<Animator>();
                foreach (Animator animator in animators)
                {
                    // Skip the main model animator
                    if (animator.gameObject.name != "KoalaModel")
                    {
                        _legsAnimator = animator;
                        break;
                    }
                }
            }
        }
        
        private void FindLegsController()
        {
            _legsController = GetComponent<DrunkenLegsAnimationController>();
        }
        
        private void TestAnimation()
        {
            if (_legsAnimator != null)
            {
                // Test the Move parameter
                _legsAnimator.SetBool("Move", true);
                Debug.Log("🧪 Testing legs animation - Move parameter set to true");
                
                // Check if it worked
                StartCoroutine(CheckTestResult());
            }
        }
        
        private System.Collections.IEnumerator CheckTestResult()
        {
            yield return new WaitForSeconds(1.0f);
            
            if (_legsAnimator != null)
            {
                bool moveParam = _legsAnimator.GetBool("Move");
                AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
                
                Debug.Log($"🔍 Test result - Move Param: {moveParam}, State: {stateInfo.shortNameHash}, Speed: {stateInfo.speed}");
                
                if (moveParam && stateInfo.speed > 0)
                {
                    Debug.Log("✅ SUCCESS: Legs animation is working!");
                }
                else
                {
                    Debug.LogError("❌ FAILED: Legs animation is still not working");
                    Debug.LogError("   Possible causes:");
                    Debug.LogError("   1. Wrong animator controller assigned");
                    Debug.LogError("   2. Animation clips missing or corrupted");
                    Debug.LogError("   3. Animator state machine not set up correctly");
                }
                
                // Reset the test
                _legsAnimator.SetBool("Move", false);
            }
        }
        
        /// <summary>
        /// Manually triggers legs animation for testing
        /// </summary>
        [ContextMenu("Test Legs Animation")]
        public void TestLegsAnimation()
        {
            if (_legsAnimator != null)
            {
                _legsAnimator.SetBool("Move", true);
                Debug.Log("🧪 Manually triggered legs animation");
            }
            else
            {
                Debug.LogError("❌ No legs animator found!");
            }
        }
        
        /// <summary>
        /// Resets legs animation to idle
        /// </summary>
        [ContextMenu("Reset Legs Animation")]
        public void ResetLegsAnimation()
        {
            if (_legsAnimator != null)
            {
                _legsAnimator.SetBool("Move", false);
                Debug.Log("🔄 Reset legs animation to idle");
            }
            else
            {
                Debug.LogError("❌ No legs animator found!");
            }
        }
    }
}
