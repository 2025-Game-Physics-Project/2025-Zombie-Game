using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 피격! 현재 HP: {hp}");

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        // 죽을 때 애니메이션 또는 사망 처리
        Destroy(gameObject);
    }
}