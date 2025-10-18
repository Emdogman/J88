using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Charge weapon specifically configured for enemies
    /// Automatically sets up charge sequence with melee weapon
    /// </summary>
    [AddComponentMenu("TopDown Engine/Weapons/Enemy Charge Weapon")]
    public class EnemyChargeWeapon : ChargeWeapon
    {
        [Header("Enemy Configuration")]
        [Tooltip("Reference to the ChaserEnemy for dynamic configuration")]
        [SerializeField] private ChaserEnemy chaserEnemy;
        
        [Tooltip("Charge duration for enemy attacks")]
        [SerializeField] private float enemyChargeDuration = 1.5f;
        
        [Tooltip("The melee weapon to use as the target weapon")]
        [SerializeField] private MeleeWeapon targetMeleeWeapon;

        public override void Initialization()
        {
            // Configure charge weapon for enemies
            if (chaserEnemy != null)
            {
                // Set up charge sequence with melee weapon
                if (Weapons.Count == 0 && targetMeleeWeapon != null)
                {
                    var chargeStep = new ChargeWeaponStep
                    {
                        TargetWeapon = targetMeleeWeapon,
                        ChargeDuration = enemyChargeDuration,
                        TriggerIfChargeInterrupted = true,
                        FlipWhenChargeWeaponFlips = true
                    };
                    
                    Weapons.Add(chargeStep);
                }
                
                // Configure release mode for automatic release
                ReleaseMode = ReleaseModes.AfterLastChargeDuration;
                AllowInitialShot = false;
            }
            
            base.Initialization();
        }

        /// <summary>
        /// Set the ChaserEnemy reference
        /// </summary>
        public void SetChaserEnemy(ChaserEnemy enemy)
        {
            chaserEnemy = enemy;
        }

        /// <summary>
        /// Set the target melee weapon
        /// </summary>
        public void SetTargetMeleeWeapon(MeleeWeapon meleeWeapon)
        {
            targetMeleeWeapon = meleeWeapon;
        }
    }
}
