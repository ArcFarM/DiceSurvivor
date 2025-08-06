using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Enemy;
using System;
using Unity.VisualScripting;

namespace DiceSurvivor.Manager {
    /// <summary>
    /// EnemyData 기반 적 풀링 매니저 (SingletonManager 상속, 불필요한 자료구조 제거)
    /// </summary>
    public class EnemyPoolManager : SingletonManager<EnemyPoolManager>
    {
        #region Variables
        [Header("일반/엘리트/보스 적 정보 배열")]
        public EnemyData[] enemyDataArray;
        public EnemyData[] eliteEnemyDataArray;
        public EnemyData[] bossEnemyDataArray;

        [Header("적 종류별 사용할 풀")]
        public Queue<GameObject> enemyPool = new Queue<GameObject>();
        public Queue<GameObject> eliteEnemyPool = new Queue<GameObject>();
        public Queue<GameObject> bossEnemyPool = new Queue<GameObject>();

        [Header("각 풀 별 최대 동시 존재할 오브젝트의 수")]
        public int maxEnemyCount = 100;
        public int maxEliteEnemyCount = 15;
        public int maxBossEnemyCount = 1;

        [Header("적 스폰 매니저 참조")]
        public EnemySpawnManager esManager;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void Awake()
        {
            base.Awake();
            //풀 구조 초기화
            InitializeEnemyPools();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// EnemyData 기준 적 스폰(풀에서 꺼내어 위치/속성 적용)
        /// </summary>
        public GameObject SpawnEnemy(EnemyData data)
        {
            // 해당 EnemyData의 풀에서 오브젝트 꺼내서 spawnmanager에 전달
            switch (data.type)
            {
                case EnemyData.EnemyType.Normal:
                    if (enemyPool.Count == 0) return null; // 풀에 오브젝트가 없으면 null 반환
                    return enemyPool.Dequeue();
                case EnemyData.EnemyType.Elite:
                    if (eliteEnemyPool.Count == 0) return null; // 풀에 오브젝트가 없으면 null 반환
                    GameObject eliteEnemyObj = eliteEnemyPool.Dequeue();
                    return eliteEnemyObj;
                case EnemyData.EnemyType.Boss:
                    if (bossEnemyPool.Count == 0) return null; // 풀에 오브젝트가 없으면 null 반환
                    GameObject bossEnemyObj = bossEnemyPool.Dequeue();
                    return bossEnemyObj;
                default:
                    Debug.LogError("Unknown enemy type: " + data.type);
                    return null;
            }
        }

        /// <summary>
        /// 풀 오브젝트 초기화
        /// </summary>
        void InitializeEnemyPools()
        {
            //각 풀에 정해진 최대 수량 만큼의 오브젝트를 미리 생성
            for (int i = 0; i < maxEnemyCount; i++)
            {
                GameObject enemyObj = Instantiate(enemyDataArray[0].body);
                //각 DataArray에 할당된 body는 EnemyActivity 컴포넌트를 갖고 있고, 여기에 EnemyData 할당
                if (enemyObj.TryGetComponent<EnemyActivity>(out EnemyActivity ea))
                {
                    ea.EnemyData = enemyDataArray[0];
                }
                else
                {
                    Debug.LogError("EnemyActivity component not found on the enemy prefab.");
                }
                enemyObj.SetActive(false);
                enemyPool.Enqueue(enemyObj);
            }
            for (int i = 0; i < maxEliteEnemyCount; i++)
            {
                GameObject eliteEnemyObj = Instantiate(eliteEnemyDataArray[0].body);
                if (eliteEnemyObj.TryGetComponent<EnemyActivity>(out EnemyActivity ea))
                {
                    ea.EnemyData = eliteEnemyDataArray[0];
                }
                else
                {
                    Debug.LogError("EnemyActivity component not found on the enemy prefab.");
                }
                eliteEnemyObj.SetActive(false);
                eliteEnemyPool.Enqueue(eliteEnemyObj);
            }
            for (int i = 0; i < maxBossEnemyCount; i++)
            {
                GameObject bossEnemyObj = Instantiate(bossEnemyDataArray[0].body);
                if (bossEnemyObj.TryGetComponent<EnemyActivity>(out EnemyActivity ea))
                {
                    ea.EnemyData = bossEnemyDataArray[0];
                }
                else
                {
                    Debug.LogError("EnemyActivity component not found on the enemy prefab.");
                }
                bossEnemyObj.SetActive(false);
                bossEnemyPool.Enqueue(bossEnemyObj);
            }
        }


        /// <summary>
        /// EnemyData 기준 적 반환(비활성화) 및 현재 웨이브 인덱스에 맞게 갱신
        /// </summary>
        public void ReturnEnemy(EnemyData data, GameObject obj)
        {
            // 오브젝트 비활성화 후 EnemyData별 풀에 반환
            // 오브젝트가 갱신되지 않았다면 -> 현재 enemyData id와 오브젝트 enemyData id가 불일치하다면 갱신
            obj.SetActive(false);
            // 검증을 위한 인덱스
            int index = EnemySpawnManager.Instance.currentWaveIndex;

            if (data.type == EnemyData.EnemyType.Normal)
            {
                if (data.enemyId != enemyDataArray[index].enemyId
                    && obj.TryGetComponent<EnemyActivity>(out EnemyActivity enemyActivity))
                {
                    // 현재 웨이브 인덱스에 맞는 EnemyData로 갱신
                    enemyActivity.EnemyData = enemyDataArray[index];
                }
                enemyPool.Enqueue(obj);
            }
            else if (data.type == EnemyData.EnemyType.Elite)
            {
                if (data.enemyId != eliteEnemyDataArray[index].enemyId
                    && obj.TryGetComponent<EnemyActivity>(out EnemyActivity enemyActivity))
                {
                    // 현재 웨이브 인덱스에 맞는 EnemyData로 갱신
                    enemyActivity.EnemyData = eliteEnemyDataArray[index];
                }
                eliteEnemyPool.Enqueue(obj);
            }
            else if (data.type == EnemyData.EnemyType.Boss)
            {
                if (data.enemyId != bossEnemyDataArray[index].enemyId
                    && obj.TryGetComponent<EnemyActivity>(out EnemyActivity enemyActivity))
                {
                    // 현재 웨이브 인덱스에 맞는 EnemyData로 갱신
                    enemyActivity.EnemyData = bossEnemyDataArray[index];
                }
                bossEnemyPool.Enqueue(obj);
            }
            else
            {
                Debug.LogError("Unknown enemy type: " + data.type);
            }
            
            
        }

        /// <summary>
        /// 모든 적 반환(전체 비활성화)
        /// </summary>
        public void ReturnAllEnemies()
        {
            //EnemySpawnManager에 있는 적 오브젝트 모두 비활성화 후 풀에 반환
            //하이어라키 상 자식 오브젝트 << Transform 접근 필요
            foreach (var enemy in esManager.enemyPar.transform)
            {
                // 각 적 오브젝트에서 EnemyActivity 컴포넌트를 찾아 EnemyData를 통해 풀에 반환
                // GameObject로 캐스팅 후 EnemyActivity 컴포넌트 접근 : 캐스팅 이유 - 설계상으로는 모두 GameObject 타입이지만, 만일을 대비
                if (enemy is GameObject enemyObj)
                {
                    EnemyActivity ea = enemyObj.GetComponent<EnemyActivity>();
                    if (ea != null)
                    {
                        ReturnEnemy(ea.EnemyData, enemyObj);
                    }
                }
            }
            foreach (var eliteEnemy in esManager.eliteEnemyPar.transform)
            {
                if (eliteEnemy is GameObject eliteEnemyObj)
                {
                    EnemyActivity ea = eliteEnemyObj.GetComponent<EnemyActivity>();
                    if (ea != null)
                    {
                        ReturnEnemy(ea.EnemyData, eliteEnemyObj);
                    }
                }
            }
            foreach (var bossEnemy in esManager.bossEnemyPar.transform)
            {
                if (bossEnemy is GameObject bossEnemyObj)
                {
                    EnemyActivity ea = bossEnemyObj.GetComponent<EnemyActivity>();
                    if (ea != null)
                    {
                        ReturnEnemy(ea.EnemyData, bossEnemyObj);
                    }
                }
            }
        }
        #endregion
    }
}