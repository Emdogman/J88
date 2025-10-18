using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simple AI action that makes the character move towards the player
    /// Replaces complex AI behaviors with straightforward player chasing
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AI Action Move Towards Player")]
    public class AIActionMoveTowardsPlayer : AIAction
    {
        [Header("Target Settings")]
        [Tooltip("Tag to search for when finding the player")]
        public string PlayerTag = "Player";
        
        [Tooltip("How often to search for player if not found (seconds)")]
        public float PlayerSearchInterval = 1f;

        [Header("Movement Settings")]
        [Tooltip("Enable enemy avoidance behavior")]
        public bool UseAvoidance = false;
        
        [Tooltip("Radius for enemy avoidance")]
        public float AvoidanceRadius = 1f;
        
        [Tooltip("Strength of avoidance force")]
        public float AvoidanceStrength = 0.3f;
        
        [Tooltip("Layer mask for enemies to avoid")]
        public LayerMask EnemyLayerMask = -1;

        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool ShowDebugInfo = false;

        protected Transform _player;
        protected float _lastPlayerSearchTime;
        protected readonly Collider2D[] _enemyHits = new Collider2D[32];
        protected ContactFilter2D _enemyFilter;

        public override void Initialization()
        {
            base.Initialization();
            
            // Setup enemy avoidance filter
            _enemyFilter = new ContactFilter2D
            {
                useTriggers = true 
            };
            _enemyFilter.SetLayerMask(EnemyLayerMask);
            
            // Try to find player immediately
            FindPlayer();
        }

        public override void PerformAction()
        {
            // Check if we need to find the player
            if (_player == null)
            {
                if (Time.time - _lastPlayerSearchTime > PlayerSearchInterval)
                {
                    FindPlayer();
                    _lastPlayerSearchTime = Time.time;
                }
                return;
            }
            
            // Calculate movement direction
            Vector2 movementDirection = CalculateMovementDirection();
            
            // Apply movement to character
            if (_brain != null)
            {
                TopDownController controller = _brain.GetComponent<TopDownController>();
                if (controller != null)
                {
                    controller.SetMovement(movementDirection);
                }
            }
            
            if (ShowDebugInfo)
            {
                Debug.Log($"AIActionMoveTowardsPlayer: Moving towards player. Direction: {movementDirection}");
            }
        }

        /// <summary>
        /// Finds the player using the specified tag
        /// </summary>
        protected virtual void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(PlayerTag);
            if (playerObject != null)
            {
                _player = playerObject.transform;
                if (ShowDebugInfo)
                {
                    Debug.Log($"AIActionMoveTowardsPlayer: Found player '{playerObject.name}' with tag '{PlayerTag}'");
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.LogWarning($"AIActionMoveTowardsPlayer: No GameObject found with tag '{PlayerTag}'");
                }
            }
        }

        /// <summary>
        /// Calculates the movement direction towards player with optional avoidance
        /// </summary>
        protected virtual Vector2 CalculateMovementDirection()
        {
            if (_player == null) return Vector2.zero;
            
            Vector2 position = transform.position;
            Vector2 direction = ((Vector2)_player.position - position).normalized;
            
            // Apply enemy avoidance if enabled
            if (UseAvoidance)
            {
                Vector2 avoidance = CalculateAvoidance(position);
                direction = (direction + avoidance * AvoidanceStrength).normalized;
            }
            
            return direction;
        }

        /// <summary>
        /// Calculates avoidance force from other enemies
        /// </summary>
        protected virtual Vector2 CalculateAvoidance(Vector2 position)
        {
            Vector2 avoidance = Vector2.zero;
            int hitCount = Physics2D.OverlapCircle(position, AvoidanceRadius, _enemyFilter, _enemyHits);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = _enemyHits[i];
                if (hit == null || hit.transform == transform) continue;

                Vector2 away = (Vector2)transform.position - (Vector2)hit.transform.position;
                float dist = away.magnitude;
                if (!(dist > 0f) || !(dist < AvoidanceRadius)) continue;
                
                float distFactor = 1f - (dist / AvoidanceRadius);
                avoidance += away / dist * distFactor;
            }

            return avoidance;
        }

        /// <summary>
        /// Manually set the player reference
        /// </summary>
        /// <param name="playerTransform">The player's transform</param>
        public virtual void SetPlayer(Transform playerTransform)
        {
            _player = playerTransform;
            if (ShowDebugInfo)
            {
                Debug.Log($"AIActionMoveTowardsPlayer: Player manually set to '{playerTransform.name}'");
            }
        }

        /// <summary>
        /// Force search for player (useful for debugging)
        /// </summary>
        [ContextMenu("Find Player")]
        public virtual void ForceFindPlayer()
        {
            FindPlayer();
        }

        /// <summary>
        /// OnGUI for debug information display
        /// </summary>
        protected virtual void OnGUI()
        {
            if (ShowDebugInfo)
            {
                GUI.Label(new Rect(10, 10, 300, 20), $"Player: {(_player != null ? _player.name : "Not Found")}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Player Tag: {PlayerTag}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Use Avoidance: {UseAvoidance}");
                if (UseAvoidance)
                {
                    GUI.Label(new Rect(10, 70, 300, 20), $"Avoidance Radius: {AvoidanceRadius:F1}");
                }
            }
        }
    }
}
