using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// A pickup item that adds beer to the BeerManager when collected
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/Beer Pickup")]
    public class BeerPickup : PickableItem
    {
        [Header("Beer Settings")]
        
        /// <summary>
        /// Amount of beer to add when picked up
        /// </summary>
        [Tooltip("Amount of beer to add when picked up")]
        public float BeerAmount = 20f;

        /// <summary>
        /// Whether to show debug information when picked up
        /// </summary>
        [Tooltip("Whether to show debug information when picked up")]
        public bool ShowDebugInfo = false;

        /// <summary>
        /// Override the Pick method to add beer to the BeerManager
        /// </summary>
        /// <param name="picker">The object that picked up the beer</param>
        protected override void Pick(GameObject picker)
        {
            base.Pick(picker);
            
            // Add beer to the BeerManager if it exists
            if (BeerManager.HasInstance)
            {
                BeerManager.Instance.AddBeer(BeerAmount);
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Beer picked up! Added {BeerAmount} beer. New level: {BeerManager.Instance.CurrentBeer:F1}");
                }
            }
            else
            {
                Debug.LogWarning("BeerManager not found! Beer pickup will not work properly.");
            }
        }

        /// <summary>
        /// Override Effects to add beer-specific effects
        /// </summary>
        protected override void Effects()
        {
            base.Effects();
            
            // You can add beer-specific effects here
            // For example, different particle effects, sounds, etc.
        }
    }
}
