using UnityEngine;
using System;

public class Box : MonoBehaviour
{
    public float hp = 1f;
    public event Action onBoxDestroyed;

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0f)
        {
            BreakBox();
        }
    }

    public void BreakBox()
    {
        // 보상 지급
        onBoxDestroyed?.Invoke();  // 스폰 매니저에 알림
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        onBoxDestroyed?.Invoke();  // 예외 상황 대비
    }
}
