using UnityEngine;

namespace DiceSurvivor.Common
{
    public enum SfxPlayType
    {
        MeleeSfx,
        RangedSfx,
        HitSfx,
        MiscSfx,
    }

    public enum MeleeWeaponSfx
    {
        Scythe,
        Staff,
        Spear,
        GreatSword,
        Hammer,
        Whip,
    }

    public enum RangedWeaponSfx
    {
        Boomerang,
        Fireball,
        Chakram,
        PoisonFlask,
        Laser,
    }

    public enum HitEffectSfx
    {
        PlayerHit,
        PlayerDeath,
        EnemyDeath,
        BoomerangHit,
        ChakramHit,
    }

    public enum MiscSfx
    {
        ButtonClick,
        StageClear,
        CollectItem,
    }
}