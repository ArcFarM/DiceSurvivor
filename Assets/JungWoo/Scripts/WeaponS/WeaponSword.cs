using DiceSurvivor.WeaponSystem;
using System.Collections.Generic;
using UnityEngine;

namespace DiceSurvivor.Weapons
{
    public class WeaponSword : MonoBehaviour
    {
        public WeaponSO weapon; // 에디터에서 할당
        private int currentLevel = 1;
        private float cooldownTimer = 0f;

        void Update()
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f && Input.GetMouseButton(0)) // 마우스 좌클릭
            {
                var data = LevelDataManager.Instance.GetLevelData(weapon.id, currentLevel);
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 dir = (mouseWorld - transform.position);
                weapon.effect.ApplyEffect(currentLevel, gameObject, data, dir);
                cooldownTimer = data.cooldown;
            }
        }

    }

}
