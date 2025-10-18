using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Reloads the current scene when the player dies
    /// </summary>
    public class SceneReloadOnDeath : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Delay before reloading the scene (in seconds)")]
        public float reloadDelay = 2f;
        
        [Tooltip("Player tag to find the player Health component")]
        public string playerTag = "Player";
        
        [Tooltip("Enable debug logging")]
        public bool debugMode = true;
        
        private Health _playerHealth;
        private bool _hasReloaded = false;
        
        private void Start()
        {
            // Find player and subscribe to death event
            SubscribeToPlayerHealth();
        }
        
        private void Update()
        {
            // Try to subscribe if we haven't found the player yet
            if (_playerHealth == null)
            {
                SubscribeToPlayerHealth();
            }
        }
        
        private void SubscribeToPlayerHealth()
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                if (debugMode)
                {
                    Debug.Log($"[SceneReloadOnDeath] Found player: {player.name}");
                }
                
                _playerHealth = player.GetComponent<Health>();
                if (_playerHealth != null)
                {
                    _playerHealth.OnDeath += OnPlayerDeath;
                    if (debugMode)
                    {
                        Debug.Log("[SceneReloadOnDeath] Successfully subscribed to player Health.OnDeath");
                    }
                }
                else
                {
                    if (debugMode)
                    {
                        Debug.LogWarning($"[SceneReloadOnDeath] Player {player.name} has no Health component!");
                    }
                }
            }
            else
            {
                if (debugMode)
                {
                    Debug.LogWarning($"[SceneReloadOnDeath] No player found with tag '{playerTag}'");
                }
            }
        }
        
        private void OnPlayerDeath()
        {
            if (_hasReloaded)
            {
                return; // Prevent multiple reloads
            }
            
            _hasReloaded = true;
            
            if (debugMode)
            {
                Debug.Log("[SceneReloadOnDeath] Player died! Reloading scene in " + reloadDelay + " seconds...");
            }
            
            // Reload the scene after delay
            Invoke(nameof(ReloadScene), reloadDelay);
        }
        
        private void ReloadScene()
        {
            if (debugMode)
            {
                Debug.Log("[SceneReloadOnDeath] Reloading scene...");
            }
            
            // Get the current scene name and reload it
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        
        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
                if (debugMode)
                {
                    Debug.Log("[SceneReloadOnDeath] Unsubscribed from player Health.OnDeath");
                }
            }
        }
        
        [ContextMenu("Test Reload")]
        public void TestReload()
        {
            if (debugMode)
            {
                Debug.Log("[SceneReloadOnDeath] Test reload triggered from context menu");
            }
            ReloadScene();
        }
    }
}
