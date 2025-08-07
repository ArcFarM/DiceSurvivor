using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public ProgressBar hpBarUI;  // 인스펙터에서 연결할 UI

    private void Start()
    {
        currentHealth = maxHealth;

        if (hpBarUI != null)
        {
            hpBarUI.MaxValue = maxHealth;
            hpBarUI.BarValue = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hpBarUI != null)
            hpBarUI.BarValue = currentHealth;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hpBarUI != null)
            hpBarUI.BarValue = currentHealth;
    }
}
