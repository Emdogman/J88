using UnityEngine;
using System;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Event triggered when the beer zone changes
    /// </summary>
    public struct BeerZoneChangeEvent
    {
        public int NewZone;
        public float BeerLevel;
        
        public BeerZoneChangeEvent(int newZone, float beerLevel)
        {
            NewZone = newZone;
            BeerLevel = beerLevel;
        }
    }

    /// <summary>
    /// Singleton manager that handles the beer meter system
    /// Tracks beer level, handles depletion over time, and manages movement stage zones
    /// </summary>
    [AddComponentMenu("TopDown Engine/Managers/Beer Manager")]
    public class BeerManager : MMSingleton<BeerManager>
    {
        [Header("Beer Meter Settings")]
        
        /// <summary>
        /// The current beer level (0-100)
        /// </summary>
        [Tooltip("The current beer level (0-100)")]
        public float CurrentBeer = 50f;

        /// <summary>
        /// The starting beer level when the game begins
        /// </summary>
        [Tooltip("The starting beer level when the game begins")]
        public float StartingBeer = 50f;

        /// <summary>
        /// How much beer is lost per second
        /// </summary>
        [Tooltip("How much beer is lost per second")]
        public float DepletionRate = 5f;

        /// <summary>
        /// The minimum beer level (always 0)
        /// </summary>
        [Tooltip("The minimum beer level (always 0)")]
        public float MinBeer = 0f;

        /// <summary>
        /// The maximum beer level (always 100)
        /// </summary>
        [Tooltip("The maximum beer level (always 100)")]
        public float MaxBeer = 100f;

        [Header("Zone Settings")]
        
        /// <summary>
        /// Zone 1 threshold (0-33%) - Stage 1 movement (Normal)
        /// </summary>
        [Tooltip("Zone 1 threshold (0-33%) - Stage 1 movement (Normal)")]
        public float Zone1Threshold = 33f;

        /// <summary>
        /// Zone 2 threshold (34-66%) - Stage 2 movement (Drift)
        /// </summary>
        [Tooltip("Zone 2 threshold (34-66%) - Stage 2 movement (Drift)")]
        public float Zone2Threshold = 66f;

        /// <summary>
        /// Zone 3 threshold (67-100%) - Stage 3 movement (High Drift)
        /// </summary>
        [Tooltip("Zone 3 threshold (67-100%) - Stage 3 movement (High Drift)")]
        public float Zone3Threshold = 100f;

        [Header("Debug")]
        
        /// <summary>
        /// Whether to show debug information
        /// </summary>
        [Tooltip("Whether to show debug information")]
        public bool ShowDebugInfo = false;

        /// <summary>
        /// Whether the beer system is active
        /// </summary>
        [Tooltip("Whether the beer system is active")]
        public bool BeerSystemActive = true;

        // Events
        public static event Action<BeerZoneChangeEvent> OnBeerZoneChanged;
        public static event Action<float> OnBeerLevelChanged;

        // Private variables
        protected int _currentZone = 2; // Start in zone 2 (middle)
        protected int _lastZone = 2;
        protected bool _initialized = false;

        /// <summary>
        /// Gets the current beer zone (1, 2, or 3)
        /// </summary>
        public int CurrentZone => _currentZone;

        /// <summary>
        /// Gets the last beer zone
        /// </summary>
        public int LastZone => _lastZone;

        protected override void Awake()
        {
            base.Awake();
            InitializeBeerSystem();
        }

        protected virtual void Start()
        {
            if (!_initialized)
            {
                InitializeBeerSystem();
            }
        }

        protected virtual void Update()
        {
            if (!BeerSystemActive)
            {
                return;
            }

            DepleteBeer();
            UpdateZone();
        }

        /// <summary>
        /// Initializes the beer system
        /// </summary>
        protected virtual void InitializeBeerSystem()
        {
            CurrentBeer = StartingBeer;
            _currentZone = GetZoneFromBeerLevel(CurrentBeer);
            _lastZone = _currentZone;
            _initialized = true;

            // Trigger initial events
            OnBeerLevelChanged?.Invoke(CurrentBeer);
            BeerZoneChangeEvent initialZoneEvent = new BeerZoneChangeEvent(_currentZone, CurrentBeer);
            OnBeerZoneChanged?.Invoke(initialZoneEvent);

            if (ShowDebugInfo)
            {
                Debug.Log($"BeerManager initialized - Beer Level: {CurrentBeer}, Zone: {_currentZone}");
            }
        }

        /// <summary>
        /// Depletes beer over time
        /// </summary>
        protected virtual void DepleteBeer()
        {
            if (CurrentBeer > MinBeer)
            {
                float oldBeer = CurrentBeer;
                CurrentBeer -= DepletionRate * Time.deltaTime;
                CurrentBeer = Mathf.Clamp(CurrentBeer, MinBeer, MaxBeer);
                
                if (ShowDebugInfo && oldBeer != CurrentBeer)
                {
                    Debug.Log($"BeerManager: Beer depleted from {oldBeer:F1} to {CurrentBeer:F1}");
                }
                
                OnBeerLevelChanged?.Invoke(CurrentBeer);
            }
        }

        /// <summary>
        /// Updates the current zone based on beer level
        /// </summary>
        protected virtual void UpdateZone()
        {
            int newZone = GetZoneFromBeerLevel(CurrentBeer);
            
            if (newZone != _currentZone)
            {
                _lastZone = _currentZone;
                _currentZone = newZone;
                
                BeerZoneChangeEvent zoneChangeEvent = new BeerZoneChangeEvent(_currentZone, CurrentBeer);
                OnBeerZoneChanged?.Invoke(zoneChangeEvent);
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Beer Zone Changed: {_lastZone} → {_currentZone} (Beer Level: {CurrentBeer:F1})");
                }
            }
        }

        /// <summary>
        /// Adds beer to the meter
        /// </summary>
        /// <param name="amount">Amount of beer to add</param>
        public virtual void AddBeer(float amount)
        {
            if (!BeerSystemActive)
            {
                return;
            }

            float oldBeer = CurrentBeer;
            CurrentBeer += amount;
            CurrentBeer = Mathf.Clamp(CurrentBeer, MinBeer, MaxBeer);
            
            if (ShowDebugInfo)
            {
                Debug.Log($"BeerManager: Beer added +{amount}, from {oldBeer:F1} to {CurrentBeer:F1}");
            }
            
            OnBeerLevelChanged?.Invoke(CurrentBeer);
        }

        /// <summary>
        /// Gets the zone number based on beer level
        /// </summary>
        /// <param name="beerLevel">The beer level to check</param>
        /// <returns>Zone number (1, 2, or 3)</returns>
        public virtual int GetZoneFromBeerLevel(float beerLevel)
        {
            if (beerLevel >= Zone2Threshold)
            {
                return 3; // Zone 3: 67-100% - Stage 3 (High Drift)
            }
            else if (beerLevel >= Zone1Threshold)
            {
                return 2; // Zone 2: 34-66% - Stage 2 (Drift)
            }
            else
            {
                return 1; // Zone 1: 0-33% - Stage 1 (Normal)
            }
        }

        /// <summary>
        /// Gets the movement stage based on the current zone
        /// </summary>
        /// <returns>Movement stage (1, 2, or 3)</returns>
        public virtual int GetMovementStageFromZone()
        {
            switch (_currentZone)
            {
                case 1:
                    return 3; // Zone 1 → Stage 3 (High Drift)
                case 2:
                    return 2; // Zone 2 → Stage 2 (Drift)
                case 3:
                    return 1; // Zone 3 → Stage 1 (Normal)
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Resets the beer system to starting values
        /// </summary>
        public virtual void ResetBeerSystem()
        {
            CurrentBeer = StartingBeer;
            _currentZone = GetZoneFromBeerLevel(CurrentBeer);
            _lastZone = _currentZone;
            
            OnBeerLevelChanged?.Invoke(CurrentBeer);
            
            if (ShowDebugInfo)
            {
                Debug.Log("Beer system reset");
            }
        }

        /// <summary>
        /// Sets the beer level to a specific value
        /// </summary>
        /// <param name="newLevel">New beer level (0-100)</param>
        public virtual void SetBeerLevel(float newLevel)
        {
            CurrentBeer = Mathf.Clamp(newLevel, MinBeer, MaxBeer);
            OnBeerLevelChanged?.Invoke(CurrentBeer);
        }

        /// <summary>
        /// Enables or disables the beer system
        /// </summary>
        /// <param name="active">Whether the system should be active</param>
        public virtual void SetBeerSystemActive(bool active)
        {
            BeerSystemActive = active;
        }

        /// <summary>
        /// Gets the beer level as a normalized value (0-1)
        /// </summary>
        /// <returns>Normalized beer level</returns>
        public virtual float GetNormalizedBeerLevel()
        {
            return CurrentBeer / MaxBeer;
        }

        /// <summary>
        /// Context menu method to test beer level changes
        /// </summary>
        [ContextMenu("Test Beer Level 25%")]
        protected virtual void TestBeerLevel25()
        {
            SetBeerLevel(25f);
        }

        /// <summary>
        /// Context menu method to test beer level changes
        /// </summary>
        [ContextMenu("Test Beer Level 50%")]
        protected virtual void TestBeerLevel50()
        {
            SetBeerLevel(50f);
        }

        /// <summary>
        /// Context menu method to test beer level changes
        /// </summary>
        [ContextMenu("Test Beer Level 75%")]
        protected virtual void TestBeerLevel75()
        {
            SetBeerLevel(75f);
        }

        protected virtual void OnGUI()
        {
            if (ShowDebugInfo)
            {
                GUI.Label(new Rect(10, 70, 300, 20), $"Beer Level: {CurrentBeer:F1}%");
                GUI.Label(new Rect(10, 90, 300, 20), $"Current Zone: {_currentZone}");
                GUI.Label(new Rect(10, 110, 300, 20), $"Movement Stage: {GetMovementStageFromZone()}");
            }
        }
    }
}
