using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("무적/피격 연출")]
    [Tooltip("피해 후 무적 유지 시간(초)")]
    public float invincibleTime = 1f;
    [Tooltip("깜빡임 간격(초)")]
    public float blinkInterval = 0.1f;
    [Tooltip("타임스케일 영향을 받지 않게 깜빡임")]
    public bool useUnscaledTimeForBlink = false;

    [Header("UI")]
    public HPBar hpBarUI; // MaxValue, CurrentValue를 가진 HP UI 래퍼

    [Header("사망 처리")]
    [Tooltip("사망 시 일시정지할지 여부")]
    public bool pauseOnDeath = true;

    // 상태
    public bool IsInvincible { get; private set; }
    public bool IsDead { get; private set; }

    // 내부
    private readonly List<Renderer> renderers = new();
    private Coroutine invincibleRoutine;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;
        CollectAllRenderers();
    }

    private void Start()
    {
        if (hpBarUI != null)
        {
            hpBarUI.MaxValue = maxHealth;
            hpBarUI.CurrentValue = currentHealth;
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            TakeDamage(20);
        }
    }
    private void OnDisable()
    {
        StopBlinkImmediate();
        SetVisible(true);
        IsInvincible = false;
    }

    // === 외부에서 호출 ===
    public void TakeDamage(float amount, bool triggerInvincible = true)
    {
        if (IsDead || amount <= 0f) return;
        if (IsInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        UpdateHPUI();

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (triggerInvincible && invincibleTime > 0f)
        {
            if (invincibleRoutine != null) StopCoroutine(invincibleRoutine);
            invincibleRoutine = StartCoroutine(InvincibleCoroutine(invincibleTime));
        }
        else
        {
            // 무적은 안 쓰고 깜빡임만 줄 때
            StartBlink(invincibleTime > 0f ? invincibleTime : 0.25f);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateHPUI();
    }

    public void DrainSilently(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        UpdateHPUI();

        if (currentHealth <= 0f) Die();
    }

    // === 무적 & 깜빡임 ===
    private IEnumerator InvincibleCoroutine(float duration)
    {
        IsInvincible = true;
        StartBlink(duration);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        IsInvincible = false;
        invincibleRoutine = null;
    }

    public void StartBlink(float duration)
    {
        if (duration <= 0f) return;
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkCoroutine(duration));
    }

    public void StopBlinkImmediate()
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = null;
        SetVisible(true);
    }

    private IEnumerator BlinkCoroutine(float duration)
    {
        float elapsed = 0f;
        bool visible = true;
        SetVisible(true);

        while (elapsed < duration)
        {
            visible = !visible;
            SetVisible(visible);

            if (useUnscaledTimeForBlink) yield return new WaitForSecondsRealtime(blinkInterval);
            else yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval;
        }

        SetVisible(true);
        blinkRoutine = null;
    }

    // === 렌더러/HP UI/사망 ===
    private void CollectAllRenderers()
    {
        renderers.Clear();
        var all = GetComponentsInChildren<Renderer>(true); // 착용 아이템까지 포함
        renderers.AddRange(all);
    }

    private void SetVisible(bool show)
    {
        if (renderers.Count == 0) CollectAllRenderers();
        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.enabled = show;
        }
    }

    private void UpdateHPUI()
    {
        if (hpBarUI != null)
            hpBarUI.CurrentValue = currentHealth;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        StopBlinkImmediate();
        SetVisible(true);
        Debug.Log("[PlayerHealth] Player Died");

        // 게임오버 UI
        GameOverUI gameOver = FindObjectOfType<GameOverUI>();
        if (gameOver != null) gameOver.ShowGameOverUI();

        if (pauseOnDeath) Time.timeScale = 0f;
    }
}
