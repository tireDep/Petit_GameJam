using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// 발사체: 활성화 시 설정된 방향(direction)으로 이동
    /// - Rigidbody 기반, MovePosition으로 직접 이동
    /// - 충돌 시 자동으로 비활성화 및 초기화
    /// </summary>
    public class Projectile : Attack
    {
        [SerializeField]
        private float speed = 10.0f;

        private Rigidbody rb;
        private Vector3 direction = Vector3.zero;
        private bool isActive = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
                Debug.LogWarning($"Projectile {gameObject.name}: Rigidbody component not found", gameObject);
        }

        private void FixedUpdate()
        {
            if (!isActive || rb == null)
                return;

            // XZ 평면에서 설정된 방향으로 이동
            Vector3 displacement = direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + displacement);
        }
        
        /// 발사체 발사: 방향과 공격자 설정
        public void Launch(Vector3 direction, GameObject attacker)
        {
            if (rb == null)
            {
                Debug.LogWarning($"Projectile {gameObject.name}: Cannot launch without Rigidbody", gameObject);
                return;
            }

            SetAttacker(attacker);
            this.direction = direction.normalized;
            isActive = true;
            gameObject.SetActive(true);
        }
        
        /// 발사체 비활성화 및 초기화
        public void Deactivate()
        {
            isActive = false;
            direction = Vector3.zero;
            ResetAttack();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider collision)
        {
            // // 자신의 공격자와의 충돌은 무시
            // if (collision.gameObject == attacker)
            //     return;

            Deactivate();
        }

        public override void ResetAttack()
        {
            isActive = false;
            direction = Vector3.zero;
            base.ResetAttack();
        }
    }
}