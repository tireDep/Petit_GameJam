using System;
using System.Buffers.Text;
using UnityEngine;

namespace Game.Combat
{
    public class CrashAttack : Attack
    {
        private void Awake()
        {
            base.attacker = base.gameObject;
        }

        public void OnCollisionEnter(Collision other)
        {
            // 플레이어인 경우에만 계산
            if (other.gameObject.CompareTag("Player") == false)
                return;

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
                SharedHealth mySharedHealth = gameObject.GetComponent<SharedHealth>();
                if(mySharedHealth != null)
                    mySharedHealth.TakeDamage(damageTaken);
            }
        }
    }
}