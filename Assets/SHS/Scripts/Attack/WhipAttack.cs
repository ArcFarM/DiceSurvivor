using DiceSurvivor.Weapon;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DiceSurvivor.Weapon
{
    public class WhipAttack : MeleeWeaponBase
    {
        #region Variables
        // 채찍 궤적을 따라 생성된 웨이포인트 리스트
        public List<Transform> wayPoints = new List<Transform>();

        // Raycast 길이 (적 탐지 거리)
        public float rayLength = 2f;
        #endregion

        #region Properties
        // 현재는 사용되지 않지만, 필요 시 속성 정의 가능
        #endregion

        #region Unity Event Methods
        // 무기 컨트롤러 초기화
        protected override void Awake()
        {
            weaponController = GetComponent<WeaponController>(); // WeaponController 컴포넌트 가져오기

            base.Awake(); // 부모 클래스 초기화
        }

        protected override void Update()
        {
            base.Update();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// 외부에서 전달받은 웨이포인트 리스트를 설정하고,
        /// 각 웨이포인트에서 Raycast를 통해 적을 감지하여 피해 처리
        /// </summary>
        /// <param name="points">웨이포인트 리스트</param>
        public void SetWayPoints(List<Transform> points)
        {
            wayPoints = points; // 웨이포인트 저장

            if (wayPoints != null && wayPoints.Count > 0)
            {
                foreach (Transform wp in wayPoints)
                {
                    Vector3 direction = wp.forward; // 웨이포인트의 전방 방향
                    Ray ray = new Ray(wp.position, direction); // Ray 생성

                    // 디버그용 Ray 시각화 (Scene 뷰에서 빨간 선으로 표시됨)
                    Debug.DrawRay(wp.position, direction * rayLength, Color.red, 1f);

                    // Raycast로 적 감지
                    if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
                    {
                        if (hit.transform.CompareTag("Enemy")) // 적 태그 확인
                        {
                            Debug.Log($"적 피해 받음 : {Weapon.damage}"); // 피해 로그 출력
                            // 실제 피해 처리 로직은 Weapon.damage를 기반으로 추가 가능
                        }
                    }
                }
            }
        }
        #endregion
    }
}
