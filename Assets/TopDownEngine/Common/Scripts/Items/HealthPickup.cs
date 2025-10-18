using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Health pickup that restores a percentage of the player's maximum health
    /// </summary>
    public class HealthPickup : PickableItem
    {
        [Header("Health Restore Settings")]
        [Tooltip("Percentage of max health to restore (0-1, where 1 = 100%)")]
        [SerializeField] private float healthRestorePercentage = 0.25f;
        
        [Tooltip("Minimum health amount to restore (if percentage is too low)")]
        [SerializeField] private float minimumHealthRestore = 10f;
        
        [Tooltip("Only heal if player health is below this percentage (0-1)")]
        [SerializeField] private float healThreshold = 1f;
        
        [Header("Visual Feedback")]
        [Tooltip("Particle effect to play on pickup")]
        [SerializeField] private GameObject pickupEffect;
        
        [Tooltip("Sound to play on pickup")]
        [SerializeField] private AudioClip pickupSound;
        
        [Header("Auto-Pickup")]
        [Tooltip("Automatically pick up when player touches it")]
        [SerializeField] private bool autoPickup = true;
        
        [Tooltip("Pickup trigger radius")]
        [SerializeField] private float pickupRadius = 1f;
        
        [Header("Debug")]
        [Tooltip("Enable debug logging")]
        [SerializeField] private bool debugMode = false;
        
        private bool _hasBeenPickedUp = false;
        private CircleCollider2D _pickupCollider;
        
        protected override void Start()
        {
            base.Start();
            
            // Setup auto-pickup collider if enabled
            if (autoPickup)
            {
                SetupAutoPickup();
            }
        }
        
        /// <summary>
        /// Sets up the auto-pickup collider
        /// </summary>
        private void SetupAutoPickup()
        {
            // Check if we already have a trigger collider
            _pickupCollider = GetComponent<CircleCollider2D>();
            
            if (_pickupCollider == null)
            {
                _pickupCollider = gameObject.AddComponent<CircleCollider2D>();
            }
            
            _pickupCollider.isTrigger = true;
            _pickupCollider.radius = pickupRadius;
        }
        
        /// <summary>
        /// Called when something enters the pickup trigger
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!autoPickup || _hasBeenPickedUp) return;
            
            // Check if it's the player
            if (other.CompareTag("Player"))
            {
                Character character = other.GetComponent<Character>();
                if (character != null && character.CharacterType == Character.CharacterTypes.Player)
                {
                    Pick(character.gameObject);
                }
            }
        }
        
        /// <summary>
        /// What happens when the object gets picked
        /// </summary>
        protected override void Pick(GameObject picker)
        {
            if (_hasBeenPickedUp) return;
            
            // Get the Health component from the picker
            Health playerHealth = picker.GetComponent<Health>();
            
            if (playerHealth == null)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"HealthPickup: No Health component found on {picker.name}");
                }
                return;
            }
            
            // Check if player needs healing
            float currentHealthPercentage = playerHealth.CurrentHealth / playerHealth.MaximumHealth;
            
            if (currentHealthPercentage >= healThreshold)
            {
                if (debugMode)
                {
                    Debug.Log($"HealthPickup: Player health is already above threshold ({currentHealthPercentage:P0} >= {healThreshold:P0})");
                }
                return;
            }
            
            // Calculate heal amount
            float healAmount = CalculateHealAmount(playerHealth);
            
            // Restore health
            float healthBefore = playerHealth.CurrentHealth;
            playerHealth.SetHealth(playerHealth.CurrentHealth + healAmount);
            float healthAfter = playerHealth.CurrentHealth;
            float actualHealed = healthAfter - healthBefore;
            
            if (debugMode)
            {
                Debug.Log($"HealthPickup: Healed {picker.name} for {actualHealed} HP ({healthBefore} → {healthAfter})");
            }
            
            // Play effects
            PlayPickupEffects();
            
            // Mark as picked up
            _hasBeenPickedUp = true;
            
            // Disable the pickup
            DisablePickup();
        }
        
        /// <summary>
        /// Calculates the amount of health to restore
        /// </summary>
        private float CalculateHealAmount(Health playerHealth)
        {
            // Calculate percentage-based heal
            float percentageHeal = playerHealth.MaximumHealth * healthRestorePercentage;
            
            // Use the higher of percentage heal or minimum heal
            float healAmount = Mathf.Max(percentageHeal, minimumHealthRestore);
            
            // Don't heal above max health
            float maxPossibleHeal = playerHealth.MaximumHealth - playerHealth.CurrentHealth;
            healAmount = Mathf.Min(healAmount, maxPossibleHeal);
            
            return healAmount;
        }
        
        /// <summary>
        /// Plays visual and audio effects
        /// </summary>
        private void PlayPickupEffects()
        {
            // Play particle effect
            if (pickupEffect != null)
            {
                GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Play sound effect
            if (pickupSound != null)
            {
                MMSoundManagerSoundPlayEvent.Trigger(pickupSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
            }
        }
        
        /// <summary>
        /// Disables the pickup after being collected
        /// </summary>
        private void DisablePickup()
        {
            // Disable collider
            if (_pickupCollider != null)
            {
                _pickupCollider.enabled = false;
            }
            
            // Disable renderer
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            
            // Destroy after a short delay (to allow effects to play)
            Destroy(gameObject, 0.5f);
        }
        
        /// <summary>
        /// Draws the pickup radius in the editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (autoPickup)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, pickupRadius);
            }
        }
        
        [ContextMenu("Test Heal Player")]
        public void TestHealPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Pick(player);
            }
            else
            {
                Debug.LogWarning("HealthPickup: No player found for testing");
            }
        }
        
        [ContextMenu("Show Debug Info")]
        public void ShowDebugInfo()
        {
            Debug.Log("=== Health Pickup Debug Info ===");
            Debug.Log($"Health Restore Percentage: {healthRestorePercentage:P0}");
            Debug.Log($"Minimum Health Restore: {minimumHealthRestore}");
            Debug.Log($"Heal Threshold: {healThreshold:P0}");
            Debug.Log($"Auto Pickup: {autoPickup}");
            Debug.Log($"Pickup Radius: {pickupRadius}");
            Debug.Log($"Has Been Picked Up: {_hasBeenPickedUp}");
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    float healAmount = CalculateHealAmount(playerHealth);
                    Debug.Log($"Player Current Health: {playerHealth.CurrentHealth}/{playerHealth.MaximumHealth}");
                    Debug.Log($"Would Heal For: {healAmount} HP");
                }
            }
        }
    }
}

