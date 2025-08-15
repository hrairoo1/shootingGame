using System.Collections.Generic;
using UnityEngine;

public class SurfaceAligner : MonoBehaviour
{
    public float rayDistance = 2f;       // ���苗��
    public LayerMask surfaceMask;        // �Ǐ]�Ώۂ̃��C���[
    public float alignSpeed = 1000f;     // ��]�X�s�[�h�i�傫���قǑ����j
    public float stickHeight = 0.5f;

    private Rigidbody rb;
    [Header("SphereCast Settings")]
    public float sphereRadius = 0.5f;       // �L�����N�^�[���a
    public float castDistance = 1f;         // SphereCast����
    //public Vector3[] localRayDirections;           // ������O���ɃI�t�Z�b�g
    List<Vector3> localRayDirections = new List<Vector3>();


    private Vector3 avgHitPoint;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // �ǂ�V��ł������Ȃ��悤��

        int horizontalSteps = 16; // ������������
        int verticalSteps = 4;    // �������������i�����������j

        for (int h = 0; h < horizontalSteps; h++)
        {
            float horizontalAngle = (360f / horizontalSteps) * h;
            for (int v = 1; v <= verticalSteps; v++) // 1����ɂ��Đ^���������
            {
                // �����p�x��0���i�����j����90���i�^���j�܂�
                float verticalAngle = (45f / verticalSteps) * v;

                // �����x�N�g������
                Quaternion rot = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
                Vector3 dir = rot * Vector3.down; // ���[�J����Ԃŉ������

                localRayDirections.Add(dir.normalized);
            }
        }
        // �L�����N�^�[�����SphereCast���΂��I�t�Z�b�g
        // ���S�����΂����C�̕����i���[�J���j
        /*localRayDirections = new Vector3[]
        {
             // �^���E�^��
    Vector3.down,
    Vector3.up,

    
    // �^���i�����̂݁j
    Vector3.forward,
    Vector3.back,
    Vector3.left,
    Vector3.right,

    // ���΂�
    Vector3.forward + Vector3.down,
    Vector3.back + Vector3.down,
    Vector3.left + Vector3.down,
    Vector3.right + Vector3.down,
    (Vector3.forward + Vector3.left).normalized + Vector3.down,
    (Vector3.forward + Vector3.right).normalized + Vector3.down,
    (Vector3.back + Vector3.left).normalized + Vector3.down,
    (Vector3.back + Vector3.right).normalized + Vector3.down,

    // ��΂�
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
            // �ڐG��
            rb.useGravity = false;

            Vector3 avgNormal = (sumNormals / hitCount).normalized;
            avgHitPoint = sumPoints / hitCount;

            // ��]�␳�i�����ɋ߂����x�Łj
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, alignSpeed * Time.fixedDeltaTime);

            // �ڒn�ʂɋz��
            //transform.position = avgHitPoint;// + avgNormal * stickHeight;
            // �������̃��C
            Vector3 finalPos = avgHitPoint;
            if (Physics.SphereCast(transform.position + transform.up * stickHeight,0, -transform.up, out RaycastHit downHit, rayDistance , surfaceMask))
            {
                Vector3 downPos = downHit.point;

                // �����C�̈ʒu�Ɖ����C�̈ʒu���ԁi0.5f�ł��傤�ǒ��ԁj
                finalPos = downPos;
            }
            transform.position = finalPos;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            // ��
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            Quaternion targetRot = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, alignSpeed * Time.fixedDeltaTime);
        }
    }
}
