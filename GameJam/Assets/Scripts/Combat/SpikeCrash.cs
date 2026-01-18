using UnityEngine;

namespace Game.Combat
{
    public class SpikeCrash : Attack
    {
        public void OnCollisionEnter(Collision other)
        {
            base.ProcessAttackDamage(other);
        }
    }
}