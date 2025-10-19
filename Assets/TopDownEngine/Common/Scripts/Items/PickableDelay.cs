using UnityEngine;
using System.Collections;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Makes a PickableItem unpickable for a specified delay after spawning
    /// Useful for enemy drops to prevent immediate pickup during combat
    /// </summary>
    [RequireComponent(typeof(PickableItem))]
    public class PickableDelay : MonoBehaviour
    {
        [Header("Pickup Delay Settings")]
        [Tooltip("Time in seconds before the item becomes pickable")]
        public float pickupDelay = 1f;
        
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private PickableItem _pickableItem;
        private ItemPicker _itemPicker;
        private bool _originalPickableState;
        private float _spawnTime;
        private bool _isReady = false;

        private void Start()
        {
            // Get references
            _pickableItem = GetComponent<PickableItem>();
            _itemPicker = GetComponent<ItemPicker>();
            
            if (_pickableItem == null && _itemPicker == null)
            {
                Debug.LogError("PickableDelay: No PickableItem or ItemPicker component found!");
                enabled = false;
                return;
            }
            
            // Store spawn time
            _spawnTime = Time.time;
            
            // Disable collider temporarily to prevent pickup
            DisablePickup();
            
            // Start the delay coroutine
            StartCoroutine(EnablePickupAfterDelay());
            
            if (showDebugInfo)
            {
                Debug.Log($"PickableDelay: Item will be pickable in {pickupDelay} seconds");
            }
        }

        /// <summary>
        /// Disables pickup by disabling the collider
        /// </summary>
        private void DisablePickup()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
            
            Collider col3D = GetComponent<Collider>();
            if (col3D != null)
            {
                col3D.enabled = false;
            }
        }

        /// <summary>
        /// Enables pickup after the delay
        /// </summary>
        private IEnumerator EnablePickupAfterDelay()
        {
            yield return new WaitForSeconds(pickupDelay);
            
            // Re-enable collider
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
            }
            
            Collider col3D = GetComponent<Collider>();
            if (col3D != null)
            {
                col3D.enabled = true;
            }
            
            _isReady = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"PickableDelay: Item is now pickable after {pickupDelay}s delay");
            }
            
            // Destroy this component once delay is complete (no longer needed)
            Destroy(this);
        }

        /// <summary>
        /// Visual feedback for delay state
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            if (!_isReady)
            {
                // Draw red sphere to indicate not ready
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                Gizmos.DrawWireSphere(transform.position, 0.3f);
            }
        }
    }
}

