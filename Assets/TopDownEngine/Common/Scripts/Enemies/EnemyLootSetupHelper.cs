using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to set up loot dropping for enemies
    /// </summary>
    public class EnemyLootSetupHelper : MonoBehaviour
    {
        [Header("Loot Configuration")]
        [Tooltip("KoalaCoinPicker prefab to assign to enemies")]
        public GameObject koalaCoinPickerPrefab;
        
        [Tooltip("Default drop rate for all enemies (0-1)")]
        [Range(0f, 1f)]
        public float defaultDropRate = 0.3f;
        
        [Tooltip("Default number of items to drop")]
        public int defaultDropAmount = 1;
        
        [Tooltip("Default random offset for drop position")]
        public float defaultDropOffset = 0.5f;

        /// <summary>
        /// Sets up loot dropping for the current GameObject
        /// </summary>
        [ContextMenu("Setup Enemy Loot")]
        public void SetupEnemyLoot()
        {
            ChaserEnemy chaserEnemy = GetComponent<ChaserEnemy>();
            if (chaserEnemy == null)
            {
                Debug.LogError("EnemyLootSetupHelper: No ChaserEnemy component found on this GameObject");
                return;
            }

            if (koalaCoinPickerPrefab == null)
            {
                Debug.LogError("EnemyLootSetupHelper: No KoalaCoinPicker prefab assigned. Please assign it in the inspector.");
                return;
            }

            // Use reflection to set private fields
            SetPrivateField(chaserEnemy, "dropPrefab", koalaCoinPickerPrefab);
            SetPrivateField(chaserEnemy, "dropRate", defaultDropRate);
            SetPrivateField(chaserEnemy, "dropAmount", defaultDropAmount);
            SetPrivateField(chaserEnemy, "dropOffset", defaultDropOffset);

            Debug.Log($"EnemyLootSetupHelper: Configured loot dropping for {gameObject.name}");
        }

        /// <summary>
        /// Sets up loot dropping for all ChaserEnemy GameObjects in the scene
        /// </summary>
        [ContextMenu("Setup All Enemies Loot")]
        public void SetupAllEnemiesLoot()
        {
            if (koalaCoinPickerPrefab == null)
            {
                Debug.LogError("EnemyLootSetupHelper: No KoalaCoinPicker prefab assigned. Please assign it in the inspector.");
                return;
            }

            ChaserEnemy[] enemies = FindObjectsOfType<ChaserEnemy>();
            int configuredCount = 0;

            foreach (ChaserEnemy enemy in enemies)
            {
                // Use reflection to set private fields
                SetPrivateField(enemy, "dropPrefab", koalaCoinPickerPrefab);
                SetPrivateField(enemy, "dropRate", defaultDropRate);
                SetPrivateField(enemy, "dropAmount", defaultDropAmount);
                SetPrivateField(enemy, "dropOffset", defaultDropOffset);
                configuredCount++;
            }

            Debug.Log($"EnemyLootSetupHelper: Configured loot dropping for {configuredCount} enemies");
        }

        /// <summary>
        /// Sets private fields using reflection
        /// </summary>
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                Debug.LogWarning($"EnemyLootSetupHelper: Could not find field '{fieldName}' in {obj.GetType().Name}");
            }
        }
    }
}
