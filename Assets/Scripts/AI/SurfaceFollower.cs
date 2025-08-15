using System.Collections.Generic;
using UnityEngine;

public class SurfaceAligner : MonoBehaviour
{
    public float rayDistance = 2f;       // 判定距離
    public LayerMask surfaceMask;        // 追従対象のレイヤー
    public float alignSpeed = 1000f;     // 回転スピード（大きいほど即時）
    public float stickHeight = 0.5f;

    private Rigidbody rb;
    [Header("SphereCast Settings")]
    public float sphereRadius = 0.5f;       // キャラクター半径
    public float castDistance = 1f;         // SphereCast距離
    //public Vector3[] localRayDirections;           // 足元や前方にオフセット
    List<Vector3> localRayDirections = new List<Vector3>();


    private Vector3 avgHitPoint;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // 壁や天井でも落ちないように

        int horizontalSteps = 16; // 水平方向分割
        int verticalSteps = 4;    // 垂直方向分割（下半分だけ）

        for (int h = 0; h < horizontalSteps; h++)
        {
            float horizontalAngle = (360f / horizontalSteps) * h;
            for (int v = 1; v <= verticalSteps; v++) // 1からにして真横を避ける
            {
                // 垂直角度は0°（水平）から90°（真下）まで
                float verticalAngle = (45f / verticalSteps) * v;

                // 方向ベクトル生成
                Quaternion rot = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
                Vector3 dir = rot * Vector3.down; // ローカル空間で下方向基準

                localRayDirections.Add(dir.normalized);
            }
        }
        // キャラクター周りにSphereCastを飛ばすオフセット
        // 中心から飛ばすレイの方向（ローカル）
        /*localRayDirections = new Vector3[]
        {
             // 真下・真上
    Vector3.down,
    Vector3.up,

    
    // 真横（水平のみ）
    Vector3.forward,
    Vector3.back,
    Vector3.left,
    Vector3.right,

    // 下斜め
    Vector3.forward + Vector3.down,
    Vector3.back + Vector3.down,
    Vector3.left + Vector3.down,
    Vector3.right + Vector3.down,
    (Vector3.forward + Vector3.left).normalized + Vector3.down,
    (Vector3.forward + Vector3.right).normalized + Vector3.down,
    (Vector3.back + Vector3.left).normalized + Vector3.down,
    (Vector3.back + Vector3.right).normalized + Vector3.down,

    // 上斜め
    Vector3.forward + Vector3.up,
    Vector3.back + Vector3.up,
    Vector3.left + Vector3.up,
    Vector3.right + Vector3.up,
    (Vector3.forward + Vector3.left).normalized + Vector3.up,
    (Vector3.forward + Vector3.right).normalized + Vector3.up,
    (Vector3.back + Vector3.left).normalized + Vector3.up,
    (Vector3.back + Vector3.right).normalized + Vector3.up
        };*/
    }

    void FixedUpdate()
    {
        AlignToSurface();
        rb.angularVelocity = Vector3.zero;
    }
    void OnDrawGizmos()
    {
        if (localRayDirections == null) return;
        
        Gizmos.color = Color.green;
        foreach (var localDir in localRayDirections)
        {
            Vector3 worldDir = transform.TransformDirection(localDir.normalized);
            Gizmos.DrawRay(transform.position + transform.up * stickHeight, worldDir * rayDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(avgHitPoint, 0.1f);

    }

    void AlignToSurface()
    {

        Vector3 sumNormals = Vector3.zero;
        Vector3 sumPoints = Vector3.zero;
        int hitCount = 0;

        foreach (var localDir in localRayDirections)
        {
            Vector3 worldDir = transform.TransformDirection(localDir.normalized);
            if (Physics.Raycast(transform.position + transform.up * stickHeight, worldDir, out RaycastHit hit, rayDistance, surfaceMask))
            {
                sumNormals += hit.normal;
                sumPoints += hit.point;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            // 接触中
            rb.useGravity = false;

            Vector3 avgNormal = (sumNormals / hitCount).normalized;
            avgHitPoint = sumPoints / hitCount;

            // 回転補正（即時に近い速度で）
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, alignSpeed * Time.fixedDeltaTime);

            // 接地面に吸着
            //transform.position = avgHitPoint;// + avgNormal * stickHeight;
            // 下方向のレイ
            Vector3 finalPos = avgHitPoint;
            if (Physics.SphereCast(transform.position + transform.up * stickHeight,0, -transform.up, out RaycastHit downHit, rayDistance , surfaceMask))
            {
                Vector3 downPos = downHit.point;

                // 横レイの位置と下レイの位置を補間（0.5fでちょうど中間）
                finalPos = downPos;
            }
            transform.position = finalPos;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            // 空中
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            Quaternion targetRot = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, alignSpeed * Time.fixedDeltaTime);
        }
    }
}
