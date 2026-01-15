using System;
using System.Collections;
using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Characters
{
    // 플레이어/적 공통의 생존 상태와 피해 진입점을 제공하는 추상 베이스 클래스
    // SharedHealth와 DamageType은 이후에 정의될 타입이므로 현재는 참조만 합니다.
    public abstract class Character : MonoBehaviour
    {
        protected SharedHealth sharedHealth;
        protected bool IsAlive => sharedHealth != null && !sharedHealth.CheckDead;
        
        protected Rigidbody rb;
        protected List<Renderer> renders = new List<Renderer>();
        protected Vector3 moveInput = Vector3.zero;
        protected bool isKnockbacked = false;   // 넉백 여부
        protected Coroutine blinkCoroutine;

        [SerializeField] 
        protected float moveSpeed = 5.0f;
        
        [SerializeField]
        protected float knockbackForce = 5.0f;
        
        [SerializeField]
        protected float knockbackDuration = 0.2f;
        
        [SerializeField]
        protected float blinkDuration = 0.1f;
        
        [SerializeField]
        protected int blinkCount = 3;
        
        [SerializeField]
        protected Material basicMaterial;
        
        [SerializeField]
        protected Material knockbackMaterial;

        virtual protected void Start()
        {
            sharedHealth = GetComponent<SharedHealth>();
        }

        // 충돌시 넉백처리
        protected virtual void SetKnockback(Vector3 knockbackDirection)
        {
            if (rb == null)
                return;

            if (isKnockbacked)
                return;
            
            // 기존 속도 zero 처리
            SetAllVelocityZero();
            
            // todo : 랜덤 넉백값 추가?
            // 넉백 처리
            isKnockbacked = true;
            knockbackDirection.y = 0.0f;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

            // 메터리얼 변경
            if (knockbackMaterial == null)
                Debug.LogWarning("knockbackMaterial is null");
            
            if (basicMaterial == null)
                Debug.LogWarning("basicMaterial is null");
            
            if(blinkCoroutine != null)
                 StopCoroutine(blinkCoroutine);
            
            blinkCoroutine = StartCoroutine(PlayBlink());
            StartCoroutine(StopKnockback(knockbackDuration));
        }

        IEnumerator StopKnockback(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            SetAllVelocityZero();
            isKnockbacked = false;
        }

        IEnumerator PlayBlink()
        {
            while (isKnockbacked)
            {
                for (int index = 0; index < renders.Count; index++)
                {
                    if (renders[index].sharedMaterial == basicMaterial)
                    {
                        renders[index].material = knockbackMaterial;
                    }
                    else if (renders[index].sharedMaterial == knockbackMaterial)
                    {
                        renders[index].material = basicMaterial;   
                    }
                }
            
                yield return new WaitForSeconds(blinkDuration);
            }

            for (int index = 0; index < renders.Count; index++)
            {
                renders[index].material = basicMaterial;   
            }
            
            blinkCoroutine = null;
        }

        void SetAllVelocityZero()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            moveInput = Vector3.zero;
        }
    }
}
