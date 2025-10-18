using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Advanced diagnostic script for drunken feet animation issues
    /// </summary>
    public class DrunkenFeetAdvancedDiagnostic : MonoBehaviour
    {
        [Header("Diagnostic Settings")]
        [Tooltip("Show detailed diagnostic GUI")]
        public bool showDiagnosticGUI = true;
        
        [Tooltip("The drunken feet animator controller to test")]
        public RuntimeAnimatorController testController;
        
        private Animator _legsAnimator;
        private string _diagnosticReport = "";
        
        void Start()
        {
            RunFullDiagnostic();
        }
        
        /// <summary>
        /// Runs a comprehensive diagnostic of the legs animation system
        /// </summary>
        [ContextMenu("Run Full Diagnostic")]
        public void RunFullDiagnostic()
        {
            Debug.Log("🔍 Starting comprehensive legs animation diagnostic...");
            _diagnosticReport = "=== LEGS ANIMATION DIAGNOSTIC REPORT ===\n\n";
            
            // Step 1: Find legs animator
            FindLegsAnimator();
            
            // Step 2: Check animator setup
            CheckAnimatorSetup();
            
            // Step 3: Check controller
            CheckController();
            
            // Step 4: Check animation clips
            CheckAnimationClips();
            
            // Step 5: Check parameters
            CheckParameters();
            
            // Step 6: Check state machine
            CheckStateMachine();
            
            // Step 7: Test animation manually
            TestAnimationManually();
            
            // Step 8: Generate recommendations
            GenerateRecommendations();
            
            Debug.Log(_diagnosticReport);
        }
        
        private void FindLegsAnimator()
        {
            _diagnosticReport += "STEP 1: FINDING LEGS ANIMATOR\n";
            _diagnosticReport += "============================\n";
            
            // Look for a child named "Legs"
            Transform legs = transform.Find("Legs");
            if (legs != null)
            {
                _diagnosticReport += $"✅ Found 'Legs' child: {legs.name}\n";
                _legsAnimator = legs.GetComponent<Animator>();
                
                if (_legsAnimator != null)
                {
                    _diagnosticReport += $"✅ Found Animator component on Legs\n";
                }
                else
                {
                    _diagnosticReport += "❌ No Animator component found on Legs child\n";
                    _diagnosticReport += "   SOLUTION: Add Animator component to Legs child\n";
                }
            }
            else
            {
                _diagnosticReport += "❌ No 'Legs' child found\n";
                _diagnosticReport += "   SOLUTION: Create a 'Legs' child with Animator component\n";
                
                // Try to find any animator in children
                Animator[] animators = GetComponentsInChildren<Animator>();
                _diagnosticReport += $"   Found {animators.Length} animator(s) in children:\n";
                foreach (Animator anim in animators)
                {
                    _diagnosticReport += $"   - {anim.gameObject.name}\n";
                }
            }
            
            _diagnosticReport += "\n";
        }
        
        private void CheckAnimatorSetup()
        {
            _diagnosticReport += "STEP 2: CHECKING ANIMATOR SETUP\n";
            _diagnosticReport += "===============================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "❌ Cannot check animator setup - no animator found\n";
                _diagnosticReport += "\n";
                return;
            }
            
            _diagnosticReport += $"✅ Animator enabled: {_legsAnimator.enabled}\n";
            _diagnosticReport += $"✅ Animator speed: {_legsAnimator.speed}\n";
            _diagnosticReport += $"✅ Animator culling mode: {_legsAnimator.cullingMode}\n";
            _diagnosticReport += $"✅ Animator update mode: {_legsAnimator.updateMode}\n";
            
            if (!_legsAnimator.enabled)
            {
                _diagnosticReport += "❌ PROBLEM: Animator is disabled\n";
                _diagnosticReport += "   SOLUTION: Enable the Animator component\n";
            }
            
            _diagnosticReport += "\n";
        }
        
        private void CheckController()
        {
            _diagnosticReport += "STEP 3: CHECKING CONTROLLER\n";
            _diagnosticReport += "============================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "❌ Cannot check controller - no animator found\n";
                _diagnosticReport += "\n";
                return;
            }
            
            if (_legsAnimator.runtimeAnimatorController == null)
            {
                _diagnosticReport += "❌ PROBLEM: No animator controller assigned\n";
                _diagnosticReport += "   SOLUTION: Assign 'Drunken feet anim.controller' to the legs animator\n";
            }
            else
            {
                string controllerName = _legsAnimator.runtimeAnimatorController.name;
                _diagnosticReport += $"✅ Controller assigned: {controllerName}\n";
                
                if (!controllerName.Contains("Drunken feet"))
                {
                    _diagnosticReport += "❌ PROBLEM: Wrong controller assigned\n";
                    _diagnosticReport += "   SOLUTION: Assign 'Drunken feet anim.controller' instead\n";
                }
                else
                {
                    _diagnosticReport += "✅ Correct controller assigned\n";
                }
            }
            
            _diagnosticReport += "\n";
        }
        
        private void CheckAnimationClips()
        {
            _diagnosticReport += "STEP 4: CHECKING ANIMATION CLIPS\n";
            _diagnosticReport += "=================================\n";
            
            if (_legsAnimator == null || _legsAnimator.runtimeAnimatorController == null)
            {
                _diagnosticReport += "❌ Cannot check animation clips - no controller assigned\n";
                _diagnosticReport += "\n";
                return;
            }
            
            // Get all animation clips from the controller
            AnimationClip[] clips = _legsAnimator.runtimeAnimatorController.animationClips;
            _diagnosticReport += $"✅ Found {clips.Length} animation clip(s) in controller:\n";
            
            foreach (AnimationClip clip in clips)
            {
                _diagnosticReport += $"   - {clip.name} (Length: {clip.length:F2}s)\n";
                
                if (clip.length == 0)
                {
                    _diagnosticReport += "     ❌ PROBLEM: Animation clip has zero length\n";
                }
            }
            
            if (clips.Length == 0)
            {
                _diagnosticReport += "❌ PROBLEM: No animation clips found in controller\n";
                _diagnosticReport += "   SOLUTION: Check if animation clips are properly assigned in the controller\n";
            }
            
            _diagnosticReport += "\n";
        }
        
        private void CheckParameters()
        {
            _diagnosticReport += "STEP 5: CHECKING PARAMETERS\n";
            _diagnosticReport += "=============================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "❌ Cannot check parameters - no animator found\n";
                _diagnosticReport += "\n";
                return;
            }
            
            // Check for Move parameter
            bool hasMoveParam = false;
            try
            {
                _legsAnimator.GetBool("Move");
                hasMoveParam = true;
                _diagnosticReport += "✅ 'Move' parameter exists\n";
            }
            catch
            {
                _diagnosticReport += "❌ PROBLEM: 'Move' parameter not found\n";
                _diagnosticReport += "   SOLUTION: Add 'Move' parameter (Bool) to the animator controller\n";
            }
            
            // Check current parameter value
            if (hasMoveParam)
            {
                bool moveValue = _legsAnimator.GetBool("Move");
                _diagnosticReport += $"✅ Current Move parameter value: {moveValue}\n";
            }
            
            _diagnosticReport += "\n";
        }
        
        private void CheckStateMachine()
        {
            _diagnosticReport += "STEP 6: CHECKING STATE MACHINE\n";
            _diagnosticReport += "===============================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "❌ Cannot check state machine - no animator found\n";
                _diagnosticReport += "\n";
                return;
            }
            
            // Get current state info
            AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
            _diagnosticReport += $"✅ Current state hash: {stateInfo.shortNameHash}\n";
            _diagnosticReport += $"✅ Current state speed: {stateInfo.speed}\n";
            _diagnosticReport += $"✅ Current state length: {stateInfo.length}\n";
            _diagnosticReport += $"✅ Is state loop: {stateInfo.loop}\n";
            
            if (stateInfo.speed == 0)
            {
                _diagnosticReport += "❌ PROBLEM: Current state has zero speed\n";
                _diagnosticReport += "   SOLUTION: Check if animation clips are properly assigned\n";
            }
            
            _diagnosticReport += "\n";
        }
        
        private void TestAnimationManually()
        {
            _diagnosticReport += "STEP 7: TESTING ANIMATION MANUALLY\n";
            _diagnosticReport += "===================================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "❌ Cannot test animation - no animator found\n";
                _diagnosticReport += "\n";
                return;
            }
            
            // Test setting Move parameter to true
            _diagnosticReport += "🧪 Testing Move parameter = true...\n";
            _legsAnimator.SetBool("Move", true);
            
            // Check if it worked
            bool moveParam = _legsAnimator.GetBool("Move");
            _diagnosticReport += $"   Move parameter after setting: {moveParam}\n";
            
            if (moveParam)
            {
                _diagnosticReport += "✅ Move parameter set successfully\n";
                
                // Check state after a short delay
                StartCoroutine(CheckStateAfterDelay());
            }
            else
            {
                _diagnosticReport += "❌ PROBLEM: Move parameter not set correctly\n";
                _diagnosticReport += "   SOLUTION: Check if 'Move' parameter exists in controller\n";
            }
            
            _diagnosticReport += "\n";
        }
        
        private System.Collections.IEnumerator CheckStateAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            
            if (_legsAnimator != null)
            {
                AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
                _diagnosticReport += $"   State after 0.5s: {stateInfo.shortNameHash}, Speed: {stateInfo.speed}\n";
                
                if (stateInfo.speed > 0)
                {
                    _diagnosticReport += "✅ SUCCESS: Animation is playing!\n";
                }
                else
                {
                    _diagnosticReport += "❌ PROBLEM: Animation not playing despite Move=true\n";
                    _diagnosticReport += "   SOLUTION: Check state machine transitions and animation clips\n";
                }
            }
        }
        
        private void GenerateRecommendations()
        {
            _diagnosticReport += "STEP 8: RECOMMENDATIONS\n";
            _diagnosticReport += "=======================\n";
            
            if (_legsAnimator == null)
            {
                _diagnosticReport += "🔧 PRIORITY 1: Create Legs child with Animator component\n";
                _diagnosticReport += "🔧 PRIORITY 2: Assign 'Drunken feet anim.controller' to legs animator\n";
            }
            else if (_legsAnimator.runtimeAnimatorController == null)
            {
                _diagnosticReport += "🔧 PRIORITY 1: Assign 'Drunken feet anim.controller' to legs animator\n";
            }
            else
            {
                _diagnosticReport += "🔧 Check the animator controller in the Animator window:\n";
                _diagnosticReport += "   1. Open 'Drunken feet anim.controller' in Animator window\n";
                _diagnosticReport += "   2. Check if 'Move' parameter exists (Bool type)\n";
                _diagnosticReport += "   3. Check if transitions are set up correctly\n";
                _diagnosticReport += "   4. Check if animation clips are assigned to states\n";
            }
            
            _diagnosticReport += "\n";
        }
        
    }
}
