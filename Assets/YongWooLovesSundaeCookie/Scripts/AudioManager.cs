using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Common;

namespace DiceSurvivor.Audio
{
    // 오디오(Sfx, Bgm)를 관리하는 클래스
    public class AudioManager : MonoBehaviour
    {
        #region Variables
        public static AudioManager Instance;

        [Header("Audio Source")]
        public AudioSource sfxSource;

        [Header("SFX Clips(Melee)")]
        public AudioClip scytheClip;
        public AudioClip staffClip;
        public AudioClip[] spearClips;
        public AudioClip greatSwordClip;
        public AudioClip hammerClip;
        public AudioClip[] whipClips;

        [Header("SFX Clips(Ranged)")]
        public AudioClip boomerangClip;
        public AudioClip[] fireballClips;
        public AudioClip chakramClip;
        public AudioClip poisonFlaskClip;
        public AudioClip laserClip;

        [Header("SFX Clips(Hit And Die)")]
        public AudioClip playerHitClip;
        public AudioClip playerDeathClip;
        public AudioClip enemyDeathClip;
        public AudioClip boomerangHitClip;
        public AudioClip chakramHitClip;

        [Header("SFX Clips(Etc)")]
        public AudioClip buttonPressClip;
        public AudioClip victoryClip;
        public AudioClip collectItemClip;

        private Dictionary<MeleeWeaponSfx, AudioClip[]> meleeWeaponSfxDict;
        private Dictionary<RangedWeaponSfx, AudioClip[]> rangedWeaponSfxDict;
        private Dictionary<HitEffectSfx, AudioClip> hitEffectSfxDict;
        private Dictionary<MiscSfx, AudioClip> miscSfxDict;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitDictionaries();
        }
        #endregion

        #region Custom Method
        private void InitDictionaries()
        {
            meleeWeaponSfxDict = new Dictionary<MeleeWeaponSfx, AudioClip[]>()
            {
                { MeleeWeaponSfx.Scythe, new AudioClip[] { scytheClip } },
                { MeleeWeaponSfx.Staff, new AudioClip[] { staffClip } },
                { MeleeWeaponSfx.Spear, spearClips },
                { MeleeWeaponSfx.GreatSword, new AudioClip[] { greatSwordClip } },
                { MeleeWeaponSfx.Hammer, new AudioClip[] { hammerClip } },
                { MeleeWeaponSfx.Whip, whipClips }
            };

            rangedWeaponSfxDict = new Dictionary<RangedWeaponSfx, AudioClip[]>()
            {
                { RangedWeaponSfx.Boomerang, new AudioClip[] { boomerangClip } },
                { RangedWeaponSfx.Fireball, fireballClips },
                { RangedWeaponSfx.Chakram, new AudioClip[] { chakramClip } },
                { RangedWeaponSfx.PoisonFlask, new AudioClip[] { poisonFlaskClip } },
                { RangedWeaponSfx.Laser, new AudioClip[] { laserClip } }
            };

            hitEffectSfxDict = new Dictionary<HitEffectSfx, AudioClip>()
            {
                { HitEffectSfx.PlayerHit, playerHitClip },
                { HitEffectSfx.PlayerDeath, playerDeathClip },
                { HitEffectSfx.EnemyDeath, enemyDeathClip },
                { HitEffectSfx.BoomerangHit, boomerangHitClip },
                { HitEffectSfx.ChakramHit, chakramHitClip }
            };

            miscSfxDict = new Dictionary<MiscSfx, AudioClip>()
            {
                { MiscSfx.ButtonClick, buttonPressClip },
                { MiscSfx.StageClear, victoryClip },
                { MiscSfx.CollectItem, collectItemClip }
            };
        }
        #endregion

        #region Play Functions
        public void PlayMelee(MeleeWeaponSfx type) => PlayRandomClipFromDict(meleeWeaponSfxDict, type);
        public void PlayRanged(RangedWeaponSfx type) => PlayRandomClipFromDict(rangedWeaponSfxDict, type);
        public void PlayHit(HitEffectSfx type)
        {
            if (hitEffectSfxDict.TryGetValue(type, out var clip))
                sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"피격 효과음 {type}이(가) 없습니다");
        }
        public void PlayMisc(MiscSfx type)
        {
            if (miscSfxDict.TryGetValue(type, out var clip))
                sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"기타 효과음 {type}이(가) 없습니다");
        }
        #endregion

        #region Helper
        private void PlayRandomClipFromDict<T>(Dictionary<T, AudioClip[]> dict, T key)
        {
            if (!dict.TryGetValue(key, out var clips) || clips == null || clips.Length == 0)
            {
                Debug.LogWarning($"{key} 사운드 클립이 설정되어 있지 않습니다.");
                return;
            }
            int randomIndex = Random.Range(0, clips.Length);
            sfxSource.PlayOneShot(clips[randomIndex]);
        }
        #endregion

        #region Rapper
        // 근접 무기 SFX
        public void PlayMeleeByIndex(int index)
        {
            PlayMelee((MeleeWeaponSfx)index);
        }

        // 원거리 무기 SFX
        public void PlayRangedByIndex(int index)
        {
            PlayRanged((RangedWeaponSfx)index);
        }

        // 피격 효과음
        public void PlayHitByIndex(int index)
        {
            PlayHit((HitEffectSfx)index);
        }

        // 기타 효과음
        public void PlayMiscByIndex(int index)
        {
            PlayMisc((MiscSfx)index);
        }
        #endregion
    }
}

