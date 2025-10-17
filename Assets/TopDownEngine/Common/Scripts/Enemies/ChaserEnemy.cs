using UnityEngine;

namespace Enemy
{
    public class ChaserEnemy : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float avoidanceRadius = 0.5f;
        [SerializeField] private float avoidanceStrength = 0.3f;
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform baseEnemyTransform; 
        [SerializeField] private Rigidbody2D rb;

        private Vector2 _movement;
        private readonly Collider2D[] _enemyHits = new Collider2D[32];
        private ContactFilter2D _enemyFilter;

        private void Awake()
        {
            _enemyFilter = new ContactFilter2D
            {
                useTriggers = true 
            };
            
            _enemyFilter.SetLayerMask(enemyLayerMask);
        }

        private void Start()
        {
            FindPlayer();
        }

        /// <summary>
        /// Finds the player by tag with retry mechanism
        /// </summary>
        private void FindPlayer()
        {
            // Try to find player by tag
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log($"ChaserEnemy: Found player '{playerObject.name}' with tag 'Player'");
            }
            else
            {
                Debug.LogWarning("ChaserEnemy: No GameObject with 'Player' tag found! Will retry...");
                // Retry after a short delay in case player spawns later
                Invoke(nameof(RetryFindPlayer), 0.5f);
            }
        }

        /// <summary>
        /// Retries finding the player after a delay
        /// </summary>
        private void RetryFindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log($"ChaserEnemy: Found player '{playerObject.name}' on retry");
            }
            else
            {
                Debug.LogError("ChaserEnemy: Still no player found! Make sure your player has the 'Player' tag.");
            }
        }

        private void Update()
        {
            // If player is still null, try to find it again
            if (player == null)
            {
                FindPlayer();
                return;
            }
            
            CalculateMovement();
        }

        private void FixedUpdate()
        {
            MoveEnemy();
            RotateTowardsMovement();
        }

        private void CalculateMovement()
        {
            var position = transform.position;
            var direction = ((Vector2)player.position - (Vector2)position).normalized;

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

            var finalDir = (direction + avoidance * avoidanceStrength).normalized;
            _movement = finalDir;
        }

        private void MoveEnemy()
        {
            rb.linearVelocity = _movement * moveSpeed;
        }

        private void RotateTowardsMovement()
        {
            if (_movement == Vector2.zero) return;
            
            var targetAngle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg;
            var currentAngle = baseEnemyTransform.eulerAngles.z;
            var newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            baseEnemyTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        /// <summary>
        /// Manually assigns the player reference
        /// </summary>
        /// <param name="playerTransform">The player's transform</param>
        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
            Debug.Log($"ChaserEnemy: Player manually assigned to '{playerTransform.name}'");
        }

        /// <summary>
        /// Context menu method to find and assign player
        /// </summary>
        [ContextMenu("Find Player")]
        private void FindPlayerContextMenu()
        {
            FindPlayer();
        }

        /// <summary>
        /// Context menu method to debug player reference
        /// </summary>
        [ContextMenu("Debug Player Reference")]
        private void DebugPlayerReference()
        {
            if (player != null)
            {
                Debug.Log($"ChaserEnemy: Player reference is valid - '{player.name}' at position {player.position}");
            }
            else
            {
                Debug.LogWarning("ChaserEnemy: Player reference is NULL!");
                
                // Try to find all GameObjects with Player tag
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                Debug.Log($"Found {players.Length} GameObjects with 'Player' tag:");
                for (int i = 0; i < players.Length; i++)
                {
                    Debug.Log($"  {i + 1}. {players[i].name} at {players[i].transform.position}");
                }
            }
        }
    }
}
