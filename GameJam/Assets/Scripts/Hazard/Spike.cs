using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Hazard
{
    public class Spike : Hazard
    {
        private void OnCollisionEnter(Collision other)
        {
            Debug.LogWarning("Hazard OnCollisionEnter!");
        }
    }
}