using System;
using Game.Combat;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public class ConnectLine : MonoBehaviour
    {
        // 타겟 정보
        [SerializeField]
        private Transform startTransform;
        
        [SerializeField]
        private Transform endTransform;
        
        [SerializeField]
        private Color startColor = Color.white;
        
        [SerializeField]
        private Color endColor = Color.white;

        [SerializeField]
        private float lineWidth = 0.1f;
        
        [SerializeField]
        private Material lineMaterial;
        
        [SerializeField]
        private int sortLayer;
        
        private LineRenderer lineRenderer;
        private const int POSITION_COUNT = 2;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if(lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            
            ResetLineRenderer();
            
            SharedHealth.OnDeath += OnDeathEvent;
        }

        private void Update()
        {
            UpdateLineRenderer();
        }

        public void ResetLineRenderer()
        {
            if (lineRenderer == null)
                return;

            if (lineMaterial == null)
                Debug.LogWarning("ResetLineRenderer lineMaterial is null");

            lineRenderer.material = lineMaterial;
            lineRenderer.material.color = Color.white;
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = POSITION_COUNT;
            lineRenderer.sortingOrder = sortLayer;

            SetEnabled(true);
        }

        public void SetTarget(Transform setStartTransform, Transform setEndTransform)
        {
            startTransform = setStartTransform;
            endTransform = setEndTransform;
        }

        public void SetEnabled(bool bSet)
        {
            if (lineRenderer == null)
                return;
            
            lineRenderer.enabled = bSet;
        }

        private void OnDeathEvent()
        {
            SetEnabled(false);
        }

        private void UpdateLineRenderer()
        {
            if (lineRenderer == null)
                return;

            if (startTransform == null || endTransform == null)
                return;

            lineRenderer.SetPosition(0, startTransform.position);
            lineRenderer.SetPosition(1, endTransform.position);
        }
    }
}