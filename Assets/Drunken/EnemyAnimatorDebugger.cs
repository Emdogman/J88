using UnityEngine;

namespace Drunken
{
    /// <summary>
    /// Debug script to diagnose why enemy animator isn't playing attack animation
    /// </summary>
    public class EnemyAnimatorDebugger : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The animator to debug (leave empty to auto-find)")]
        public Animator targetAnimator;
        
        [Header("Settings")]
        [Tooltip("Show debug GUI")]
        public bool showDebugGUI = true;
        
        [Tooltip("Log every frame")]
        public bool logEveryFrame = false;
        
        private string _debugInfo = "";
        
        void Start()
        {
            if (targetAnimator == null)
            {
                // Try to find Model child
                Transform modelChild = transform.Find("Model");
                if (modelChild != null)
                {
                    targetAnimator = modelChild.GetComponent<Animator>();
                }
                
                // Fallback to any animator
                if (targetAnimator == null)
                {
                    targetAnimator = GetComponentInChildren<Animator>();
                }
            }
            
            if (targetAnimator != null)
            {
                Debug.Log($"EnemyAnimatorDebugger: Monitoring animator on {targetAnimator.gameObject.name}");
                CheckAnimatorSetup();
            }
            else
            {
                Debug.LogError("EnemyAnimatorDebugger: No animator found!");
            }
        }
        
        void Update()
        {
            if (targetAnimator == null) return;
            
            UpdateDebugInfo();
            
            if (logEveryFrame)
            {
                Debug.Log(_debugInfo);
            }
        }
        
        private void CheckAnimatorSetup()
        {
            if (targetAnimator == null) return;
            
            Debug.Log("=== ANIMATOR SETUP CHECK ===");
            Debug.Log($"Animator GameObject: {targetAnimator.gameObject.name}");
            Debug.Log($"Animator Enabled: {targetAnimator.enabled}");
            Debug.Log($"Animator Controller: {(targetAnimator.runtimeAnimatorController != null ? targetAnimator.runtimeAnimatorController.name : "NONE")}");
            
            if (targetAnimator.runtimeAnimatorController == null)
            {
                Debug.LogError("❌ NO ANIMATOR CONTROLLER ASSIGNED!");
                return;
            }
            
            // Check for Attack parameter
            bool hasAttackParam = false;
            foreach (var param in targetAnimator.parameters)
            {
                Debug.Log($"Parameter: {param.name} (Type: {param.type})");
                if (param.name == "Attack")
                {
                    hasAttackParam = true;
                    if (param.type != AnimatorControllerParameterType.Trigger)
                    {
                        Debug.LogError($"❌ 'Attack' parameter is {param.type}, should be Trigger!");
                    }
                    else
                    {
                        Debug.Log("✅ 'Attack' parameter is correctly set as Trigger");
                    }
                }
            }
            
            if (!hasAttackParam)
            {
                Debug.LogError("❌ NO 'Attack' PARAMETER FOUND IN ANIMATOR CONTROLLER!");
            }
            
            // Check animation clips
            var clips = targetAnimator.runtimeAnimatorController.animationClips;
            Debug.Log($"Animation clips in controller: {clips.Length}");
            foreach (var clip in clips)
            {
                Debug.Log($"  - {clip.name} (Length: {clip.length}s)");
            }
            
            if (clips.Length == 0)
            {
                Debug.LogError("❌ NO ANIMATION CLIPS IN CONTROLLER!");
            }
        }
        
        private void UpdateDebugInfo()
        {
            if (targetAnimator == null) return;
            
            _debugInfo = "=== ENEMY ANIMATOR DEBUG ===\n";
            _debugInfo += $"GameObject: {targetAnimator.gameObject.name}\n";
            _debugInfo += $"Enabled: {targetAnimator.enabled}\n";
            _debugInfo += $"Controller: {(targetAnimator.runtimeAnimatorController != null ? targetAnimator.runtimeAnimatorController.name : "NONE")}\n";
            _debugInfo += $"Speed: {targetAnimator.speed}\n";
            _debugInfo += $"Layer Count: {targetAnimator.layerCount}\n";
            
            if (targetAnimator.layerCount > 0)
            {
                AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
                _debugInfo += $"\nCurrent State:\n";
                _debugInfo += $"  Hash: {stateInfo.fullPathHash}\n";
                _debugInfo += $"  Length: {stateInfo.length}s\n";
                _debugInfo += $"  Speed: {stateInfo.speed}\n";
                _debugInfo += $"  Normalized Time: {stateInfo.normalizedTime:F2}\n";
                _debugInfo += $"  Loop: {stateInfo.loop}\n";
                
                // Try to get state name
                var clips = targetAnimator.runtimeAnimatorController.animationClips;
                foreach (var clip in clips)
                {
                    if (stateInfo.IsName(clip.name))
                    {
                        _debugInfo += $"  Current Animation: {clip.name}\n";
                        break;
                    }
                }
            }
            
            // Check parameters
            _debugInfo += $"\nParameters:\n";
            foreach (var param in targetAnimator.parameters)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        _debugInfo += $"  {param.name} (Bool): {targetAnimator.GetBool(param.name)}\n";
                        break;
                    case AnimatorControllerParameterType.Float:
                        _debugInfo += $"  {param.name} (Float): {targetAnimator.GetFloat(param.name)}\n";
                        break;
                    case AnimatorControllerParameterType.Int:
                        _debugInfo += $"  {param.name} (Int): {targetAnimator.GetInteger(param.name)}\n";
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        _debugInfo += $"  {param.name} (Trigger)\n";
                        break;
                }
            }
        }
        
        void OnGUI()
        {
            if (!showDebugGUI || targetAnimator == null) return;
            
            GUI.Box(new Rect(10, 10, 400, 600), "Enemy Animator Debugger");
            GUI.Label(new Rect(20, 40, 380, 550), _debugInfo);
            
            if (GUI.Button(new Rect(20, 610, 180, 30), "Manually Trigger Attack"))
            {
                targetAnimator.SetTrigger("Attack");
                Debug.Log("EnemyAnimatorDebugger: Manually triggered Attack");
            }
            
            if (GUI.Button(new Rect(210, 610, 180, 30), "Check Setup"))
            {
                CheckAnimatorSetup();
            }
        }
        
        [ContextMenu("Check Animator Setup")]
        public void ManualCheckSetup()
        {
            CheckAnimatorSetup();
        }
        
        [ContextMenu("Trigger Attack")]
        public void ManualTriggerAttack()
        {
            if (targetAnimator != null)
            {
                targetAnimator.SetTrigger("Attack");
                Debug.Log("EnemyAnimatorDebugger: Manually triggered Attack via context menu");
            }
        }
    }
}

