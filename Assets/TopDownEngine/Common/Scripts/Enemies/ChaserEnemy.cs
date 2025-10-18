using UnityEngine;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Advanced enemy AI that dashes at long distances and performs melee attacks when close
    /// Implements dash damage and melee attacks using TopDownEngine weapon system
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
        [SerializeField] private float chargeCooldown = 5f;

        [Header("Orbiting Behavior")]
        [Tooltip("Distance from player to maintain orbit")]
        [SerializeField] private float flank_Distance = 3f;

        [Tooltip("Random offset applied to orbit position for natural movement")]
        [SerializeField] private float flank_Position_Offset = 1f;

        [Tooltip("Cooldown before recalculating orbit position")]
        [SerializeField] private float reposition_Cooldown = 2f;
        
        [Tooltip("Minimum movement threshold to prevent micro-movements")]
        [SerializeField] private float movementDeadZone = 0.1f;

        [Header("Movement Settings")]
        [Tooltip("How fast the enemy moves")]
        [SerializeField] private float moveSpeed = 3f;
        
        [Tooltip("How fast the enemy rotates to face the player (degrees per second)")]
        [SerializeField] private float rotationSpeed = 180f;
        
        [Tooltip("Radius for enemy avoidance behavior")]
        [SerializeField] private float avoidanceRadius = 1.5f;
        
        [Tooltip("Strength of avoidance force")]
        [SerializeField] private float avoidanceStrength = 1.0f;
        
        [Tooltip("Minimum distance to maintain from other enemies")]
        [SerializeField] private float minEnemyDistance = 1.0f;
        
        [Tooltip("Layer mask for other enemies to avoid")]
        [SerializeField] private LayerMask enemyLayerMask = -1;

        [Header("AI Intelligence Settings")]
        [Tooltip("Enable movement prediction for more accurate attacks")]
        [SerializeField] private bool usePredictiveAttacks = true;
        
        [Tooltip("How far ahead to predict player position (seconds)")]
        [SerializeField] private float predictionTime = 0.3f;
        
        [Tooltip("Min and max charge cooldown for unpredictability")]
        [SerializeField] private Vector2 chargeCooldownRange = new Vector2(3.5f, 6f);
        
        [Tooltip("Chance to do a surprise attack (no telegraph) 0-1")]
        [SerializeField] private float surpriseAttackChance = 0.15f;
        
        [Tooltip("How much to vary orbit distance for dynamic movement")]
        [SerializeField] private float orbitDistanceVariation = 0.8f;
        
        [Tooltip("Enable strafing movement patterns")]
        [SerializeField] private bool enableStrafing = true;
        
        [Tooltip("Time between strafing movements")]
        [SerializeField] private float strafingInterval = 2.5f;

        [Header("Attack Settings")]
        [Tooltip("Enable continuous movement while attacking")]
        [SerializeField] private bool attackWhileMoving = true;
        
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

        [Header("Loot Drop System")]
        [Tooltip("Rate at which enemies drop coins (0-1, where 1 = 100% chance)")]
        [SerializeField] private float coinDropRate = 0.3f;
        
        [Tooltip("Prefab to drop when enemy dies (KoalaCoinPicker)")]
        [SerializeField] private GameObject coinDropPrefab;
        
        [Tooltip("Number of coin items to drop")]
        [SerializeField] private int coinDropAmount = 1;
        
        [Tooltip("Rate at which enemies drop health pickups (0-1, where 1 = 100% chance)")]
        [SerializeField] private float healthDropRate = 0.3f;
        
        [Tooltip("Health pickup prefab to drop when enemy dies")]
        [SerializeField] private GameObject healthDropPrefab;
        
        [Tooltip("Random offset for drop position")]
        [SerializeField] private float dropOffset = 0.5f;

        [Header("Attack Interruption")]
        [Tooltip("How long the enemy stays interrupted when hit by player (seconds)")]
        [SerializeField] private float attackInterruptDuration = 0.5f;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        [SerializeField] private bool ShowDebugInfo = false;

        // Attack States
        public enum AttackState
        {
            Orbiting,           // Moving to/maintaining orbit position
            TelegraphingCharge, // Preparing charge attack
            Charging,           // Executing charge attack
            MeleeAttack        // Close range melee (if kept)
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
        
        // Attack state management
        private AttackState _currentAttackState = AttackState.Orbiting;
        private float _lastAttackTime;
        private float _lastChargeTime;
        
        // Orbiting behavior
        private Vector2 _targetOrbitPosition;
        private float _lastRepositionTime;
        private bool _hasReachedOrbitPosition;
        private float _orbitAngle;
        private Vector2 _lastPlayerPosition;
        private Vector2 _smoothedMovement; // Smoothed movement vector to prevent twitching
        private float _movementSmoothing = 0.1f; // How much to smooth movement changes
        
        // Charge attack management
        private bool _isTelegraphing = false;
        private bool _isCharging = false;
        private float _telegraphStartTime;
        private float _chargeStartTime;
        private float _chargeDuration = 0.8f;
        private bool _hasDealtChargeDamage = false;
        private Vector2 _chargeTargetPosition; // Fixed target position for charge attack
        private float _chargeEndTime; // Time when charge ended
        private float _chargeRecoveryTime = 0.5f; // Time to wait before recalculating orbit after charge
        
        // Loot drop tracking
        private bool _hasDroppedLoot;
        
        // AI Intelligence tracking
        private float _lastStrafeTime;
        private float _dynamicChargeCooldown;
        private Rigidbody2D _playerRigidbody;
        
        // Attack interruption
        private bool _isAttackInterrupted = false;
        private float _attackInterruptEndTime;

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
            
            // Subscribe to death event for loot dropping
            if (_health != null)
            {
                _health.OnDeath += HandleDeathAndDropLoot;
                _health.OnHit += OnEnemyHit; // Subscribe to hit event for attack interruption
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from death event to prevent memory leaks
            if (_health != null)
            {
                _health.OnDeath -= HandleDeathAndDropLoot;
                _health.OnHit -= OnEnemyHit; // Unsubscribe from hit event
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
            
            if (_character == null)
            {
                Debug.LogError($"ChaserEnemy: Character component required on {gameObject.name}");
            }
            
            if (_characterHandleWeapon == null)
            {
                Debug.LogError($"ChaserEnemy: CharacterHandleWeapon component required on {gameObject.name}");
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
            
            if (ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: Weapons configured for {gameObject.name}");
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
                _playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
                
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
        /// Updates the attack state based on distance to player
        /// </summary>
        private void UpdateAttackState()
        {
            if (player == null) return;
            
            // Check if we're currently interrupted from being hit
            if (_isAttackInterrupted)
            {
                if (Time.time >= _attackInterruptEndTime)
                {
                    _isAttackInterrupted = false;
                    if (ShowDebugInfo)
                    {
                        Debug.Log("ChaserEnemy: Attack interruption ended, resuming normal behavior");
                    }
                }
                else
                {
                    // Stay in orbiting state while interrupted
                    _currentAttackState = AttackState.Orbiting;
                    return;
                }
            }
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Current attack must complete before checking for new attacks
            if (_isCharging || _isTelegraphing) 
            {
                UpdateChargeAttack();
                return;
            }
            
            // Use dynamic charge cooldown for unpredictability
            if (_dynamicChargeCooldown == 0)
            {
                _dynamicChargeCooldown = Random.Range(chargeCooldownRange.x, chargeCooldownRange.y);
            }
            
            bool canCharge = Time.time - _lastChargeTime > _dynamicChargeCooldown;
            
            // Check for charge attack opportunity
            if (distanceToPlayer <= enemy_Charge_Radius && canCharge)
            {
                // Randomly decide if this should be a surprise attack
                bool doSurpriseAttack = Random.value < surpriseAttackChance;
                
                if (doSurpriseAttack)
                {
                    // Surprise attack - skip telegraph and charge immediately
                    _isCharging = true;
                    _chargeStartTime = Time.time;
                    _lastChargeTime = Time.time;
                    _hasDealtChargeDamage = false;
                    _currentAttackState = AttackState.Charging;
                    
                    // Still predict target position for accuracy
                    _chargeTargetPosition = PredictPlayerPosition();
                    
                    // Set new random cooldown for next charge
                    _dynamicChargeCooldown = Random.Range(chargeCooldownRange.x, chargeCooldownRange.y);
                    
                    if (ShowDebugInfo)
                    {
                        Debug.Log("ChaserEnemy: Surprise charge attack!");
                    }
                }
                else
                {
                    // Normal telegraph attack
                    _currentAttackState = AttackState.TelegraphingCharge;
                    StartChargeTelegraph();
                    
                    // Set new random cooldown for next charge
                    _dynamicChargeCooldown = Random.Range(chargeCooldownRange.x, chargeCooldownRange.y);
                }
            }
            // Check for melee attack if very close
            else if (distanceToPlayer <= enemy_Melee_Radius && Time.time - _lastAttackTime > attackCooldown)
            {
                _currentAttackState = AttackState.MeleeAttack;
                PerformMeleeAttack();
            }
            // Default: maintain orbit
            else
            {
                _currentAttackState = AttackState.Orbiting;
            }
        }

        /// <summary>
        /// Calculates movement towards player
        /// </summary>
        private void CalculateMovement()
        {
            if (player == null) return;
            
            // If interrupted, stop moving
            if (_isAttackInterrupted)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // If charging, movement is handled by charge logic
            if (_isCharging)
            {
                // Use fixed target position instead of current player position
                Vector2 direction = (_chargeTargetPosition - (Vector2)transform.position).normalized;
                _movement = direction;
                return;
            }
            
            // If telegraphing, stop moving
            if (_isTelegraphing)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // Brief pause after charge to prevent twitching
            if (Time.time - _chargeEndTime < _chargeRecoveryTime)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // If attacking in place, stop moving
            if (_currentAttackState == AttackState.MeleeAttack && !attackWhileMoving)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // Calculate orbit position
            CalculateOrbitPosition();
            
            // Move toward orbit position
            Vector2 directionToOrbit = (_targetOrbitPosition - (Vector2)transform.position).normalized;
            
            // Add avoidance from other enemies
            Vector2 avoidance = CalculateAvoidance();
            
            // Add strafing for more dynamic movement
            Vector2 strafeMovement = Vector2.zero;
            if (enableStrafing && Time.time - _lastStrafeTime > strafingInterval)
            {
                // Randomly decide to strafe
                if (Random.value < 0.5f)
                {
                    // Strafe perpendicular to direction to player
                    Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
                    Vector2 perpendicular = Vector2.Perpendicular(directionToPlayer);
                    
                    // Randomly choose strafe direction
                    float strafeDirection = Random.value < 0.5f ? 1f : -1f;
                    strafeMovement = perpendicular * strafeDirection * 0.4f;
                    
                    _lastStrafeTime = Time.time;
                }
            }
            
            // Combine movement with avoidance and strafing
            _movement = (directionToOrbit + avoidance * avoidanceStrength + strafeMovement).normalized;
            
            // Check if reached orbit position with larger tolerance to prevent constant repositioning
            float distanceToOrbit = Vector2.Distance(transform.position, _targetOrbitPosition);
            _hasReachedOrbitPosition = distanceToOrbit < 1.0f; // Increased tolerance
        }

        /// <summary>
        /// Calculates the target orbit position around the player
        /// </summary>
        private void CalculateOrbitPosition()
        {
            if (player == null) return;
            
            // Don't recalculate orbit immediately after charge ends
            if (Time.time - _chargeEndTime < _chargeRecoveryTime)
            {
                return;
            }
            
            Vector2 currentPlayerPosition = player.position;
            float playerMovedDistance = Vector2.Distance(currentPlayerPosition, _lastPlayerPosition);
            
            // Only recalculate if:
            // 1. Cooldown has passed AND (we don't have a target position OR player moved significantly)
            // 2. OR if we don't have a target position yet
            // 3. OR if we're too far from our current orbit position
            float distanceToCurrentOrbit = Vector2.Distance(transform.position, _targetOrbitPosition);
            bool shouldRecalculate = (Time.time - _lastRepositionTime >= reposition_Cooldown && 
                                    (_targetOrbitPosition == Vector2.zero || playerMovedDistance > 2f)) ||
                                    _targetOrbitPosition == Vector2.zero ||
                                    distanceToCurrentOrbit > flank_Distance * 2f; // If we're way off target
            
            if (!shouldRecalculate)
            {
                return;
            }
            
            // Calculate base orbit position
            // Distribute enemies around the player by using their instance ID for variation
            _orbitAngle = (GetInstanceID() % 360) * Mathf.Deg2Rad;
            
            // Calculate position on orbit
            Vector2 orbitDirection = new Vector2(Mathf.Cos(_orbitAngle), Mathf.Sin(_orbitAngle));
            
            // Apply random offset for natural formation (only when recalculating)
            Vector2 randomOffset = Random.insideUnitCircle * flank_Position_Offset;
            
            // Apply dynamic orbit distance variation for less predictable movement
            float dynamicOrbitDistance = flank_Distance + Random.Range(-orbitDistanceVariation, orbitDistanceVariation);
            dynamicOrbitDistance = Mathf.Max(dynamicOrbitDistance, 1f); // Ensure it doesn't get too close
            
            // Calculate final orbit position with dynamic distance
            _targetOrbitPosition = currentPlayerPosition + orbitDirection * dynamicOrbitDistance + randomOffset;
            
            _lastRepositionTime = Time.time;
            _lastPlayerPosition = currentPlayerPosition;
            _hasReachedOrbitPosition = false;
            
            if (ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: Recalculated orbit position to {_targetOrbitPosition} (Player moved: {playerMovedDistance:F2})");
            }
        }


        /// <summary>
        /// Calculates avoidance force from other enemies
        /// </summary>
        private Vector2 CalculateAvoidance()
        {
            Vector2 avoidance = Vector2.zero;
            Vector2 position = transform.position;
            int hitCount = Physics2D.OverlapCircle(position, avoidanceRadius, _enemyFilter, _enemyHits);

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _enemyHits[i];
                if (hit == null || hit.transform == transform) continue;

                Vector2 away = (Vector2)transform.position - (Vector2)hit.transform.position;
                float dist = away.magnitude;
                if (dist > 0f && dist < avoidanceRadius)
                {
                    // Calculate avoidance force
                    float distFactor = 1f - (dist / avoidanceRadius);
                    
                    // Apply stronger force if below minimum distance
                    if (dist < minEnemyDistance)
                    {
                        distFactor *= 2f; // Double the avoidance force
                    }
                    
                    // Apply exponential falloff for stronger separation
                    distFactor = Mathf.Pow(distFactor, 2f);
                avoidance += away / dist * distFactor;
            }
            }

            return avoidance;
        }


        /// <summary>
        /// Predicts player position based on their current velocity
        /// </summary>
        private Vector2 PredictPlayerPosition()
        {
            if (player == null) return Vector2.zero;
            
            // If prediction is enabled and we have player rigidbody
            if (usePredictiveAttacks && _playerRigidbody != null)
            {
                Vector2 playerVelocity = _playerRigidbody.linearVelocity;
                Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Predicted player position from {player.position} to {predictedPosition}");
                }
                
                return predictedPosition;
            }
            
            // Fall back to current position
            return player.position;
        }
        
        /// <summary>
        /// Starts the charge telegraph phase
        /// </summary>
        private void StartChargeTelegraph()
        {
            _isTelegraphing = true;
            _telegraphStartTime = Time.time;
            _lastChargeTime = Time.time;
            _hasDealtChargeDamage = false;
            
            // Use predictive targeting for more accurate attacks
            _chargeTargetPosition = PredictPlayerPosition();
        }

        /// <summary>
        /// Updates charge attack behavior
        /// </summary>
        private void UpdateChargeAttack()
        {
            if (_isTelegraphing)
            {
                // Check if telegraph time is over
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
                // Check if charge duration is over
                if (Time.time - _chargeStartTime >= _chargeDuration)
                {
                    _isCharging = false;
                    _chargeEndTime = Time.time;
                    _currentAttackState = AttackState.Orbiting;
                    
                    // Stop movement briefly to prevent twitching
                    _movement = Vector2.zero;
                }
                else
                {
                    // Move towards fixed target position during charge
                    Vector2 direction = (_chargeTargetPosition - (Vector2)transform.position).normalized;
                    _movement = direction;
                }
            }
        }

        /// <summary>
        /// Handles collision detection for charge attacks
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if we hit the player
            if (collision.gameObject.CompareTag(playerTag))
            {
                // Deal charge damage if charging
                if (_isCharging && !_hasDealtChargeDamage)
                {
                    DealChargeDamage(collision.gameObject.GetComponent<Collider2D>());
                }
            }
        }


        /// <summary>
        /// Deals charge damage to the target
        /// </summary>
        private void DealChargeDamage(Collider2D target)
        {
            // Get the Health component from the target
            var targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                // Deal high damage
                targetHealth.Damage(enemy_Charge_Attack_Damage, gameObject, 0f, 0f, Vector2.zero, null);
                _hasDealtChargeDamage = true;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Dealt {enemy_Charge_Attack_Damage} charge damage to {target.name}");
                }
            }
        }

        /// <summary>
        /// Performs a melee attack
        /// </summary>
        private void PerformMeleeAttack()
        {
            if (_characterHandleWeapon == null || _characterHandleWeapon.CurrentWeapon == null) return;
            
            // Check if we have a melee weapon
            var meleeWeapon = _characterHandleWeapon.CurrentWeapon.GetComponent<MeleeWeapon>();
            if (meleeWeapon != null)
            {
                // Trigger attack animation
                TriggerAttackAnimation();
                
                _characterHandleWeapon.ShootStart();
                _lastAttackTime = Time.time;
            }
        }
        
        /// <summary>
        /// Triggers the attack animation via EnemyAnimationController
        /// </summary>
        private void TriggerAttackAnimation()
        {
            // Use GetComponent with string name to avoid namespace issues
            var animController = GetComponent("EnemyAnimationController");
            if (animController != null)
            {
                // Use reflection to call the method
                var method = animController.GetType().GetMethod("TriggerAttackAnimation");
                if (method != null)
                {
                    method.Invoke(animController, null);
                }
            }
        }

        /// <summary>
        /// Moves the enemy using Rigidbody2D with smoothed movement
        /// </summary>
        private void MoveEnemy()
        {
            float currentSpeed = moveSpeed;
            
            // Apply speed boost during charge
            if (_isCharging)
            {
                currentSpeed *= 2.5f; // Charge speed multiplier
            }
            
            // Apply dead zone to prevent micro-movements
            if (_movement.magnitude < movementDeadZone)
            {
                _movement = Vector2.zero;
            }
            
            // Smooth movement changes to prevent twitching
            _smoothedMovement = Vector2.Lerp(_smoothedMovement, _movement, _movementSmoothing);
            
            // Apply dead zone to smoothed movement as well
            if (_smoothedMovement.magnitude < movementDeadZone)
            {
                _smoothedMovement = Vector2.zero;
            }
            
            // Apply smoothed movement
            rb.linearVelocity = _smoothedMovement * currentSpeed;
        }

        /// <summary>
        /// Rotates the enemy to always face the player
        /// </summary>
        private void RotateTowardsPlayer()
        {
            if (player == null) return;
            
            // Always face the player, regardless of movement
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            
            // Calculate target rotation angle to face player
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            
            // Apply 90-degree offset if needed (depends on sprite orientation)
            // Unity sprites typically face right by default
            targetAngle -= 90f;
            
            // Smoothly rotate towards target angle
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
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
        /// Debug visualization using Gizmos
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            
            // Draw flank distance orbit
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(player.position, flank_Distance);
            
            // Draw flank distance with max offset
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireSphere(player.position, flank_Distance + flank_Position_Offset);
            Gizmos.DrawWireSphere(player.position, flank_Distance - flank_Position_Offset);
            
            // Draw charge radius
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(player.position, enemy_Charge_Radius);
            
            // Draw melee radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, enemy_Melee_Radius);
            
            // Draw target orbit position
            if (_targetOrbitPosition != Vector2.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_targetOrbitPosition, 0.3f);
                Gizmos.DrawLine(transform.position, _targetOrbitPosition);
            }
            
            // Draw avoidance radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
            
            // Draw line to player
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.position);
            
            // Draw current state indicator
            Gizmos.color = GetStateColor();
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.2f);
        }

        /// <summary>
        /// Gets color for current attack state
        /// </summary>
        private Color GetStateColor()
        {
            switch (_currentAttackState)
            {
                case AttackState.Orbiting: return Color.cyan;
                case AttackState.TelegraphingCharge: return Color.orange;
                case AttackState.Charging: return Color.red;
                case AttackState.MeleeAttack: return Color.magenta;
                default: return Color.gray;
            }
        }


        /// <summary>
        /// Handles death and drops loot when enemy dies
        /// Called by Health.OnDeath event
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
        /// Called when the enemy gets hit by the player
        /// Interrupts any ongoing attack
        /// </summary>
        private void OnEnemyHit()
        {
            // Interrupt any ongoing attack
            if (_currentAttackState == AttackState.MeleeAttack || 
                _currentAttackState == AttackState.TelegraphingCharge || 
                _currentAttackState == AttackState.Charging)
            {
                _isAttackInterrupted = true;
                _attackInterruptEndTime = Time.time + attackInterruptDuration;
                
                // Stop any ongoing attack
                _isTelegraphing = false;
                _isCharging = false;
                _currentAttackState = AttackState.Orbiting;
                
                // Stop movement briefly
                _movement = Vector2.zero;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Attack interrupted by player hit! Interrupted for {attackInterruptDuration}s");
                }
            }
        }


        /// <summary>
        /// Drops loot based on drop rate and amount
        /// Enemies can drop either coins OR health pickup, not both
        /// </summary>
        private void DropLoot()
        {
            // Decide what to drop: health pickup or coins
            // Roll for health pickup first (since it's rarer/more valuable)
            float healthRoll = Random.Range(0f, 1f);
            
            if (healthRoll <= healthDropRate && healthDropPrefab != null)
            {
                // Drop health pickup
                DropHealthPickup();
            }
            else
            {
                // Try to drop coins instead
                DropCoins();
            }
        }
        
        /// <summary>
        /// Drops health pickup at enemy death location
        /// </summary>
        private void DropHealthPickup()
        {
            if (healthDropPrefab == null)
            {
                if (ShowDebugInfo)
                {
                    Debug.LogWarning($"ChaserEnemy: No health drop prefab assigned to {gameObject.name}");
                }
                return;
            }
            
            // Calculate random drop position
            Vector3 randomOffset = new Vector3(
                Random.Range(-dropOffset, dropOffset),
                Random.Range(-dropOffset, dropOffset),
                0f
            );
            
            Vector3 dropPosition = transform.position + randomOffset;
            
            // Instantiate the health pickup
            GameObject droppedHealth = Instantiate(healthDropPrefab, dropPosition, Quaternion.identity);
            
            if (ShowDebugInfo)
            {
                Debug.Log($"ChaserEnemy: Dropped health pickup at {dropPosition}");
            }
        }
        
        /// <summary>
        /// Drops coins at enemy death location
        /// </summary>
        private void DropCoins()
        {
            if (coinDropPrefab == null)
            {
                if (ShowDebugInfo)
                {
                    Debug.LogWarning($"ChaserEnemy: No coin drop prefab assigned to {gameObject.name}");
                }
                return;
            }

            // Check if we should drop coins based on drop rate
            if (Random.Range(0f, 1f) <= coinDropRate)
            {
                for (int i = 0; i < coinDropAmount; i++)
                {
                    // Calculate random drop position (target position)
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-dropOffset, dropOffset),
                        Random.Range(-dropOffset, dropOffset),
                        0f
                    );
                    
                    Vector3 targetPosition = transform.position + randomOffset;
                    
                    // Instantiate the drop prefab at enemy position (will animate to target)
                    GameObject droppedItem = Instantiate(coinDropPrefab, transform.position, Quaternion.identity);
                    
                    // Add animation component and start animation
                    CoinDropAnimation animation = droppedItem.AddComponent<CoinDropAnimation>();
                    animation.StartAnimation(transform.position, targetPosition);
                    
                    if (ShowDebugInfo)
                    {
                        Debug.Log($"ChaserEnemy: Dropped {coinDropPrefab.name} - animating from {transform.position} to {targetPosition}");
                    }
                }
            }
        }
    }
}

