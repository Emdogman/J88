using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Manual fix script for drunken feet animation issues
    /// </summary>
    public class DrunkenFeetManualFix : MonoBehaviour
    {
        [Header("Manual Fix Steps")]
        [Tooltip("Step 1: Create Legs child with Animator")]
        public bool step1_CreateLegsChild = true;
        
        [Tooltip("Step 2: Assign controller to legs animator")]
        public bool step2_AssignController = true;
        
        [Tooltip("Step 3: Add legs animation controller")]
        public bool step3_AddController = true;
        
        [Tooltip("Step 4: Test the setup")]
        public bool step4_TestSetup = true;
        
        private Animator _legsAnimator;
        private DrunkenLegsAnimationController _legsController;
        
        /// <summary>
        /// Applies manual fixes step by step
        /// </summary>
        [ContextMenu("Apply Manual Fixes")]
        public void ApplyManualFixes()
        {
            Debug.Log("🔧 Starting manual fixes for drunken feet animation...");
            
            if (step1_CreateLegsChild)
            {
                Step1_CreateLegsChild();
            }
            
            if (step2_AssignController)
            {
                Step2_AssignController();
            }
            
            if (step3_AddController)
            {
                Step3_AddController();
            }
            
            if (step4_TestSetup)
            {
                Step4_TestSetup();
            }
            
            Debug.Log("🎉 Manual fixes completed!");
        }
        
        private void Step1_CreateLegsChild()
        {
            Debug.Log("STEP 1: Creating Legs child with Animator...");
            
            // Check if Legs child already exists
            Transform legs = transform.Find("Legs");
            if (legs == null)
            {
                Debug.Log("❌ No Legs child found. Creating one...");
                
                // Create Legs child
                GameObject legsChild = new GameObject("Legs");
                legsChild.transform.SetParent(transform);
                legsChild.transform.localPosition = Vector3.zero;
                legsChild.transform.localRotation = Quaternion.identity;
                legsChild.transform.localScale = Vector3.one;
                
                // Add Animator component
                _legsAnimator = legsChild.AddComponent<Animator>();
                _legsAnimator.enabled = true;
                _legsAnimator.speed = 1.0f;
                
                Debug.Log("✅ Created Legs child with Animator component");
            }
            else
            {
                Debug.Log("✅ Legs child already exists");
                _legsAnimator = legs.GetComponent<Animator>();
                
                if (_legsAnimator == null)
                {
                    Debug.Log("❌ No Animator component on Legs child. Adding one...");
                    _legsAnimator = legs.gameObject.AddComponent<Animator>();
                    _legsAnimator.enabled = true;
                    _legsAnimator.speed = 1.0f;
                    Debug.Log("✅ Added Animator component to Legs child");
                }
                else
                {
                    Debug.Log("✅ Legs child already has Animator component");
                }
            }
        }
        
        private void Step2_AssignController()
        {
            Debug.Log("STEP 2: Assigning controller to legs animator...");
            
            if (_legsAnimator == null)
            {
                Debug.LogError("❌ No legs animator found! Run Step 1 first.");
                return;
            }
            
            // Try to find the drunken feet controller
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Drunken feet anim");
            if (controller == null)
            {
                // Try to find it in the Drunken folder
                controller = Resources.Load<RuntimeAnimatorController>("Assets/Drunken/Drunken feet anim");
            }
            
            if (controller == null)
            {
                Debug.LogError("❌ Could not find 'Drunken feet anim.controller'!");
                Debug.LogError("   SOLUTION: Manually assign the controller in the Inspector");
                Debug.LogError("   1. Select the Legs child");
                Debug.LogError("   2. In the Animator component, drag 'Drunken feet anim.controller' to the Controller field");
                return;
            }
            
            _legsAnimator.runtimeAnimatorController = controller;
            Debug.Log("✅ Assigned controller to legs animator");
        }
        
        private void Step3_AddController()
        {
            Debug.Log("STEP 3: Adding legs animation controller...");
            
            // Check if legs controller already exists
            _legsController = GetComponent<DrunkenLegsAnimationController>();
            if (_legsController == null)
            {
                Debug.Log("❌ No DrunkenLegsAnimationController found. Adding one...");
                _legsController = gameObject.AddComponent<DrunkenLegsAnimationController>();
                Debug.Log("✅ Added DrunkenLegsAnimationController component");
            }
            else
            {
                Debug.Log("✅ DrunkenLegsAnimationController already exists");
            }
            
            // Configure the controller
            if (_legsController != null && _legsAnimator != null)
            {
                _legsController.legsAnimator = _legsAnimator;
                _legsController.syncWithMainAnimation = true;
                _legsController.movementThreshold = 0.1f;
                _legsController.transitionDuration = 0.1f;
                Debug.Log("✅ Configured legs animation controller");
            }
        }
        
        private void Step4_TestSetup()
        {
            Debug.Log("STEP 4: Testing the setup...");
            
            if (_legsAnimator == null)
            {
                Debug.LogError("❌ No legs animator found! Run previous steps first.");
                return;
            }
            
            // Test 1: Check if animator is enabled
            if (!_legsAnimator.enabled)
            {
                Debug.Log("❌ Legs animator is disabled. Enabling...");
                _legsAnimator.enabled = true;
            }
            
            // Test 2: Check if controller is assigned
            if (_legsAnimator.runtimeAnimatorController == null)
            {
                Debug.LogError("❌ No controller assigned to legs animator!");
                Debug.LogError("   SOLUTION: Manually assign 'Drunken feet anim.controller' in the Inspector");
                return;
            }
            
            // Test 3: Check if Move parameter exists
            try
            {
                bool moveParam = _legsAnimator.GetBool("Move");
                Debug.Log($"✅ Move parameter exists. Current value: {moveParam}");
            }
            catch
            {
                Debug.LogError("❌ Move parameter not found in controller!");
                Debug.LogError("   SOLUTION: Check the animator controller has a 'Move' parameter (Bool type)");
                return;
            }
            
            // Test 4: Test animation manually
            Debug.Log("🧪 Testing animation manually...");
            _legsAnimator.SetBool("Move", true);
            
            // Check if it worked
            StartCoroutine(CheckTestResult());
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
                    Debug.LogError("   NEXT STEPS:");
                    Debug.LogError("   1. Open 'Drunken feet anim.controller' in Animator window");
                    Debug.LogError("   2. Check if 'Move' parameter exists (Bool type)");
                    Debug.LogError("   3. Check if transitions are set up correctly");
                    Debug.LogError("   4. Check if animation clips are assigned to states");
                }
                
                // Reset the test
                _legsAnimator.SetBool("Move", false);
            }
        }
        
        /// <summary>
        /// Provides instructions for manual controller setup
        /// </summary>
        [ContextMenu("Show Manual Setup Instructions")]
        public void ShowManualSetupInstructions()
        {
            Debug.Log("🔧 MANUAL SETUP INSTRUCTIONS:");
            Debug.Log("=============================");
            Debug.Log("1. Open the Animator window (Window → Animation → Animator)");
            Debug.Log("2. Select 'Drunken feet anim.controller' from Assets/Drunken folder");
            Debug.Log("3. Check if the controller has:");
            Debug.Log("   - 'Move' parameter (Bool type)");
            Debug.Log("   - 'New State' (idle state)");
            Debug.Log("   - 'Drunken feet' (active state)");
            Debug.Log("   - Transitions between states based on 'Move' parameter");
            Debug.Log("4. If missing, create them manually in the Animator window");
            Debug.Log("5. Assign animation clips to the states");
            Debug.Log("6. Test by setting Move parameter to true/false");
        }
    }
}
