using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Enables a UI Image (like Game Over screen) when the player dies
    /// Listens to player Health.OnDeath event
    /// </summary>
    [AddComponentMenu("TopDown Engine/GUI/UI Image On Player Death")]
    public class UIImageOnPlayerDeath : MonoBehaviour
    {
        [Header("UI Reference")]
        [Tooltip("The UI Image (or GameObject) to enable when player dies")]
        public GameObject targetUI;
        
        [Tooltip("Auto-disable the UI on Start (so it only appears on death)")]
        public bool autoDisableOnStart = true;
        
        [Header("Player Detection")]
        [Tooltip("Player tag to find the player Health component")]
        public string playerTag = "Player";
        
        [Header("Timing")]
        [Tooltip("Delay before showing the UI (seconds)")]
        public float showDelay = 0f;
        
        [Header("Debug")]
        [Tooltip("Enable debug logging")]
        public bool showDebugInfo = false;

        private Health _playerHealth;
        private bool _hasTriggered = false;

        private void Start()
        {
            // Auto-disable UI at start
            if (autoDisableOnStart && targetUI != null)
            {
                targetUI.SetActive(false);
                
                if (showDebugInfo)
                {
                    Debug.Log($"UIImageOnPlayerDeath: Auto-disabled {targetUI.name}");
                }
            }
            
            // Find and subscribe to player
            SubscribeToPlayerHealth();
        }

        private void Update()
        {
            // Keep trying to subscribe if player not found yet
            if (_playerHealth == null)
            {
                SubscribeToPlayerHealth();
            }
        }

        /// <summary>
        /// Finds player and subscribes to death event
        /// </summary>
        private void SubscribeToPlayerHealth()
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                _playerHealth = player.GetComponent<Health>();
                if (_playerHealth != null)
                {
                    _playerHealth.OnDeath += OnPlayerDeath;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"UIImageOnPlayerDeath: Subscribed to {player.name} Health.OnDeath");
                    }
                }
                else
                {
                    if (showDebugInfo)
                    {
                        Debug.LogWarning($"UIImageOnPlayerDeath: Player found but no Health component!");
                    }
                }
            }
        }

        /// <summary>
        /// Called when player dies
        /// </summary>
        private void OnPlayerDeath()
        {
            if (_hasTriggered) return; // Prevent multiple triggers
            _hasTriggered = true;
            
            if (showDebugInfo)
            {
                Debug.Log("UIImageOnPlayerDeath: Player died! Showing UI...");
            }
            
            if (showDelay > 0f)
            {
                Invoke(nameof(ShowUI), showDelay);
            }
            else
            {
                ShowUI();
            }
        }

        /// <summary>
        /// Shows the target UI
        /// </summary>
        private void ShowUI()
        {
            if (targetUI != null)
            {
                targetUI.SetActive(true);
                
                if (showDebugInfo)
                {
                    Debug.Log($"UIImageOnPlayerDeath: Enabled {targetUI.name}");
                }
            }
            else
            {
                Debug.LogError("UIImageOnPlayerDeath: Target UI is not assigned!");
            }
        }

        /// <summary>
        /// Manually reset (useful for testing or respawn systems)
        /// </summary>
        [ContextMenu("Reset")]
        public void Reset()
        {
            _hasTriggered = false;
            
            if (targetUI != null)
            {
                targetUI.SetActive(false);
            }
            
            if (showDebugInfo)
            {
                Debug.Log("UIImageOnPlayerDeath: Reset");
            }
        }

        /// <summary>
        /// Manually trigger (for testing)
        /// </summary>
        [ContextMenu("Test Trigger")]
        public void TestTrigger()
        {
            OnPlayerDeath();
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
            }
        }
    }
}

