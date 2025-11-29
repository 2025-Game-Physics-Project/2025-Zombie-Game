using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestIconAnimator : MonoBehaviour
{
    public Image icon;

    RectTransform rt;
    Vector2 defaultPos;
    Vector2 activePos;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (icon == null) icon = GetComponent<Image>();

        defaultPos = rt.anchoredPosition;
        activePos = defaultPos + new Vector2(0, 20);   // 위로 20px 이동
    }

    void Start()
    {
        // 1초 뒤에 활성화 연출
        Invoke(nameof(Activate), 1f);

        // 3초 뒤에 비활성화 연출
        Invoke(nameof(Deactivate), 3f);
    }

    public void Activate()
    {
        rt.DOAnchorPos(activePos, 0.3f).SetEase(Ease.OutQuad);
        icon.DOColor(new Color(1f, 0.8f, 0f), 0.3f);  // 노란빛
        rt.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Deactivate()
    {
        rt.DOAnchorPos(defaultPos, 0.3f).SetEase(Ease.OutQuad);
        icon.DOColor(Color.white, 0.3f);
        rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
}
