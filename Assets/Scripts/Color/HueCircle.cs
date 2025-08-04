using UnityEngine;

public class HueCircle : MonoBehaviour
{
    public Transform hueMarker; // �}�[�J�[ (�_��̎q�I�u�W�F�N�g)

    public void SetHue(float hue)
    {
        // �F���ɉ����ă}�[�J�[�̉�]�p�x��ύX
        float angle = hue * 360f;
        hueMarker.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
