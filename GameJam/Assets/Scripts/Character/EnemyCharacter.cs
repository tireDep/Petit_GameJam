using System;
using Game.Combat;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Game.Characters
{
    public class EnemyCharacter : Character
    {
        [SerializeField]
        private Transform target;				// 추적할 대상 Transform

        protected new virtual void Start()
        {
            base.Start();
            
            rb = GetComponent<Rigidbody>();
            renders.AddRange(GetComponentsInChildren<Renderer>());
        }

        // 추적할 대상 설정
        public void SetTarget(Transform setTarget)
        {
            target = setTarget;
            if (target == null)
                Debug.LogWarning($"{nameof(EnemyCharacter)}: Target set to null.");
        }

        private void Update()
        {
            if (isKnockbacked)
                return;
            
            // Target 확인
            if (target == null)
            {
                moveInput = Vector3.zero;
                return;
            }
            
            if (!isAlive)
            {
                moveInput = Vector3.zero;
                return;
            }
            
            SharedHealth sharedHealth = target.GetComponent<SharedHealth>();
            if (sharedHealth != null && sharedHealth.CheckDead)
            {
                moveInput = Vector3.zero;
                return;   
            }
            
            // 플레이어 방향 계산
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f;
            
            // 정규화된 방향 벡터 (XZ 평면)
            moveInput = directionToTarget.normalized;
            
            if (moveInput.sqrMagnitude > 0f)
            {
                // moveInput 방향으로 LookRotation 적용 (Y축만)
                Quaternion lookRotation = Quaternion.LookRotation(moveInput);
                transform.rotation = lookRotation;
            }
        }

        private void FixedUpdate()
        {
            if (!isAlive) 
                return;
            
            if (rb == null) 
                return;

            // Target 재확인 (Update와의 동기화)
            if (target == null)
                moveInput = Vector3.zero;

            // MovePosition으로 이동 (XZ 평면, Y축 없음)
            Vector3 displacement = moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + displacement);
        }

        private void OnCollisionEnter(Collision other)
        {
            SetKnockback((transform.position - other.transform.position).normalized);
        }
    }
}
