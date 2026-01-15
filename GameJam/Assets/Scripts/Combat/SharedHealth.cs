using System.ComponentModel;
using UnityEngine;

namespace Game.Combat
{
    public class SharedHealth : MonoBehaviour
    {
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
                Debug.Log("SET DEAD!");
            }
        }
        
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDead = false;
        }
        
    }
}