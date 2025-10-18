using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Enemy spawner that spawns enemies outside camera view with increasing difficulty over time
    /// </summary>
    [AddComponentMenu("TopDown Engine/Enemies/Enemy Spawner")]
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Prefab to spawn (ChaserEnemy)")]
        public GameObject enemyPrefab;
        
        [Tooltip("Initial time between spawns in seconds")]
        public float initialSpawnInterval = 5f;
        
        [Tooltip("Minimum time between spawns in seconds")]
        public float minSpawnInterval = 1f;
        
        [Tooltip("How fast the spawn rate increases (0.1 = 10% faster every difficulty increase)")]
        [Range(0f, 1f)]
        public float spawnRateIncrease = 0.1f;
        
        [Tooltip("Maximum number of concurrent enemies (0 = unlimited)")]
        public int maxEnemies = 0;
        
        [Tooltip("Enable/disable spawning")]
        public bool isSpawning = true;

        [Header("Spawn Position")]
        [Tooltip("Distance from camera edge to spawn enemies")]
        public float spawnDistanceFromCamera = 2f;
        
        [Tooltip("Extra padding around camera bounds")]
        public Vector2 spawnAreaPadding = new Vector2(1f, 1f);
        
        [Tooltip("Spawn from all sides or specific sides")]
        public bool spawnOnAllSides = true;
        
        [Tooltip("Which sides to spawn from (if not all sides)")]
        public SpawnSide[] allowedSpawnSides = { SpawnSide.Top, SpawnSide.Bottom, SpawnSide.Left, SpawnSide.Right };

        [Header("Manual Spawn Area Restriction")]
        [Tooltip("Enable manual spawn area restriction (clamps spawn positions to defined bounds)")]
        public bool useManualSpawnArea = false;
        
        [Tooltip("Minimum X coordinate where enemies can spawn")]
        public float spawnAreaMinX = -20f;
        
        [Tooltip("Maximum X coordinate where enemies can spawn")]
        public float spawnAreaMaxX = 20f;
        
        [Tooltip("Minimum Y coordinate where enemies can spawn")]
        public float spawnAreaMinY = -20f;
        
        [Tooltip("Maximum Y coordinate where enemies can spawn")]
        public float spawnAreaMaxY = 20f;

        [Header("Difficulty Scaling")]
        [Tooltip("Enable difficulty scaling over time")]
        public bool enableDifficultyScaling = true;
        
        [Tooltip("Time between difficulty increases in seconds")]
        public float difficultyIncreaseInterval = 30f;
        
        [Tooltip("Custom curve for spawn rate (optional)")]
        public AnimationCurve spawnRateCurve;

        [Header("References")]
        [Tooltip("Main camera (auto-finds Camera.main if null)")]
        public Camera mainCamera;
        
        [Tooltip("Parent for spawned enemies (optional)")]
        public Transform spawnParent;
        
        [Tooltip("Layer mask for obstacles/walls to avoid spawning in")]
        public LayerMask obstacleLayerMask = 1 << 8; // Default to Obstacles layer

        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;
        
        [Tooltip("Show spawn zones in Scene view")]
        public bool showSpawnZones = true;

        public enum SpawnSide
        {
            Top,
            Bottom,
            Left,
            Right
        }

        // Private variables
        private float _currentSpawnInterval;
        private float _gameStartTime;
        private int _activeEnemyCount;
        private List<GameObject> _spawnedEnemies = new List<GameObject>();
        private Coroutine _spawnCoroutine;
        private float _lastDifficultyIncrease;

        private void Start()
        {
            InitializeSpawner();
            StartSpawning();
        }

        private void Update()
        {
            if (enableDifficultyScaling)
            {
                UpdateDifficulty();
            }
        }

        /// <summary>
        /// Initialize the spawner
        /// </summary>
        private void InitializeSpawner()
        {
            // Auto-find camera if not assigned
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                }
            }

            if (mainCamera == null)
            {
                Debug.LogError("EnemySpawner: No camera found! Please assign a camera or ensure Camera.main exists.");
                return;
            }

            // Initialize spawn settings
            _currentSpawnInterval = initialSpawnInterval;
            _gameStartTime = Time.time;
            _lastDifficultyIncrease = _gameStartTime;

            // Create spawn parent if not assigned
            if (spawnParent == null)
            {
                GameObject spawnParentObj = new GameObject("Spawned Enemies");
                spawnParent = spawnParentObj.transform;
            }

            Debug.Log($"EnemySpawner initialized - Initial spawn interval: {_currentSpawnInterval}s");
        }

        /// <summary>
        /// Start the spawning coroutine
        /// </summary>
        private void StartSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _spawnCoroutine = StartCoroutine(SpawnLoop());
        }

        /// <summary>
        /// Stop spawning
        /// </summary>
        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            isSpawning = false;
        }

        /// <summary>
        /// Main spawn loop coroutine
        /// </summary>
        private IEnumerator SpawnLoop()
        {
            while (isSpawning)
            {
                yield return new WaitForSeconds(_currentSpawnInterval);
                
                if (CanSpawnEnemy())
                {
                    SpawnEnemy();
                }
            }
        }

        /// <summary>
        /// Check if we can spawn an enemy
        /// </summary>
        private bool CanSpawnEnemy()
        {
            if (enemyPrefab == null) return false;
            if (maxEnemies > 0 && _activeEnemyCount >= maxEnemies) return false;
            return true;
        }

        /// <summary>
        /// Spawn a single enemy
        /// </summary>
        private void SpawnEnemy()
        {
            Vector2 spawnPosition = CalculateSpawnPosition();
            
            if (spawnPosition == Vector2.zero)
            {
                Debug.LogWarning("EnemySpawner: Could not find valid spawn position!");
                return;
            }

            // Instantiate enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, spawnParent);
            _spawnedEnemies.Add(enemy);
            _activeEnemyCount++;

            // Clean up destroyed enemies from list
            _spawnedEnemies.RemoveAll(go => go == null);
            _activeEnemyCount = _spawnedEnemies.Count;

            if (showDebugInfo)
            {
                Debug.Log($"EnemySpawner: Spawned enemy at {spawnPosition}. Active enemies: {_activeEnemyCount}");
            }
        }

        /// <summary>
        /// Calculate spawn position outside camera view and within manual bounds
        /// </summary>
        private Vector2 CalculateSpawnPosition()
        {
            if (mainCamera == null) return Vector2.zero;

            // Get camera bounds in world space
            Vector2 cameraBoundsMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector2 cameraBoundsMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            // Add padding to bounds
            cameraBoundsMin -= spawnAreaPadding;
            cameraBoundsMax += spawnAreaPadding;

            // If using manual spawn area, restrict the random range to stay within bounds
            if (useManualSpawnArea)
            {
                cameraBoundsMin.x = Mathf.Max(cameraBoundsMin.x, spawnAreaMinX);
                cameraBoundsMin.y = Mathf.Max(cameraBoundsMin.y, spawnAreaMinY);
                cameraBoundsMax.x = Mathf.Min(cameraBoundsMax.x, spawnAreaMaxX);
                cameraBoundsMax.y = Mathf.Min(cameraBoundsMax.y, spawnAreaMaxY);
            }

            // Try multiple times to find a valid spawn position
            int maxAttempts = 30;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Choose spawn side
                SpawnSide spawnSide = ChooseSpawnSide();
                
                // Calculate position based on side
                Vector2 spawnPosition = CalculatePositionForSide(spawnSide, cameraBoundsMin, cameraBoundsMax);
                
                // Final clamp to ensure within manual bounds (safety check)
                if (useManualSpawnArea)
                {
                    spawnPosition.x = Mathf.Clamp(spawnPosition.x, spawnAreaMinX, spawnAreaMaxX);
                    spawnPosition.y = Mathf.Clamp(spawnPosition.y, spawnAreaMinY, spawnAreaMaxY);
                }
                
                // Verify position is still outside camera view (after clamping)
                if (!IsPositionOutsideCameraView(spawnPosition))
                {
                    continue; // Try again if clamping brought it into view
                }
                
                // Check if position is valid (not on obstacle/wall)
                if (IsValidSpawnPosition(spawnPosition))
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"EnemySpawner: Valid spawn position found at {spawnPosition} after {attempt + 1} attempts");
                    }
                    return spawnPosition;
                }
            }
            
            // If we couldn't find a valid position after max attempts
            if (showDebugInfo)
            {
                Debug.LogWarning("EnemySpawner: Could not find valid spawn position after max attempts");
            }
            
            return Vector2.zero;
        }

        /// <summary>
        /// Calculate spawn position for a specific side
        /// </summary>
        private Vector2 CalculatePositionForSide(SpawnSide spawnSide, Vector2 cameraBoundsMin, Vector2 cameraBoundsMax)
        {
            Vector2 spawnPosition = Vector2.zero;
            
            switch (spawnSide)
            {
                case SpawnSide.Top:
                    spawnPosition = new Vector2(
                        Random.Range(cameraBoundsMin.x, cameraBoundsMax.x),
                        cameraBoundsMax.y + spawnDistanceFromCamera
                    );
                    break;
                    
                case SpawnSide.Bottom:
                    spawnPosition = new Vector2(
                        Random.Range(cameraBoundsMin.x, cameraBoundsMax.x),
                        cameraBoundsMin.y - spawnDistanceFromCamera
                    );
                    break;
                    
                case SpawnSide.Left:
                    spawnPosition = new Vector2(
                        cameraBoundsMin.x - spawnDistanceFromCamera,
                        Random.Range(cameraBoundsMin.y, cameraBoundsMax.y)
                    );
                    break;
                    
                case SpawnSide.Right:
                    spawnPosition = new Vector2(
                        cameraBoundsMax.x + spawnDistanceFromCamera,
                        Random.Range(cameraBoundsMin.y, cameraBoundsMax.y)
                    );
                    break;
            }
            
            return spawnPosition;
        }

        /// <summary>
        /// Check if a position is outside the camera view
        /// </summary>
        private bool IsPositionOutsideCameraView(Vector2 position)
        {
            if (mainCamera == null) return true;
            
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
            
            // Position is outside if viewport coordinates are < 0 or > 1
            return viewportPoint.x < 0 || viewportPoint.x > 1 || 
                   viewportPoint.y < 0 || viewportPoint.y > 1;
        }

        /// <summary>
        /// Check if a spawn position is valid (not overlapping with obstacles/walls)
        /// </summary>
        private bool IsValidSpawnPosition(Vector2 position)
        {
            // Check if position overlaps with any obstacles
            Collider2D hit = Physics2D.OverlapCircle(position, 0.5f, obstacleLayerMask);
            
            if (hit != null)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"EnemySpawner: Position {position} overlaps with obstacle: {hit.name}");
                }
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Choose which side to spawn from
        /// </summary>
        private SpawnSide ChooseSpawnSide()
        {
            if (spawnOnAllSides)
            {
                return (SpawnSide)Random.Range(0, 4);
            }
            else
            {
                return allowedSpawnSides[Random.Range(0, allowedSpawnSides.Length)];
            }
        }

        /// <summary>
        /// Update difficulty scaling
        /// </summary>
        private void UpdateDifficulty()
        {
            float currentTime = Time.time;
            
            if (currentTime - _lastDifficultyIncrease >= difficultyIncreaseInterval)
            {
                IncreaseDifficulty();
                _lastDifficultyIncrease = currentTime;
            }
        }

        /// <summary>
        /// Increase spawn rate (decrease interval)
        /// </summary>
        private void IncreaseDifficulty()
        {
            float oldInterval = _currentSpawnInterval;
            
            if (spawnRateCurve != null && spawnRateCurve.length > 0)
            {
                // Use custom curve
                float timeProgress = (Time.time - _gameStartTime) / 300f; // 5 minutes max
                float curveValue = spawnRateCurve.Evaluate(timeProgress);
                _currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, curveValue);
            }
            else
            {
                // Use linear decrease
                _currentSpawnInterval *= (1f - spawnRateIncrease);
            }
            
            // Clamp to minimum
            _currentSpawnInterval = Mathf.Max(_currentSpawnInterval, minSpawnInterval);
            
            if (showDebugInfo)
            {
                Debug.Log($"EnemySpawner: Difficulty increased! Spawn interval: {oldInterval:F2}s -> {_currentSpawnInterval:F2}s");
            }
        }

        /// <summary>
        /// Get camera bounds for visualization
        /// </summary>
        private Vector2[] GetCameraBounds()
        {
            if (mainCamera == null) return new Vector2[0];

            Vector2 cameraBoundsMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector2 cameraBoundsMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            return new Vector2[]
            {
                cameraBoundsMin - spawnAreaPadding,
                cameraBoundsMax + spawnAreaPadding
            };
        }

        /// <summary>
        /// Draw spawn zones in Scene view
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showSpawnZones || mainCamera == null) return;

            Vector2[] bounds = GetCameraBounds();
            if (bounds.Length < 2) return;

            Vector2 min = bounds[0];
            Vector2 max = bounds[1];

            // Draw camera view bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((min + max) / 2, max - min);

            // Draw spawn zones
            Gizmos.color = Color.red;
            
            // Top spawn zone
            Vector2 topCenter = new Vector2((min.x + max.x) / 2, max.y + spawnDistanceFromCamera / 2);
            Vector2 topSize = new Vector2(max.x - min.x, spawnDistanceFromCamera);
            Gizmos.DrawWireCube(topCenter, topSize);
            
            // Bottom spawn zone
            Vector2 bottomCenter = new Vector2((min.x + max.x) / 2, min.y - spawnDistanceFromCamera / 2);
            Vector2 bottomSize = new Vector2(max.x - min.x, spawnDistanceFromCamera);
            Gizmos.DrawWireCube(bottomCenter, bottomSize);
            
            // Left spawn zone
            Vector2 leftCenter = new Vector2(min.x - spawnDistanceFromCamera / 2, (min.y + max.y) / 2);
            Vector2 leftSize = new Vector2(spawnDistanceFromCamera, max.y - min.y);
            Gizmos.DrawWireCube(leftCenter, leftSize);
            
            // Right spawn zone
            Vector2 rightCenter = new Vector2(max.x + spawnDistanceFromCamera / 2, (min.y + max.y) / 2);
            Vector2 rightSize = new Vector2(spawnDistanceFromCamera, max.y - min.y);
            Gizmos.DrawWireCube(rightCenter, rightSize);
            
            // Draw manual spawn area restriction if enabled
            if (useManualSpawnArea)
            {
                Gizmos.color = Color.green;
                Vector2 manualCenter = new Vector2(
                    (spawnAreaMinX + spawnAreaMaxX) / 2,
                    (spawnAreaMinY + spawnAreaMaxY) / 2
                );
                Vector2 manualSize = new Vector2(
                    spawnAreaMaxX - spawnAreaMinX,
                    spawnAreaMaxY - spawnAreaMinY
                );
                Gizmos.DrawWireCube(manualCenter, manualSize);
                
                // Draw corners for clarity
                Gizmos.DrawWireSphere(new Vector2(spawnAreaMinX, spawnAreaMinY), 0.5f);
                Gizmos.DrawWireSphere(new Vector2(spawnAreaMaxX, spawnAreaMinY), 0.5f);
                Gizmos.DrawWireSphere(new Vector2(spawnAreaMinX, spawnAreaMaxY), 0.5f);
                Gizmos.DrawWireSphere(new Vector2(spawnAreaMaxX, spawnAreaMaxY), 0.5f);
            }
        }

    }
}
