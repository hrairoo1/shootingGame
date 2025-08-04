
using UnityEngine;
using UnityEngine.EventSystems;

public class Cartridge : MonoBehaviour
{
    private float speed = 50f;  // 初速
    private float lifeTime = 5f; // 寿命
    private Vector3 velocity; // 速度を独自に管理
    private Rigidbody rb; // Rigidbodyコンポーネント
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
        // X軸回転（0度のとき）とY軸回転（90度のとき）をブレンド
        Vector3 xRotation = Vector3.up; // X回転 (縦回転)
        Vector3 yRotation = Vector3.forward;    // Y回転 (横回転)
                                                // -180 ~ 180 を -1 ~ 1 の範囲に正規化
        float angleRad = Mathf.Deg2Rad * cartridgeYangle;

        // X軸成分とY軸成分を trigonometric 関数で補間
        Vector3 rotateAxis = rotate * (xRotation * Mathf.Cos(angleRad) + yRotation * Mathf.Sin(angleRad)).normalized;


        // ランダム回転速度を適用
        float randomRotateSpeed = rotateSpeed;
        if (rotateRandomness != 0)
        {
            randomRotateSpeed += Random.Range(-rotateRandomness, rotateRandomness);
        }

        // ボールに回転を与える
        rb.angularVelocity = rotateAxis * randomRotateSpeed;

        if (rotateRandomness != 0)
        {
            //回転のランダム強さ
            randomRotateSpeed = rotateSpeed + Random.Range(-rotateRandomness, rotateRandomness);
        }

        

        // ボールに回転を与える
        rb.angularVelocity = rotateAxis * randomRotateSpeed;

        Quaternion cartridgeAccuracy = GetAccuracyAdjustedDirection(0.7f);
        velocity = ((rotate * cartridgeAccuracy) * Vector3.forward * speed * randomSpeed ) + parentVelocity; // 初速度を設定
        rb.velocity = velocity;
        Destroy(gameObject, GameSettings.Instance.cartridgeLifeTime);
    }
    Quaternion GetAccuracyAdjustedDirection(float accuracy)
    {
        // 精度が低いほど、ランダムに向きをズラす（accuracy = 1.0 ならズレなし）
        float maxAngleOffset = Mathf.Lerp(10f, 0f, accuracy);

        Quaternion accuracyRotation = Quaternion.Euler(
            Random.Range(-maxAngleOffset, maxAngleOffset),  // 上下のズレ
            Random.Range(-maxAngleOffset, maxAngleOffset),  // 左右のズレ
            0f
        );

        return accuracyRotation;
    }
}
