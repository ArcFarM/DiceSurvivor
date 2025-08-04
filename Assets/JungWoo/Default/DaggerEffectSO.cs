using DiceSurvivor.Weapons;
using UnityEngine;

namespace DiceSurvivor.WeaponSystem
{
    [CreateAssetMenu(menuName ="Game/Effects/DaggerfEffect")]
    public class DaggerEffectSO : EffectSOBase
    {
        public GameObject projectilePrefab;

        public override void ApplyEffect(int level, GameObject owner, LevelData data, Vector3 mouseDirection)
        {
            // data: LevelData (아래 Json 구조 참고)
            float angleStep = 30f; // 부채꼴 각도 (예: 30도)
            float startAngle = -((data.effects.projectileCount - 1) * angleStep) / 2f;

            for (int repeat = 0; repeat < data.effects.throwCount; repeat++)
            {
                for (int i = 0; i < data.effects.projectileCount; i++)
                {
                    float angle = startAngle + i * angleStep;
                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    Vector3 dir = rotation * mouseDirection.normalized;
                    // 투사체 생성
                    GameObject proj = GameObject.Instantiate(projectilePrefab, owner.transform.position, Quaternion.LookRotation(Vector3.forward, dir));
                    var projectile = proj.GetComponent<Projectile>();
                    projectile.damage = data.damage;
                    projectile.owner = owner;
                    // 필요시 속도 등 세팅
                }
            }
        }
    }
}
