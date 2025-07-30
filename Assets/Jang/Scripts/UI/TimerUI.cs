using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (GameTimerManager.Instance == null || !GameTimerManager.Instance.isTimerRunning)
            return;

        timerText.text = GameTimerManager.Instance.GetFormattedTime();
    }
}
