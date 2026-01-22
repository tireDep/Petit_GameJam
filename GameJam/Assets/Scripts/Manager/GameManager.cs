using System;
using System.Collections;
using Game.Combat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        // 정적 변수
        public static GameManager Instance = null;

        // 대기 시간
        [SerializeField]
        private float waitSeconds = 3.0f;
        
        // UI 표시용 대기시간
        private float curSeconds {get; set; }
        
        // 현재 진행중인 코루틴 체크
        private Coroutine curCoroutine;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            ResetData();
        }

        public void Update()
        {
            if (curSeconds > 0.0f)
            {
                curSeconds -= Time.deltaTime;
                
                Debug.Log("GameManager" + curSeconds);
            }
        }

        // 현재 씬 재시작 코루틴
        IEnumerator StartRestartCorou()
        {
            yield return new WaitForSeconds(waitSeconds);
            
            RestartScene();
        }

        // 현재 씬 재시작
        private void RestartScene()
        {
            ResetData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 다음씬 시작 코루틴
        IEnumerator StartNextSceneCorou()
        {
            yield return new WaitForSeconds(waitSeconds);

            StartNextScene();
        }

        // 다음 씬 시작
        private void StartNextScene()
        {
            ResetData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // 사망 이벤트 처리
        public void OnDeathEvent(string tag)
        {
            if (curCoroutine != null)
                return;
            
            curSeconds = waitSeconds;
            if (Equals(tag, "Player"))
            {
                curCoroutine = StartCoroutine(StartRestartCorou());   
            }
            else if(Equals(tag, "Enemy"))
            {
                curCoroutine = StartCoroutine(StartNextSceneCorou());
            }
            else
            {
                curSeconds = 0.0f;
            }
        }

        // 데이터 초기화
        private void ResetData()
        {
            curCoroutine = null;
            curSeconds = 0.0f;
        }
    }
}