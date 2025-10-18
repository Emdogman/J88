using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to ensure enemies have proper Health and Character components for taking damage
    /// </summary>
    public class EnemyHealthSetupHelper : MonoBehaviour
    {
        [Header("Enemy Setup")]
        [Tooltip("Layer to set enemies to (9 = Enemies layer)")]
        public int enemyLayer = 9;
        
        [Tooltip("Health amount for enemies")]
        public float enemyHealth = 50f;
        
        [Tooltip("Collider radius for enemies")]
        public float colliderRadius = 0.5f;

        /// <summary>
        /// Sets up the current GameObject as a proper enemy that can take damage
        /// </summary>
        [ContextMenu("Setup Enemy Health")]
        public void SetupEnemyHealth()
        {
            GameObject enemy = gameObject;
            
            // Set layer to Enemies (Layer 9)
            enemy.layer = enemyLayer;
            
            // Add Character component if missing
            Character character = enemy.GetComponent<Character>();
            if (character == null)
            {
                character = enemy.AddComponent<Character>();
                Debug.Log($"EnemyHealthSetupHelper: Added Character component to {enemy.name}");
            }
            
            // Add Health component if missing
            Health health = enemy.GetComponent<Health>();
            if (health == null)
            {
                health = enemy.AddComponent<Health>();
                Debug.Log($"EnemyHealthSetupHelper: Added Health component to {enemy.name}");
            }
            
            // Set health values
            health.MaximumHealth = enemyHealth;
            health.CurrentHealth = enemyHealth;
            
            // Ensure we have a non-trigger collider for damage detection
            CircleCollider2D collider = enemy.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = enemy.AddComponent<CircleCollider2D>();
            }
            collider.radius = colliderRadius;
            collider.isTrigger = false; // Important: must be non-trigger for damage detection
            
            // Add Rigidbody2D if missing
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = enemy.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
            }
            
            Debug.Log($"EnemyHealthSetupHelper: Setup complete for {enemy.name} on Layer {enemyLayer}");
        }
        
        /// <summary>
        /// Sets up all ChaserEnemy GameObjects in the scene
        /// </summary>
        [ContextMenu("Setup All Enemies in Scene")]
        public void SetupAllEnemiesInScene()
        {
            ChaserEnemy[] enemies = FindObjectsOfType<ChaserEnemy>();
            
            foreach (ChaserEnemy enemy in enemies)
            {
                EnemyHealthSetupHelper helper = enemy.GetComponent<EnemyHealthSetupHelper>();
                if (helper == null)
                {
                    helper = enemy.gameObject.AddComponent<EnemyHealthSetupHelper>();
                }
                helper.enemyLayer = this.enemyLayer;
                helper.enemyHealth = this.enemyHealth;
                helper.colliderRadius = this.colliderRadius;
                helper.SetupEnemyHealth();
            }
            
            Debug.Log($"EnemyHealthSetupHelper: Setup complete for {enemies.Length} enemies in scene");
        }
    }
}