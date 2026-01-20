using UnityEngine;
using System;

namespace Game.Combat
{
    public enum DamageType
    {
        DT_INVALID = 0,
        
        DT_PLAYER,
        DT_ENEMY,
        DT_SHARED,
        DT_ENVIRONMENTAL,
        DT_INSTANTKILL,
        
        DT_MAX,
    }
    
    public struct AttackInfo
    {
        public DamageType damageType;  // 데미지 타입
        public float damage; // 데미지
        
        public AttackInfo(DamageType damageType, float damage)
        {
            this.damageType = damageType;
            this.damage = damage;
        }
    }
    
    public abstract class Attack : MonoBehaviour
    {
        protected AttackInfo attackInfo;    // 공격 정보

        protected GameObject attacker { get; set; } // 런타임에 설정되는 공격자(주체)
        
        // 세팅용 데미지 타입
        [SerializeField]
        private DamageType settingDamageType;
        
        // 세팅용 데미지
        [SerializeField]
        private float settingDamage;

        protected virtual void Start()
        {
            attackInfo.damageType = settingDamageType;
            attackInfo.damage = settingDamage;

            if (attacker == null)
                attacker = gameObject;
        }

        // 재사용 시 초기화
        public virtual void ResetAttack()
        {
            attacker = null;
        }

        // 데미지 계산 함수(입힌 데미지, 받은 데미지)
        public void CalculateDamage(out float outDamageDealt, out float outDamageTaken)
        {
            // 데미지 초기화
            outDamageDealt = 0.0f;
            outDamageTaken = 0.0f;
            
            switch (attackInfo.damageType)
            {
                case DamageType.DT_ENVIRONMENTAL:
                {
                    outDamageDealt = attackInfo.damage;
                }
                    break;
                case DamageType.DT_INSTANTKILL:
                {
                    outDamageDealt = attackInfo.damage;
                }
                    break;
                case DamageType.DT_PLAYER:
                case DamageType.DT_ENEMY:
                case DamageType.DT_SHARED:
                {
                    outDamageDealt = attackInfo.damage;
                    outDamageTaken = attackInfo.damage;
                }
                    break;
                default:
                {
                    Debug.LogWarning("CalculateDamage DamageType not implemented");
                }
                    break;
            }
        }

        public virtual bool ProcessAttackDamage(Collision other)
        {
            if (other.gameObject == attacker)
                return false;
            
            CalculateDamage(out var damageDealt, out var damageTaken);
            
            // 입힌 데미지 처리
            if (damageDealt > 0.0f)
            {
                SharedHealth sharedHealth = other.gameObject.GetComponent<SharedHealth>();
                if(sharedHealth != null)
                    sharedHealth.TakeDamage(damageDealt);
            }

            // 받은 데미지 처리
            if (damageTaken > 0.0f)
            {
                SharedHealth mySharedHealth = attacker.GetComponent<SharedHealth>();
                if(mySharedHealth != null)
                    mySharedHealth.TakeDamage(damageTaken);
            }
            
            return true;
        }
    }
}