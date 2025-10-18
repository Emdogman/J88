using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to automatically set up weapons for ChaserEnemy
    /// Creates and configures both charge and melee weapons
    /// </summary>
    [AddComponentMenu("TopDown Engine/Enemies/Enemy Weapon Setup Helper")]
    public class EnemyWeaponSetupHelper : MonoBehaviour
    {
        [Header("Weapon Configuration")]
        [Tooltip("Charge duration for enemy attacks")]
        [SerializeField] private float chargeDuration = 1.5f;
        
        [Tooltip("Melee weapon damage")]
        [SerializeField] private float meleeDamage = 10f;
        
        [Tooltip("Melee weapon radius")]
        [SerializeField] private float meleeRadius = 1.5f;
        
        [Tooltip("Layer mask for what the enemy can attack")]
        [SerializeField] private LayerMask attackLayerMask = -1;

        [Header("References")]
        [Tooltip("The ChaserEnemy to configure")]
        [SerializeField] private ChaserEnemy chaserEnemy;
        
        [Tooltip("Weapon attachment point (child GameObject)")]
        [SerializeField] private Transform weaponAttachment;

        /// <summary>
        /// Creates and configures weapons for the ChaserEnemy
        /// </summary>
        [ContextMenu("Setup Enemy Weapons")]
        public void SetupEnemyWeapons()
        {
            if (chaserEnemy == null)
            {
                chaserEnemy = GetComponent<ChaserEnemy>();
                if (chaserEnemy == null)
                {
                    Debug.LogError("EnemyWeaponSetupHelper: No ChaserEnemy found!");
                    return;
                }
            }

            // Create weapon attachment if not exists
            if (weaponAttachment == null)
            {
                GameObject attachment = new GameObject("WeaponAttachment");
                attachment.transform.SetParent(chaserEnemy.transform);
                attachment.transform.localPosition = Vector3.zero;
                weaponAttachment = attachment.transform;
            }

            // Create melee weapon
            GameObject meleeWeaponObj = CreateMeleeWeapon();
            if (meleeWeaponObj != null)
            {
                meleeWeaponObj.transform.SetParent(weaponAttachment);
                meleeWeaponObj.transform.localPosition = Vector3.zero;
            }

            // Create charge weapon
            GameObject chargeWeaponObj = CreateChargeWeapon();
            if (chargeWeaponObj != null)
            {
                chargeWeaponObj.transform.SetParent(weaponAttachment);
                chargeWeaponObj.transform.localPosition = Vector3.zero;
            }

            Debug.Log("EnemyWeaponSetupHelper: Weapons configured successfully!");
        }

        /// <summary>
        /// Creates a melee weapon for the enemy
        /// </summary>
        private GameObject CreateMeleeWeapon()
        {
            GameObject meleeWeaponObj = new GameObject("EnemyMeleeWeapon");
            
            // Add EnemyMeleeWeapon component
            var enemyMeleeWeapon = meleeWeaponObj.AddComponent<EnemyMeleeWeapon>();
            enemyMeleeWeapon.SetChaserEnemy(chaserEnemy);
            
            // Configure damage area
            enemyMeleeWeapon.MeleeDamageAreaMode = MeleeWeapon.MeleeDamageAreaModes.Generated;
            enemyMeleeWeapon.DamageAreaShape = MeleeWeapon.MeleeDamageAreaShapes.Circle;
            enemyMeleeWeapon.AreaSize = new Vector3(meleeRadius * 2f, meleeRadius * 2f, 1f);
            enemyMeleeWeapon.AreaOffset = Vector3.zero;
            
            // Configure damage
            enemyMeleeWeapon.MinDamageCaused = meleeDamage;
            enemyMeleeWeapon.MaxDamageCaused = meleeDamage;
            enemyMeleeWeapon.TargetLayerMask = attackLayerMask;
            
            // Configure timing
            enemyMeleeWeapon.InitialDelay = 0f;
            enemyMeleeWeapon.ActiveDuration = 0.5f;
            
            return meleeWeaponObj;
        }

        /// <summary>
        /// Creates a charge weapon for the enemy
        /// </summary>
        private GameObject CreateChargeWeapon()
        {
            GameObject chargeWeaponObj = new GameObject("EnemyChargeWeapon");
            
            // Add EnemyChargeWeapon component
            var enemyChargeWeapon = chargeWeaponObj.AddComponent<EnemyChargeWeapon>();
            enemyChargeWeapon.SetChaserEnemy(chaserEnemy);
            
            // Configure charge settings
            enemyChargeWeapon.ReleaseMode = ChargeWeapon.ReleaseModes.AfterLastChargeDuration;
            enemyChargeWeapon.AllowInitialShot = false;
            
            return chargeWeaponObj;
        }

        /// <summary>
        /// Auto-assigns the ChaserEnemy reference
        /// </summary>
        private void Awake()
        {
            if (chaserEnemy == null)
            {
                chaserEnemy = GetComponent<ChaserEnemy>();
            }
        }
    }
}
