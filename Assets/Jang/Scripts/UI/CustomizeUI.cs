using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CustomizeUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Dropdown headPropDropdown;
    public TMP_Dropdown eyeDropdown;
    public TMP_Dropdown mouthDropdown;

    [Header("Preview Character")]
    public GameObject previewCharacter;

    [Header("Customization Parts")]
    public GameObject[] headProps;
    public GameObject[] eyes;
    public GameObject[] mouths;

    private void Start()
    {
        SetupDropdowns();
        LoadCustomization();
        UpdatePreview();

        // 드롭다운 변경 이벤트 연결
        headPropDropdown.onValueChanged.AddListener(UpdateHeadProp);
        eyeDropdown.onValueChanged.AddListener(UpdateEye);
        mouthDropdown.onValueChanged.AddListener(UpdateMouth);
    }

    private void SetupDropdowns()
    {
        // 드롭다운 초기화
        headPropDropdown.ClearOptions();
        eyeDropdown.ClearOptions();
        mouthDropdown.ClearOptions();

        // 이름으로 옵션 구성
        List<string> headNames = headProps.Select(h => h.name).ToList();
        List<string> eyeNames = eyes.Select(e => e.name).ToList();
        List<string> mouthNames = mouths.Select(m => m.name).ToList();

        headPropDropdown.AddOptions(headNames);
        eyeDropdown.AddOptions(eyeNames);
        mouthDropdown.AddOptions(mouthNames);
    }

    private void UpdatePreview()
    {
        UpdateHeadProp(headPropDropdown.value);
        UpdateEye(eyeDropdown.value);
        UpdateMouth(mouthDropdown.value);
    }

    public void UpdateHeadProp(int index)
    {
        for (int i = 0; i < headProps.Length; i++)
        {
            if (headProps[i] != null)
                headProps[i].SetActive(i == index);
        }
    }

    public void UpdateEye(int index)
    {
        for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i] != null)
                eyes[i].SetActive(i == index);
        }
    }

    public void UpdateMouth(int index)
    {
        for (int i = 0; i < mouths.Length; i++)
        {
            if (mouths[i] != null)
                mouths[i].SetActive(i == index);
        }
    }

    public void SaveCustomization()
    {
        PlayerPrefs.SetInt("HeadIndex", headPropDropdown.value);
        PlayerPrefs.SetInt("EyeIndex", eyeDropdown.value);
        PlayerPrefs.SetInt("MouthIndex", mouthDropdown.value);
        PlayerPrefs.Save();
        Debug.Log("Customization Saved!");
    }

    private void LoadCustomization()
    {
        int headIndex = PlayerPrefs.GetInt("HeadIndex", 0);
        int eyeIndex = PlayerPrefs.GetInt("EyeIndex", 0);
        int mouthIndex = PlayerPrefs.GetInt("MouthIndex", 0);

        headPropDropdown.value = Mathf.Clamp(headIndex, 0, headProps.Length - 1);
        eyeDropdown.value = Mathf.Clamp(eyeIndex, 0, eyes.Length - 1);
        mouthDropdown.value = Mathf.Clamp(mouthIndex, 0, mouths.Length - 1);
    }
}
