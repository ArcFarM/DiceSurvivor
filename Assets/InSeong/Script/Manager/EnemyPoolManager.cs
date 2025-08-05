using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Manager {
    /// <summary>
    /// EnemyData 기반 적 풀링 매니저 (SingletonManager 상속, 불필요한 자료구조 제거)
    /// </summary>
    public class EnemyPoolManager : SingletonManager<EnemyPoolManager> {
        // 1. EnemyData 배열 (Inspector에서 등록)
        public EnemyData[] enemyDataArray;

        // 2. EnemyData별 풀 자료구조
        public Queue<GameObject> enemyPool = new Queue<GameObject>();
        public Queue<GameObject> eliteEnemyPool = new Queue<GameObject>();
        public Queue<GameObject> bossEnemyPool = new Queue<GameObject>();

        protected override void Awake() {
            base.Awake();
            // EnemyData 기반으로 풀 구조 초기화
            // 각 EnemyData.body(프리팹)로 오브젝트 여러 개 미리 생성 후 큐에 등록
        }

        /// <summary>
        /// EnemyData 기준 적 스폰(풀에서 꺼내어 위치/속성 적용)
        /// </summary>
        public GameObject SpawnEnemy(EnemyData data, Vector3 pos, Quaternion rot) {
            // 해당 EnemyData의 풀에서 오브젝트 꺼내거나 새로 생성
            // 위치/회전 지정
            // EnemyData의 속성(능력치/보상/계수 등) 적용
            // 활성화 후 반환
            return null; // 실제 객체 반환 (임시)
        }

        /// <summary>
        /// EnemyData 기준 적 반환(비활성화)
        /// </summary>
        public void ReturnEnemy(EnemyData data, GameObject obj) {
            // 오브젝트 비활성화 후 EnemyData별 풀에 반환
        }

        /// <summary>
        /// 모든 적 반환(전체 비활성화)
        /// </summary>
        public void ReturnAllEnemies() {
            // 모든 EnemyData별 풀을 순회하며 활성 적 반환
        }
    }
}