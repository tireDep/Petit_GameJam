using System;
using Game.Combat;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    // 연결선 클래스
    public class ConnectLine : MonoBehaviour
    {
        [SerializeField]
        private Transform startTransform;			    // 시작 지점
        
        [SerializeField]
        private Transform endTransform;				    // 종료 지점
        
        [SerializeField]
        private Color startColor = Color.white;		// 시작 색상
        
        [SerializeField]
        private Color endColor = Color.white;		// 종료 색상

        [SerializeField]
        private float lineWidth = 0.1f;				    // 라인 굵기
        
        [SerializeField]
        private Material lineMaterial;				    // 라인 재질
        
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

		// 연결선 초기화
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

            SetEnabled(true);
        }

		// 타겟 설정
        public void SetTarget(Transform setStartTransform, Transform setEndTransform)
        {
            startTransform = setStartTransform;
            endTransform = setEndTransform;
        }

		// 활성화 설정
        public void SetEnabled(bool bSet)
        {
            if (lineRenderer == null)
                return;
            
            lineRenderer.enabled = bSet;
        }

		// 사망 이벤트
        private void OnDeathEvent()
        {
            SetEnabled(false);
        }

		// 연결선 표시 업데이트
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