using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    public Renderer bodyRenderer;
    public GameObject[] headProps;
    public GameObject[] eyeVariants;
    public GameObject[] mouthVariants;
    public Color[] colors;

    void Start()
    {
        int colorIndex = PlayerPrefs.GetInt("BodyColor", 0);
        int headIndex = PlayerPrefs.GetInt("HeadPropIndex", 0);
        int eyeIndex = PlayerPrefs.GetInt("EyeIndex", 0);
        int mouthIndex = PlayerPrefs.GetInt("MouthIndex", 0);

        if (bodyRenderer != null && colorIndex < colors.Length)
            bodyRenderer.material.color = colors[colorIndex];

        ApplySelection(headProps, headIndex);
        ApplySelection(eyeVariants, eyeIndex);
        ApplySelection(mouthVariants, mouthIndex);
    }

    void ApplySelection(GameObject[] list, int selected)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] != null)
                list[i].SetActive(i == selected);
        }
    }
}
