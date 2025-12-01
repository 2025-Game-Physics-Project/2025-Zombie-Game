using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 3f;
    public float currentHealth;

    public Texture2D heart;
    public Material heartMat;

    private float hpDisplay;
    private float hpTarget;

    private float shakeTime = 0f;
    public DamageFlashController flashController;  // 드래그 넣기


    void Awake()
    {
        currentHealth = maxHealth;
        hpDisplay = hpTarget = currentHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        hpTarget = currentHealth;

        StopAllCoroutines();
        StartCoroutine(DamageAnim());

        flashController.Flash();

        // 데미지 먹으면 흔들림 타이머 리셋
        shakeTime = 0.5f; // 0.5초 흔들기

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        float fill = hpDisplay / maxHealth;
        heartMat.SetFloat("_Fill", fill);

        // 흔들림 계산
        Vector2 shakeOffset = Vector2.zero;

        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            float healthState = Mathf.Ceil(hpDisplay); // 1,2,3

            float intensity = 0f;

            // HP별 흔들림 강도
            if (healthState == 2)      intensity = 2f;  // 약하게
            else if (healthState == 1) intensity = 6f;  // 강하게

            shakeOffset = new Vector2(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity)
            );
        }

        Rect r = new Rect(20 + shakeOffset.x, 20 + shakeOffset.y, 80, 80);

        Graphics.DrawTexture(r, heart, heartMat);
    }

    IEnumerator DamageAnim()
    {
        float start = hpDisplay;
        float end = hpTarget;

        float t = 0f;
        float animSpeed = 2f; // 0.5초 정도

        while (t < 1f)
        {
            hpDisplay = Mathf.Lerp(start, end, t);
            t += Time.deltaTime * animSpeed;
            yield return null;
        }

        hpDisplay = end;
    }
}
