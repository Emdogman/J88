using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Melee weapon specifically configured for enemies
    /// Uses enemy_Melee_Radius and default_Attack_Damage from ChaserEnemy
    /// </summary>
    [AddComponentMenu("TopDown Engine/Weapons/Enemy Melee Weapon")]
    public class EnemyMeleeWeapon : MeleeWeapon
    {
        [Header("Enemy Configuration")]
        [Tooltip("Reference to the ChaserEnemy for dynamic configuration")]
        [SerializeField] private ChaserEnemy chaserEnemy;
        
        [Tooltip("Override damage with enemy's default attack damage")]
        [SerializeField] private bool useEnemyDamage = true;
        
        [Tooltip("Override area size with enemy's melee radius")]
        [SerializeField] private bool useEnemyRadius = true;

        public override void Initialization()
        {
            // Configure weapon based on enemy settings
            if (chaserEnemy != null)
            {
                if (useEnemyDamage)
                {
                    // Use reflection to get the private field
                    var damageField = typeof(ChaserEnemy).GetField("default_Attack_Damage", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (damageField != null)
                    {
                        float enemyDamage = (float)damageField.GetValue(chaserEnemy);
                        MinDamageCaused = enemyDamage;
                        MaxDamageCaused = enemyDamage;
                    }
                }
                
                if (useEnemyRadius)
                {
                    // Use reflection to get the private field
                    var radiusField = typeof(ChaserEnemy).GetField("enemy_Melee_Radius", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (radiusField != null)
                    {
                        float enemyRadius = (float)radiusField.GetValue(chaserEnemy);
                        AreaSize = new Vector3(enemyRadius * 2f, enemyRadius * 2f, 1f);
                    }
                }
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
    }
}
