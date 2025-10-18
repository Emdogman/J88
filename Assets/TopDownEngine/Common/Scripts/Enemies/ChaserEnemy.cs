using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Enemy that chases the player using tag-based detection
    /// Includes enemy avoidance behavior to prevent clustering
    /// </summary>
    [AddComponentMenu("TopDown Engine/Enemies/Chaser Enemy")]
    public class ChaserEnemy : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("How fast the enemy moves")]
        [SerializeField] private float moveSpeed = 3f;
        
        [Tooltip("How fast the enemy rotates to face movement direction")]
        [SerializeField] private float rotationSpeed = 180f;
        
        [Tooltip("Radius for enemy avoidance behavior")]
        [SerializeField] private float avoidanceRadius = 0.5f;
        
        [Tooltip("Strength of avoidance force")]
        [SerializeField] private float avoidanceStrength = 0.3f;
        
        [Tooltip("Layer mask for other enemies to avoid")]
        [SerializeField] private LayerMask enemyLayerMask = -1;

        [Header("Attack Settings")]
        [Tooltip("Enable continuous movement while attacking")]
        [SerializeField] private bool attackWhileMoving = true;
        
        [Tooltip("Attack range - how close to get before attacking")]
        [SerializeField] private float attackRange = 1.5f;
        
        [Tooltip("Attack damage")]
        [SerializeField] private float attackDamage = 10f;
        
        [Tooltip("Attack cooldown in seconds")]
        [SerializeField] private float attackCooldown = 1f;
        
        [Tooltip("Layer mask for what the enemy can attack")]
        [SerializeField] private LayerMask attackLayerMask = -1;

        [Header("References")]
        [Tooltip("The player transform to chase")]
        [SerializeField] private Transform player;
        
        [Tooltip("Transform that will be rotated to face movement direction (auto-assigned if null)")]
        [SerializeField] private Transform baseEnemyTransform; 
        
        [Tooltip("Rigidbody2D for physics movement (auto-assigned if null)")]
        [SerializeField] private Rigidbody2D rb;

        [Header("Player Detection")]
        [Tooltip("Tag to search for when finding the player")]
        [SerializeField] private string playerTag = "Player";
        
        [Tooltip("How often to search for player if not found (seconds)")]
        [SerializeField] private float playerSearchInterval = 1f;

        private Vector2 _movement;
        private readonly Collider2D[] _enemyHits = new Collider2D[32];
        private ContactFilter2D _enemyFilter;
        private float _lastPlayerSearchTime;

        private void Awake()
        {
            // Auto-assign references if not set
            AutoAssignReferences();
            
            // Setup enemy avoidance filter
            _enemyFilter = new ContactFilter2D
            {
                useTriggers = true 
            };
            _enemyFilter.SetLayerMask(enemyLayerMask);
        }

        private void Start()
        {
            // Try to find player immediately
            FindPlayer();
        }

        private void Update()
        {
            // Check if we need to find the player
            if (player == null)
            {
                if (Time.time - _lastPlayerSearchTime > playerSearchInterval)
                {
                    FindPlayer();
                    _lastPlayerSearchTime = Time.time;
                }
                return;
            }
            
            CalculateMovement();
        }

        private void FixedUpdate()
        {
            if (player == null) return;
            
            MoveEnemy();
            RotateTowardsMovement();
        }

        /// <summary>
        /// Automatically assigns required component references
        /// </summary>
        private void AutoAssignReferences()
        {
            // Auto-assign Rigidbody2D if not set
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    Debug.LogError($"ChaserEnemy: No Rigidbody2D found on {gameObject.name}. Please add a Rigidbody2D component.");
                }
                else if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Auto-assigned Rigidbody2D on {gameObject.name}");
                }
            }
            
            // Auto-assign baseEnemyTransform if not set
            if (baseEnemyTransform == null)
            {
                baseEnemyTransform = transform;
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Auto-assigned baseEnemyTransform to {gameObject.name}.transform");
                }
            }
            
            // Validate required components
            ValidateComponents();
        }

        /// <summary>
        /// Validates that all required components are present
        /// </summary>
        private void ValidateComponents()
        {
            bool hasErrors = false;
            
            if (rb == null)
            {
                Debug.LogError($"ChaserEnemy: Rigidbody2D is required on {gameObject.name}");
                hasErrors = true;
            }
            
            if (baseEnemyTransform == null)
            {
                Debug.LogError($"ChaserEnemy: baseEnemyTransform is required on {gameObject.name}");
                hasErrors = true;
            }
            
            if (hasErrors)
            {
                Debug.LogError($"ChaserEnemy: Setup incomplete on {gameObject.name}. Please check the component requirements.");
            }
            else if (ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: All required components validated on {gameObject.name}");
            }
        }

        /// <summary>
        /// Finds the player using the specified tag
        /// </summary>
        private void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Found player '{playerObject.name}' with tag '{playerTag}'");
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.LogWarning($"ChaserEnemy: No GameObject found with tag '{playerTag}'");
                }
            }
        }

        /// <summary>
        /// Calculates movement direction towards player with enemy avoidance
        /// </summary>
        private void CalculateMovement()
        {
            var position = transform.position;
            var direction = ((Vector2)player.position - (Vector2)position).normalized;

            // Calculate avoidance from other enemies
            var avoidance = Vector2.zero;
            var hitCount = Physics2D.OverlapCircle(position, avoidanceRadius, _enemyFilter, _enemyHits);

            for (var i = 0; i < hitCount; i++)
            {
                var hit = _enemyHits[i];
                if (hit == null || hit.transform == transform) continue;

                var away = (Vector2)transform.position - (Vector2)hit.transform.position;
                var dist = away.magnitude;
                if (!(dist > 0f) || !(dist < avoidanceRadius)) continue;
                
                var distFactor = 1f - (dist / avoidanceRadius);
                avoidance += away / dist * distFactor;
            }

            // Combine player direction with avoidance
            var finalDir = (direction + avoidance * avoidanceStrength).normalized;
            _movement = finalDir;
        }

        /// <summary>
        /// Moves the enemy using Rigidbody2D
        /// </summary>
        private void MoveEnemy()
        {
            rb.linearVelocity = _movement * moveSpeed;
        }

        /// <summary>
        /// Rotates the enemy to face movement direction
        /// </summary>
        private void RotateTowardsMovement()
        {
            if (_movement == Vector2.zero) return;
            
            var targetAngle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg;
            var currentAngle = baseEnemyTransform.eulerAngles.z;
            var newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            baseEnemyTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        /// <summary>
        /// Manually set the player reference
        /// </summary>
        /// <param name="playerTransform">The player's transform</param>
        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
            if (ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: Player manually set to '{playerTransform.name}'");
            }
        }

        /// <summary>
        /// Force search for player (useful for debugging)
        /// </summary>
        [ContextMenu("Find Player")]
        public void ForceFindPlayer()
        {
            FindPlayer();
        }

        /// <summary>
        /// Manually assign all references (useful for debugging)
        /// </summary>
        [ContextMenu("Auto-Assign References")]
        public void ForceAutoAssignReferences()
        {
            AutoAssignReferences();
        }

        /// <summary>
        /// Debug info display
        /// </summary>
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool ShowDebugInfo = false;

        private void OnGUI()
        {
            if (ShowDebugInfo)
            {
                GUI.Label(new Rect(10, 10, 300, 20), $"Player: {(player != null ? player.name : "Not Found")}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Movement: {_movement}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Speed: {(rb != null ? rb.linearVelocity.magnitude.ToString("F2") : "No Rigidbody2D")}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Rigidbody2D: {(rb != null ? "Found" : "Missing")}");
                GUI.Label(new Rect(10, 90, 300, 20), $"Base Transform: {(baseEnemyTransform != null ? "Found" : "Missing")}");
            }
        }
    }
}
