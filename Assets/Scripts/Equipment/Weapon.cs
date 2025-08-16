using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Weapon : MonoBehaviour
{
    public string weaponId;
    public string weaponModel;
    private int currentMuzzleIndex = 0;
    private int currentCartridgeIndex = 0;
    private int currentBackBlastIndex = 0;
    public float weight = 0f;
    public string modelNumber;
    public string weaponName;
    public string mountPosition;
    private WeaponInfo weaponInfo;
    [Header("Ammo")]
    public string ammoPrefabPath = "Assets/Cylinder.prefab";
    public float ammoDamage = 1f;
    public float ammoSize = 1f;
    public int ammoCount = 10;
    public int currentAmmoCount = 0;
    public float bulletSpeed = 50f;
    public float fireRate = 0.1f;
    private float currentFireRate = 0f;
    public bool isReload = false;
    public float reloadTime = 1f;
    public float chargeReload = 0f;
    private float currentChargeReload = 0f;
    public float currentReloadTime = 0f;
    public float gravityFactor = 0f;
    public float lifeTime = 50f;
    public int fireBurst = 0;
    public float fireBustRate = 0.1f;
    public bool semiAuto = false;
    public float boostDelay = 0f;
    public float initialVeliocity = 0f;
    public float initialGravity = 0f;
    private bool FireEnd = false;
    public int fireCount = 1;
    [Tooltip("0:gatring 1:chargeGun 2:fire delay gun")]
    public int spinUpType = 0;
    public float spinUp = 0;
    private float currentSpinUp = 0;
    private bool isSpinUp = false;
    [Tooltip("0:Good 1:Bad")]
    public float spread = 1f;
    [Tooltip("0:Bad 1:Good")]
    public float accuracy = 1f;

    [Header("Explosion")]
    public float explosion = 0f;
    public int explosionType = 0;

    [Header("Lock-On Settings")]
    public LayerMask lockOnLayer;  // ロックオンする対象のレイヤー
    public float lockOnTime = 0f;
    public float lockOnRange = 0f;
    public Vector2 lockOnSize = new Vector2(0f, 0f);
    public float homingAccuracy = 0.5f;
    public float startHoming = 0f;
    public bool allowMultiLock = false;
    public bool shootMuzzleRotate = false;

    private Camera mainCamera;
    private List<GameObject> lockedTargets = new List<GameObject>();
    private GameObject currentLockingTarget = null; // 今ロック中の敵
    private bool isLocking = false;
    private bool isLockOnActive = false;

    [Header("Recoil Settings")]
    public float recoilAmountX = 5f;   // 縦方向の反動
    public float recoilRecovery = 2f;  // 反動が戻る力
    public float recoilSpread = 1f;    // 横方向のブレ
    public float maxRecoilRadius = 10f;// 最大拡散範囲

    [Header("Cartridge")]
    public string cartridgePrefabPath = "Assets/cartridge.prefab";
    public float cartridgeSize = 1f;
    public float cartridgeSpeed = 0f;
    public float cartridgeLifeTime = 50f;
    public float cartridgeDelay = 0.1f;
    public float cartridgeRotateSpeed = 0f;
    public float cartridgeRandomness = 0f;
    public float cartridgeYAngle = 0f;
    public float cartridgeRotate = 0f;

    private Animator animator;

    private Vector3 currentRecoilOffset; // 現在の反動位置
    private Vector3 recoilVelocity;      // 反動の速度（滑らかに戻る用）

    public void WeaponStart()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        currentAmmoCount = ammoCount;
        weaponInfo = GetComponent<WeaponInfo>();
    }
    public void MountPoint(string point)
    {
        mountPosition = point;
    }
    void Update()
    {
        RangeWeapon();
    }
    void RangeWeapon()
    {
        // 反動のリカバリー処理
        currentRecoilOffset = Vector3.SmoothDamp(
            currentRecoilOffset,
            Vector3.zero,
            ref recoilVelocity,
            recoilRecovery);
        if (currentFireRate > 0 && (!FireEnd || !semiAuto))
        {
            currentFireRate -= fireRate / fireRate * Time.deltaTime;
        }
        //スピンアップタイプ
        if (currentSpinUp >= 0 && isSpinUp == false)
        {
            if (spinUpType == 0) currentSpinUp -= Time.deltaTime;
            if (spinUpType == 1) currentSpinUp = 0;

        }
        if (spinUpType == 2)
        {
            if (currentSpinUp >= spinUp)
            {
                if (isSpinUp == true) StartCoroutine(StartFire());
                currentSpinUp = 0;
                isSpinUp = false;
            }
            if (isSpinUp == true)
            {
                if (currentSpinUp <= spinUp)
                {
                    currentSpinUp += Time.deltaTime;
                }
            }

        }
        //リロード時間
        if (currentReloadTime > 0)
        {
            currentReloadTime -= Time.deltaTime;
        }
        else if (currentReloadTime <= 0 && isReload)
        {
            currentAmmoCount = ammoCount;
            isReload = false;
            //animator.SetBool("reload", false);
        }
        if (chargeReload != 0 && currentAmmoCount < ammoCount && !(FireEnd || isReload))
        {
            currentChargeReload -= Time.deltaTime;
            if (currentChargeReload <= 0)
            {
                currentAmmoCount++;
                currentChargeReload = chargeReload;
            }
        }
    }

    public IEnumerator Shoot()
    {
        
        if (currentAmmoCount <= 0) yield break;
        if (isReload) yield break;if (spinUp != 0)
        {
            if (currentSpinUp < spinUp)
            {
                if (spinUpType == 0 || spinUpType == 1)
                {
                    currentSpinUp += 1 * Time.deltaTime;
                    isSpinUp = true;
                    yield break;
                } else if (spinUpType == 1)
                {

                }
            }
            if(spinUpType == 2)
            {
                isSpinUp = true;
                yield break;
            }
        }
        if (weaponInfo.muzzlePoints.Count == 0) yield break;
        if (currentFireRate > 0) yield break;

        if (!isLockOnActive && lockOnTime != 0f)
        {
            StartLockOn();
        }
        else if (isLockOnActive)
        {
            if (!isLocking && lockedTargets.Count < currentAmmoCount)
            {
                SearchForNextTarget();
            }

            ValidateLockOnTargets();

        }
        if (lockOnTime != 0) yield break;
        if ((semiAuto || FireEnd)) yield break;
        FireEnd = true;

        if (false)
        {
            StartCoroutine(PerformSlash());
            yield break;
        }
        // 反動適用
        ApplyRecoil();
        StartCoroutine(StartFire());
    }
    //格闘
    private IEnumerator PerformSlash()
    {
        Debug.Log("格闘");

        // 攻撃モーション再生
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        // 攻撃判定タイミング（モーションの中間）
        //yield return new WaitForSeconds(0.1f);

        float attackRange = 3f;   // 剣の距離
        float attackAngle = 60f;  // 扇形判定
        //LayerMask hitLayer = LayerMask.GetMask("Enemy");

        // 扇状範囲内の敵を取得
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * (attackRange / 2),
                                                attackRange / 2);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) <= attackAngle / 2)
            {
                Unit unit = hit.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.TakeDamage(ammoDamage); // 既存の ammoDamage を流用
                }
            }
        }

        // 攻撃後のクールタイム
        yield return new WaitForSeconds(fireRate);
        FireEnd = false;
    }
    public IEnumerator StartFire()
    {
        if (fireBurst != 0)
        {
            semiAuto = true;
            for (int i = 0; i < fireBurst; i++)
            {
                Fire();
                yield return new WaitForSeconds(fireBustRate);
                if (currentAmmoCount <= 0) break;
            }
        }
        else
        {
            Fire();
        }
        EndFire();
    }
    public void EndFire()
    {
        FireEnd = false;
        currentFireRate = fireRate;
        if (currentAmmoCount <= 0 || (lockOnTime != 0 && chargeReload == 0) && lockedTargets.Count != 0)
        {
            Reload();
        }
    }


    void Fire()
    {
        Vector3 parentVelocity = Vector3.zero;
        Transform root = transform.root;
        Rigidbody parentRb = root.GetComponent<Rigidbody>();
        if (parentRb != null)
        {
            parentVelocity = parentRb.velocity;
        }
        //animator.SetFloat("fireNozzle", currentMuzzleIndex);
        //animator.SetTrigger("fire");

        // 銃口の位置から発射
        int layerMask = ~LayerMask.GetMask("Bullet", "Player");
        Transform muzzle = weaponInfo.muzzlePoints[currentMuzzleIndex];
        Transform cartridgeExit = null;
        if (weaponInfo.cartridgePoints.Count != 0) cartridgeExit = weaponInfo.cartridgePoints[currentCartridgeIndex];
        Transform backBlast = null;
        if (weaponInfo.backBrlastPoints.Count != 0) backBlast = weaponInfo.backBrlastPoints[currentBackBlastIndex];

        Vector3 direction = GetShootDirection(muzzle);
        // まず、精度 (accuracy) に応じた基本の方向を決める
        Vector3 adjustedDirection = GetAccuracyAdjustedDirection(direction, accuracy);
        for (int i = 0; i < fireCount; i++)
        {
            // その方向をベースに Spread で拡散させる
            Vector3 finalDirection = GetSpreadAdjustedDirection(adjustedDirection, spread);


            // アドレスからプレハブを非同期ロードしてインスタンス化
            Addressables.InstantiateAsync(ammoPrefabPath, muzzle.position, Quaternion.LookRotation(finalDirection)).Completed += OnBulletLoaded;
        }
        if (weaponInfo.cartridgePoints.Count != 0) StartCoroutine(InstantiateCartridge(cartridgeExit, parentVelocity));

        if (weaponInfo.backBrlastPoints.Count != 0) StartCoroutine(InstantiateBackBlast(backBlast));

        // 次の銃口に切り替え
        if (weaponInfo.muzzlePoints.Count != 0) currentMuzzleIndex = (currentMuzzleIndex + 1) % weaponInfo.muzzlePoints.Count;
        if (weaponInfo.cartridgePoints.Count != 0) currentCartridgeIndex = (currentCartridgeIndex + 1) % weaponInfo.cartridgePoints.Count;
        if (weaponInfo.backBrlastPoints.Count != 0) currentBackBlastIndex = (currentBackBlastIndex + 1) % weaponInfo.backBrlastPoints.Count;
        if (chargeReload != 0)
        {
            currentChargeReload = chargeReload;
        }
        currentAmmoCount--;
    }
    void StartLockOn()
    {
        semiAuto = true;
        isLockOnActive = true;
        isLocking = false;
        lockedTargets.Clear();
        currentLockingTarget = null;

        SearchForNextTarget();
    }

    void SearchForNextTarget()
    {
        if (isLocking || lockedTargets.Count >= currentAmmoCount) return;

        List<GameObject> potentialTargets = new List<GameObject>();
        Collider[] allTargets = Physics.OverlapSphere(transform.position, lockOnRange);

        foreach (Collider target in allTargets)
        {
            Unit unit = target.GetComponent<Unit>();
            if (target.CompareTag("Enemy") || (unit != null && unit.faction == Unit.Faction.Enemy))
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
                if (IsWithinLockOnArea(screenPos))
                {
                    potentialTargets.Add(target.gameObject);
                }
            }
        }

        // 近い順にソート
        potentialTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );

        List<GameObject> newTargets = new List<GameObject>();

        // 🔹 未ロックの敵を優先的にロックオン
        foreach (var target in potentialTargets)
        {
            if (!lockedTargets.Contains(target))
            {
                newTargets.Add(target);
                if (lockedTargets.Count + newTargets.Count >= currentAmmoCount)
                    break;
            }
        }

        // 🔹 ロックオンを開始
        foreach (var target in newTargets)
        {
            StartCoroutine(LockOnTarget(target));
        }
    }


    IEnumerator LockOnTarget(GameObject target)
    {
        if (target == null || isLocking) yield break;

        isLocking = true;
        currentLockingTarget = target;
        LockOnUI.Instance.ShowLockingIndicator(target);

        float startTime = Time.time;

        while (Time.time - startTime < lockOnTime)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
            if (!IsWithinLockOnArea(screenPos))
            {
                LockOnUI.Instance.RemoveLockIndicator(target, mountPosition); //  UI を消す
                isLocking = false;
                currentLockingTarget = null;
                yield break;
            }
            if (target == null)
            {
                yield break;
            }
            yield return null;
        }

        if (target != null)
        {
            lockedTargets.Add(target);
            LockOnUI.Instance.ConfirmLockOn(target, mountPosition);
        }
        if (isLockOnActive == false) yield break;
        isLocking = false;
        currentLockingTarget = null;
        SearchForNextTarget();
    }

    void ValidateLockOnTargets()
    {
        if (currentLockingTarget != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(currentLockingTarget.transform.position);

            if (currentLockingTarget != null && !IsWithinLockOnArea(screenPos))
            {
                LockOnUI.Instance.RemoveLockIndicator(currentLockingTarget, mountPosition);
                isLocking = false;
                currentLockingTarget = null;
            }
        }
    }

    bool IsWithinLockOnArea(Vector3 screenPos)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 halfSize = lockOnSize / 2;

        return (screenPos.x >= screenCenter.x - halfSize.x && screenPos.x <= screenCenter.x + halfSize.x) &&
               (screenPos.y >= screenCenter.y - halfSize.y && screenPos.y <= screenCenter.y + halfSize.y) &&
               screenPos.z > 0;  // 画面の前方にいることを確認
    }
    IEnumerator MissileShoot()
    {

        isLockOnActive = false;
        isLocking = false;
        if (lockOnTime != 0)
        {
            Debug.Log(lockedTargets.Count);
            for (int i = 0; i < lockedTargets.Count; i++)
            {
                FireMissile(lockedTargets[i].transform);
                if (i >= lockedTargets.Count - 1) break;
                yield return new WaitForSeconds(fireRate); // 発射間隔
                if (currentAmmoCount <= 0)
                {
                    break;
                }
            }
        }
        CancelLockOn();
        lockedTargets.Clear();
        EndFire();
    }
    void FireMissile(Transform target)
    {
        if (lockedTargets.Count == 0) return;
        Transform muzzle = weaponInfo.muzzlePoints[currentMuzzleIndex];
        Addressables.InstantiateAsync(ammoPrefabPath, muzzle.position, (shootMuzzleRotate == false) ? Quaternion.LookRotation(Camera.main.transform.forward) : muzzle.rotation)
            .Completed += (handle) =>
            {
                GameObject missileObj = handle.Result;
                Bullet missile = missileObj.AddComponent<Bullet>();
                missile.Initialize(ammoDamage, bulletSpeed, gravityFactor, lifeTime, explosion, explosionType, ammoSize, boostDelay, initialVeliocity, initialGravity, target, homingAccuracy, startHoming);
            };
        // 次の銃口に切り替え
        if (weaponInfo.muzzlePoints.Count != 0) currentMuzzleIndex = (currentMuzzleIndex + 1) % weaponInfo.muzzlePoints.Count;
        if (weaponInfo.cartridgePoints.Count != 0) currentCartridgeIndex = (currentCartridgeIndex + 1) % weaponInfo.cartridgePoints.Count;
        if (weaponInfo.backBrlastPoints.Count != 0) currentBackBlastIndex = (currentBackBlastIndex + 1) % weaponInfo.backBrlastPoints.Count;
        currentAmmoCount--;
        LockOnUI.Instance.ClearLockedIndicatorSet(mountPosition);
        if(currentLockingTarget != null)
        LockOnUI.Instance.RemoveLockIndicator(currentLockingTarget.gameObject,mountPosition);
    }
    void OnBulletLoaded(AsyncOperationHandle<GameObject> handle)
    {
        GameObject prefab = handle.Result;
        Bullet bullet = prefab.AddComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(ammoDamage, bulletSpeed, gravityFactor, lifeTime, explosion, explosionType, ammoSize, boostDelay, initialVeliocity, initialGravity, null, homingAccuracy, startHoming);
        }
    }
    public void Reload()
    {
        if (isReload || reloadTime == -1) return;
        //animator.SetBool("reload", true);
        currentReloadTime = reloadTime;
        currentAmmoCount = 0;
        isReload = true;
    }

    public void ToggleSemiAuto(bool semi)
    {
        semiAuto = semi;
    }
    public void ButtonUp()
    {
        if (isSpinUp == true && spinUpType <= 1) isSpinUp = false;
        if (isLockOnActive)
        {
            isLockOnActive = false;
            if (lockedTargets.Count == 0)
            {
                CancelLockOn();
                return;
            }
            StartCoroutine(MissileShoot());
        }
    }

    public void CancelLockOn()
    {

        isLockOnActive = false;
        isLocking = false;
        StopAllCoroutines();
        LockOnUI.Instance.ClearLockedIndicatorSet(mountPosition);

        if (currentLockingTarget != null)
            LockOnUI.Instance.RemoveLockIndicator(currentLockingTarget, mountPosition);

        lockedTargets.Clear();
        currentLockingTarget = null;
    }

    // **カメラの中心方向を発射点から向かわせる**
    Vector3 GetShootDirection(Transform muzzleTransform)
    {
        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 画面中央のレイ
        Vector3 targetPoint;

        if (Physics.Raycast(cameraRay, out RaycastHit hit, 1000f, ~LayerMask.GetMask("Bullet","Cartridge", "Player"))) // 1000ユニットまでの範囲でヒット判定
        {
            targetPoint = hit.point; // ヒットしたオブジェクトの位置をターゲットにする
            return (targetPoint - muzzleTransform.position).normalized;
        }
        return Camera.main.transform.forward; // 何もヒットしなかった場合は、現在の向きをそのまま返す
    }

    Vector3 GetAccuracyAdjustedDirection(Vector3 originalDirection, float accuracy)
    {

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 画面中央のレイを取得
        Vector3 Ray = ray.direction.normalized;
        // 精度が低いほど、ランダムに向きをズラす（accuracy = 1.0 ならズレなし）
        float maxAngleOffset = Mathf.Lerp(0f, 90f, 1f - accuracy);
        
        Quaternion accuracyRotation = Quaternion.Euler(
            Random.Range(-maxAngleOffset, maxAngleOffset),  // 上下のズレ
            Random.Range(-maxAngleOffset, maxAngleOffset),  // 左右のズレ
            0f
        );
        return accuracyRotation * originalDirection;
    }
    Vector3 GetSpreadAdjustedDirection(Vector3 baseDirection, float spread)
    {
        // コーン状に広がるように、ランダムな位置を生成
        float spreadRadius = Random.Range(0f, spread);
        float angle = Random.Range(0f, 360f); // 360°の中でランダムな方向へ

        // コーン状に拡散させるためのオフセットベクトル（ローカル座標）
        Vector3 spreadOffset = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(spreadRadius, 0f, 0f);

        // `baseDirection` を基準に拡散方向を変換（ワールド座標へ）
        Vector3 adjustedDirection = (Quaternion.LookRotation(baseDirection) * spreadOffset) + baseDirection;

        return adjustedDirection.normalized;
    }
    IEnumerator InstantiateCartridge(Transform cartridgeExit, Vector3 parentVelocity)
    {
        yield return new WaitForSeconds(cartridgeDelay);
        // アドレスからプレハブを非同期ロードしてインスタンス化// 薬莢用のアドレスをロード
        var handle = Addressables.InstantiateAsync(cartridgePrefabPath, cartridgeExit.position, cartridgeExit.rotation * Quaternion.Euler(0f, 90f + cartridgeRotate, 0f));

        // パラメータを渡せるようにデリゲートをキャプチャ
        handle.Completed += (op) => OnCartridgeLoaded(op, cartridgeExit, parentVelocity);
    }
    IEnumerator InstantiateBackBlast(Transform backBlast)
    {
        yield return new WaitForSeconds(cartridgeDelay);
        // アドレスからプレハブを非同期ロードしてインスタンス化// 薬莢用のアドレスをロード
        var handle = Addressables.InstantiateAsync(cartridgePrefabPath, backBlast.position, backBlast.rotation * Quaternion.Euler(0f, 90f + cartridgeRotate, 0f));

        // パラメータを渡せるようにデリゲートをキャプチャ
        handle.Completed += (op) => OnBackBlastLoaded(op, backBlast);
    }
    void OnCartridgeLoaded(AsyncOperationHandle<GameObject> handle, Transform cartridgeExit, Vector3 parentVelocity)
    {
        GameObject cartridgeObj = handle.Result;
        Cartridge cartridge = cartridgeObj.AddComponent<Cartridge>();

        if (cartridge != null)
        {
            cartridge.Initialize(cartridgeSpeed, cartridgeLifeTime, cartridgeExit.transform.rotation, parentVelocity, cartridgeRotateSpeed, cartridgeRandomness, cartridgeYAngle, cartridgeSize);
        }
    }
    void OnBackBlastLoaded(AsyncOperationHandle<GameObject> handle, Transform backBlast)
    {
        GameObject backBlastObj = handle.Result;
    }

    private void ApplyRecoil()
    {
        // 反動のランダムなブレ
        float randomSpreadX = Random.Range(-recoilSpread, recoilSpread);
        float randomSpreadY = Random.Range(0, recoilAmountX);

        // エイム位置のズレを考慮（ズレているときは反動を軽減）
        float aimOffsetFactor = Mathf.Clamp01(currentRecoilOffset.magnitude / maxRecoilRadius);
        float recoilMultiplier = 1 - aimOffsetFactor; // ズレが大きいほど反動が減る

        // 反動適用
        currentRecoilOffset += new Vector3(randomSpreadX, randomSpreadY, 0) * recoilMultiplier;
    }

    public Vector3 GetRecoilOffset()
    {
        return currentRecoilOffset;
    }
}
