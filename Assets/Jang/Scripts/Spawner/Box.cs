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
        // ���� ����
        onBoxDestroyed?.Invoke();  // ���� �Ŵ����� �˸�
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        onBoxDestroyed?.Invoke();  // ���� ��Ȳ ���
    }
}
