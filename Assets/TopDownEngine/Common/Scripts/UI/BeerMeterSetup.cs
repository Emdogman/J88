using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simple setup script to quickly add a beer meter to any scene
    /// Just add this component to any GameObject and it will create a complete beer meter system
    /// </summary>
    [AddComponentMenu("TopDown Engine/UI/Beer Meter Setup")]
    public class BeerMeterSetup : MonoBehaviour
    {
        [Header("Auto Setup Options")]
        
        /// <summary>
        /// Whether to create the BeerManager if it doesn't exist
        /// </summary>
        [Tooltip("Whether to create the BeerManager if it doesn't exist")]
        public bool CreateBeerManager = true;

        /// <summary>
        /// Whether to show debug information during setup
        /// </summary>
        [Tooltip("Whether to show debug information during setup")]
        public bool ShowDebugInfo = true;

        /// <summary>
        /// Whether to destroy this setup component after setup is complete
        /// </summary>
        [Tooltip("Whether to destroy this setup component after setup is complete")]
        public bool DestroyAfterSetup = true;

        protected virtual void Start()
        {
            SetupBeerMeterSystem();
        }

        /// <summary>
        /// Sets up the complete beer meter system
        /// </summary>
        [ContextMenu("Setup Beer Meter System")]
        public virtual void SetupBeerMeterSystem()
        {
            if (ShowDebugInfo)
            {
                Debug.Log("Setting up Beer Meter System...");
            }

            // Create BeerManager if needed
            if (CreateBeerManager && !BeerManager.HasInstance)
            {
                GameObject beerManagerGO = new GameObject("BeerManager");
                beerManagerGO.AddComponent<BeerManager>();
                
                if (ShowDebugInfo)
                {
                    Debug.Log("Created BeerManager");
                }
            }

            // Create BeerMeterUI
            GameObject beerMeterGO = new GameObject("BeerMeterUI");
            BeerMeterUI beerMeterUI = beerMeterGO.AddComponent<BeerMeterUI>();
            beerMeterUI.ShowDebugInfo = ShowDebugInfo;

            // Position the beer meter in the top-right corner
            RectTransform rectTransform = beerMeterGO.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = beerMeterGO.AddComponent<RectTransform>();
            }

            // Set up positioning (top center, thin and long)
            rectTransform.anchorMin = new Vector2(0.2f, 0.9f);
            rectTransform.anchorMax = new Vector2(0.8f, 0.95f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            if (ShowDebugInfo)
            {
                Debug.Log("Created BeerMeterUI");
            }

            // Destroy this setup component if requested
            if (DestroyAfterSetup)
            {
                if (ShowDebugInfo)
                {
                    Debug.Log("Beer Meter System setup complete! Destroying setup component.");
                }
                Destroy(this);
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.Log("Beer Meter System setup complete!");
                }
            }
        }

        /// <summary>
        /// Quick setup method that can be called from code
        /// </summary>
        /// <returns>The created BeerMeterUI component</returns>
        public static BeerMeterUI QuickSetup()
        {
            // Create BeerManager if it doesn't exist
            if (!BeerManager.HasInstance)
            {
                GameObject beerManagerGO = new GameObject("BeerManager");
                beerManagerGO.AddComponent<BeerManager>();
            }

            // Create BeerMeterUI
            GameObject beerMeterGO = new GameObject("BeerMeterUI");
            BeerMeterUI beerMeterUI = beerMeterGO.AddComponent<BeerMeterUI>();

            // Set up positioning
            RectTransform rectTransform = beerMeterGO.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = beerMeterGO.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = new Vector2(0.2f, 0.9f);
            rectTransform.anchorMax = new Vector2(0.8f, 0.95f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return beerMeterUI;
        }
    }
}
