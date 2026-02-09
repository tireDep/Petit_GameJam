using System.ComponentModel;
using Manager;
using UnityEngine;

namespace Game.Combat
{
    public class SharedHealth : MonoBehaviour
    {
        public delegate void OnDeathDelegate();			// Delegate 타입선언       
        public static event OnDeathDelegate OnDeath;	// 이벤트 선언
        
        [SerializeField] 
        private float maxHealth = 10.0f;				// 최대 HP
        
        [SerializeField]
        private float currentHealth = 0.0f;				// 현재 HP

        [SerializeField]
        private bool isDead = false;					// 사망 상태 체크
        
		// 사망 상태 반환
        public bool CheckDead { get { return isDead; } }

        private void Start()
        {
            ResetHealth();
        }

		// 데미지 계산 함수
        public void TakeDamage(float damageTaken)
        {
            if (isDead)
                return;
            
            currentHealth -= damageTaken;
            if (currentHealth <= 0.0f)
            {
                isDead = true;
                gameObject.SetActive(false);
                Debug.Log("SET DEAD!");
                
                // ?. : null 체크랑 동일
                OnDeath?.Invoke();
                
                GameManager gameManager = GameManager.Instance;
                if (gameManager != null)
                    gameManager.OnDeathEvent(gameObject.tag);
            }
        }
        
		// HP 초기화
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDead = false;
        }
    }
}