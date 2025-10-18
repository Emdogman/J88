using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to easily set up the Beer Damage Multiplier system
    /// </summary>
    public class BeerDamageMultiplierSetupHelper : MonoBehaviour
    {
        [Header("Setup Options")]
        [Tooltip("Whether to show debug information during setup")]
        public bool ShowDebugInfo = true;
        
        [Tooltip("Whether to create BeerManager if it doesn't exist")]
        public bool CreateBeerManager = true;
        
        [Tooltip("Whether to destroy this helper after setup")]
        public bool DestroyAfterSetup = true;
        
        [Header("Damage Scaling Settings")]
        [Tooltip("Base damage multiplier when beer level is 0")]
        public float BaseDamageMultiplier = 1.0f;
        
        [Tooltip("Maximum damage multiplier when beer level is 100")]
        public float MaxDamageMultiplier = 2.5f;
        
        [Tooltip("Damage multiplier for Stage 1 (0-33% beer)")]
        public float Stage1Multiplier = 1.0f;
        
        [Tooltip("Damage multiplier for Stage 2 (34-66% beer)")]
        public float Stage2Multiplier = 1.5f;
        
        [Tooltip("Damage multiplier for Stage 3 (67-99% beer)")]
        public float Stage3Multiplier = 2.0f;
        
        [Tooltip("Damage multiplier for Stage 4 (100% beer)")]
        public float Stage4Multiplier = 2.5f;
        
        /// <summary>
        /// Sets up the complete beer damage multiplier system
        /// </summary>
        [ContextMenu("Setup Beer Damage Multiplier System")]
        public virtual void SetupBeerDamageMultiplierSystem()
        {
            if (ShowDebugInfo)
            {
                Debug.Log("=== Setting up Beer Damage Multiplier System ===");
            }
            
            // Step 1: Ensure BeerManager exists
            SetupBeerManager();
            
            // Step 2: Add BeerDamageMultiplier to all weapons
            SetupWeaponDamageMultipliers();
            
            if (ShowDebugInfo)
            {
                Debug.Log("=== Beer Damage Multiplier System Setup Complete! ===");
                Debug.Log("✅ BeerManager: " + (BeerManager.HasInstance ? "Found" : "Missing"));
                Debug.Log("✅ Weapon Damage Multipliers: Added to all weapons");
            }
            
            if (DestroyAfterSetup)
            {
                Destroy(this);
            }
        }
        
        /// <summary>
        /// Sets up BeerManager if it doesn't exist
        /// </summary>
        protected virtual void SetupBeerManager()
        {
            if (!BeerManager.HasInstance)
            {
                if (CreateBeerManager)
                {
                    GameObject beerManagerGO = new GameObject("BeerManager");
                    BeerManager beerManager = beerManagerGO.AddComponent<BeerManager>();
                    beerManager.ShowDebugInfo = ShowDebugInfo;
                    beerManager.BeerSystemActive = true;
                    beerManager.CurrentBeer = 50f; // Start at 50%
                    beerManager.StartingBeer = 50f;
                    beerManager.DepletionRate = 1f; // 1% per second
                    
                    if (ShowDebugInfo)
                    {
                        Debug.Log("✅ Created BeerManager");
                    }
                }
                else
                {
                    Debug.LogWarning("❌ BeerManager not found! Please create one manually or enable 'Create BeerManager'.");
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.Log("✅ BeerManager already exists");
                }
            }
        }
        
        /// <summary>
        /// Adds BeerDamageMultiplier to all weapons in the scene
        /// </summary>
        protected virtual void SetupWeaponDamageMultipliers()
        {
            // Find all weapons in the scene
            Weapon[] weapons = FindObjectsOfType<Weapon>();
            
            int weaponsModified = 0;
            
            foreach (Weapon weapon in weapons)
            {
                // Skip if already has BeerDamageMultiplier
                if (weapon.GetComponent<BeerDamageMultiplier>() != null)
                {
                    continue;
                }
                
                // Add BeerDamageMultiplier component
                BeerDamageMultiplier damageMultiplier = weapon.gameObject.AddComponent<BeerDamageMultiplier>();
                
                // Configure settings
                damageMultiplier.BaseDamageMultiplier = BaseDamageMultiplier;
                damageMultiplier.MaxDamageMultiplier = MaxDamageMultiplier;
                damageMultiplier.Stage1Multiplier = Stage1Multiplier;
                damageMultiplier.Stage2Multiplier = Stage2Multiplier;
                damageMultiplier.Stage3Multiplier = Stage3Multiplier;
                damageMultiplier.Stage4Multiplier = Stage4Multiplier;
                damageMultiplier.ShowDebugInfo = ShowDebugInfo;
                damageMultiplier.IsActive = true;
                
                weaponsModified++;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"✅ Added BeerDamageMultiplier to {weapon.name}");
                }
            }
            
            if (ShowDebugInfo)
            {
                Debug.Log($"✅ Modified {weaponsModified} weapons with damage multipliers");
            }
        }
        
        /// <summary>
        /// Test damage scaling with different beer levels
        /// </summary>
        [ContextMenu("Test Damage Scaling")]
        public virtual void TestDamageScaling()
        {
            if (!BeerManager.HasInstance)
            {
                Debug.LogError("❌ BeerManager not found! Run setup first.");
                return;
            }
            
            StartCoroutine(TestDamageScalingCoroutine());
        }
        
        /// <summary>
        /// Test damage scaling coroutine
        /// </summary>
        private System.Collections.IEnumerator TestDamageScalingCoroutine()
        {
            if (ShowDebugInfo)
            {
                Debug.Log("=== Testing Damage Scaling ===");
            }
            
            // Test different beer levels
            float[] testLevels = { 0f, 25f, 50f, 75f, 90f, 100f };
            
            foreach (float beerLevel in testLevels)
            {
                BeerManager.Instance.CurrentBeer = beerLevel;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Testing beer level: {beerLevel}%");
                }
                
                yield return new WaitForSeconds(1f);
            }
            
            if (ShowDebugInfo)
            {
                Debug.Log("=== Damage Scaling Test Complete ===");
            }
        }
        
        /// <summary>
        /// Reset all weapons to original damage
        /// </summary>
        [ContextMenu("Reset All Weapon Damage")]
        public virtual void ResetAllWeaponDamage()
        {
            BeerDamageMultiplier[] damageMultipliers = FindObjectsOfType<BeerDamageMultiplier>();
            
            foreach (BeerDamageMultiplier multiplier in damageMultipliers)
            {
                multiplier.ResetDamage();
            }
            
            if (ShowDebugInfo)
            {
                Debug.Log($"✅ Reset damage for {damageMultipliers.Length} weapons");
            }
        }
        
        /// <summary>
        /// Show status of all weapons
        /// </summary>
        [ContextMenu("Show All Weapon Status")]
        public virtual void ShowAllWeaponStatus()
        {
            BeerDamageMultiplier[] damageMultipliers = FindObjectsOfType<BeerDamageMultiplier>();
            
            Debug.Log($"=== Weapon Damage Status ({damageMultipliers.Length} weapons) ===");
            
            foreach (BeerDamageMultiplier multiplier in damageMultipliers)
            {
                multiplier.ShowStatus();
            }
        }
    }
}
