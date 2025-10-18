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
        [Tooltip("Distance within which dash attack can begin")]
        [SerializeField] private float enemy_Dash_Radius = 4f;
        
        [Tooltip("Distance for default melee attack")]
        [SerializeField] private float enemy_Melee_Radius = 1.5f;
        
        [Tooltip("Distance within which charge attack can begin")]
        [SerializeField] private float enemy_Charge_Radius = 5f;
        
        [Tooltip("Time to telegraph before charge attack")]
        [SerializeField] private float enemy_Charge_Telegraph_Time = 1.5f;
        
        [Tooltip("Damage for default attack")]
        [SerializeField] private float default_Attack_Damage = 10f;
        
        [Tooltip("High damage for charge attack")]
        [SerializeField] private float enemy_Charge_Attack_Damage = 25f;
        
        [Tooltip("Dash speed multiplier")]
        [SerializeField] private float dashSpeedMultiplier = 2f;
        
        [Tooltip("Damage dealt by dash attack")]
        [SerializeField] private float dashDamage = 15f;
        
        [Tooltip("Cooldown between dash attacks in seconds")]
        [SerializeField] private float dashCooldown = 3f;
        
        [Tooltip("Cooldown between charge attacks in seconds")]
        [SerializeField] private float chargeCooldown = 5f;

        [Header("Movement Settings")]
        [Tooltip("How fast the enemy moves")]
        [SerializeField] private float moveSpeed = 3f;
        
        [Tooltip("How fast the enemy rotates to face movement direction")]
        [SerializeField] private float rotationSpeed = 180f;
        
        [Tooltip("Radius for enemy avoidance behavior")]
        [SerializeField] private float avoidanceRadius = 1.5f;
        
        [Tooltip("Strength of avoidance force")]
        [SerializeField] private float avoidanceStrength = 1.0f;
        
        [Tooltip("Minimum distance to maintain from other enemies")]
        [SerializeField] private float minEnemyDistance = 1.0f;
        
        [Tooltip("Layer mask for other enemies to avoid")]
        [SerializeField] private LayerMask enemyLayerMask = -1;

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

        [Header("Debug")]
        [Tooltip("Show debug information")]
        [SerializeField] private bool ShowDebugInfo = false;

        // Attack States
        public enum AttackState
        {
            Idle,
            Dashing,
            TelegraphingCharge,
            Charging,
            MeleeAttack
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
        private AttackState _currentAttackState = AttackState.Idle;
        private float _lastAttackTime;
        private float _lastDashTime;
        private float _lastChargeTime;
        private bool _isDashing = false;
        private float _dashStartTime;
        private float _dashDuration = 0.5f;
        private bool _hasDealtDashDamage = false;
        private Vector2 _dashTargetPosition;
        
        // Charge attack management
        private bool _isTelegraphing = false;
        private bool _isCharging = false;
        private float _telegraphStartTime;
        private float _chargeStartTime;
        private float _chargeDuration = 0.8f;
        private bool _hasDealtChargeDamage = false;

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
        /// Updates the attack state based on distance to player
        /// </summary>
        private void UpdateAttackState()
        {
            if (player == null) return;
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Check if we can attack (not on cooldown)
            bool canAttack = Time.time - _lastAttackTime > attackCooldown;
            bool canDash = Time.time - _lastDashTime > dashCooldown;
            bool canCharge = Time.time - _lastChargeTime > chargeCooldown;
            
            // Update charge attack states
            UpdateChargeAttack();
            
            if (distanceToPlayer <= enemy_Melee_Radius && canAttack)
            {
                // Within melee range - perform melee attack
                _currentAttackState = AttackState.MeleeAttack;
                PerformMeleeAttack();
            }
            else if (distanceToPlayer <= enemy_Charge_Radius && canCharge && !_isTelegraphing && !_isCharging)
            {
                // Within charge range - start telegraphing
                _currentAttackState = AttackState.TelegraphingCharge;
                StartChargeTelegraph();
            }
            else if (distanceToPlayer <= enemy_Dash_Radius && canDash && !_isDashing && !_isTelegraphing && !_isCharging)
            {
                // Within dash range but not melee or charge - start dash
                _currentAttackState = AttackState.Dashing;
                StartDash();
            }
            else if (!_isTelegraphing && !_isCharging)
            {
                // Too far for attacks - move towards player
                _currentAttackState = AttackState.Idle;
            }
        }

        /// <summary>
        /// Calculates movement towards player
        /// </summary>
        private void CalculateMovement()
        {
            if (player == null) return;
            
            // Update dash behavior if dashing
            UpdateDashMovement();
            
            // Only move if not attacking or if attackWhileMoving is true
            if (_currentAttackState == AttackState.MeleeAttack && !attackWhileMoving)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // If dashing or charging, movement is already set by their respective methods
            if (_isDashing || _isCharging) return;
            
            // During telegraphing, stop moving
            if (_isTelegraphing)
            {
                _movement = Vector2.zero;
                return;
            }
            
            // Calculate direction to player
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            
            // Add avoidance from other enemies
            Vector2 avoidance = CalculateAvoidance();
            
            // Combine movement with avoidance
            _movement = (direction + avoidance * avoidanceStrength).normalized;
        }

        /// <summary>
        /// Starts a dash towards the player's current position
        /// </summary>
        private void StartDash()
        {
            _isDashing = true;
            _dashStartTime = Time.time;
            _lastDashTime = Time.time;
            _lastAttackTime = Time.time;
            _hasDealtDashDamage = false;
            
            // Capture the player's position at the start of the dash
            if (player != null)
            {
                _dashTargetPosition = player.position;
            }
        }

        /// <summary>
        /// Updates dash behavior
        /// </summary>
        private void UpdateDash()
        {
            if (!_isDashing) return;
            
            // Check if dash duration is over
            if (Time.time - _dashStartTime >= _dashDuration)
            {
                _isDashing = false;
                _currentAttackState = AttackState.Idle;
                return;
            }
            
            // Move towards the captured target position (not current player position)
            Vector2 direction = (_dashTargetPosition - (Vector2)transform.position).normalized;
            _movement = direction;
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
        /// Updates dash behavior during movement calculation
        /// </summary>
        private void UpdateDashMovement()
        {
            if (_isDashing)
            {
                UpdateDash();
            }
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
                    _currentAttackState = AttackState.Idle;
                }
                else
                {
                    // Move towards player during charge
                    if (player != null)
                    {
                        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
                        _movement = direction;
                    }
                }
            }
        }

        /// <summary>
        /// Handles collision detection for dash and charge attacks
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if we hit the player
            if (other.CompareTag(playerTag))
            {
                // Deal dash damage if dashing
                if (_isDashing && !_hasDealtDashDamage)
                {
                    DealDashDamage(other);
                }
                // Deal charge damage if charging
                else if (_isCharging && !_hasDealtChargeDamage)
                {
                    DealChargeDamage(other);
                }
            }
        }

        /// <summary>
        /// Deals dash damage to the target
        /// </summary>
        private void DealDashDamage(Collider2D target)
        {
            // Get the Health component from the target
            var targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                // Deal damage
                targetHealth.Damage(dashDamage, gameObject, 0f, 0f, Vector2.zero, null);
                _hasDealtDashDamage = true;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"ChaserEnemy: Dealt {dashDamage} dash damage to {target.name}");
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
                _characterHandleWeapon.ShootStart();
                _lastAttackTime = Time.time;
            }
        }

        /// <summary>
        /// Moves the enemy using Rigidbody2D
        /// </summary>
        private void MoveEnemy()
        {
            float currentSpeed = moveSpeed;
            
            // Apply dash speed multiplier if dashing
            if (_isDashing)
            {
                currentSpeed *= dashSpeedMultiplier;
            }
            
            rb.linearVelocity = _movement * currentSpeed;
        }

        /// <summary>
        /// Rotates the enemy to face movement direction
        /// Disabled for surrounding behavior - enemies maintain their original orientation
        /// </summary>
        private void RotateTowardsMovement()
        {
            // Disabled rotation for surrounding behavior
            // Enemies maintain their original sprite orientation while moving
            return;
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
            
            // Draw charge radius
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(player.position, enemy_Charge_Radius);
            
            // Draw dash radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, enemy_Dash_Radius);
            
            // Draw melee radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, enemy_Melee_Radius);
            
            // Draw avoidance radius around this enemy
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
            
            // Draw minimum distance around this enemy
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, minEnemyDistance);
            
            // Draw line to player
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
            
            // Draw dash target if dashing
            if (_isDashing && _dashTargetPosition != Vector2.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_dashTargetPosition, 0.3f);
                Gizmos.DrawLine(transform.position, _dashTargetPosition);
            }
            
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
                case AttackState.Idle: return Color.white;
                case AttackState.Dashing: return Color.yellow;
                case AttackState.TelegraphingCharge: return Color.orange;
                case AttackState.Charging: return Color.red;
                case AttackState.MeleeAttack: return Color.magenta;
                default: return Color.gray;
            }
        }

        private void OnGUI()
        {
            if (ShowDebugInfo)
            {
                float timeSinceLastDash = Time.time - _lastDashTime;
                float timeSinceLastCharge = Time.time - _lastChargeTime;
                bool canDash = timeSinceLastDash > dashCooldown;
                bool canCharge = timeSinceLastCharge > chargeCooldown;
                
                GUI.Label(new Rect(10, 10, 300, 20), $"Player: {(player != null ? player.name : "Not Found")}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Movement: {_movement}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Speed: {(rb != null ? rb.linearVelocity.magnitude.ToString("F2") : "No Rigidbody2D")}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Attack State: {_currentAttackState}");
                GUI.Label(new Rect(10, 90, 300, 20), $"Is Dashing: {_isDashing}");
                GUI.Label(new Rect(10, 110, 300, 20), $"Is Telegraphing: {_isTelegraphing}");
                GUI.Label(new Rect(10, 130, 300, 20), $"Is Charging: {_isCharging}");
                GUI.Label(new Rect(10, 150, 300, 20), $"Can Dash: {canDash}");
                GUI.Label(new Rect(10, 170, 300, 20), $"Can Charge: {canCharge}");
                GUI.Label(new Rect(10, 190, 300, 20), $"Dash Cooldown: {(canDash ? "Ready" : $"{dashCooldown - timeSinceLastDash:F1}s")}");
                GUI.Label(new Rect(10, 210, 300, 20), $"Charge Cooldown: {(canCharge ? "Ready" : $"{chargeCooldown - timeSinceLastCharge:F1}s")}");
                GUI.Label(new Rect(10, 230, 300, 20), $"Dash Target: {_dashTargetPosition}");
                GUI.Label(new Rect(10, 250, 300, 20), $"Distance to Player: {(player != null ? Vector2.Distance(transform.position, player.position).ToString("F2") : "N/A")}");
            }
        }
    }
}
