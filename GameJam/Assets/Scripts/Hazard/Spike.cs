using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Hazard
{
    // 가시 장애물 클래스
    public class Spike : Hazard
    {
        private void OnCollisionEnter(Collision other)
        {
            Debug.LogWarning("Hazard OnCollisionEnter!");
        }
    }
}