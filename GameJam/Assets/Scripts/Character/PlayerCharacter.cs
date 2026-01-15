using System;
using System.Collections;
using System.Collections.Generic;
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
    // - 발사체: 좌클릭으로 마우스 방향 발사, 오브젝트 풀 방식
    public class PlayerCharacter : Character
    {
        [SerializeField]
        private Game.Combat.Projectile projectilePrefab;

        [SerializeField]
        private int maxProjectiles = 5;

        [SerializeField]
        private float shootCooldown = 0.3f;

        private List<Game.Combat.Projectile> projectilePool = new List<Game.Combat.Projectile>();
        private int currentProjectileCount = 0;
        private float lastShootTime = -1f;

        private BoxCollider triggerPosition;    // 발사체 발사 위치

        [SerializeField]
        private float reloadTime = 1;   // 리로드 시간

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            renders.AddRange(GetComponentsInChildren<Renderer>());
            triggerPosition = GetComponentInChildren<BoxCollider>();

            InitializeProjectilePool();
        }
        
        /// 발사체 풀 초기화: 최대 개수만큼 미리 생성
        private void InitializeProjectilePool()
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning("Projectile prefab is not assigned", gameObject);
                return;
            }

            for (int i = 0; i < maxProjectiles; i++)
            {
                Game.Combat.Projectile projectile = Instantiate(projectilePrefab);
                projectile.gameObject.SetActive(false);
                projectilePool.Add(projectile);
            }

            currentProjectileCount = 0;
        }
        
        /// 사용 가능한 발사체 풀에서 하나 가져오기
        private Game.Combat.Projectile GetProjectileFromPool()
        {
            // 이미 활성화된 발사체가 있으면 재사용하지 않음
            if (currentProjectileCount >= maxProjectiles)
                return null;

            Game.Combat.Projectile projectile = projectilePool[currentProjectileCount];
            currentProjectileCount++;
            return projectile;
        }
        
        /// 발사체 풀에 반환
        public void ReturnProjectileToPool()
        {
            for (int index = 0; index < maxProjectiles; index++)
            {
                Game.Combat.Projectile projectile = projectilePool[index];
                
                if (projectile == null || projectile.gameObject.activeSelf == true)
                    continue;
                
                projectile.Deactivate();
                
            }
            
            currentProjectileCount = 0;
            Debug.Log("Projectile Returned to Pool");
        }

        // Input Actions
        void OnMove(InputValue value)
        {
            if (isKnockbacked)
            {
                Debug.Log("Knockbacked");
                return;
            }
            
            if (!IsAlive)
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
            
            // 이동 방향으로 캐릭터 90도씩 정확히 회전 (W=90도, D=180도, S=270도, A=0도)
            if (moveInput != Vector3.zero)
            {
                // input.x: -1(A) ~ +1(D), input.y: -1(S) ~ +1(W)
                // 대각선 입력 시 45도씩 추가 (예: W+D = 135도)
                float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + 90f;
                if (angle < 0) 
                    angle += 360f;
                
                Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
                transform.rotation = targetRotation;
            }
        }

        void OnAttack(InputValue value)
        {
            if (!IsAlive)
                return;

            // 쿨타임 확인
            if (Time.time - lastShootTime < shootCooldown)
                return;

            ShootProjectile();
            lastShootTime = Time.time;
        }
        
        /// 발사체 발사: triggerPosition 위치에서 캐릭터 전방 방향으로
        private void ShootProjectile()
        {
            Game.Combat.Projectile projectile = GetProjectileFromPool();
            if (projectile == null)
            {
                Debug.LogWarning("No available projectiles in pool", gameObject);
                return;
            }

            // 발사 위치: triggerPosition의 위치
            Vector3 shootPosition = triggerPosition != null 
                ? triggerPosition.bounds.center 
                : transform.position;

            // 발사 방향 : 플레이어가 바라보는 방향
            Vector3 shootDirection = -transform.right; // left
            shootDirection.y = 0f; // XZ 평면으로 제한

            // 발사체 위치 설정 후 발사
            projectile.transform.position = shootPosition;
            projectile.Launch(shootDirection, gameObject);

            // 모든 발사체 발사 후, 리로드
            if(currentProjectileCount >= maxProjectiles)
                StartCoroutine(ReloadProjectiles());
        }
        
        IEnumerator ReloadProjectiles()
        {
            yield return new WaitForSeconds(reloadTime);
            
            ReturnProjectileToPool();
        }

        private void Update()
        {
            if (!IsAlive)
                return;
        }

        private void FixedUpdate()
        {
            if (!IsAlive) 
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
