using UnityEngine;
using UnityEngine.Events;
using DiceSurvivor.Manager;

/// <summary>
/// 게임 내 시간 경과를 관리하고 일정 시간에 맞춰 이벤트를 발생시킴
/// - 5분: 중간 보스 소환
/// - 10분: 최종 보스 소환
/// </summary>
public class GameTimerManager : SingletonManager<GameTimerManager>
{
    #region Variables
    public float totalTime = 600f;  // 10분 (600초)
    public float remainingTime;     // 남은 시간
    public bool isTimerRunning = true;

    public UnityEvent onMidBossSpawn;    // 5분 남았을 때
    public UnityEvent onFinalBossSpawn;  // 0초일 때

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
        remainingTime = Mathf.Max(remainingTime, 0f); // 음수 방지

        float timePassed = totalTime - remainingTime;

        // 5분 경과 시 중간보스 이벤트 호출
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
    // UI에 타이머를 표시할 수 있도록 00:00 형식으로 반환
    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
    #endregion
}
