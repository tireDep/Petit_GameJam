using UnityEngine;
using System;

namespace Game.Combat
{
    [Serializable]
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
        [SerializeField]
        protected AttackInfo attackInfo;    // 공격 정보

        protected GameObject attacker; // 런타임에 설정되는 공격자(주체)

        // 공격자 설정
        public virtual void SetAttacker(GameObject attacker)
        {
            this.attacker = attacker;
        }
         
        // 재사용 시 초기화
        public virtual void ResetAttack()
        {
            attacker = null;
        }
    }
}