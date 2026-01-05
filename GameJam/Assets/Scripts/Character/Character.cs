using System;
using System.Collections;
using UnityEngine;

namespace Game.Characters
{
    // 플레이어/적 공통의 생존 상태와 피해 진입점을 제공하는 추상 베이스 클래스
    // SharedHealth와 DamageType은 이후에 정의될 타입이므로 현재는 참조만 합니다.
    public abstract class Character : MonoBehaviour
    {
        // 살아있는지 여부
        // 외부에서 접근 가능하도록 protected로 둠 — 파생 클래스가 상태를 변경/확인할 수 있어야 함
        protected bool isAlive = true;
        
        protected Rigidbody rb;
        protected Renderer render;
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

        // Inspector에서 할당하거나 생성자/바인딩으로 주입할 수 있게 protected로 노출
        // SharedHealth는 여러 캐릭터가 공유하는 체력 시스템을 나타냅니다.
        // [SerializeField]
        // protected SharedHealth sharedHealth;

        // // 외부에서 SharedHealth를 바인딩할 때 사용
        // public virtual void BindSharedHealth(SharedHealth hp)
        // {
        //     if (hp == null)
        //     {
        //         Debug.LogWarning("BindSharedHealth received null SharedHealth reference.");
        //         return;
        //     }
        // 
        //     sharedHealth = hp;
        // }

        // // 피해 진입점
        // // DamageType에 따라 공유데미지/즉사/환경데미지를 분리 처리함
        // public virtual void ReceiveDamage(DamageType type, int amount, string reason)
        // {
        //     if (!isAlive)
        //     {
        //         Debug.LogWarning($"ReceiveDamage called but character already dead. Reason: {reason}");
        //         return;
        //     }
        // 
        //     switch (type)
        //     {
        //         case DamageType.Shared:
        //             // 공유 데미지는 SharedHealth에 위임 — 여러 캐릭터가 같은 풀의 영향을 받음
        //             if (sharedHealth == null)
        //             {
        //                 Debug.LogWarning("Shared damage requested but SharedHealth is not assigned.");
        //                 return;
        //             }
        // 
        //             sharedHealth.ApplySharedDamage(amount, reason);
        //             break;
        // 
        //         case DamageType.InstantKill:
        //             // 즉사는 공유 체력에 영향을 주지 않음 — 공유와 비공유 데미지를 분리하기 위함
        //             Kill(reason);
        //             break;
        // 
        //         case DamageType.Environmental:
        //             // 이번 범위에서는 환경 데미지를 즉사로 처리함. 확장 시 안전한 감소 로직으로 변경 가능.
        //             Kill(reason);
        //             break;
        // 
        //         default:
        //             Debug.LogWarning($"Unhandled DamageType: {type}. Treating as no-op.");
        //             break;
        //     }
        // 
        // }

        // 기본 Kill 동작: 비활성화
        // 단순/안정 우선으로 게임잼 규칙에 따라 즉시 오브젝트를 숨깁니다.
        public virtual void Kill(string reason)
        {
            if (!isAlive)
            {
                Debug.LogWarning($"Kill called but character already dead. Reason: {reason}");
                return;
            }

            isAlive = false;
            gameObject.SetActive(false);
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
                if (render.sharedMaterial == basicMaterial)
                {
                    render.material = knockbackMaterial;
                }
                else if (render.sharedMaterial == knockbackMaterial)
                {
                    render.material = basicMaterial;   
                }
            
                yield return new WaitForSeconds(blinkDuration);
            }
            
            render.material = basicMaterial;
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
