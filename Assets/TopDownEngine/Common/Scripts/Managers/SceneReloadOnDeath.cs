using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Reloads the current scene when the player dies
    /// Can wait for player input (Space key) instead of auto-reload
    /// </summary>
    public class SceneReloadOnDeath : MonoBehaviour
    {
        [Header("Reload Mode")]
        [Tooltip("Wait for Space key press instead of auto-reload after delay")]
        public bool waitForSpaceKey = true;
        
        [Header("Settings")]
        [Tooltip("Delay before reloading the scene (in seconds) - only used if waitForSpaceKey is false")]
        public float reloadDelay = 2f;
        
        [Tooltip("Player tag to find the player Health component")]
        public string playerTag = "Player";
        
        [Tooltip("Enable debug logging")]
        public bool debugMode = true;
        
        private Health _playerHealth;
        private bool _hasReloaded = false;
        private bool _waitingForInput = false;
        
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
            
            // Check for Space key press if waiting for input
            if (_waitingForInput && Input.GetKeyDown(KeyCode.Space))
            {
                if (debugMode)
                {
                    Debug.Log("[SceneReloadOnDeath] Space pressed! Reloading scene...");
                }
                ReloadScene();
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
            
            if (waitForSpaceKey)
            {
                // Wait for player to press Space
                _waitingForInput = true;
                
                if (debugMode)
                {
                    Debug.Log("[SceneReloadOnDeath] Player died! Waiting for Space key press to reload...");
                }
            }
            else
            {
                // Auto-reload after delay (old behavior)
                _hasReloaded = true;
                
                if (debugMode)
                {
                    Debug.Log("[SceneReloadOnDeath] Player died! Reloading scene in " + reloadDelay + " seconds...");
                }
                
                Invoke(nameof(ReloadScene), reloadDelay);
            }
        }
        
        private void ReloadScene()
        {
            _hasReloaded = true; // Prevent multiple reloads
            
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

