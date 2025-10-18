using UnityEngine;
using MoreMountains.TopDownEngine;

namespace Drunken
{
    /// <summary>
    /// Debug script to help diagnose why drunken feet animation isn't working
    /// </summary>
    public class DrunkenFeetDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [Tooltip("Show debug GUI")]
        public bool showDebugGUI = true;
        
        [Tooltip("Update frequency in seconds")]
        public float updateFrequency = 0.5f;
        
        [Tooltip("Enable detailed logging")]
        public bool enableDetailedLogging = true;
        
        // References
        private DrunkenAnimationController _mainController;
        private DrunkenLegsAnimationController _legsController;
        private Animator _legsAnimator;
        private Character _character;
        
        // Debug info
        private float _lastUpdateTime;
        private string _debugInfo = "";
        private bool _lastMoveState = false;
        
        void Start()
        {
            FindComponents();
            LogInitialSetup();
        }
        
        void Update()
        {
            if (Time.time - _lastUpdateTime >= updateFrequency)
            {
                UpdateDebugInfo();
                _lastUpdateTime = Time.time;
            }
        }
        
        private void FindComponents()
        {
            _mainController = GetComponent<DrunkenAnimationController>();
            _legsController = GetComponent<DrunkenLegsAnimationController>();
            _character = GetComponent<Character>();
            
            // Find legs animator
            Transform legs = transform.Find("Legs");
            if (legs != null)
            {
                _legsAnimator = legs.GetComponent<Animator>();
            }
            
            if (enableDetailedLogging)
            {
                Debug.Log($"DrunkenFeetDebugger: Found components - MainController: {_mainController != null}, LegsController: {_legsController != null}, LegsAnimator: {_legsAnimator != null}, Character: {_character != null}");
            }
        }
        
        private void LogInitialSetup()
        {
            Debug.Log("=== DRUNKEN FEET DEBUG - INITIAL SETUP ===");
            
            // Check legs animator
            if (_legsAnimator == null)
            {
                Debug.LogError("❌ PROBLEM: No legs animator found!");
                Debug.LogError("   Solution: Ensure the 'Legs' child has an Animator component");
            }
            else
            {
                Debug.Log($"✅ Legs animator found: {_legsAnimator.name}");
                Debug.Log($"   Enabled: {_legsAnimator.enabled}");
                Debug.Log($"   Controller: {(_legsAnimator.runtimeAnimatorController != null ? _legsAnimator.runtimeAnimatorController.name : "None")}");
                Debug.Log($"   Speed: {_legsAnimator.speed}");
            }
            
            // Check legs controller
            if (_legsController == null)
            {
                Debug.LogError("❌ PROBLEM: No DrunkenLegsAnimationController found!");
                Debug.LogError("   Solution: Add DrunkenLegsAnimationController component to the koala prefab");
            }
            else
            {
                Debug.Log($"✅ Legs controller found: {_legsController.name}");
                Debug.Log($"   Sync with main: {_legsController.syncWithMainAnimation}");
                Debug.Log($"   Movement threshold: {_legsController.movementThreshold}");
            }
            
            // Check main controller
            if (_mainController == null)
            {
                Debug.LogWarning("⚠️ No main animation controller found - legs will use direct movement detection");
            }
            else
            {
                Debug.Log($"✅ Main controller found: {_mainController.name}");
            }
            
            // Check character
            if (_character == null)
            {
                Debug.LogError("❌ PROBLEM: No Character component found!");
                Debug.LogError("   Solution: Ensure the koala prefab has a Character component");
            }
            else
            {
                Debug.Log($"✅ Character found: {_character.name}");
                Debug.Log($"   Movement state: {_character.MovementState.CurrentState}");
            }
        }
        
        private void UpdateDebugInfo()
        {
            _debugInfo = "=== DRUNKEN FEET DEBUG ===\n";
            
            // Character movement info
            if (_character != null)
            {
                bool isMoving = _character.MovementState.CurrentState != CharacterStates.MovementStates.Idle;
                _debugInfo += $"Character Moving: {isMoving} (State: {_character.MovementState.CurrentState})\n";
            }
            
            // Main controller info
            if (_mainController != null)
            {
                bool mainMoving = _mainController.IsMoving();
                _debugInfo += $"Main Controller Moving: {mainMoving}\n";
            }
            
            // Legs controller info
            if (_legsController != null)
            {
                bool legsMoving = _legsController.IsMoving();
                _debugInfo += $"Legs Controller Moving: {legsMoving}\n";
                _debugInfo += $"Legs Sync: {_legsController.syncWithMainAnimation}\n";
            }
            
            // Legs animator info
            if (_legsAnimator != null)
            {
                _debugInfo += $"\n--- LEGS ANIMATOR ---\n";
                _debugInfo += $"Enabled: {_legsAnimator.enabled}\n";
                _debugInfo += $"Controller: {(_legsAnimator.runtimeAnimatorController != null ? _legsAnimator.runtimeAnimatorController.name : "None")}\n";
                _debugInfo += $"Speed: {_legsAnimator.speed}\n";
                
                // Check Move parameter
                bool moveParam = _legsAnimator.GetBool("Move");
                _debugInfo += $"Move Parameter: {moveParam}\n";
                
                // Check current state
                AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
                _debugInfo += $"Current State: {stateInfo.shortNameHash}\n";
                _debugInfo += $"State Speed: {stateInfo.speed}\n";
                
                // Check if state changed
                if (moveParam != _lastMoveState)
                {
                    Debug.Log($"🔄 Move parameter changed from {_lastMoveState} to {moveParam}");
                    _lastMoveState = moveParam;
                }
            }
            else
            {
                _debugInfo += "\n--- LEGS ANIMATOR ---\nNOT FOUND!\n";
            }
            
            // Check for common problems
            CheckForCommonProblems();
        }
        
