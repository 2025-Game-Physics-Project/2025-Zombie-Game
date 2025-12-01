using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashController : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.3f;  // 붉게 번쩍하는 시간
    public float maxAlpha = 0.6f;       // 최대 빨간 강도

    private Coroutine flashCoroutine;

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // 1) 처음: 빨간색 진하게
        float t = 0f;
        while (t < flashDuration)
        {
            float alpha = Mathf.Lerp(maxAlpha, 0f, t / flashDuration);
            flashImage.color = new Color(0.8f, 0f, 0f, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        // 마지막 보정
        flashImage.color = new Color(1, 0, 0, 0);
    }
}
