using UnityEngine;
using UnityEngine.UI;

public class SaturationBrightnessBox : MonoBehaviour
{
    public RectTransform satBrightMarker; // ���x�E�ʓx�̃}�[�J�[
    public Image satBrightIndicator;      // �w�i�̐F��ς���C���[�W

    public void SetSaturationBrightness(float hue, float saturation, float brightness)
    {
        satBrightIndicator.color = Color.HSVToRGB(hue, 1, 1);

        float x = saturation * satBrightIndicator.rectTransform.rect.width;
        float y = brightness * satBrightIndicator.rectTransform.rect.height;

        satBrightMarker.anchoredPosition = new Vector2(x, y);
    }
}
