using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;          
using DiceSurvivor.Manager;

public class AudioManager : SingletonManager<AudioManager>
{
    #region Variables
    [Header("Sound List")]
    public Sound[] sounds;                 // 인스펙터에서 등록

    [Header("Mixer")]
    public AudioMixer audioMixer;          // 전체 Mixer
    public AudioMixerGroup bgmGroup;       // BGM 그룹
    public AudioMixerGroup sfxGroup;       // SFX 그룹

    private string currentBgm = "";        // 현재 재생 중 BGM 이름
    private readonly Dictionary<string, Sound> soundMap = new();
    #endregion

    #region Unity Event Method
    protected override void Awake()
    {
        base.Awake();

        // Mixer 그룹 자동 탐색(비어있을 때만)
        if (audioMixer != null)
        {
            if (bgmGroup == null)
            {
                var g = audioMixer.FindMatchingGroups("BGM");
                if (g != null && g.Length > 0) bgmGroup = g[0];
            }
            if (sfxGroup == null)
            {
                var g = audioMixer.FindMatchingGroups("SFX");
                if (g != null && g.Length > 0) sfxGroup = g[0];
            }
            // 그래도 없으면 Master라도 할당
            if (bgmGroup == null || sfxGroup == null)
            {
                var master = audioMixer.FindMatchingGroups("Master");
                if (master != null && master.Length > 0)
                {
                    if (bgmGroup == null) bgmGroup = master[0];
                    if (sfxGroup == null) sfxGroup = master[0];
                }
            }
        }

        // 사운드 셋업
        if (sounds != null)
        {
            foreach (var s in sounds)
            {
                if (s == null) continue;
                if (s.source == null) s.source = gameObject.AddComponent<AudioSource>();

                // AudioSource 속성 채우기
                s.source.clip = s.clip;
                s.source.volume = Mathf.Clamp01(s.volume);
                s.source.pitch = Mathf.Clamp(s.pitch, 0.1f, 3f);
                s.source.loop = s.loop;
                s.source.playOnAwake = false;
                s.source.spatialBlend = s.spitalBlend ? 1f : 0f;   // ← 당신의 필드(spitalBlend) 사용!

                // Mixer Group 할당
                var group = s.isBgm ? bgmGroup : sfxGroup;
                if (group != null) s.source.outputAudioMixerGroup = group;

                // 빠른 조회용 맵에 등록
                if (!string.IsNullOrEmpty(s.name))
                {
                    if (!soundMap.ContainsKey(s.name)) soundMap.Add(s.name, s);
                    else Debug.LogWarning($"[AudioManager] 중복된 사운드 이름: {s.name}");
                }
                else
                {
                    Debug.LogWarning("[AudioManager] 이름이 비어 있는 Sound가 있습니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning("[AudioManager] sounds 배열이 비었습니다.");
        }
    }
    #endregion

    #region Custom Method
    private bool TryGetSound(string name, out Sound s)
    {
        s = null;
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[AudioManager] 빈 이름으로 사운드를 찾을 수 없습니다.");
            return false;
        }
        if (!soundMap.TryGetValue(name, out s) || s == null || s.source == null)
        {
            Debug.LogWarning($"[AudioManager] '{name}' 사운드를 찾지 못했거나 AudioSource가 없습니다.");
            return false;
        }
        return true;
    }

    // SFX/일반 사운드 재생
    public void Play(string name)
    {
        if (!TryGetSound(name, out var s)) return;
        s.source.volume = Mathf.Clamp01(s.volume);
        s.source.pitch = Mathf.Clamp(s.pitch, 0.1f, 3f);
        s.source.Play();
    }

    public void Stop(string name)
    {
        if (!TryGetSound(name, out var s)) return;
        s.source.Stop();
    }

    // BGM 재생(중복 방지)
    public void PlayBgm(string name)
    {
        if (string.Equals(currentBgm, name)) return;

        if (!string.IsNullOrEmpty(currentBgm))
            Stop(currentBgm);

        if (!TryGetSound(name, out var s)) return;

        if (!s.isBgm)
            Debug.LogWarning($"[AudioManager] '{name}'은(는) BGM으로 표시되어 있지 않습니다(isBgm=false).");

        currentBgm = name;
        s.source.loop = true; // 보통 BGM은 루프
        s.source.volume = Mathf.Clamp01(s.volume);
        s.source.pitch = Mathf.Clamp(s.pitch, 0.1f, 3f);
        s.source.Play();
    }

    public void StopBgm()
    {
        if (string.IsNullOrEmpty(currentBgm)) return;
        Stop(currentBgm);
        currentBgm = "";
    }

    // (선택) Mixer 파라미터로 볼륨 조절: Exposed Parameter 만들어 둬야 함
    public void SetBgmVolume(float linear01)
    {
        if (audioMixer == null) return;
        float dB = Mathf.Approximately(linear01, 0f) ? -80f : Mathf.Log10(Mathf.Clamp01(linear01)) * 20f;
        audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSfxVolume(float linear01)
    {
        if (audioMixer == null) return;
        float dB = Mathf.Approximately(linear01, 0f) ? -80f : Mathf.Log10(Mathf.Clamp01(linear01)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }
    #endregion
}
