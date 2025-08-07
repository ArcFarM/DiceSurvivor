using UnityEngine;
using System.Collections.Generic;
using DiceSurvivor.Common;

namespace DiceSurvivor.Sfx
{
    /// <summary>
    /// 근접 무기 Sfx
    /// </summary>
    public class WeaponSound : MonoBehaviour
    {
        #region Variables
        [Header("Random Sfx")]
        public AudioClip[] spearClips;
        public AudioClip[] whipClips;

        [Header("Single Sfx")]
        public AudioClip greatSwordClip;
        public AudioClip scytheClip;
        public AudioClip hammerClip;
        public AudioClip staffClip;

        private Dictionary<WeaponType, AudioClip[]> randomSfxMap;
        private Dictionary<WeaponType, AudioClip> singleSfxMap;

        private AudioSource audioSource;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            audioSource = this.GetComponent<AudioSource>();

            randomSfxMap = new Dictionary<WeaponType, AudioClip[]>
            {
                { WeaponType.Spear, spearClips },
                { WeaponType.Whip, whipClips }
            };

            singleSfxMap = new Dictionary<WeaponType, AudioClip>
            {
                { WeaponType.Scythe, scytheClip },
                { WeaponType.GreatSword, greatSwordClip },
                { WeaponType.Hammer, hammerClip },
                { WeaponType.Staff, staffClip }
            };
        }
        #endregion

        #region Custom Method
        public void PlayRandomSfx(WeaponType type)
        {
            if(randomSfxMap.TryGetValue(type, out AudioClip[] clips)
                && clips.Length > 0)
            {
                var index = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[index]);
            }
        }

        public void PlaySingleSfx(WeaponType type)
        {
            if(singleSfxMap.TryGetValue(type, out AudioClip clip)
                && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public void PlayWeaponSfx(string weaponTypeName)
        {
            if(System.Enum.TryParse(weaponTypeName, out WeaponType type))
            {
                if(randomSfxMap.ContainsKey(type))
                {
                    PlayRandomSfx(type);
                }
                else if(singleSfxMap.ContainsKey(type))
                {
                    PlaySingleSfx(type);
                }
                else
                {
                    Debug.LogWarning($"Unknown weapon type or missing sound: {type}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid weapon type string: {weaponTypeName}");
            }
        }
        #endregion
    }
}

