using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script for easy EnemySpawner setup
    /// </summary>
    public class EnemySpawnerSetupHelper : MonoBehaviour
    {
        [Header("Auto Setup")]
        [Tooltip("Automatically find and assign ChaserEnemy prefab")]
        public bool autoFindEnemyPrefab = true;
        
        [Tooltip("Automatically find main camera")]
        public bool autoFindCamera = true;
        
        [Tooltip("Create spawn parent GameObject")]
        public bool createSpawnParent = true;

        /// <summary>
        /// Sets up the EnemySpawner with default values
        /// </summary>
        [ContextMenu("Setup Enemy Spawner")]
        public void SetupEnemySpawner()
        {
            EnemySpawner spawner = GetComponent<EnemySpawner>();
            if (spawner == null)
            {
                Debug.LogError("EnemySpawnerSetupHelper: No EnemySpawner component found on this GameObject!");
                return;
            }

            // Auto-find camera
            if (autoFindCamera && spawner.mainCamera == null)
            {
                spawner.mainCamera = Camera.main;
                if (spawner.mainCamera == null)
                {
                    spawner.mainCamera = FindObjectOfType<Camera>();
                }
                Debug.Log($"EnemySpawnerSetupHelper: Auto-assigned camera: {spawner.mainCamera?.name}");
            }

            // Auto-find enemy prefab
            if (autoFindEnemyPrefab && spawner.enemyPrefab == null)
            {
                // Look for ChaserEnemy prefab in Resources or common locations
                GameObject enemyPrefab = Resources.Load<GameObject>("ChaserEnemy");
                if (enemyPrefab == null)
                {
                    // Try to find any GameObject with ChaserEnemy component
                    ChaserEnemy[] existingEnemies = FindObjectsOfType<ChaserEnemy>();
                    if (existingEnemies.Length > 0)
                    {
                        Debug.LogWarning("EnemySpawnerSetupHelper: Found ChaserEnemy in scene but no prefab. Please manually assign the ChaserEnemy prefab.");
                    }
                    else
                    {
                        Debug.LogWarning("EnemySpawnerSetupHelper: No ChaserEnemy prefab found. Please create a ChaserEnemy prefab and assign it manually.");
                    }
                }
                else
                {
                    spawner.enemyPrefab = enemyPrefab;
                    Debug.Log($"EnemySpawnerSetupHelper: Auto-assigned enemy prefab: {enemyPrefab.name}");
                }
            }

            // Create spawn parent
            if (createSpawnParent && spawner.spawnParent == null)
            {
                GameObject spawnParentObj = new GameObject("Spawned Enemies");
                spawnParentObj.transform.SetParent(transform);
                spawner.spawnParent = spawnParentObj.transform;
                Debug.Log("EnemySpawnerSetupHelper: Created spawn parent GameObject");
            }

            // Set default values if not already set
            SetDefaultValues(spawner);

            Debug.Log("EnemySpawnerSetupHelper: Setup complete!");
        }

        /// <summary>
        /// Sets default values for the spawner
        /// </summary>
        private void SetDefaultValues(EnemySpawner spawner)
        {
            // Only set values if they haven't been customized
            if (spawner.initialSpawnInterval == 5f) // Default value
            {
                spawner.initialSpawnInterval = 5f;
                spawner.minSpawnInterval = 1f;
                spawner.spawnRateIncrease = 0.1f;
                spawner.spawnDistanceFromCamera = 2f;
                spawner.spawnAreaPadding = new Vector2(1f, 1f);
                spawner.enableDifficultyScaling = true;
                spawner.difficultyIncreaseInterval = 30f;
                spawner.showDebugInfo = true;
                spawner.showSpawnZones = true;
                
                Debug.Log("EnemySpawnerSetupHelper: Applied default spawner values");
            }
        }

        /// <summary>
        /// Validates the spawner setup
        /// </summary>
        [ContextMenu("Validate Setup")]
        public void ValidateSetup()
        {
            EnemySpawner spawner = GetComponent<EnemySpawner>();
            if (spawner == null)
            {
                Debug.LogError("EnemySpawnerSetupHelper: No EnemySpawner component found!");
                return;
            }

            bool isValid = true;

            // Check required components
            if (spawner.enemyPrefab == null)
            {
                Debug.LogError("EnemySpawnerSetupHelper: Enemy Prefab is not assigned!");
                isValid = false;
            }

            if (spawner.mainCamera == null)
            {
                Debug.LogError("EnemySpawnerSetupHelper: Main Camera is not assigned!");
                isValid = false;
            }

            // Check values
            if (spawner.initialSpawnInterval <= 0)
            {
                Debug.LogError("EnemySpawnerSetupHelper: Initial Spawn Interval must be greater than 0!");
                isValid = false;
            }

            if (spawner.minSpawnInterval <= 0)
            {
                Debug.LogError("EnemySpawnerSetupHelper: Min Spawn Interval must be greater than 0!");
                isValid = false;
            }

            if (spawner.minSpawnInterval >= spawner.initialSpawnInterval)
            {
                Debug.LogWarning("EnemySpawnerSetupHelper: Min Spawn Interval should be less than Initial Spawn Interval for difficulty scaling to work!");
            }

            if (spawner.spawnDistanceFromCamera <= 0)
            {
                Debug.LogError("EnemySpawnerSetupHelper: Spawn Distance From Camera must be greater than 0!");
                isValid = false;
            }

            if (isValid)
            {
                Debug.Log("EnemySpawnerSetupHelper: Setup validation passed!");
            }
            else
            {
                Debug.LogError("EnemySpawnerSetupHelper: Setup validation failed! Please fix the issues above.");
            }
        }

        /// <summary>
        /// Creates a complete spawner setup
        /// </summary>
        [ContextMenu("Create Complete Spawner")]
        public void CreateCompleteSpawner()
        {
            // Create spawner GameObject
            GameObject spawnerObj = new GameObject("Enemy Spawner");
            spawnerObj.transform.position = Vector3.zero;

            // Add EnemySpawner component
            EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();

            // Add this helper component
            EnemySpawnerSetupHelper helper = spawnerObj.AddComponent<EnemySpawnerSetupHelper>();

            // Run setup
            helper.SetupEnemySpawner();

            Debug.Log("EnemySpawnerSetupHelper: Created complete spawner setup!");
        }
    }
}
