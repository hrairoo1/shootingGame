
using UnityEngine;
using UnityEngine.EventSystems;

public class Cartridge : MonoBehaviour
{
    private float speed = 50f;  // ����
    private float lifeTime = 5f; // ����
    private Vector3 velocity; // ���x��Ǝ��ɊǗ�
    private Rigidbody rb; // Rigidbody�R���|�[�l���g
    private float rotateSpeed = 0f;
    private float rotateRandomness = 0f;

    public void Initialize(float cartridgeSpeed, float lifetime, Quaternion rotate, Vector3 parentVelocity, float cartridgeRotateSpeed, float cartridgeRandomness, float cartridgeYangle, float cartridgeSize)
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = new Vector3(cartridgeSize, cartridgeSize, cartridgeSize);
        float randomSpeed = Random.Range(0.9f, 1.1f);
        speed = cartridgeSpeed;
        lifeTime = lifetime;
        rotateSpeed = cartridgeRotateSpeed;
        rotateRandomness = cartridgeRandomness;
        // X����]�i0�x�̂Ƃ��j��Y����]�i90�x�̂Ƃ��j���u�����h
        Vector3 xRotation = Vector3.up; // X��] (�c��])
        Vector3 yRotation = Vector3.forward;    // Y��] (����])
                                                // -180 ~ 180 �� -1 ~ 1 �͈̔͂ɐ��K��
        float angleRad = Mathf.Deg2Rad * cartridgeYangle;

        // X��������Y�������� trigonometric �֐��ŕ��
        Vector3 rotateAxis = rotate * (xRotation * Mathf.Cos(angleRad) + yRotation * Mathf.Sin(angleRad)).normalized;


        // �����_����]���x��K�p
        float randomRotateSpeed = rotateSpeed;
        if (rotateRandomness != 0)
        {
            randomRotateSpeed += Random.Range(-rotateRandomness, rotateRandomness);
        }

        // �{�[���ɉ�]��^����
        rb.angularVelocity = rotateAxis * randomRotateSpeed;

        if (rotateRandomness != 0)
        {
            //��]�̃����_������
            randomRotateSpeed = rotateSpeed + Random.Range(-rotateRandomness, rotateRandomness);
        }

        

        // �{�[���ɉ�]��^����
        rb.angularVelocity = rotateAxis * randomRotateSpeed;

        Quaternion cartridgeAccuracy = GetAccuracyAdjustedDirection(0.7f);
        velocity = ((rotate * cartridgeAccuracy) * Vector3.forward * speed * randomSpeed ) + parentVelocity; // �����x��ݒ�
        rb.velocity = velocity;
        Destroy(gameObject, GameSettings.Instance.cartridgeLifeTime);
    }
    Quaternion GetAccuracyAdjustedDirection(float accuracy)
    {
        // ���x���Ⴂ�قǁA�����_���Ɍ������Y�����iaccuracy = 1.0 �Ȃ�Y���Ȃ��j
        float maxAngleOffset = Mathf.Lerp(10f, 0f, accuracy);

        Quaternion accuracyRotation = Quaternion.Euler(
            Random.Range(-maxAngleOffset, maxAngleOffset),  // �㉺�̃Y��
            Random.Range(-maxAngleOffset, maxAngleOffset),  // ���E�̃Y��
            0f
        );

        return accuracyRotation;
    }
}
