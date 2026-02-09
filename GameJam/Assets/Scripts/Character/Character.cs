using System;
using System.Collections;
using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Characters
{
    // 캐릭터 추상 베이스 클래스
    public abstract class Character : MonoBehaviour
    {
        protected SharedHealth sharedHealth;	// 체력
        protected bool isAlive => sharedHealth != null && !sharedHealth.CheckDead;
        
        protected Rigidbody rb;
        protected List<Renderer> renders = new List<Renderer>();
        protected Vector3 moveInput = Vector3.zero;	// 입력 방향
        protected bool isKnockbacked = false;   	// 넉백 여부
        protected Coroutine blinkCoroutine;			// 깜빡임 코루틴

        [SerializeField] 
        protected float moveSpeed = 5.0f;			// 이동 속도
        
        [SerializeField]
        protected float knockbackForce = 5.0f;		// 넉백 힘
        
        [SerializeField]
        protected float knockbackDuration = 0.2f;	// 넉백 시간
        
        [SerializeField]
        protected float blinkDuration = 0.1f;		// 깜빡임 시간
        
        [SerializeField]
        protected Material basicMaterial;			// 기본 재질
        
        [SerializeField]
        protected Material knockbackMaterial;		// 넉백 재질

        protected virtual void Start()
        {
            sharedHealth = GetComponent<SharedHealth>();
        }

        // 충돌시 넉백처리
        protected virtual void SetKnockback( Vector3 knockbackDirection)
        {
            if (isActiveAndEnabled == false)
                return;
            
            if (rb == null)
                return;

            if (isKnockbacked || isAlive == false)
                return;
            
            // 기존 속도 zero 처리
            SetAllVelocityZero();
            
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

        // 넉백 처리 코루틴
        IEnumerator StopKnockback(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            SetAllVelocityZero();
            isKnockbacked = false;
        }

        // 깜박임 처리 코루틴
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

        // 이동 멈춤
        void SetAllVelocityZero()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            moveInput = Vector3.zero;
        }
    }
}
