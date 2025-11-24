using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public float deathDelay = 3f;   // 죽는 애니 재생 후 제거까지 대기 시간

    private Animator animator;
    private Collider col;
    private ZombieAI zombieAI;

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        zombieAI = GetComponent<ZombieAI>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Zombie HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // AI 멈추기
        if (zombieAI != null)
        {
            enabled = false;        // 더 이상 Update 안 돌도록
            zombieAI.enabled = false;
        }

        // 충돌 제거 (총알/플레이어 더 이상 안 맞게)
        if (col != null)
            col.enabled = false;

        // 죽는 애니메이션 재생
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Die");      // Animator에 있는 Die 트리거
        }

        // deathDelay 후에 오브젝트 삭제
        Destroy(gameObject, deathDelay);
    }
    
}
