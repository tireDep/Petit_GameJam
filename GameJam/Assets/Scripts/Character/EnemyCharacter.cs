using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Characters
{
    // EnemyCharacter: Rigidbody 기반 플레이어 직접 추적 및 바라보기
    // - 한 파일에 한 클래스
    // - 이동은 XZ 평면, Y축 이동 없음
    // - FixedUpdate에서 Rigidbody.MovePosition 사용
    // - 플레이어를 바라봄 (Y축 회전만)
    public class EnemyCharacter : Character
    {
        // todo : 추후 삭제할수도
        [SerializeField]
        private float stopDistance = 1f;

        [SerializeField]
        private Transform target;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            render = GetComponentInChildren<Renderer>();
        }

        /// 추적할 대상(플레이어 Transform)을 설정합니다.
        public void SetTarget(Transform t)
        {
            target = t;
            if (target == null)
            {
                Debug.LogWarning($"{nameof(EnemyCharacter)}: Target set to null.");
            }
        }

        private void Update()
        {
            if (isKnockbacked)
                return;
            
            if (!isAlive)
            {
                moveInput = Vector3.zero;
                return;
            }

            // Target 확인
            if (target == null)
            {
                moveInput = Vector3.zero;
                return;
            }

            // 플레이어 방향 계산
            Vector3 directionToTarget = target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            // stopDistance 내에 있으면 이동 중단
            if (distanceToTarget <= stopDistance)
            {
                moveInput = Vector3.zero;
            }
            else
            {
                // 정규화된 방향 벡터 (XZ 평면)
                directionToTarget.y = 0f;
                moveInput = directionToTarget.normalized;
            }

            // 바라보기: Y축 회전만 적용
            // Update에서 회전을 처리합니다.
            // 이유: 플레이어를 항상 바라봐야 하는 시각적 요구사항이므로,
            // 입력 기반이 아닌 명확한 대상 추적이 목표입니다.
            // Update에서 처리해야 시각적 부드러움이 있고, 물리와 충돌하지 않습니다.
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
            {
                moveInput = Vector3.zero;
            }

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
