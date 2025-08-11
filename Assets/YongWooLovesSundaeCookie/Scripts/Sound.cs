using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Common;

namespace DiceSurvivor.Sfx
{
    /// <summary>
    /// 모든 Sfx(근접, 원거리, 피격)
    /// </summary>
    public class Sound : MonoBehaviour
    {
        #region Variables
        // 근접 무기(랜덤 효과음)
        [Header("Random Sfx (Close-Ranged)")]
        public AudioClip[] spearClips;
        public AudioClip[] whipClips;

        // 근접 무기(단일 효과음)
        [Header("Single Sfx (Close-Ranged)")]
        public AudioClip greatSwordClip;
        public AudioClip scytheClip;
        public AudioClip hammerClip;
        public AudioClip staffClip;

        // 원거리 무기(랜덤 효과음)
        [Header("Random Sfx (Long-Ranged)")]
        public AudioClip[] fireballClips;

        // 원거리 무기(단일 효과음)
        [Header("Single Sfx (Long-Ranged)")]
        public AudioClip boomerangClip;
        public AudioClip chakramClip;
        public AudioClip flaskBreakClip;
        public AudioClip laserClip;

        // 피격 효과음
        [Header("Is Attacked Sfx")]
        public AudioClip isAttackedPlayer;
        public AudioClip isAttackedEnemy;
        public AudioClip boomerangHit;
        public AudioClip chakramHit;

        private Dictionary<CloseWeaponType, AudioClip[]> randomSfxCloseMap;
        private Dictionary<CloseWeaponType, AudioClip> singleSfxCloseMap;

        private Dictionary<LongWeaponType, AudioClip[]> randomSfxLongMap;
        private Dictionary<LongWeaponType, AudioClip> singleSfxLongMap;

        private Dictionary<string, AudioClip> isAttackedSfx;

        private AudioSource audioSource;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 참조
            audioSource = this.GetComponent<AudioSource>();

            randomSfxCloseMap = new Dictionary<CloseWeaponType, AudioClip[]>
            {
                { CloseWeaponType.Spear, spearClips },
                { CloseWeaponType.Whip, whipClips }
            };

            singleSfxCloseMap = new Dictionary<CloseWeaponType, AudioClip>
            {
                { CloseWeaponType.Scythe, scytheClip },
                { CloseWeaponType.GreatSword, greatSwordClip },
                { CloseWeaponType.Hammer, hammerClip },
                { CloseWeaponType.Staff, staffClip }
            };

            randomSfxLongMap = new Dictionary<LongWeaponType, AudioClip[]>
            {
                { LongWeaponType.Fireball, fireballClips }
            };

            singleSfxLongMap = new Dictionary<LongWeaponType, AudioClip>
            {
                { LongWeaponType.Boomerang, boomerangClip },
                { LongWeaponType.Chakram, chakramClip },
                { LongWeaponType.PoisonFlask, flaskBreakClip },
                { LongWeaponType.Laser, laserClip }
            };

            isAttackedSfx = new Dictionary<string, AudioClip>
            {
                { "Player", isAttackedPlayer },
                { "Enemy", isAttackedEnemy },
                { "BoomerangHit", boomerangHit },
                { "ChakramHit", chakramHit}
            };
        }
        #endregion

        #region Custom Method
        // 근접 무기 효과음(랜덤) 재생
        public void PlayRandomSfxClose(CloseWeaponType cType)
        {
            if(randomSfxCloseMap.TryGetValue(cType, out AudioClip[] clips)
                && clips.Length > 0)
            {
                var index = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[index]);
            }
        }

        // 근접 무기 효과음(단일) 재생
        public void PlaySingleSfxClose(CloseWeaponType cType)
        {
            if(singleSfxCloseMap.TryGetValue(cType, out AudioClip clip)
                && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        // 원거리 무기 효과음(랜덤) 재생
        public void PlayRandomSfxLong(LongWeaponType lType)
        {
            if (randomSfxLongMap.TryGetValue(lType, out AudioClip[] clips)
                && clips.Length > 0)
            {
                var index = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[index]);
            }
        }

        // 원거리 무기 효과음(단일) 재생
        public void PlaySingleSfxLong(LongWeaponType lType)
        {
            if (singleSfxLongMap.TryGetValue(lType, out AudioClip clip)
                && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        // 근접 무기 효과음 근접 무기 이름 입력 받아 재생
        public void PlayWeaponSfxClose(string weaponTypeName)
        {
            if(System.Enum.TryParse(weaponTypeName, out CloseWeaponType cType))
            {
                if(randomSfxCloseMap.ContainsKey(cType))
                {
                    PlayRandomSfxClose(cType);
                }
                else if(singleSfxCloseMap.ContainsKey(cType))
                {
                    PlaySingleSfxClose(cType);
                }
                else
                {
                    Debug.LogWarning($"Unknown weapon type or missing sound: {cType}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid weapon type string: {weaponTypeName}");
            }
        }

        // 원거리 무기 효과음 근접 무기 이름 입력 받아 재생
        public void PlayWeaponSfxLong(string weaponTypeName)
        {
            if (System.Enum.TryParse(weaponTypeName, out LongWeaponType lType))
            {
                if (randomSfxLongMap.ContainsKey(lType))
                {
                    PlayRandomSfxLong(lType);
                }
                else if (singleSfxLongMap.ContainsKey(lType))
                {
                    PlaySingleSfxLong(lType);
                }
                else
                {
                    Debug.LogWarning($"Unknown weapon type or missing sound: {lType}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid weapon type string: {weaponTypeName}");
            }
        }

        // 피격 효과음 재생
        public void PlayIsAttackedSfx(string target)
        {
            if(isAttackedSfx.TryGetValue(target, out AudioClip clip)
                && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"No attacked SFX found for target: {target}");
            }
        }
        #endregion
    }
}

