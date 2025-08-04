using UnityEngine;
using UnityEngine.UI;

public class SaturationBrightnessBox : MonoBehaviour
{
    public RectTransform satBrightMarker; // 明度・彩度のマーカー
    public Image satBrightIndicator;      // 背景の色を変えるイメージ

    public void SetSaturationBrightness(float hue, float saturation, float brightness)
    {
        satBrightIndicator.color = Color.HSVToRGB(hue, 1, 1);

        float x = saturation * satBrightIndicator.rectTransform.rect.width;
        float y = brightness * satBrightIndicator.rectTransform.rect.height;

        satBrightMarker.anchoredPosition = new Vector2(x, y);
    }
}
