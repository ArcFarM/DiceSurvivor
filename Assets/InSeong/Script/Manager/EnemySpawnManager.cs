using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Manager {
    /// <summary>
    /// EnemySpawnManager (SingletonManager 상속, EnemyData 활용, 불필요한 자료구조 제거)
    /// </summary>
    public class EnemySpawnManager : SingletonManager<EnemySpawnManager> {
        // 1. EnemyPoolManager 참조 (싱글톤)
        // 2. 스폰 위치(배열/리스트)
        // 3. 웨이브 및 시간/레벨 관리 변수
        // 4. EnemyData 기반으로 적 종류/구성 관리

        protected override void Awake() {
            base.Awake();
            // 초기 웨이브/적 구성 세팅
            // 적 스폰 스케쥴링 시작
        }

        void Update() {
            // 게임 시간/웨이브 진행 체크
            // 웨이브 교체 타이밍 감지 시 적 교체 로직 실행
            // 주기적 적 스폰 관리
        }

        /// <summary>
        /// EnemyData 기준 적 스폰 (풀에서 꺼내어 위치 지정 및 활성화)
        /// </summary>
        void SpawnEnemy(EnemyData enemyData, Vector3 spawnPos) {
            // EnemyPoolManager.Instance.SpawnEnemy(enemyData, spawnPos, Quaternion.identity)
            // EnemyData의 속성 자동 적용
        }

        /// <summary>
        /// 웨이브 교체
        /// </summary>
        void ChangeWave(int waveIndex) {
            // EnemyPoolManager.Instance.ReturnAllEnemies()
            // EnemyData 배열/리스트에서 다음 웨이브 적 구성 선택 및 스폰
        }

        /// <summary>
        /// 적 전체 반환(예: 웨이브 완전 교체, 보스 등장 등)
        /// </summary>
        void RemoveAllEnemies() {
            // EnemyPoolManager.Instance.ReturnAllEnemies()
        }

        /// <summary>
        /// 웨이브/시간에 따른 적 스폰 정보 업데이트
        /// </summary>
        void UpdateSpawnConfig() {
            // EnemyData 배열/리스트에서 현재 웨이브/시간에 맞는 적 종류/수/난이도 갱신
        }
    }
}