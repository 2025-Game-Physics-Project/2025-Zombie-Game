using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GunIconAnimator : MonoBehaviour
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
        activePos = defaultPos + new Vector2(0, 20);
    }

    public void Activate()
    {
        rt.DOAnchorPos(activePos, 0.3f).SetEase(Ease.OutQuad);
        icon.DOColor(new Color(1f, 0.8f, 0f), 0.3f);
        rt.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Deactivate()
    {
        rt.DOAnchorPos(defaultPos, 0.3f).SetEase(Ease.OutQuad);
        icon.DOColor(Color.white, 0.3f);
        rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
}
