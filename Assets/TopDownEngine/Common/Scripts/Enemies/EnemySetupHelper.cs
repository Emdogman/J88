using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Helper script to automatically set up a ChaserEnemy with all required components
    /// </summary>
    [System.Serializable]
    public class EnemySetupHelper : MonoBehaviour
    {
        [Header("Auto Setup Settings")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private float enemyRadius = 0.5f;
        [SerializeField] private string enemyLayerName = "Enemy";
        [SerializeField] private string playerTag = "Player";

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupChaserEnemy();
            }
        }

        /// <summary>
        /// Automatically sets up a ChaserEnemy with all required components
        /// </summary>
        [ContextMenu("Setup Chaser Enemy")]
        public void SetupChaserEnemy()
        {
            // Add Rigidbody2D if not present
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.linearDamping = 1f;
                rb.angularDamping = 1f;
                Debug.Log("Added Rigidbody2D to " + gameObject.name);
            }

            // Add CircleCollider2D if not present
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
                collider.radius = enemyRadius;
                collider.isTrigger = false;
                Debug.Log("Added CircleCollider2D to " + gameObject.name);
            }

            // Add SpriteRenderer if not present
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/DefaultEnemy"); // You can change this
                spriteRenderer.sortingLayerName = "Default";
                spriteRenderer.sortingOrder = 0;
                Debug.Log("Added SpriteRenderer to " + gameObject.name);
            }

            // Add ChaserEnemy script if not present
            ChaserEnemy chaserEnemy = GetComponent<ChaserEnemy>();
            if (chaserEnemy == null)
            {
                chaserEnemy = gameObject.AddComponent<ChaserEnemy>();
                Debug.Log("Added ChaserEnemy script to " + gameObject.name);
            }

            // Set up layer
            SetupLayer();

            // Configure ChaserEnemy references
            ConfigureChaserEnemy(chaserEnemy, rb);

            // Try to find and assign player
            FindAndAssignPlayer(chaserEnemy);

            Debug.Log("ChaserEnemy setup complete for " + gameObject.name);
        }

        /// <summary>
        /// Sets up the enemy layer
        /// </summary>
        private void SetupLayer()
        {
            // Try to set the layer
            int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
            if (enemyLayer != -1)
            {
                gameObject.layer = enemyLayer;
                Debug.Log("Set layer to " + enemyLayerName);
            }
            else
            {
                Debug.LogWarning("Layer '" + enemyLayerName + "' not found. Please create it in Tags and Layers.");
            }
        }

        /// <summary>
        /// Configures the ChaserEnemy script with proper references
        /// </summary>
        private void ConfigureChaserEnemy(ChaserEnemy chaserEnemy, Rigidbody2D rb)
        {
            // Use reflection to set private fields (since they're serialized)
            var chaserType = typeof(ChaserEnemy);
            
            // Set baseEnemyTransform
            var baseEnemyTransformField = chaserType.GetField("baseEnemyTransform", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (baseEnemyTransformField != null)
            {
                baseEnemyTransformField.SetValue(chaserEnemy, transform);
            }

            // Set rb
            var rbField = chaserType.GetField("rb", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (rbField != null)
            {
                rbField.SetValue(chaserEnemy, rb);
            }

            // Set enemyLayerMask
            var enemyLayerMaskField = chaserType.GetField("enemyLayerMask", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (enemyLayerMaskField != null)
            {
                int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
                if (enemyLayer != -1)
                {
                    LayerMask layerMask = 1 << enemyLayer;
                    enemyLayerMaskField.SetValue(chaserEnemy, layerMask);
                }
            }

            Debug.Log("Configured ChaserEnemy references");
        }

        /// <summary>
        /// Finds and assigns the player to the ChaserEnemy
        /// </summary>
        private void FindAndAssignPlayer(ChaserEnemy chaserEnemy)
        {
            // Try to find player by tag
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                chaserEnemy.SetPlayer(playerObject.transform);
                Debug.Log($"EnemySetupHelper: Found and assigned player '{playerObject.name}'");
            }
            else
            {
                Debug.LogWarning($"EnemySetupHelper: No GameObject with '{playerTag}' tag found. Enemy will try to find player automatically.");
            }
        }

        /// <summary>
        /// Creates a complete enemy prefab setup
        /// </summary>
        [ContextMenu("Create Complete Enemy Setup")]
        public void CreateCompleteEnemySetup()
        {
            // Create a new GameObject
            GameObject enemy = new GameObject("ChaserEnemy");
            enemy.transform.position = Vector3.zero;

            // Add this helper script
            EnemySetupHelper helper = enemy.AddComponent<EnemySetupHelper>();
            helper.autoSetupOnStart = false; // We'll do it manually
            helper.enemyRadius = enemyRadius;
            helper.enemyLayerName = enemyLayerName;
            helper.playerTag = playerTag;

            // Run the setup
            helper.SetupChaserEnemy();

            Debug.Log("Complete enemy setup created: " + enemy.name);
        }
    }
}
