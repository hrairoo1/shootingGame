using UnityEngine;

public class HueCircle : MonoBehaviour
{
    public Transform hueMarker; // マーカー (棒状の子オブジェクト)

    public void SetHue(float hue)
    {
        // 色相に応じてマーカーの回転角度を変更
        float angle = hue * 360f;
        hueMarker.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
