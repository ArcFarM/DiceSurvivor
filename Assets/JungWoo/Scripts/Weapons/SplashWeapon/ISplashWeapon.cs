using UnityEngine;

namespace DiceSurvivor.Weapon

{
    /// <summary>
    /// Splash(범위) 무기의 공통 인터페이스
    /// </summary>
    public interface ISplashWeapon
    {
        // 기본 속성
        float Radius { get; set; }           // 효과 범위
        float DotDamage { get; set; }        // 도트 데미지
        float Duration { get; set; }          // 효과 지속시간 (감속 등)
        float DamageInterval { get; set; }    // 데미지 간격

        // 투사체 관련 (일부 Splash 무기용)
        float ProjectileSize { get; set; }
        float ProjectileSpeed { get; set; }
        int ProjectileCount { get; set; }

        // 폭발 관련 (일부 Splash 무기용)
        float ExplosionRadius { get; set; }
        float ExplosionDamage { get; set; }

        // 기본 메서드
        void InitializeSplashWeapon(WeaponStats stats);
        void ApplyAreaEffect();
        void ApplyDotDamage(GameObject target);
        void ApplyDebuff(GameObject target);
        void UpdateAreaOfEffect();
    }
}