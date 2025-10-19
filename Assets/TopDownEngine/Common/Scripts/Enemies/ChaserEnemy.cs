using UnityEngine;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simplified enemy AI with stable movement and clear attack patterns
    /// Moves toward player intelligently with proper distance management
    /// </summary>
    [AddComponentMenu("TopDown Engine/Enemies/Chaser Enemy")]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(Health))]
    public class ChaserEnemy : MonoBehaviour
    {
        [Header("Attack Behavior")]
        [Tooltip("Distance for default melee attack")]
        [SerializeField] private float enemy_Melee_Radius = 1.5f;
        
        [Tooltip("Distance within which charge attack can begin")]
        [SerializeField] private float enemy_Charge_Radius = 5f;
        
        [Tooltip("Time to telegraph before charge attack")]
        [SerializeField] private float enemy_Charge_Telegraph_Time = 1.5f;
        
        [Tooltip("Damage for default attack")]
        [SerializeField] private float default_Attack_Damage = 10f;
        
        [Tooltip("High damage for charge attack")]
        [SerializeField] private float enemy_Charge_Attack_Damage = 20f;
        
        [Tooltip("Cooldown between charge attacks in seconds")]
        [SerializeField] private float chargeCooldown = 4f;

        [Header("Movement Settings")]
        [Tooltip("Ideal distance to maintain from player")]
        [SerializeField] private float idealDistance = 3f;
        
        [Tooltip("Minimum distance from player (will back away if closer)")]
        [SerializeField] private float minDistance = 2f;
        
        [Tooltip("Maximum distance from player (will approach if further)")]
        [SerializeField] private float maxDistance = 6f;
        
        [Tooltip("How fast the enemy moves")]
        [SerializeField] private float moveSpeed = 2.5f;
        
        [Tooltip("Add slight random movement variation to prevent freezing")]
        [SerializeField] private bool useRandomVariation = true;
        
        [Tooltip("Amount of random variation (0-1)")]
        [Range(0f, 0.5f)]
        [SerializeField] private float randomVariationAmount = 0.15f;
        
        [Tooltip("How fast the enemy rotates to face the player (degrees per second)")]
        [SerializeField] private float rotationSpeed = 180f;
        
        [Tooltip("Radius for enemy avoidance behavior")]
        [SerializeField] private float avoidanceRadius = 1.5f;
        
        [Tooltip("Strength of avoidance force")]
        [SerializeField] private float avoidanceStrength = 0.8f;
        
        [Tooltip("Minimum distance to maintain from other enemies")]
        [SerializeField] private float minEnemyDistance = 1.2f;
        
        [Tooltip("Layer mask for other enemies to avoid")]
        [SerializeField] private LayerMask enemyLayerMask = -1;

        [Header("Attack Settings")]
        [Tooltip("Attack cooldown in seconds")]
        [SerializeField] private float attackCooldown = 1f;
        
        [Tooltip("Duration of the attack animation (time to pause movement)")]
        [SerializeField] private float attackAnimationDuration = 0.5f;
        
        [Tooltip("Layer mask for what the enemy can attack")]
        [SerializeField] private LayerMask attackLayerMask = -1;

        [Header("References")]
        [Tooltip("The player transform to chase")]
        [SerializeField] private Transform player;
        
        [Tooltip("Rigidbody2D for physics movement (auto-assigned if null)")]
        [SerializeField] private Rigidbody2D rb;
        
        [Tooltip("Tilemap to check for walkable tiles (optional, auto-finds if null)")]
        [SerializeField] private UnityEngine.Tilemaps.Tilemap walkableTilemap;

        [Header("Player Detection")]
        [Tooltip("Tag to search for when finding the player")]
        [SerializeField] private string playerTag = "Player";
        
        [Tooltip("How often to search for player if not found (seconds)")]
        [SerializeField] private float playerSearchInterval = 1f;

        [Header("Loot Drop System")]
        [Tooltip("Rate at which enemies drop coins (0-1, where 1 = 100% chance)")]
        [SerializeField] private float coinDropRate = 0.3f;
        
        [Tooltip("Prefab to drop when enemy dies (KoalaCoinPicker)")]
        [SerializeField] private GameObject coinDropPrefab;
        
        [Tooltip("Number of coin items to drop")]
        [SerializeField] private int coinDropAmount = 1;
        
        [Tooltip("Random offset for drop position")]
        [SerializeField] private float dropOffset = 0.5f;

        [Header("Attack Interruption")]
        [Tooltip("How long the enemy stays interrupted when hit by player (seconds)")]
        [SerializeField] private float attackInterruptDuration = 0.5f;

        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the enemy starts a melee attack")]
        public MMFeedbacks MeleeAttackStartFeedbacks;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        [SerializeField] private bool ShowDebugInfo = false;

        // Attack States
        public enum AttackState
        {
            Idle,               // Default state - moving normally
            TelegraphingCharge, // Preparing charge attack
            Charging,           // Executing charge attack
            MeleeAttack         // Close range melee
        }

        // Private fields
        private Vector2 _movement;
        private readonly Collider2D[] _enemyHits = new Collider2D[32];
        private ContactFilter2D _enemyFilter;
        private float _lastPlayerSearchTime;
        
        // TopDownEngine components
        private Character _character;
        private CharacterHandleWeapon _characterHandleWeapon;
        private Health _health;
        private Animator _animator;
        
        // Attack state management
        private AttackState _currentAttackState = AttackState.Idle;
        private float _lastAttackTime;
        private float _lastChargeTime;
        
        // Charge attack management
        private bool _isTelegraphing = false;
        private bool _isCharging = false;
        private float _telegraphStartTime;
        private float _chargeStartTime;
        private float _chargeDuration = 0.8f;
        private bool _hasDealtChargeDamage = false;
        private Vector2 _chargeTargetPosition;
        
        // Loot drop tracking
        private bool _hasDroppedLoot;
        
        // Attack interruption
        private bool _isAttackInterrupted = false;
        private float _attackInterruptEndTime;
        
        // Attack animation control
        private bool _isAttacking = false;
        
        // State stability
        private float _stateCommitTime = 0.3f; // Minimum time to stay in a state
        private float _lastStateChangeTime;
        private AttackState _committedState = AttackState.Idle;

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
            
            // Initialize TopDownEngine components
            InitializeTopDownEngineComponents();
        }

        private void Start()
        {
            // Try to find player immediately
            FindPlayer();
            
            // Setup weapons if not already configured
            SetupEnemyWeapons();
            
            // Subscribe to events
            if (_health != null)
            {
                _health.OnDeath += HandleDeathAndDropLoot;
                _health.OnHit += OnEnemyHit;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (_health != null)
            {
                _health.OnDeath -= HandleDeathAndDropLoot;
                _health.OnHit -= OnEnemyHit;
            }
        }

        /// <summary>
        /// Initialize TopDownEngine components
        /// </summary>
        private void InitializeTopDownEngineComponents()
        {
            _character = GetComponent<Character>();
            _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
            _health = GetComponent<Health>();
            _animator = GetComponent<Animator>();
            
            if (_character == null)
            {
                Debug.LogError($"ChaserEnemy: Character component required on {gameObject.name}");
            }
            
            if (_characterHandleWeapon == null)
            {
                Debug.LogError($"ChaserEnemy: CharacterHandleWeapon component required on {gameObject.name}");
            }
            
            if (_animator == null)
            {
                Debug.LogWarning($"ChaserEnemy: Animator component not found on {gameObject.name}");
            }
            
            if (_health == null)
            {
                Debug.LogError($"ChaserEnemy: Health component required on {gameObject.name}");
            }
        }

        /// <summary>
        /// Sets up weapons for the enemy if not already configured
        /// </summary>
        private void SetupEnemyWeapons()
        {
            if (_characterHandleWeapon == null) return;
            
            // Check if we already have weapons
            if (_characterHandleWeapon.CurrentWeapon != null) return;
            
            // Create weapon attachment point
            Transform weaponAttachment = transform.Find("WeaponAttachment");
            if (weaponAttachment == null)
            {
                GameObject attachment = new GameObject("WeaponAttachment");
                attachment.transform.SetParent(transform);
                attachment.transform.localPosition = Vector3.zero;
                weaponAttachment = attachment.transform;
            }
            
            // Create melee weapon
            GameObject meleeWeaponObj = CreateMeleeWeapon();
            if (meleeWeaponObj != null)
            {
                meleeWeaponObj.transform.SetParent(weaponAttachment);
                meleeWeaponObj.transform.localPosition = Vector3.zero;
                
                // Set as initial weapon
                var meleeWeapon = meleeWeaponObj.GetComponent<EnemyMeleeWeapon>();
                if (meleeWeapon != null)
                {
                    _characterHandleWeapon.ChangeWeapon(meleeWeapon, "EnemyMeleeWeapon");
                }
            }
        }

        /// <summary>
        /// Creates a melee weapon for the enemy
        /// </summary>
        private GameObject CreateMeleeWeapon()
        {
            GameObject meleeWeaponObj = new GameObject("EnemyMeleeWeapon");
            
            // Add EnemyMeleeWeapon component
            var enemyMeleeWeapon = meleeWeaponObj.AddComponent<EnemyMeleeWeapon>();
            enemyMeleeWeapon.SetChaserEnemy(this);
            
            // Configure damage area
            enemyMeleeWeapon.MeleeDamageAreaMode = MeleeWeapon.MeleeDamageAreaModes.Generated;
            enemyMeleeWeapon.DamageAreaShape = MeleeWeapon.MeleeDamageAreaShapes.Circle;
            enemyMeleeWeapon.AreaSize = new Vector3(enemy_Melee_Radius * 2f, enemy_Melee_Radius * 2f, 1f);
            enemyMeleeWeapon.AreaOffset = Vector3.zero;
            
            // Configure damage
            enemyMeleeWeapon.MinDamageCaused = default_Attack_Damage;
            enemyMeleeWeapon.MaxDamageCaused = default_Attack_Damage;
            enemyMeleeWeapon.TargetLayerMask = attackLayerMask;
            
            // Configure timing
            enemyMeleeWeapon.InitialDelay = 0f;
            enemyMeleeWeapon.ActiveDuration = 0.5f;
            
            return meleeWeaponObj;
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
            
            // Update attack state and behavior
            UpdateAttackState();
            CalculateMovement();
        }

        private void FixedUpdate()
        {
            if (player == null) return;
            
            MoveEnemy();
            RotateTowardsPlayer();
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
                    Debug.LogError($"ChaserEnemy: No Rigidbody2D found on {gameObject.name}");
                }
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
                    Debug.Log($"ChaserEnemy: Found player '{playerObject.name}'");
                }
            }
        }

        /// <summary>
        /// Updates the attack state based on distance to player
        /// Simple priority system: Melee > Charge > Move
        /// </summary>
        private void UpdateAttackState()
        {
            if (player == null) return;
            
            // Check if interrupted
            if (_isAttackInterrupted)
            {
                if (Time.time >= _attackInterruptEndTime)
                {
                    _isAttackInterrupted = false;
                }
                else
                {
                    _currentAttackState = AttackState.Idle;
                    return;
                }
            }
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Current attack must complete
            if (_isCharging || _isTelegraphing) 
            {
                UpdateChargeAttack();
                return;
            }
            
            bool canCharge = Time.time - _lastChargeTime > chargeCooldown;
            
            // Priority 1: Melee attack if close (always preferred when in range)
            if (distanceToPlayer <= enemy_Melee_Radius && Time.time - _lastAttackTime > attackCooldown)
            {
                _currentAttackState = AttackState.MeleeAttack;
                PerformMeleeAttack();
                
                
            }
            // Priority 2: Charge attack ONLY if already far away (not when player gets close)
            // Charge is used to close distance from afar, not as a reaction to player approaching
            else if (distanceToPlayer > enemy_Melee_Radius + 1f && 
                     distanceToPlayer <= enemy_Charge_Radius && 
                     canCharge)
            {
                _currentAttackState = AttackState.TelegraphingCharge;
                StartChargeTelegraph();
            }
            // Priority 3: Normal movement (chase player)
            else
            {
                _currentAttackState = AttackState.Idle;
            }
        }

        /// <summary>
        /// Calculates movement - enemy always approaches player, never backs away
        /// </summary>
        private void CalculateMovement()
        {
            if (player == null) return;
            
            // Stop if interrupted
            if (_isAttackInterrupted)
            {
                _movement = Vector2.zero;
                rb.linearVelocity = Vector2.zero;
                return;
            }
            
            // Charging movement
            if (_isCharging)
            {
                Vector2 direction = (_chargeTargetPosition - (Vector2)transform.position).normalized;
                _movement = direction;
                return;
            }
            
            // Stop during telegraph or melee
            if (_isTelegraphing || _currentAttackState == AttackState.MeleeAttack)
            {
                _movement = Vector2.zero;
                rb.linearVelocity = Vector2.zero;
                return;
            }
            
            // Normal movement - always chase player, don't stop
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            
            // Always move toward player
            _movement = directionToPlayer;
            
            // Add slight random variation to prevent getting stuck
            if (useRandomVariation)
            {
                // Add a small perpendicular offset for more natural movement
                Vector2 perpendicular = Vector2.Perpendicular(directionToPlayer);
                float randomOffset = Mathf.PerlinNoise(Time.time * 0.5f + GetInstanceID(), 0) * 2f - 1f;
                _movement += perpendicular * randomOffset * randomVariationAmount;
                _movement = _movement.normalized;
            }
            
            // Slow down slightly when very close (but keep moving)
            if (distanceToPlayer < enemy_Melee_Radius)
            {
                _movement *= 0.5f; // Slower when in melee range
            }
            
            // Add enemy avoidance
            Vector2 avoidance = CalculateAvoidance();
            if (avoidance != Vector2.zero)
            {
                _movement = (_movement + avoidance * avoidanceStrength).normalized;
            }
        }

        /// <summary>
        /// Simple linear avoidance from other enemies
        /// </summary>
        private Vector2 CalculateAvoidance()
        {
            Vector2 avoidance = Vector2.zero;
            int hitCount = Physics2D.OverlapCircle(transform.position, avoidanceRadius, _enemyFilter, _enemyHits);

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _enemyHits[i];
                if (hit == null || hit.transform == transform) continue;

                Vector2 away = (Vector2)transform.position - (Vector2)hit.transform.position;
                float dist = away.magnitude;
                
                if (dist > 0f && dist < minEnemyDistance)
                {
                    avoidance += away.normalized;
                }
            }

            return avoidance.normalized;
        }

        /// <summary>
        /// Starts charge telegraph
        /// </summary>
        private void StartChargeTelegraph()
        {
            _isTelegraphing = true;
            _telegraphStartTime = Time.time;
            _lastChargeTime = Time.time;
            _hasDealtChargeDamage = false;
            _chargeTargetPosition = player.position;
        }

        /// <summary>
        /// Updates charge attack behavior
        /// </summary>
        private void UpdateChargeAttack()
        {
            if (_isTelegraphing)
            {
                if (Time.time - _telegraphStartTime >= enemy_Charge_Telegraph_Time)
                {
                    _isTelegraphing = false;
                    _isCharging = true;
                    _chargeStartTime = Time.time;
                    _currentAttackState = AttackState.Charging;
                }
            }
            else if (_isCharging)
            {
                if (Time.time - _chargeStartTime >= _chargeDuration)
                {
                    _isCharging = false;
                    _currentAttackState = AttackState.Idle;
                    _movement = Vector2.zero;
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Collision detection for charge attacks
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(playerTag) && _isCharging && !_hasDealtChargeDamage)
            {
                var targetHealth = collision.gameObject.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.Damage(enemy_Charge_Attack_Damage, gameObject, 0f, 0f, Vector2.zero, null);
                    _hasDealtChargeDamage = true;
                }
            }
        }

        /// <summary>
        /// Performs melee attack
        /// </summary>
        private void PerformMeleeAttack()
        {
            if (_characterHandleWeapon == null || _characterHandleWeapon.CurrentWeapon == null) return;
            
            var meleeWeapon = _characterHandleWeapon.CurrentWeapon.GetComponent<MeleeWeapon>();
            if (meleeWeapon != null)
            {
                MeleeAttackStartFeedbacks?.PlayFeedbacks(transform.position);
                TriggerAttackAnimation();
                _characterHandleWeapon.ShootStart();
                _lastAttackTime = Time.time;
                
                // Stop movement during attack
                StartCoroutine(AttackMovementPause());
            }
        }
        
        /// <summary>
        /// Pauses movement during attack animation
        /// </summary>
        private System.Collections.IEnumerator AttackMovementPause()
        {
            _isAttacking = true;
            rb.linearVelocity = Vector2.zero; // Stop immediately
            
            if (ShowDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Attack started - movement paused");
            }
            
            // Wait for attack animation to complete
            yield return new WaitForSeconds(attackAnimationDuration);
            
            _isAttacking = false;
            
            if (ShowDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Attack ended - movement resumed");
            }
        }
        
        /// <summary>
        /// Triggers attack animation
        /// </summary>
        private void TriggerAttackAnimation()
        {
            // The MeleeWeapon component handles setting the Attack trigger automatically
            // No additional code needed here - just keeping this method for potential future use
            
            // Try to call EnemyAnimationController if it exists (for compatibility)
            var animController = GetComponent("EnemyAnimationController");
            if (animController != null)
            {
                var method = animController.GetType().GetMethod("TriggerAttackAnimation");
                method?.Invoke(animController, null);
            }
        }

        /// <summary>
        /// Moves the enemy with stable velocity control
        /// </summary>
        private void MoveEnemy()
        {
            // Don't move during attack animation
            if (_isAttacking)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            
            if (_movement.magnitude < 0.01f)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            
            float currentSpeed = moveSpeed;
            
            // Charge speed boost
            if (_isCharging)
            {
                currentSpeed *= 2.5f;
            }
            
            // Apply velocity directly - no smoothing needed with stable movement
            rb.linearVelocity = _movement * currentSpeed;
        }

        /// <summary>
        /// Rotates enemy to always face the player
        /// </summary>
        private void RotateTowardsPlayer()
        {
            if (player == null) return;
            
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// Called when enemy gets hit
        /// </summary>
        private void OnEnemyHit()
        {
            if (_currentAttackState == AttackState.MeleeAttack || 
                _currentAttackState == AttackState.TelegraphingCharge || 
                _currentAttackState == AttackState.Charging)
            {
                _isAttackInterrupted = true;
                _attackInterruptEndTime = Time.time + attackInterruptDuration;
                
                _isTelegraphing = false;
                _isCharging = false;
                _currentAttackState = AttackState.Idle;
                _movement = Vector2.zero;
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Handles death and loot drops
        /// </summary>
        private void HandleDeathAndDropLoot()
        {
            if (!_hasDroppedLoot)
            {
                DropLoot();
                _hasDroppedLoot = true;
            }
        }
        
        /// <summary>
        /// Drops loot - beer only (with tilemap validation)
        /// </summary>
        private void DropLoot()
        {
            if (coinDropPrefab == null || Random.Range(0f, 1f) > coinDropRate) return;

            // Auto-find tilemap if not assigned
            if (walkableTilemap == null)
            {
                FindWalkableTilemap();
            }

            for (int i = 0; i < coinDropAmount; i++)
            {
                Vector3 targetPosition = GetValidCoinDropPosition();
                
                GameObject droppedItem = Instantiate(coinDropPrefab, transform.position, Quaternion.identity);
                
                // Add pickup delay (1 second before pickable)
                PickableDelay pickupDelay = droppedItem.AddComponent<PickableDelay>();
                pickupDelay.pickupDelay = 1f;
                
                // Add drop animation
                CoinDropAnimation animation = droppedItem.AddComponent<CoinDropAnimation>();
                animation.StartAnimation(transform.position, targetPosition);
            }
        }
        
        /// <summary>
        /// Finds a valid position for beer drop that's on a walkable tile
        /// </summary>
        private Vector3 GetValidCoinDropPosition()
        {
            const int maxAttempts = 8; // Try 8 random positions
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-dropOffset, dropOffset),
                    Random.Range(-dropOffset, dropOffset),
                    0f
                );
                
                Vector3 candidatePosition = transform.position + randomOffset;
                
                // Check if position is on a walkable tile
                if (IsPositionWalkable(candidatePosition))
                {
                    return candidatePosition;
                }
            }
            
            // Fallback: spawn at enemy position if no valid position found
            if (ShowDebugInfo)
            {
                Debug.LogWarning($"{gameObject.name}: Could not find valid beer drop position, using enemy position");
            }
            return transform.position;
        }
        
        /// <summary>
        /// Checks if a world position is on a walkable tile
        /// </summary>
        private bool IsPositionWalkable(Vector3 worldPosition)
        {
            // If no tilemap assigned, allow all positions (failsafe)
            if (walkableTilemap == null)
            {
                return true;
            }
            
            // Convert world position to tilemap cell coordinates
            Vector3Int cellPosition = walkableTilemap.WorldToCell(worldPosition);
            
            // Check if there's a tile at this position
            bool hasTile = walkableTilemap.HasTile(cellPosition);
            
            if (ShowDebugInfo && !hasTile)
            {
                Debug.Log($"Position {worldPosition} (cell {cellPosition}) has no walkable tile");
            }
            
            return hasTile;
        }
        
        /// <summary>
        /// Auto-finds the walkable tilemap in the scene
        /// </summary>
        private void FindWalkableTilemap()
        {
            // Look for common tilemap names
            string[] tilemapNames = { "Ground", "Floor", "Walkable", "Base", "Tilemap" };
            
            foreach (string name in tilemapNames)
            {
                GameObject tilemapObj = GameObject.Find(name);
                if (tilemapObj != null)
                {
                    walkableTilemap = tilemapObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                    if (walkableTilemap != null)
                    {
                        if (ShowDebugInfo)
                        {
                            Debug.Log($"ChaserEnemy: Auto-found walkable tilemap '{name}'");
                        }
                        return;
                    }
                }
            }
            
            // Fallback: find any tilemap in scene
            walkableTilemap = FindObjectOfType<UnityEngine.Tilemaps.Tilemap>();
            if (walkableTilemap != null && ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: Auto-found tilemap '{walkableTilemap.name}'");
            }
            else if (ShowDebugInfo)
            {
                Debug.LogWarning("ChaserEnemy: No tilemap found in scene. Beer will drop without validation.");
            }
        }

        /// <summary>
        /// Debug visualization
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            
            // Ideal distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, idealDistance);
            
            // Min/Max range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(player.position, maxDistance);
            
            // Attack ranges
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(player.position, enemy_Charge_Radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, enemy_Melee_Radius);
            
            // Avoidance
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
            
            // Line to player
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.position);
            
            // State indicator
            Gizmos.color = GetStateColor();
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.2f);
        }

        private Color GetStateColor()
        {
            switch (_currentAttackState)
            {
                case AttackState.Idle: return Color.cyan;
                case AttackState.TelegraphingCharge: return Color.orange;
                case AttackState.Charging: return Color.red;
                case AttackState.MeleeAttack: return Color.magenta;
                default: return Color.gray;
            }
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        [ContextMenu("Find Player")]
        public void ForceFindPlayer()
        {
            FindPlayer();
        }
    }
}
