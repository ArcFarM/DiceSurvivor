using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceSurvivor.Manager;
using DiceSurvivor.Enemy;

namespace DiceSurvivor.Weapon
{
    /// <summary>
    /// Boomerang 무기 - 가장 가까운 적에게 투척 후 돌아오는 무기
    /// </summary>
    public class BoomerangWeapon : RangedWeaponBase
    {
        [Header("Boomerang Specific")]
        [SerializeField] private GameObject boomerangPrefab;           // 부메랑 프리팹
        [SerializeField] private float rotationSpeed = 720f;           // 회전 속도 (도/초)

        [Header("Runtime")]
        private List<BoomerangProjectile> activeBoomerangs;            // 활성 부메랑 목록
        private float attackTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // 리스트 초기화
            activeBoomerangs = new List<BoomerangProjectile>();
        }

        protected override void Update()
        {
            // 쿨다운 체크
            attackTimer += Time.deltaTime;

            if (attackTimer >= cooldown)
            {
                Attack();
                attackTimer = 0f;
            }

            // 비활성 부메랑 정리
            CleanupInactiveBoomerangs();
        }

        protected override void ShootProjectile(GameObject target, int projectileCount)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                LaunchBoomerang(target, Weapon.cooldown);
            }            
        }

        /// <summary>
        /// 부메랑 발사
        /// </summary>
        private void LaunchBoomerang(GameObject target, float delay)
        {
            StartCoroutine(LaunchBoomerangWithDelay(target, delay));
        }

        /// <summary>
        /// 딜레이 후 부메랑 발사
        /// </summary>
        private IEnumerator LaunchBoomerangWithDelay(GameObject target, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 부메랑 생성
            GameObject boomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);

            // BoomerangProjectile 컴포넌트 추가/설정
            BoomerangProjectile projectile = boomerang.GetComponent<BoomerangProjectile>();
            if (projectile == null)
            {
                projectile = boomerang.AddComponent<BoomerangProjectile>();
            }

            // 초기 방향 설정 (타겟이 있으면 타겟 방향, 없으면 정면)
            Vector3 initialDirection;
            if (target != null)
            {
                initialDirection = (target.transform.position - transform.position).normalized;
            }
            else
            {
                initialDirection = transform.forward;
            }

            activeBoomerangs.Add(projectile);

            Debug.Log($"[Boomerang] 발사! 타겟: {(target != null ? target.name : "없음")}");
        }

        /// <summary>
        /// 비활성 부메랑 정리
        /// </summary>
        private void CleanupInactiveBoomerangs()
        {
            activeBoomerangs.RemoveAll(boomerang => boomerang == null || !boomerang.IsActive);
        }
    }

    
}