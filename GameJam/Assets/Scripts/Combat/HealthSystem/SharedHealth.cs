using System.ComponentModel;
using UnityEngine;

namespace Game.Combat
{
    public class SharedHealth : MonoBehaviour
    {
        // Delegate 타입선언
        public delegate void OnDeathDelegate();

        // 이벤트 선언
        public static event OnDeathDelegate OnDeath;
        
        [SerializeField] 
        private float maxHealth = 10.0f;
        
        [SerializeField]
        private float currentHealth = 0.0f;

        [SerializeField]
        private bool isDead = false;
        
        public bool CheckDead { get { return isDead; } }

        private void Start()
        {
            ResetHealth();
        }

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
            }
        }
        
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDead = false;
        }
        
    }
}