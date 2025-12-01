using UnityEngine;

public class HeartSliceRenderer
{
    Texture2D source;
    Texture2D mask;
    int size;

    public HeartSliceRenderer(Texture2D tex)
    {
        source = tex;
        size = tex.width; // 정사각형 가정
        mask = new Texture2D(size, size, TextureFormat.ARGB32, false);
    }

    // fill: 0~1, 원의 몇 퍼센트를 보여줄지
    public Texture2D GetSlice(float fill)
    {
        float angleLimit = fill * 360f; // 0~360도

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // 중심 기준 각도 계산
                float dx = x - size * 0.5f;
                float dy = y - size * 0.5f;
                float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;

                // 각도가 보여줄 범위 안이면 픽셀을 그대로
                if (angle <= angleLimit)
                {
                    mask.SetPixel(x, y, source.GetPixel(x, y));
                }
                else
                {
                    mask.SetPixel(x, y, new Color(0,0,0,0)); // 투명
                }
            }
        }

        mask.Apply();
        return mask;
    }
}
