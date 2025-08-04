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
    private float speed = 50f;  // 初速
    private float gravityFactor = 1f; // 落下速度の調整
    private float lifeTime = 5f; // 寿命
    private float boostDelay = 0f;
    private float currentBoostDelay = 0f;
    private float initialVeliocity = 0f;
    private float initialGravity = 0f;
    private float explosion = 0f;
    private int explosionType = 0;
    private Vector3 velocity; // 速度を独自に管理
    private string explosiveRadiusPath = "Assets/ExplosionRadius.prefab";
    private float startHoming = 0f;
    private float currentStartHoming = 0f;
    private float radius = 0.1f; // SphereCastの半径
    private LayerMask layerMask; // レイヤーマスク追加 // レイヤーマスク追加
    private Transform target; // 誘導対象
    private float turnSpeed = 5f; // 旋回速度
    private Rigidbody rb;
    private bool useTransformMovement = false;
    private BurretInfo burretInfo;

    private Vector3 moveDirection;
    private float moveDistance;

    void Awake()
    {
        layerMask = ~LayerMask.GetMask("Bullet"); // Awake で設定
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

        velocity = transform.forward * speed; // 初速度を設定
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // 重力オフ

        if (boostDelay != 0)
        {
            StartCoroutine(ApplyCustomGravity());
            rb.useGravity = true;
            // 一瞬だけ力を加える
            rb.AddForce(transform.forward * initialVeliocity, ForceMode.Impulse);
        }
        else
        {
            useTransformMovement = true; // Transform移動に切り替え
            rb.isKinematic = true;  // 物理演算を停止
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
            // Rigidbodyでの移動中の判定
            moveDirection = rb.velocity.normalized;
            moveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        }
        else
        {
            // Transformでの移動中の判定
            moveDirection = velocity.normalized;
            moveDistance = velocity.magnitude * Time.fixedDeltaTime;
        }
    }
    private void FixedUpdate()
    {
        RaycastHit hit;

        // **切り替え前のSphereCast判定**
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

        // **誘導開始の判定**
        if (currentStartHoming < startHoming)
        {
            currentStartHoming += Time.deltaTime;
        }

        // **ブースト遅延中は移動処理をスキップ**
        if (currentBoostDelay < boostDelay)
        {
            currentBoostDelay += Time.deltaTime;
            return;
        }

        // **ここで SphereCast をもう一度実行 (移動方法切り替え前)**
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

        // **Transform での移動に切り替え後の処理**
        moveDirection = velocity.normalized;
        moveDistance = velocity.magnitude * Time.fixedDeltaTime;

        if (Physics.SphereCast(transform.position, radius, moveDirection, out hit, moveDistance, layerMask))
        {
            HandleCollision(hit);
            return;
        }

        // **誘導処理**
        if (target && currentStartHoming >= startHoming)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            velocity = Vector3.RotateTowards(velocity, targetDirection * speed, turnSpeed * Time.fixedDeltaTime, float.MaxValue);

        }
        else
        {
            velocity += Vector3.down * (9.81f * gravityFactor * Time.fixedDeltaTime);
        }

        // **移動と向きの更新**
        transform.position += velocity * Time.fixedDeltaTime;
        transform.forward = velocity.normalized;
    }


    private void HandleCollision(RaycastHit hit)//命中判定
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
        // **衝突したオブジェクトがダメージを受けられるか確認**
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
