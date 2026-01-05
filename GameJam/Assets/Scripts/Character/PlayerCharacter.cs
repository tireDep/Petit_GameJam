using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters
{
    // PlayerCharacter: 간단한 Rigidbody 기반 직접 이동 구현
    // - 한 파일에 한 클래스
    // - 입력: WASD (Horizontal/Vertical)
    // - 이동은 XZ 평면
    // - FixedUpdate에서 Rigidbody.MovePosition 사용
    public class PlayerCharacter : Character
    {
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            render = GetComponentInChildren<Renderer>();
        }

        // Input Actions
        void OnMove(InputValue value)
        {
            if (isKnockbacked)
                return;
            
            if (!isAlive)
            {
                moveInput = Vector2.zero;
                return;
            }
            
            Vector2 input = value.Get<Vector2>();
            if (input == Vector2.zero)
            {
                moveInput = Vector2.zero;
                return;
            }
            
            Vector3 moveValue = new Vector3(input.x, 0f, input.y);
            if (input.sqrMagnitude > 1f) 
                input.Normalize();
            
            moveInput = moveValue;
        }

        private void Update()
        {
            if (!isAlive)
                return;
        }

        private void FixedUpdate()
        {
            if (!isAlive) 
                return;

            if (rb == null)
                return;

            // Rigidbody.MovePosition을 사용하면 물리 시스템과 충돌/interpolation을 지키며 이동합니다.
            Vector3 displacement = moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + displacement);
        }

        private void OnCollisionEnter(Collision other)
        {
            SetKnockback(other.contacts[0].normal);
        }
    }
}
