using Game.Interfaces;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Bullet : MonoBehaviour
{
    private float ammoDamage = 10f;
    private float speed = 50f;  // ����
    private float gravityFactor = 1f; // �������x�̒���
    private float lifeTime = 5f; // ����
    private float boostDelay = 0f;
    private float currentBoostDelay = 0f;
    private float initialVeliocity = 0f;
    private float initialGravity = 0f;
    private float explosion = 0f;
    private int explosionType = 0;
    private Vector3 velocity; // ���x��Ǝ��ɊǗ�
    private string explosiveRadiusPath = "Assets/ExplosionRadius.prefab";
    private float startHoming = 0f;
    private float currentStartHoming = 0f;
    private float radius = 0.1f; // SphereCast�̔��a
    private LayerMask layerMask; // ���C���[�}�X�N�ǉ� // ���C���[�}�X�N�ǉ�
    private Transform target; // �U���Ώ�
    private float turnSpeed = 5f; // ���񑬓x
    private Rigidbody rb;
    private bool useTransformMovement = false;
    private BurretInfo burretInfo;

    private Vector3 moveDirection;
    private float moveDistance;

    void Awake()
    {
        layerMask = ~LayerMask.GetMask("Bullet"); // Awake �Őݒ�
        burretInfo = GetComponent<BurretInfo>();

    }

    public void Initialize(float damage, float bulletSpeed, float gravityMultiplier, float lifetime, float explosionSize, int Type, float ammoSize, float boost, float IV, float IG, Transform newTarget, float newTurnSpeed, float shoming)
    {
        transform.localScale = new Vector3(ammoSize, ammoSize, ammoSize);
        ammoDamage = damage;
        speed = bulletSpeed;
        gravityFactor = gravityMultiplier;
        lifeTime = lifetime;
        explosion = explosionSize;
        explosionType = Type;
        boostDelay = boost;
        initialVeliocity = IV;
        initialGravity = IG;
        target = newTarget;
        turnSpeed = newTurnSpeed;
        startHoming = shoming;
        foreach (Transform child in burretInfo.effect)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startSize = main.startSize.constant * ammoSize;
        }

        velocity = transform.forward * speed; // �����x��ݒ�
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // �d�̓I�t

        if (boostDelay != 0)
        {
            StartCoroutine(ApplyCustomGravity());
            rb.useGravity = true;
            // ��u�����͂�������
            rb.AddForce(transform.forward * initialVeliocity, ForceMode.Impulse);
        }
        else
        {
            useTransformMovement = true; // Transform�ړ��ɐ؂�ւ�
            rb.isKinematic = true;  // �������Z���~
        }

            Destroy(gameObject, lifeTime);
    }
    IEnumerator ApplyCustomGravity()
    {
        while (!useTransformMovement)
        {
            rb.AddForce(Physics.gravity * initialGravity, ForceMode.Acceleration);
            yield return null;
        }
    }

    void Update()
    {
        if (!useTransformMovement)
        {
            // Rigidbody�ł̈ړ����̔���
            moveDirection = rb.velocity.normalized;
            moveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        }
        else
        {
            // Transform�ł̈ړ����̔���
            moveDirection = velocity.normalized;
            moveDistance = velocity.magnitude * Time.fixedDeltaTime;
        }
    }
    private void FixedUpdate()
    {
        RaycastHit hit;

        // **�؂�ւ��O��SphereCast����**
        if (!useTransformMovement)
        {
            moveDirection = rb.velocity.normalized;
            moveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;

            if (Physics.SphereCast(transform.position, radius, moveDirection, out hit, moveDistance, layerMask))
            {
                HandleCollision(hit);
                return;
            }
        }

        // **�U���J�n�̔���**
        if (currentStartHoming < startHoming)
        {
            currentStartHoming += Time.deltaTime;
        }

        // **�u�[�X�g�x�����͈ړ��������X�L�b�v**
        if (currentBoostDelay < boostDelay)
        {
            currentBoostDelay += Time.deltaTime;
            return;
        }

        // **������ SphereCast ��������x���s (�ړ����@�؂�ւ��O)**
        if (!useTransformMovement)
        {
            if (Physics.SphereCast(transform.position, radius, moveDirection, out hit, moveDistance, layerMask))
            {
                HandleCollision(hit);
                return;
            }

            useTransformMovement = true;
            rb.isKinematic = true;
        }

        // **Transform �ł̈ړ��ɐ؂�ւ���̏���**
        moveDirection = velocity.normalized;
        moveDistance = velocity.magnitude * Time.fixedDeltaTime;

        if (Physics.SphereCast(transform.position, radius, moveDirection, out hit, moveDistance, layerMask))
        {
            HandleCollision(hit);
            return;
        }

        // **�U������**
        if (target && currentStartHoming >= startHoming)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            velocity = Vector3.RotateTowards(velocity, targetDirection * speed, turnSpeed * Time.fixedDeltaTime, float.MaxValue);

        }
        else
        {
            velocity += Vector3.down * (9.81f * gravityFactor * Time.fixedDeltaTime);
        }

        // **�ړ��ƌ����̍X�V**
        transform.position += velocity * Time.fixedDeltaTime;
        transform.forward = velocity.normalized;
    }


    private void HandleCollision(RaycastHit hit)//��������
    {
        foreach( Transform child in burretInfo.effect)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            ps.Emit(1);
            var emission = ps.emission;
            emission.enabled = false;
            var main = ps.main;
            main.loop = false;
            child.transform.parent = null;
            child.transform.position = hit.point;
            var trails = ps.trails;
            trails.attachRibbonsToTransform = false;
        }
        if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(ammoDamage);
        }
        if (explosion > 0f)
        {
            Addressables.InstantiateAsync(explosiveRadiusPath, hit.point, Quaternion.identity).Completed += OnExplosiveRadiusLoaded;

            GameObject explosiveEffect = Instantiate(GameSettings.Instance.Explosive1, hit.point, Quaternion.identity);
            explosiveEffect.transform.localScale = new Vector3(explosion, explosion, explosion);
        }
        Destroy(gameObject);
    }

    void OnExplosiveRadiusLoaded(AsyncOperationHandle<GameObject> handle)
    {
        GameObject prefab = handle.Result;
        ExplosiveRadius explosiveRadius = prefab.AddComponent<ExplosiveRadius>();

        if (explosiveRadius != null)
        {
            explosiveRadius.Initialize(ammoDamage, explosion);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        foreach (Transform child in burretInfo.effect)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>(); 
            ps.Emit(1);
            var emission = ps.emission;
            emission.enabled = false;
            var main = ps.main;
            main.loop = false;
            child.transform.parent = null;
            var trails = ps.trails;
            trails.attachRibbonsToTransform = false;
        }
        // **�Փ˂����I�u�W�F�N�g���_���[�W���󂯂��邩�m�F**
        if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(ammoDamage);
        }

        if (explosion > 0f)
        {
            Addressables.InstantiateAsync(explosiveRadiusPath, transform.position, Quaternion.identity).Completed += OnExplosiveRadiusLoaded;
            GameObject explosiveEffect = Instantiate(GameSettings.Instance.Explosive1, transform.position, Quaternion.identity);
            explosiveEffect.transform.localScale = new Vector3(explosion, explosion, explosion);
        }
        Destroy(gameObject); ;
    }
    
}
