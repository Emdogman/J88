using UnityEngine;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Spawns health pickups near player but outside camera view, inside level bounds
    /// Maximum of 3 active health pickups at once
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/Health Pickup Spawner")]
    public class HealthPickupSpawner : MonoBehaviour
    {
        [Header("Health Pickup Settings")]
        [Tooltip("Health pickup prefab to spawn")]
        public GameObject healthPickupPrefab;
        
        [Tooltip("Maximum number of health pickups active at once")]
        public int maxActivePickups = 3;
        
        [Tooltip("Time between spawn attempts (seconds)")]
        public float spawnInterval = 10f;
        
        [Tooltip("Enable/disable spawning")]
        public bool isSpawning = true;

        [Header("Spawn Position")]
        [Tooltip("Distance from camera edge to spawn pickups")]
        public float spawnDistanceFromCamera = 2f;
        
        [Tooltip("Minimum distance from player to spawn")]
        public float minDistanceFromPlayer = 5f;
        
        [Tooltip("Maximum distance from player to spawn")]
        public float maxDistanceFromPlayer = 10f;

        [Header("Level Bounds")]
        [Tooltip("Use manual spawn area bounds")]
        public bool useManualBounds = false;
        
        [Tooltip("Minimum X coordinate")]
        public float boundsMinX = -20f;
        
        [Tooltip("Maximum X coordinate")]
        public float boundsMaxX = 20f;
        
        [Tooltip("Minimum Y coordinate")]
        public float boundsMinY = -20f;
        
        [Tooltip("Maximum Y coordinate")]
        public float boundsMaxY = 20f;
        
        [Tooltip("Tilemap to check for walkable tiles (optional, auto-finds if null)")]
        public UnityEngine.Tilemaps.Tilemap walkableTilemap;

        [Header("References")]
        [Tooltip("Main camera (auto-finds if null)")]
        public Camera mainCamera;
        
        [Tooltip("Player transform (auto-finds by tag if null)")]
        public Transform player;
        
        [Tooltip("Player tag to find player")]
        public string playerTag = "Player";

        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;
        
        [Tooltip("Show spawn zones in Scene view")]
        public bool showSpawnZones = true;

        private List<GameObject> _activePickups = new List<GameObject>();
        private float _lastSpawnTime;
        private float _nextSpawnTime;

        private void Start()
        {
            // Auto-find references
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
            
            if (walkableTilemap == null)
            {
                FindWalkableTilemap();
            }
            
            _lastSpawnTime = Time.time;
            _nextSpawnTime = Time.time + spawnInterval;
        }

        private void Update()
        {
            if (!isSpawning) return;
            if (healthPickupPrefab == null) return;
            if (player == null) return;

            // Clean up destroyed pickups from list
            _activePickups.RemoveAll(pickup => pickup == null);

            // Check if we should spawn
            if (Time.time >= _nextSpawnTime && _activePickups.Count < maxActivePickups)
            {
                TrySpawnHealthPickup();
                _nextSpawnTime = Time.time + spawnInterval;
            }
        }

        /// <summary>
        /// Attempts to spawn a health pickup
        /// </summary>
        private void TrySpawnHealthPickup()
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            
            if (spawnPosition != Vector3.zero)
            {
                GameObject pickup = Instantiate(healthPickupPrefab, spawnPosition, Quaternion.identity);
                _activePickups.Add(pickup);
                
                if (showDebugInfo)
                {
                    Debug.Log($"HealthPickupSpawner: Spawned health pickup at {spawnPosition}. Active: {_activePickups.Count}/{maxActivePickups}");
                }
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning("HealthPickupSpawner: Could not find valid spawn position!");
                }
            }
        }

        /// <summary>
        /// Gets a valid spawn position outside camera view, near player, inside bounds
        /// </summary>
        private Vector3 GetValidSpawnPosition()
        {
            const int maxAttempts = 20;
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 candidatePosition = GenerateOffscreenPosition();
                
                // Check all validation criteria
                if (IsPositionValid(candidatePosition))
                {
                    return candidatePosition;
                }
            }
            
            return Vector3.zero; // Failed to find valid position
        }

        /// <summary>
        /// Generates a position outside camera view near the player
        /// </summary>
        private Vector3 GenerateOffscreenPosition()
        {
            if (mainCamera == null || player == null) return Vector3.zero;

            // Get camera bounds
            float camHeight = mainCamera.orthographicSize * 2f;
            float camWidth = camHeight * mainCamera.aspect;
            Vector3 camPos = mainCamera.transform.position;

            // Choose random side
            int side = Random.Range(0, 4);
            Vector3 position = Vector3.zero;

            switch (side)
            {
                case 0: // Top
                    position = new Vector3(
                        camPos.x + Random.Range(-camWidth / 2f, camWidth / 2f),
                        camPos.y + camHeight / 2f + spawnDistanceFromCamera,
                        player.position.z
                    );
                    break;
                case 1: // Bottom
                    position = new Vector3(
                        camPos.x + Random.Range(-camWidth / 2f, camWidth / 2f),
                        camPos.y - camHeight / 2f - spawnDistanceFromCamera,
                        player.position.z
                    );
                    break;
                case 2: // Left
                    position = new Vector3(
                        camPos.x - camWidth / 2f - spawnDistanceFromCamera,
                        camPos.y + Random.Range(-camHeight / 2f, camHeight / 2f),
                        player.position.z
                    );
                    break;
                case 3: // Right
                    position = new Vector3(
                        camPos.x + camWidth / 2f + spawnDistanceFromCamera,
                        camPos.y + Random.Range(-camHeight / 2f, camHeight / 2f),
                        player.position.z
                    );
                    break;
            }

            return position;
        }

        /// <summary>
        /// Checks if a position is valid for spawning
        /// </summary>
        private bool IsPositionValid(Vector3 position)
        {
            // Check distance from player
            float distanceToPlayer = Vector3.Distance(position, player.position);
            if (distanceToPlayer < minDistanceFromPlayer || distanceToPlayer > maxDistanceFromPlayer)
            {
                return false;
            }

            // Check manual bounds
            if (useManualBounds)
            {
                if (position.x < boundsMinX || position.x > boundsMaxX ||
                    position.y < boundsMinY || position.y > boundsMaxY)
                {
                    return false;
                }
            }

            // Check tilemap walkability
            if (walkableTilemap != null)
            {
                Vector3Int cellPosition = walkableTilemap.WorldToCell(position);
                if (!walkableTilemap.HasTile(cellPosition))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Auto-finds the walkable tilemap in the scene
        /// </summary>
        private void FindWalkableTilemap()
        {
            string[] tilemapNames = { "Ground", "Floor", "Walkable", "Base", "Tilemap" };
            
            foreach (string name in tilemapNames)
            {
                GameObject tilemapObj = GameObject.Find(name);
                if (tilemapObj != null)
                {
                    walkableTilemap = tilemapObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                    if (walkableTilemap != null)
                    {
                        if (showDebugInfo)
                        {
                            Debug.Log($"HealthPickupSpawner: Auto-found walkable tilemap '{name}'");
                        }
                        return;
                    }
                }
            }
            
            walkableTilemap = FindObjectOfType<UnityEngine.Tilemaps.Tilemap>();
            if (walkableTilemap != null && showDebugInfo)
            {
                Debug.Log($"HealthPickupSpawner: Auto-found tilemap '{walkableTilemap.name}'");
            }
        }

        /// <summary>
        /// Manually spawn a pickup now
        /// </summary>
        [ContextMenu("Spawn Health Pickup Now")]
        public void SpawnNow()
        {
            if (_activePickups.Count < maxActivePickups)
            {
                TrySpawnHealthPickup();
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning($"HealthPickupSpawner: Already at max pickups ({maxActivePickups})");
                }
            }
        }

        /// <summary>
        /// Clear all active pickups
        /// </summary>
        [ContextMenu("Clear All Pickups")]
        public void ClearAllPickups()
        {
            foreach (GameObject pickup in _activePickups)
            {
                if (pickup != null)
                {
                    Destroy(pickup);
                }
            }
            _activePickups.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("HealthPickupSpawner: Cleared all pickups");
            }
        }

        /// <summary>
        /// Debug visualization
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showSpawnZones || mainCamera == null) return;

            // Draw camera bounds
            float camHeight = mainCamera.orthographicSize * 2f;
            float camWidth = camHeight * mainCamera.aspect;
            Vector3 camPos = mainCamera.transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(camPos.x, camPos.y, 0f), new Vector3(camWidth, camHeight, 0.1f));

            // Draw spawn zones
            Gizmos.color = Color.green;
            
            // Top
            Vector3 topCenter = new Vector3(camPos.x, camPos.y + camHeight / 2f + spawnDistanceFromCamera, 0f);
            Gizmos.DrawWireCube(topCenter, new Vector3(camWidth, 0.5f, 0.1f));
            
            // Bottom
            Vector3 bottomCenter = new Vector3(camPos.x, camPos.y - camHeight / 2f - spawnDistanceFromCamera, 0f);
            Gizmos.DrawWireCube(bottomCenter, new Vector3(camWidth, 0.5f, 0.1f));
            
            // Left
            Vector3 leftCenter = new Vector3(camPos.x - camWidth / 2f - spawnDistanceFromCamera, camPos.y, 0f);
            Gizmos.DrawWireCube(leftCenter, new Vector3(0.5f, camHeight, 0.1f));
            
            // Right
            Vector3 rightCenter = new Vector3(camPos.x + camWidth / 2f + spawnDistanceFromCamera, camPos.y, 0f);
            Gizmos.DrawWireCube(rightCenter, new Vector3(0.5f, camHeight, 0.1f));

            // Draw manual bounds if enabled
            if (useManualBounds)
            {
                Gizmos.color = Color.cyan;
                float width = boundsMaxX - boundsMinX;
                float height = boundsMaxY - boundsMinY;
                Vector3 center = new Vector3((boundsMinX + boundsMaxX) / 2f, (boundsMinY + boundsMaxY) / 2f, 0f);
                Gizmos.DrawWireCube(center, new Vector3(width, height, 0.1f));
            }

            // Draw player range
            if (player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(player.position, maxDistanceFromPlayer);
            }
        }
    }
}

