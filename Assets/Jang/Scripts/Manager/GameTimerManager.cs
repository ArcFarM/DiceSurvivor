using UnityEngine;
using UnityEngine.Events;
using MainGame.Manager; 

/// <summary>
/// ���� �� �ð� ����� �����ϰ� ���� �ð��� ���� �̺�Ʈ�� �߻���Ŵ
/// - 5��: �߰� ���� ��ȯ
/// - 10��: ���� ���� ��ȯ
/// </summary>
public class GameTimerManager : SingletonManager<GameTimerManager>
{
    #region Variables
    public float totalTime = 600f;  // 10�� (600��)
    public float remainingTime;     // ���� �ð�
    public bool isTimerRunning = true;

    public UnityEvent onMidBossSpawn;    // 5�� ������ ��
    public UnityEvent onFinalBossSpawn;  // 0���� ��

    private bool midBossSpawned = false;
    private bool finalBossSpawned = false;
    #endregion

    #region Unity Event Method
    protected override void OnSingletonAwake()
    {
        remainingTime = totalTime; 
    }
    void Update()
    {
        if (!isTimerRunning || remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0f); // ���� ����

        float timePassed = totalTime - remainingTime;

        // 5�� ��� �� �߰����� �̺�Ʈ ȣ��
        if (!midBossSpawned && timePassed >= 300f)
        {
            midBossSpawned = true;
            onMidBossSpawn.Invoke();
        }

        if (!finalBossSpawned && remainingTime <= 0f)
        {
            finalBossSpawned = true;
            isTimerRunning = false;
            onFinalBossSpawn.Invoke();
        }
    }
    #endregion

    #region Custom Method
    // UI�� Ÿ�̸Ӹ� ǥ���� �� �ֵ��� 00:00 �������� ��ȯ
    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
    #endregion
}
