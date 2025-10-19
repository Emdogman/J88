using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Scales weapon damage based on beer level/drunkenness stage
    /// More drunk = More damage
    /// </summary>
    [AddComponentMenu("TopDown Engine/Weapons/Beer Damage Multiplier")]
    public class BeerDamageMultiplier : MonoBehaviour
    {
        [Header("Damage Multipliers by Stage")]
        [Tooltip("Damage multiplier for Stage 1 (0-33% beer) - Sober")]
        public float stage1Multiplier = 1.0f;
        
        [Tooltip("Damage multiplier for Stage 2 (34-66% beer) - Tipsy")]
        public float stage2Multiplier = 1.5f;
        
        [Tooltip("Damage multiplier for Stage 3 (67-100% beer) - Very Drunk")]
        public float stage3Multiplier = 2.0f;
        
        [Header("Settings")]
        [Tooltip("Enable this damage multiplier")]
        public bool isActive = true;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;

        private MeleeWeapon _meleeWeapon;
        private DamageOnTouch _damageOnTouch;
        private DamageOnTouch _childDamageArea; // The damage area created by MeleeWeapon
        private float _originalMinDamage;
        private float _originalMaxDamage;
        private int _lastZone = -1;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!isActive) return;
            if (!BeerManager.HasInstance) return;

            // Check if zone changed
            int currentZone = BeerManager.Instance.CurrentZone;
            if (currentZone != _lastZone)
            {
                UpdateDamageMultiplier(currentZone);
                _lastZone = currentZone;
            }
        }

        /// <summary>
        /// Initialize the component
        /// </summary>
        private void Initialize()
        {
            // Try MeleeWeapon first
            _meleeWeapon = GetComponent<MeleeWeapon>();
            if (_meleeWeapon != null)
            {
                _originalMinDamage = _meleeWeapon.MinDamageCaused;
                _originalMaxDamage = _meleeWeapon.MaxDamageCaused;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerDamageMultiplier: Initialized on MeleeWeapon {gameObject.name}. Base damage: {_originalMinDamage}-{_originalMaxDamage}");
                }
            }
            else
            {
                // Try DamageOnTouch as fallback
                _damageOnTouch = GetComponentInChildren<DamageOnTouch>();
                if (_damageOnTouch != null)
                {
                    _originalMinDamage = _damageOnTouch.MinDamageCaused;
                    _originalMaxDamage = _damageOnTouch.MaxDamageCaused;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"BeerDamageMultiplier: Initialized on DamageOnTouch. Base damage: {_originalMinDamage}-{_originalMaxDamage}");
                    }
                }
                else
                {
                    Debug.LogError($"BeerDamageMultiplier: No MeleeWeapon or DamageOnTouch found on {gameObject.name}!");
                    enabled = false;
                    return;
                }
            }

            // Set initial multiplier if BeerManager exists
            if (BeerManager.HasInstance)
            {
                _lastZone = BeerManager.Instance.CurrentZone;
                UpdateDamageMultiplier(_lastZone);
            }
        }

        /// <summary>
        /// Updates damage based on current zone
        /// </summary>
        private void UpdateDamageMultiplier(int zone)
        {
            float multiplier = GetMultiplierForZone(zone);
            
            // Apply multiplier to appropriate component
            if (_meleeWeapon != null)
            {
                _meleeWeapon.MinDamageCaused = _originalMinDamage * multiplier;
                _meleeWeapon.MaxDamageCaused = _originalMaxDamage * multiplier;
                
                // IMPORTANT: Also update the child DamageArea that MeleeWeapon creates
                if (_childDamageArea == null)
                {
                    _childDamageArea = GetComponentInChildren<DamageOnTouch>();
                }
                
                if (_childDamageArea != null)
                {
                    _childDamageArea.MinDamageCaused = _originalMinDamage * multiplier;
                    _childDamageArea.MaxDamageCaused = _originalMaxDamage * multiplier;
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerDamageMultiplier: Zone {zone} → Multiplier {multiplier}x. New damage: {_meleeWeapon.MinDamageCaused:F1}-{_meleeWeapon.MaxDamageCaused:F1}");
                }
            }
            else if (_damageOnTouch != null)
            {
                _damageOnTouch.MinDamageCaused = _originalMinDamage * multiplier;
                _damageOnTouch.MaxDamageCaused = _originalMaxDamage * multiplier;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerDamageMultiplier: Zone {zone} → Multiplier {multiplier}x. New damage: {_damageOnTouch.MinDamageCaused:F1}-{_damageOnTouch.MaxDamageCaused:F1}");
                }
            }
        }

        /// <summary>
        /// Gets multiplier for a specific zone
        /// </summary>
        private float GetMultiplierForZone(int zone)
        {
            // Zone mapping: Higher beer = More drunk = More damage
            switch (zone)
            {
                case 1: // 0-33% beer = Sober (Stage 1) = Low damage
                    return stage1Multiplier;
                case 2: // 34-66% beer = Tipsy (Stage 2) = Medium damage
                    return stage2Multiplier;
                case 3: // 67-100% beer = Very Drunk (Stage 3) = High damage
                    return stage3Multiplier;
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Reset damage to original values
        /// </summary>
        [ContextMenu("Reset Damage")]
        public void ResetDamage()
        {
            if (_meleeWeapon != null)
            {
                _meleeWeapon.MinDamageCaused = _originalMinDamage;
                _meleeWeapon.MaxDamageCaused = _originalMaxDamage;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerDamageMultiplier: Damage reset to original: {_originalMinDamage}-{_originalMaxDamage}");
                }
            }
            else if (_damageOnTouch != null)
            {
                _damageOnTouch.MinDamageCaused = _originalMinDamage;
                _damageOnTouch.MaxDamageCaused = _originalMaxDamage;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerDamageMultiplier: Damage reset to original: {_originalMinDamage}-{_originalMaxDamage}");
                }
            }
        }

        /// <summary>
        /// Show current status
        /// </summary>
        [ContextMenu("Show Status")]
        public void ShowStatus()
        {
            if (BeerManager.HasInstance)
            {
                float currentMultiplier = GetMultiplierForZone(BeerManager.Instance.CurrentZone);
                Debug.Log($"=== {gameObject.name} Status ===");
                Debug.Log($"Original Damage: {_originalMinDamage}-{_originalMaxDamage}");
                Debug.Log($"Current Zone: {BeerManager.Instance.CurrentZone}");
                Debug.Log($"Current Multiplier: {currentMultiplier}x");
                
                if (_meleeWeapon != null)
                {
                    Debug.Log($"Current Damage: {_meleeWeapon.MinDamageCaused:F1}-{_meleeWeapon.MaxDamageCaused:F1}");
                }
                else if (_damageOnTouch != null)
                {
                    Debug.Log($"Current Damage: {_damageOnTouch.MinDamageCaused:F1}-{_damageOnTouch.MaxDamageCaused:F1}");
                }
            }
        }
    }
}

