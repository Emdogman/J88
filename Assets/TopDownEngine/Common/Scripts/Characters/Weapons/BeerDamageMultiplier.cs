using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Multiplies weapon damage based on beer meter level
    /// Higher beer levels = higher damage multiplier
    /// </summary>
    [AddComponentMenu("TopDown Engine/Weapons/Beer Damage Multiplier")]
    public class BeerDamageMultiplier : MonoBehaviour
    {
        [Header("Beer Damage Scaling")]
        
        /// <summary>
        /// Base damage multiplier when beer level is 0
        /// </summary>
        [Tooltip("Base damage multiplier when beer level is 0")]
        public float BaseDamageMultiplier = 1.0f;
        
        /// <summary>
        /// Maximum damage multiplier when beer level is 100
        /// </summary>
        [Tooltip("Maximum damage multiplier when beer level is 100")]
        public float MaxDamageMultiplier = 2.5f;
        
        /// <summary>
        /// How the damage scaling curve works
        /// </summary>
        [Tooltip("How the damage scaling curve works")]
        public AnimationCurve DamageScalingCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 2.5f);
        
        /// <summary>
        /// Whether to show debug information
        /// </summary>
        [Tooltip("Whether to show debug information")]
        public bool ShowDebugInfo = false;
        
        /// <summary>
        /// Whether the damage multiplier is active
        /// </summary>
        [Tooltip("Whether the damage multiplier is active")]
        public bool IsActive = true;
        
        [Header("Beer Stages")]
        
        /// <summary>
        /// Damage multiplier for Stage 1 (0-33% beer)
        /// </summary>
        [Tooltip("Damage multiplier for Stage 1 (0-33% beer)")]
        public float Stage1Multiplier = 1.0f;
        
        /// <summary>
        /// Damage multiplier for Stage 2 (34-66% beer)
        /// </summary>
        [Tooltip("Damage multiplier for Stage 2 (34-66% beer)")]
        public float Stage2Multiplier = 1.5f;
        
        /// <summary>
        /// Damage multiplier for Stage 3 (67-99% beer)
        /// </summary>
        [Tooltip("Damage multiplier for Stage 3 (67-99% beer)")]
        public float Stage3Multiplier = 2.0f;
        
        /// <summary>
        /// Damage multiplier for Stage 4 (100% beer - before stun)
        /// </summary>
        [Tooltip("Damage multiplier for Stage 4 (100% beer - before stun)")]
        public float Stage4Multiplier = 2.5f;
        
        [Header("Visual Feedback")]
        
        /// <summary>
        /// Whether to show damage multiplier in UI
        /// </summary>
        [Tooltip("Whether to show damage multiplier in UI")]
        public bool ShowDamageMultiplierInUI = true;
        
        /// <summary>
        /// UI text to display current damage multiplier
        /// </summary>
        [Tooltip("UI text to display current damage multiplier")]
        public UnityEngine.UI.Text DamageMultiplierText;
        
        // Private variables
        private BeerManager _beerManager;
        private Weapon _weapon;
        private MeleeWeapon _meleeWeapon;
        private HitscanWeapon _hitscanWeapon;
        private DamageOnTouch _damageOnTouch;
        private float _originalMinDamage;
        private float _originalMaxDamage;
        private float _currentMultiplier = 1.0f;
        private int _lastBeerStage = 1;
        
        /// <summary>
        /// Gets the current damage multiplier
        /// </summary>
        public float CurrentDamageMultiplier => _currentMultiplier;
        
        /// <summary>
        /// Gets the current beer stage
        /// </summary>
        public int CurrentBeerStage => _lastBeerStage;
        
        protected virtual void Awake()
        {
            _weapon = GetComponent<Weapon>();
            if (_weapon == null)
            {
                Debug.LogError("BeerDamageMultiplier: No Weapon component found! This component must be attached to a weapon.");
                enabled = false;
                return;
            }
            
            // Get specific weapon types
            _meleeWeapon = GetComponent<MeleeWeapon>();
            _hitscanWeapon = GetComponent<HitscanWeapon>();
            
            // Store original damage values based on weapon type
            if (_meleeWeapon != null)
            {
                _originalMinDamage = _meleeWeapon.MinDamageCaused;
                _originalMaxDamage = _meleeWeapon.MaxDamageCaused;
            }
            else if (_hitscanWeapon != null)
            {
                _originalMinDamage = _hitscanWeapon.MinDamageCaused;
                _originalMaxDamage = _hitscanWeapon.MaxDamageCaused;
            }
            else
            {
                Debug.LogWarning("BeerDamageMultiplier: No MeleeWeapon or HitscanWeapon found! Damage scaling will not work.");
                enabled = false;
                return;
            }
        }
        
        protected virtual void Start()
        {
            // Find BeerManager
            _beerManager = BeerManager.Instance;
            if (_beerManager == null)
            {
                Debug.LogWarning("BeerDamageMultiplier: BeerManager not found! Damage scaling will not work.");
                enabled = false;
                return;
            }
            
            // Find DamageOnTouch component (for MeleeWeapon)
            if (_meleeWeapon != null)
            {
                _damageOnTouch = GetComponentInChildren<DamageOnTouch>();
                if (_damageOnTouch == null)
                {
                    Debug.LogWarning("BeerDamageMultiplier: No DamageOnTouch component found for MeleeWeapon. Damage scaling may not work properly.");
                }
            }
            
            // Subscribe to beer level changes
            BeerManager.OnBeerLevelChanged += OnBeerLevelChanged;
            BeerManager.OnBeerZoneChanged += OnBeerZoneChanged;
            
            // Apply initial damage scaling
            UpdateDamageMultiplier();
            
            if (ShowDebugInfo)
            {
                Debug.Log($"BeerDamageMultiplier: Initialized - Base: {BaseDamageMultiplier}, Max: {MaxDamageMultiplier}");
            }
        }
        
        protected virtual void OnDestroy()
        {
            // Unsubscribe from events
            BeerManager.OnBeerLevelChanged -= OnBeerLevelChanged;
            BeerManager.OnBeerZoneChanged -= OnBeerZoneChanged;
        }
        
        /// <summary>
        /// Called when beer level changes
        /// </summary>
        protected virtual void OnBeerLevelChanged(float newBeerLevel)
        {
            if (!IsActive) return;
            
            UpdateDamageMultiplier();
        }
        
        /// <summary>
        /// Called when beer zone changes
        /// </summary>
        protected virtual void OnBeerZoneChanged(BeerZoneChangeEvent zoneEvent)
        {
            if (!IsActive) return;
            
            _lastBeerStage = zoneEvent.NewZone;
            UpdateDamageMultiplier();
            
            if (ShowDebugInfo)
            {
                Debug.Log($"BeerDamageMultiplier: Beer zone changed to {zoneEvent.NewZone}, multiplier: {_currentMultiplier:F2}");
            }
        }
        
        /// <summary>
        /// Updates the damage multiplier based on current beer level
        /// </summary>
        protected virtual void UpdateDamageMultiplier()
        {
            if (_beerManager == null) return;
            
            float beerLevel = _beerManager.CurrentBeer;
            float normalizedBeerLevel = beerLevel / 100f; // Normalize to 0-1
            
            // Calculate multiplier based on beer level
            float multiplier = CalculateDamageMultiplier(beerLevel);
            
            // Apply multiplier to weapon damage based on weapon type
            if (_meleeWeapon != null)
            {
                _meleeWeapon.MinDamageCaused = _originalMinDamage * multiplier;
                _meleeWeapon.MaxDamageCaused = _originalMaxDamage * multiplier;
                
                // Also update the DamageOnTouch component if it exists
                if (_damageOnTouch != null)
                {
                    _damageOnTouch.MinDamageCaused = _originalMinDamage * multiplier;
                    _damageOnTouch.MaxDamageCaused = _originalMaxDamage * multiplier;
                }
            }
            else if (_hitscanWeapon != null)
            {
                _hitscanWeapon.MinDamageCaused = _originalMinDamage * multiplier;
                _hitscanWeapon.MaxDamageCaused = _originalMaxDamage * multiplier;
            }
            
            _currentMultiplier = multiplier;
            
            // Update UI if available
            UpdateDamageMultiplierUI();
            
            if (ShowDebugInfo)
            {
                float currentMinDamage = _meleeWeapon != null ? _meleeWeapon.MinDamageCaused : _hitscanWeapon.MinDamageCaused;
                float currentMaxDamage = _meleeWeapon != null ? _meleeWeapon.MaxDamageCaused : _hitscanWeapon.MaxDamageCaused;
                string damageOnTouchInfo = "";
                if (_damageOnTouch != null)
                {
                    damageOnTouchInfo = $" (DamageOnTouch: {_damageOnTouch.MinDamageCaused:F1}-{_damageOnTouch.MaxDamageCaused:F1})";
                }
                Debug.Log($"BeerDamageMultiplier: Beer level {beerLevel:F1} -> Multiplier {multiplier:F2} (Weapon: {currentMinDamage:F1}-{currentMaxDamage:F1}){damageOnTouchInfo}");
            }
        }
        
        /// <summary>
        /// Calculates damage multiplier based on beer level
        /// </summary>
        protected virtual float CalculateDamageMultiplier(float beerLevel)
        {
            // Use stage-based scaling
            if (beerLevel <= 33f)
            {
                return Stage1Multiplier;
            }
            else if (beerLevel <= 66f)
            {
                // Interpolate between stage 1 and 2
                float t = (beerLevel - 33f) / 33f;
                return Mathf.Lerp(Stage1Multiplier, Stage2Multiplier, t);
            }
            else if (beerLevel <= 99f)
            {
                // Interpolate between stage 2 and 3
                float t = (beerLevel - 66f) / 33f;
                return Mathf.Lerp(Stage2Multiplier, Stage3Multiplier, t);
            }
            else
            {
                // Stage 4 (100% beer)
                return Stage4Multiplier;
            }
        }
        
        /// <summary>
        /// Updates the damage multiplier UI display
        /// </summary>
        protected virtual void UpdateDamageMultiplierUI()
        {
            if (!ShowDamageMultiplierInUI || DamageMultiplierText == null) return;
            
            DamageMultiplierText.text = $"Damage: {_currentMultiplier:F1}x";
        }
        
        /// <summary>
        /// Resets damage to original values
        /// </summary>
        [ContextMenu("Reset Damage")]
        public virtual void ResetDamage()
        {
            if (_meleeWeapon != null)
            {
                _meleeWeapon.MinDamageCaused = _originalMinDamage;
                _meleeWeapon.MaxDamageCaused = _originalMaxDamage;
                
                // Also reset the DamageOnTouch component if it exists
                if (_damageOnTouch != null)
                {
                    _damageOnTouch.MinDamageCaused = _originalMinDamage;
                    _damageOnTouch.MaxDamageCaused = _originalMaxDamage;
                }
            }
            else if (_hitscanWeapon != null)
            {
                _hitscanWeapon.MinDamageCaused = _originalMinDamage;
                _hitscanWeapon.MaxDamageCaused = _originalMaxDamage;
            }
            
            _currentMultiplier = 1.0f;
            
            if (ShowDebugInfo)
            {
                Debug.Log("BeerDamageMultiplier: Damage reset to original values");
            }
        }
        
        /// <summary>
        /// Force update damage multiplier
        /// </summary>
        [ContextMenu("Force Update Multiplier")]
        public virtual void ForceUpdateMultiplier()
        {
            UpdateDamageMultiplier();
        }
        
        /// <summary>
        /// Test damage multiplier with specific beer level
        /// </summary>
        [ContextMenu("Test Beer Level 50")]
        public virtual void TestBeerLevel50()
        {
            if (_beerManager != null)
            {
                _beerManager.CurrentBeer = 50f;
                Debug.Log("BeerDamageMultiplier: Set beer level to 50 for testing");
            }
        }
        
        /// <summary>
        /// Test damage multiplier with specific beer level
        /// </summary>
        [ContextMenu("Test Beer Level 80")]
        public virtual void TestBeerLevel80()
        {
            if (_beerManager != null)
            {
                _beerManager.CurrentBeer = 80f;
                Debug.Log("BeerDamageMultiplier: Set beer level to 80 for testing");
            }
        }
        
        /// <summary>
        /// Test damage multiplier with specific beer level
        /// </summary>
        [ContextMenu("Test Beer Level 100")]
        public virtual void TestBeerLevel100()
        {
            if (_beerManager != null)
            {
                _beerManager.CurrentBeer = 100f;
                Debug.Log("BeerDamageMultiplier: Set beer level to 100 for testing");
            }
        }
        
        /// <summary>
        /// Show current status
        /// </summary>
        [ContextMenu("Show Status")]
        public virtual void ShowStatus()
        {
            if (_beerManager != null)
            {
                float currentMinDamage = _meleeWeapon != null ? _meleeWeapon.MinDamageCaused : (_hitscanWeapon != null ? _hitscanWeapon.MinDamageCaused : 0f);
                float currentMaxDamage = _meleeWeapon != null ? _meleeWeapon.MaxDamageCaused : (_hitscanWeapon != null ? _hitscanWeapon.MaxDamageCaused : 0f);
                
                Debug.Log($"BeerDamageMultiplier Status:");
                Debug.Log($"  - Beer Level: {_beerManager.CurrentBeer:F1}");
                Debug.Log($"  - Beer Stage: {_lastBeerStage}");
                Debug.Log($"  - Current Multiplier: {_currentMultiplier:F2}");
                Debug.Log($"  - Weapon Type: {(_meleeWeapon != null ? "MeleeWeapon" : (_hitscanWeapon != null ? "HitscanWeapon" : "Unknown"))}");
                Debug.Log($"  - Weapon Min Damage: {currentMinDamage:F1}");
                Debug.Log($"  - Weapon Max Damage: {currentMaxDamage:F1}");
                Debug.Log($"  - Original Min Damage: {_originalMinDamage:F1}");
                Debug.Log($"  - Original Max Damage: {_originalMaxDamage:F1}");
                if (_damageOnTouch != null)
                {
                    Debug.Log($"  - DamageOnTouch Min: {_damageOnTouch.MinDamageCaused:F1}");
                    Debug.Log($"  - DamageOnTouch Max: {_damageOnTouch.MaxDamageCaused:F1}");
                }
                else
                {
                    Debug.Log($"  - DamageOnTouch: Not found");
                }
            }
            else
            {
                Debug.LogError("BeerDamageMultiplier: BeerManager not found!");
            }
        }
    }
}
