using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Comprehensive debug script for the beer meter system
    /// Add this to any GameObject to debug the entire beer system
    /// </summary>
    [AddComponentMenu("TopDown Engine/UI/Beer System Debugger")]
    public class BeerSystemDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        
        /// <summary>
        /// Whether to show debug information
        /// </summary>
        [Tooltip("Whether to show debug information")]
        public bool ShowDebugInfo = true;

        /// <summary>
        /// Whether to automatically create all components
        /// </summary>
        [Tooltip("Whether to automatically create all components")]
        public bool AutoCreateComponents = true;

        /// <summary>
        /// Whether to show on-screen debug information
        /// </summary>
        [Tooltip("Whether to show on-screen debug information")]
        public bool ShowOnScreenDebug = true;

        protected BeerManager _beerManager;
        protected BeerMeterUI _beerMeterUI;
        protected CharacterMovementStaged _characterMovement;

        protected virtual void Start()
        {
            if (AutoCreateComponents)
            {
                CreateAllComponents();
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
            else if (Input.GetKeyDown(KeyCode.R))
            {
                RefreshAllComponents();
            }
        }

        /// <summary>
        /// Creates all beer system components
        /// </summary>
        [ContextMenu("Create All Components")]
        public virtual void CreateAllComponents()
        {
            // Create BeerManager
            if (!BeerManager.HasInstance)
            {
                GameObject beerManagerGO = new GameObject("BeerManager");
                _beerManager = beerManagerGO.AddComponent<BeerManager>();
                _beerManager.ShowDebugInfo = ShowDebugInfo;
                Debug.Log("Created BeerManager");
            }
            else
            {
                _beerManager = BeerManager.Instance;
            }

            // Create BeerMeterUI
            _beerMeterUI = FindObjectOfType<BeerMeterUI>();
            if (_beerMeterUI == null)
            {
                GameObject beerMeterGO = new GameObject("BeerMeterUI");
                _beerMeterUI = beerMeterGO.AddComponent<BeerMeterUI>();
                _beerMeterUI.ShowDebugInfo = ShowDebugInfo;
                Debug.Log("Created BeerMeterUI");
            }

            // Find CharacterMovementStaged
            _characterMovement = FindObjectOfType<CharacterMovementStaged>();
            if (_characterMovement == null)
            {
                Debug.LogWarning("CharacterMovementStaged not found! Make sure your character has this component.");
            }
            else
            {
                _characterMovement.ShowDebugInfo = ShowDebugInfo;
                Debug.Log("Found CharacterMovementStaged");
            }
        }

        /// <summary>
        /// Tests the beer system with a specific level
        /// </summary>
        /// <param name="beerLevel">The beer level to test (0-100)</param>
        public virtual void TestBeerLevel(float beerLevel)
        {
            if (_beerManager != null)
            {
                _beerManager.SetBeerLevel(beerLevel);
                Debug.Log($"Testing beer level: {beerLevel}%");
            }
            else
            {
                Debug.LogWarning("BeerManager not found! Create components first.");
            }
        }

        /// <summary>
        /// Refreshes all components
        /// </summary>
        [ContextMenu("Refresh All Components")]
        public virtual void RefreshAllComponents()
        {
            if (_beerManager != null)
            {
                float currentBeer = _beerManager.CurrentBeer;
                Debug.Log($"BeerManager - Current Beer: {currentBeer:F1}%, Zone: {_beerManager.CurrentZone}");
            }

            if (_beerMeterUI != null)
            {
                _beerMeterUI.RefreshFromBeerManager();
            }

            if (_characterMovement != null)
            {
                _characterMovement.RefreshFromBeerManager();
            }

            Debug.Log("All components refreshed!");
        }

        /// <summary>
        /// Gets the current beer level from BeerManager
        /// </summary>
        /// <returns>Current beer level (0-100)</returns>
        public virtual float GetCurrentBeerLevel()
        {
            if (_beerManager != null)
            {
                return _beerManager.CurrentBeer;
            }
            return 0f;
        }

        /// <summary>
        /// Gets the current movement stage from CharacterMovementStaged
        /// </summary>
        /// <returns>Current movement stage</returns>
        public virtual CharacterMovementStaged.MovementStage GetCurrentMovementStage()
        {
            if (_characterMovement != null)
            {
                return _characterMovement.CurrentStage;
            }
            return CharacterMovementStaged.MovementStage.Stage1_Normal;
        }

        /// <summary>
        /// Tests beer level 25%
        /// </summary>
        [ContextMenu("Test Beer Level 25%")]
        public virtual void TestBeerLevel25()
        {
            TestBeerLevel(25f);
        }

        /// <summary>
        /// Tests beer level 50%
        /// </summary>
        [ContextMenu("Test Beer Level 50%")]
        public virtual void TestBeerLevel50()
        {
            TestBeerLevel(50f);
        }

        /// <summary>
        /// Tests beer level 75%
        /// </summary>
        [ContextMenu("Test Beer Level 75%")]
        public virtual void TestBeerLevel75()
        {
            TestBeerLevel(75f);
        }

        protected virtual void OnGUI()
        {
            if (ShowOnScreenDebug)
            {
                int yOffset = 10;
                
                GUI.Label(new Rect(10, yOffset, 400, 20), "=== Beer System Debugger ===");
                yOffset += 25;
                
                GUI.Label(new Rect(10, yOffset, 400, 20), "Press 1,2,3,0,9 to test beer levels, R to refresh");
                yOffset += 25;
                
                if (_beerManager != null)
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), $"BeerManager - Level: {_beerManager.CurrentBeer:F1}%, Zone: {_beerManager.CurrentZone}");
                    yOffset += 20;
                }
                else
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), "BeerManager: NOT FOUND");
                    yOffset += 20;
                }
                
                if (_beerMeterUI != null)
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), "BeerMeterUI: FOUND");
                    yOffset += 20;
                }
                else
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), "BeerMeterUI: NOT FOUND");
                    yOffset += 20;
                }
                
                if (_characterMovement != null)
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), $"CharacterMovement - Stage: {_characterMovement.CurrentStage}, Deceleration: {_characterMovement.Deceleration}");
                    yOffset += 20;
                }
                else
                {
                    GUI.Label(new Rect(10, yOffset, 400, 20), "CharacterMovementStaged: NOT FOUND");
                    yOffset += 20;
                }
            }
        }
    }
}
