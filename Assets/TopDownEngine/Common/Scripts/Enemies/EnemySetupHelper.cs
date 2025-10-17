using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to automatically set up ChaserEnemy components
    /// Use this to quickly create enemies with proper configuration
    /// </summary>
    [AddComponentMenu("TopDown Engine/Enemies/Enemy Setup Helper")]
    public class EnemySetupHelper : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        [Tooltip("Name for the enemy GameObject")]
        public string enemyName = "ChaserEnemy";
        
        [Tooltip("Move speed for the enemy")]
        public float moveSpeed = 3f;
        
        [Tooltip("Rotation speed for the enemy")]
        public float rotationSpeed = 180f;
        
        [Tooltip("Avoidance radius for enemy clustering prevention")]
        public float avoidanceRadius = 0.5f;
        
        [Tooltip("Avoidance strength")]
        public float avoidanceStrength = 0.3f;
        
        [Tooltip("Player tag to search for")]
        public string playerTag = "Player";
        
        [Tooltip("Enemy layer mask")]
        public LayerMask enemyLayerMask = -1;

        [Header("Physics Settings")]
        [Tooltip("Linear drag for Rigidbody2D")]
        public float linearDrag = 1f;
        
        [Tooltip("Angular drag for Rigidbody2D")]
        public float angularDrag = 1f;
        
        [Tooltip("Collider radius")]
        public float colliderRadius = 0.5f;

        [Header("Debug")]
        [Tooltip("Show debug info for created enemy")]
        public bool showDebugInfo = true;

        /// <summary>
        /// Creates a complete ChaserEnemy setup
        /// </summary>
        [ContextMenu("Create ChaserEnemy")]
        public void CreateChaserEnemy()
        {
            // Create the enemy GameObject
            GameObject enemy = new GameObject(enemyName);
            
            // Add Transform (already exists)
            Transform enemyTransform = enemy.transform;
            
            // Add Rigidbody2D
            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearDamping = linearDrag;
            rb.angularDamping = angularDrag;
            rb.gravityScale = 0f; // Top-down game
            
            // Add CircleCollider2D
            CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
            collider.radius = colliderRadius;
            collider.isTrigger = false;
            
            // Add ChaserEnemy script
            ChaserEnemy chaserEnemy = enemy.AddComponent<ChaserEnemy>();
            
            // Configure ChaserEnemy using reflection to set private fields
            SetPrivateField(chaserEnemy, "moveSpeed", moveSpeed);
            SetPrivateField(chaserEnemy, "rotationSpeed", rotationSpeed);
            SetPrivateField(chaserEnemy, "avoidanceRadius", avoidanceRadius);
            SetPrivateField(chaserEnemy, "avoidanceStrength", avoidanceStrength);
            SetPrivateField(chaserEnemy, "enemyLayerMask", enemyLayerMask);
            SetPrivateField(chaserEnemy, "playerTag", playerTag);
            SetPrivateField(chaserEnemy, "ShowDebugInfo", showDebugInfo);
            
            // Try to find and assign player
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                SetPrivateField(chaserEnemy, "player", player.transform);
                Debug.Log($"EnemySetupHelper: Found and assigned player '{player.name}' to enemy");
            }
            else
            {
                Debug.LogWarning($"EnemySetupHelper: No GameObject found with tag '{playerTag}'. Enemy will search for player automatically.");
            }
            
            // Add SpriteRenderer for visual representation
            SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDefaultEnemySprite();
            spriteRenderer.color = Color.red;
            
            // Position enemy
            enemyTransform.position = transform.position;
            
            Debug.Log($"EnemySetupHelper: Created ChaserEnemy '{enemyName}' with all components configured");
        }

        /// <summary>
        /// Creates multiple enemies in a pattern
        /// </summary>
        [ContextMenu("Create Enemy Group")]
        public void CreateEnemyGroup()
        {
            Vector3[] positions = {
                transform.position + Vector3.left * 2f,
                transform.position + Vector3.right * 2f,
                transform.position + Vector3.up * 2f,
                transform.position + Vector3.down * 2f
            };
            
            for (int i = 0; i < positions.Length; i++)
            {
                string enemyName = $"{this.enemyName}_{i + 1}";
                CreateEnemyAtPosition(enemyName, positions[i]);
            }
            
            Debug.Log($"EnemySetupHelper: Created {positions.Length} enemies in group pattern");
        }

        /// <summary>
        /// Creates a single enemy at specified position
        /// </summary>
        private void CreateEnemyAtPosition(string name, Vector3 position)
        {
            GameObject enemy = new GameObject(name);
            
            // Add all required components
            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearDamping = linearDrag;
            rb.angularDamping = angularDrag;
            rb.gravityScale = 0f;
            
            CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
            collider.radius = colliderRadius;
            collider.isTrigger = false;
            
            ChaserEnemy chaserEnemy = enemy.AddComponent<ChaserEnemy>();
            SetPrivateField(chaserEnemy, "moveSpeed", moveSpeed);
            SetPrivateField(chaserEnemy, "rotationSpeed", rotationSpeed);
            SetPrivateField(chaserEnemy, "avoidanceRadius", avoidanceRadius);
            SetPrivateField(chaserEnemy, "avoidanceStrength", avoidanceStrength);
            SetPrivateField(chaserEnemy, "enemyLayerMask", enemyLayerMask);
            SetPrivateField(chaserEnemy, "playerTag", playerTag);
            SetPrivateField(chaserEnemy, "ShowDebugInfo", showDebugInfo);
            
            SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDefaultEnemySprite();
            spriteRenderer.color = Color.red;
            
            enemy.transform.position = position;
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
                Debug.LogWarning($"EnemySetupHelper: Could not find field '{fieldName}' in {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// Creates a simple default sprite for the enemy
        /// </summary>
        private Sprite CreateDefaultEnemySprite()
        {
            // Create a simple colored square texture
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.red;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Validates the setup configuration
        /// </summary>
        [ContextMenu("Validate Setup")]
        public void ValidateSetup()
        {
            bool isValid = true;
            
            if (string.IsNullOrEmpty(enemyName))
            {
                Debug.LogError("EnemySetupHelper: Enemy name cannot be empty");
                isValid = false;
            }
            
            if (moveSpeed <= 0)
            {
                Debug.LogError("EnemySetupHelper: Move speed must be greater than 0");
                isValid = false;
            }
            
            if (string.IsNullOrEmpty(playerTag))
            {
                Debug.LogError("EnemySetupHelper: Player tag cannot be empty");
                isValid = false;
            }
            
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player == null)
            {
                Debug.LogWarning($"EnemySetupHelper: No GameObject found with tag '{playerTag}'. Make sure your player has this tag.");
            }
            
            if (isValid)
            {
                Debug.Log("EnemySetupHelper: Setup configuration is valid");
            }
        }
    }
}
