using UnityEngine;

namespace Game.Hazard
{
    public abstract class Hazard : MonoBehaviour
    {
        [SerializeField] 
        protected float damage = 1.0f;
    }
}