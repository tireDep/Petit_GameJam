using System.ComponentModel;
using UnityEngine;

namespace Game.Combat
{
    public class SharedHealth : MonoBehaviour
    {
        [SerializeField] 
        private float maxHealth = 10.0f;
        
        [ReadOnly(false)]
        private float currentHealth = 0.0f;

        // 데미지 계산 함수
        public void CalculateDamage(DamageType damageType, float damage)
        {
            switch (damageType)
            {
                case DamageType.DT_ENEMY:
                {
                    
                }
                    break;
                case DamageType.DT_ENVIRONMENTAL:
                {
                    
                }
                    break;
                case DamageType.DT_INSTANTKILL:
                {
                    
                }
                    break;
                case DamageType.DT_SHARED:
                {
                    
                }
                    break;
                default:
                {
                    Debug.LogWarning("CalculateDamage DamageType not implemented");
                }
                    break;
            }
        }
        
    }
}