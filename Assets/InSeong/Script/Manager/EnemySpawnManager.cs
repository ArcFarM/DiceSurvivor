using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Manager {
    /// <summary>
    /// EnemySpawnManager (SingletonManager 상속, EnemyData 활용, 불필요한 자료구조 제거)
    /// </summary>
    public class EnemySpawnManager : SingletonManager<EnemySpawnManager>
    {
        #region Variables
        //오브젝트 풀 싱글톤 참조
        EnemyPoolManager epManager;
        // 플레이어 주변 위치에서 적 스폰해야 하므로 플레이어 위치 참조
        public Transform playerPos;
        [Header("적 스폰 관련 변수")]
        public float minSpawnOffset = 5f; // 플레이어 주변에서 적 스폰할 반경 최소치
        public float maxSpawnOffset = 10f; // 플레이어 주변에서 적 스폰할 반경 최대치

        public float spawnInterval = 0.2f; // 적 스폰 간격 (초)
        public float spawnTimer = 0f; // 스폰 타이머
        // 3. 웨이브 및 시간/레벨 관리 변수
        public int currentWaveIndex = 0; // 현재 웨이브 인덱스
        public float waveDuration = 60f; // 웨이브 지속 시간 (초)
        public float timeSinceLastWave = 0f; // 마지막 웨이브 이후 경과 시간

        //소환된 적들은 모두 Hierarchy 창 내 이 오브젝트의 자식으로 관리
        public GameObject enemyPar;
        public GameObject eliteEnemyPar;
        public GameObject bossEnemyPar;

        #endregion

        #region Unity Event Methods
        protected override void Awake()
        {
            base.Awake();
            // 초기 웨이브/적 구성 세팅
            // 적 스폰 스케쥴링 시작
            epManager = EnemyPoolManager.Instance;
            //플레이어는 단 하나만 존재하므로
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        }

        void Update()
        {
            // 게임 시간/웨이브 진행 체크
            timeSinceLastWave += Time.deltaTime;
            if (timeSinceLastWave >= waveDuration)
            {
                // 웨이브 교체 로직 실행
                ChangeWave(currentWaveIndex + 1);
                timeSinceLastWave = 0f;
            }
            // 주기적 적 스폰 관리
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                //일반 적 스폰 로직 실행
                SpawnEnemy(epManager.enemyDataArray[currentWaveIndex]);
                spawnTimer = 0f;
                //TODO : 일정 조건을 만족했을 때 엘리트적/보스 적 소환 추가
            }
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// EnemyData 기준 적 스폰 (풀에서 꺼내어 위치 지정 및 활성화)
        /// </summary>
        public void SpawnEnemy(EnemyData enemyData)
        {
            GameObject enemyObj = epManager.SpawnEnemy(enemyData);
            if (enemyObj != null)
            {
                // 플레이어 위치 기준 min ~ maxSpawnOffset 내에서 랜덤 위치 지정
                GetOffset(out Vector3 offset);
                enemyObj.transform.position = playerPos.position + offset;

                // 적 타입에 맞춰서 부모 오브젝트 지정
                switch (enemyData.type)
                {
                    case EnemyData.EnemyType.Normal:
                        enemyObj.transform.SetParent(enemyPar.transform);
                        break;
                    case EnemyData.EnemyType.Elite:
                        enemyObj.transform.SetParent(eliteEnemyPar.transform);
                        break;
                    case EnemyData.EnemyType.Boss:
                        enemyObj.transform.SetParent(bossEnemyPar.transform);
                        break;
                }

                // 적의 EnemyActivity 컴포넌트에 데이터 설정 - EnemyData는 EnemyPoolManager의 DataArray의 waveIndex번째 데이터
                if (enemyObj.TryGetComponent<EnemyActivity>(out EnemyActivity enemyActivity))
                {
                    enemyActivity.EnemyData = enemyData;
                }
                else Debug.LogError("EnemyActivity component not found on spawned enemy object.");

                //적 활성화
                enemyObj.SetActive(true);
            }
        }

        void GetOffset(out Vector3 offset)
        {
            float offSetX = Random.Range(-maxSpawnOffset, maxSpawnOffset);
            float offSetZ = Random.Range(-maxSpawnOffset, maxSpawnOffset);
            //최소치보다 큰 무작위 값으로 오프셋을 설정
            while (Mathf.Abs(offSetX) < minSpawnOffset)
            {
                offSetX = Random.Range(-maxSpawnOffset, maxSpawnOffset);
            }
            while(Mathf.Abs(offSetZ) < minSpawnOffset)
            {
                offSetZ = Random.Range(-maxSpawnOffset, maxSpawnOffset);
            }
            offset = new Vector3(offSetX, 0f, offSetZ);
        }

        /// <summary>
        /// 웨이브 교체
        /// </summary>
        void ChangeWave(int waveIndex)
        {
            // 현재 웨이브 인덱스 갱신
            currentWaveIndex = waveIndex;
            // EnemyData 배열/리스트에서 다음 웨이브 적 구성 선택 및 스폰
            epManager.ReturnAllEnemies();
            //ReturnAllEnemies로 모든 비활성화 된 적을 풀에 반환하고, ReturnEnemy에서 현재 인덱스로 갱신
        }
        #endregion
    }
}