        private void CheckForCommonProblems()
        {
            _debugInfo += "\n--- PROBLEM DIAGNOSIS ---\n";
            
            // Problem 1: No legs animator
            if (_legsAnimator == null)
            {
                _debugInfo += "❌ No legs animator found\n";
            }
            else
            {
                // Problem 2: No controller assigned
                if (_legsAnimator.runtimeAnimatorController == null)
                {
                    _debugInfo += "❌ No animator controller assigned\n";
                }
                else
                {
                    // Problem 3: Wrong controller
                    if (!_legsAnimator.runtimeAnimatorController.name.Contains("Drunken feet"))
                    {
                        _debugInfo += "❌ Wrong controller assigned (should be 'Drunken feet anim')\n";
                    }
                    else
                    {
                        _debugInfo += "✅ Correct controller assigned\n";
                    }
                }
                
                // Problem 4: Animator disabled
                if (!_legsAnimator.enabled)
                {
                    _debugInfo += "❌ Legs animator is disabled\n";
                }
                else
                {
                    _debugInfo += "✅ Legs animator is enabled\n";
                }
                
                // Problem 5: Move parameter not changing
                bool moveParam = _legsAnimator.GetBool("Move");
                if (!moveParam && _character != null)
                {
                    bool shouldBeMoving = _character.MovementState.CurrentState != CharacterStates.MovementStates.Idle;
                    if (shouldBeMoving)
                    {
                        _debugInfo += "❌ Character is moving but Move parameter is false\n";
                    }
                }
            }
            
            // Problem 6: No legs controller
            if (_legsController == null)
            {
                _debugInfo += "❌ No legs animation controller found\n";
            }
            else
            {
                _debugInfo += "✅ Legs animation controller found\n";
            }
        }
        
        void OnGUI()
        {
            if (!showDebugGUI) return;
            
            // Display debug info
            GUI.Label(new Rect(10, 10, 600, 400), _debugInfo);
            
            // Add control buttons
            if (GUI.Button(new Rect(10, 420, 150, 30), "Force Legs Move"))
            {
                if (_legsAnimator != null)
                {
                    _legsAnimator.SetBool("Move", true);
                    Debug.Log("🔧 Manually set legs Move parameter to true");
                }
            }
            
            if (GUI.Button(new Rect(170, 420, 150, 30), "Force Legs Idle"))
            {
                if (_legsAnimator != null)
                {
                    _legsAnimator.SetBool("Move", false);
                    Debug.Log("🔧 Manually set legs Move parameter to false");
                }
            }
            
            if (GUI.Button(new Rect(330, 420, 150, 30), "Toggle Legs Sync"))
            {
                if (_legsController != null)
                {
                    _legsController.SetSyncWithMainAnimation(!_legsController.syncWithMainAnimation);
                    Debug.Log($"🔧 Toggled legs sync to {_legsController.syncWithMainAnimation}");
                }
            }
            
            if (GUI.Button(new Rect(490, 420, 150, 30), "Test Movement"))
            {
                TestMovement();
            }
        }
        
        private void TestMovement()
        {
            Debug.Log("🧪 Testing movement detection...");
            
            if (_character != null)
            {
                Debug.Log($"   Character state: {_character.MovementState.CurrentState}");
                bool isMoving = _character.MovementState.CurrentState != CharacterStates.MovementStates.Idle;
                Debug.Log($"   Should be moving: {isMoving}");
            }
            
            if (_mainController != null)
            {
                Debug.Log($"   Main controller moving: {_mainController.IsMoving()}");
            }
            
            if (_legsController != null)
            {
                Debug.Log($"   Legs controller moving: {_legsController.IsMoving()}");
            }
            
            if (_legsAnimator != null)
            {
                bool moveParam = _legsAnimator.GetBool("Move");
                Debug.Log($"   Legs Move parameter: {moveParam}");
            }
        }
        
        [ContextMenu("Force Legs Animation")]
        public void ForceLegsAnimation()
        {
            if (_legsAnimator != null)
            {
                _legsAnimator.SetBool("Move", true);
                Debug.Log("🔧 Forced legs animation - Move parameter set to true");
                
                // Check if it worked after a short delay
                StartCoroutine(CheckAnimationAfterDelay());
            }
            else
            {
                Debug.LogError("❌ No legs animator found!");
            }
        }
        
        private System.Collections.IEnumerator CheckAnimationAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            
            if (_legsAnimator != null)
            {
                bool moveParam = _legsAnimator.GetBool("Move");
                AnimatorStateInfo stateInfo = _legsAnimator.GetCurrentAnimatorStateInfo(0);
                
                Debug.Log($"🔍 After 0.5s - Move Param: {moveParam}, State: {stateInfo.shortNameHash}, Speed: {stateInfo.speed}");
                
                if (moveParam && stateInfo.speed > 0)
                {
                    Debug.Log("✅ Legs animation is working!");
                }
                else
                {
                    Debug.LogError("❌ Legs animation is not working - check animator controller setup");
                }
            }
        }
    }
}
