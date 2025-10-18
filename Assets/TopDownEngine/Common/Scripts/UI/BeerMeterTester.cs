using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simple tester script to help debug the beer meter system
    /// Add this to any GameObject to test the beer meter functionality
    /// </summary>
    [AddComponentMenu("TopDown Engine/UI/Beer Meter Tester")]
    public class BeerMeterTester : MonoBehaviour
    {
        [Header("Test Settings")]
        
        /// <summary>
        /// Whether to show debug information
        /// </summary>
        [Tooltip("Whether to show debug information")]
        public bool ShowDebugInfo = true;

        /// <summary>
        /// Whether to automatically create BeerManager and BeerMeterUI
        /// </summary>
        [Tooltip("Whether to automatically create BeerManager and BeerMeterUI")]
        public bool AutoCreateComponents = true;

        protected virtual void Start()
        {
            if (AutoCreateComponents)
            {
                CreateBeerSystem();
            }
        }

        protected virtual void Update()
        {
            // Test with keyboard input
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestBeerLevel(25f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TestBeerLevel(50f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TestBeerLevel(75f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                TestBeerLevel(0f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                TestBeerLevel(100f);
            }
        }

        /// <summary>
        /// Creates the complete beer system for testing
        /// </summary>
        [ContextMenu("Create Beer System")]
        public virtual void CreateBeerSystem()
        {
            // Create BeerManager if it doesn't exist
            if (!BeerManager.HasInstance)
            {
                GameObject beerManagerGO = new GameObject("BeerManager");
                BeerManager beerManager = beerManagerGO.AddComponent<BeerManager>();
                beerManager.ShowDebugInfo = ShowDebugInfo;
                
                if (ShowDebugInfo)
                {
                    Debug.Log("Created BeerManager for testing");
                }
            }

            // Create BeerMeterUI if it doesn't exist
            BeerMeterUI existingUI = FindObjectOfType<BeerMeterUI>();
            if (existingUI == null)
            {
                GameObject beerMeterGO = new GameObject("BeerMeterUI");
                BeerMeterUI beerMeterUI = beerMeterGO.AddComponent<BeerMeterUI>();
                beerMeterUI.ShowDebugInfo = ShowDebugInfo;
                
                if (ShowDebugInfo)
                {
                    Debug.Log("Created BeerMeterUI for testing");
                }
            }
        }

        /// <summary>
        /// Tests the beer meter with a specific level
        /// </summary>
        /// <param name="beerLevel">The beer level to test (0-100)</param>
        [ContextMenu("Test Beer Level 25%")]
        public virtual void TestBeerLevel25()
        {
            TestBeerLevel(25f);
        }

        /// <summary>
        /// Tests the beer meter with a specific level
        /// </summary>
        [ContextMenu("Test Beer Level 50%")]
        public virtual void TestBeerLevel50()
        {
            TestBeerLevel(50f);
        }

        /// <summary>
        /// Tests the beer meter with a specific level
        /// </summary>
        [ContextMenu("Test Beer Level 75%")]
        public virtual void TestBeerLevel75()
        {
            TestBeerLevel(75f);
        }

        /// <summary>
        /// Tests the beer meter with a specific level
        /// </summary>
        /// <param name="beerLevel">The beer level to test (0-100)</param>
        public virtual void TestBeerLevel(float beerLevel)
        {
            if (BeerManager.HasInstance)
            {
                BeerManager.Instance.SetBeerLevel(beerLevel);
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Testing beer level: {beerLevel}%");
                }
            }
            else
            {
                Debug.LogWarning("BeerManager not found! Create the beer system first.");
            }
        }

    }
}
