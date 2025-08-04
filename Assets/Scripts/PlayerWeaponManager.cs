using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.GraphicsBuffer;

public class PlayerWeaponManager : MonoBehaviour
{
    public Dictionary<string, WeaponSlot> weaponSlots = new Dictionary<string, WeaponSlot>();
    public PlayerWeaponManager()
    {
        // 初期化で事前定義したキーを使って weaponSlots にデータを追加
        weaponSlots["MainR"] = new WeaponSlot();
        weaponSlots["MainL"] = new WeaponSlot();
        weaponSlots["SubR"] = new WeaponSlot();
        weaponSlots["SubL"] = new WeaponSlot();
        weaponSlots["ShoulderR"] = new WeaponSlot();
        weaponSlots["ShoulderL"] = new WeaponSlot();
    }
    //private bool isUsingSubWeapons = false; // サブ武器使用中か
    public PlayerMovement playerMovement;

    private bool isUsingSubWeaponsR = false;
    private bool isUsingSubWeaponsL = false;
    private bool _isWeaponChange = true;
    public bool isWeaponChange
    {
        get { return _isWeaponChange; }
        set
        {
            if (_isWeaponChange != value) // 値が変化した場合のみ
            {
                _isWeaponChange = value;
            }
        }
    }

    public class WeaponSlot
    {
        public Transform mountPoint; // 武器を装備する位置 (ボーンなど)
        public Weapon equippedWeapon; // 装備されている武器
        public string weaponModelPath; // 武器のプレハブのアドレス
        public GameObject weaponModelInstance; // 実際に生成された武器のモデル
    }

    void Awake()
    {
        // 各スロットをセット (マウントポイントの検索 + 武器の取得)
        SetWeaponSlot("MainR");
        SetWeaponSlot("MainL");
        SetWeaponSlot("SubR");
        SetWeaponSlot("SubL");
        SetWeaponSlot("ShoulderR");
        SetWeaponSlot("ShoulderL");

        // 初期状態の武器表示を設定
        UpdateWeaponVisibility("MainR", true);
        //playerMovement.ChangeMass();
    }
    // 指定されたスロット名で武器スロットをセット
    public void SetWeaponSlot(string slotName)
    {
        Transform mountPoint = transform.Find($"WeaponHolder/{slotName}");
        if (mountPoint == null)
        {
            Debug.LogError($"スロット {slotName} が見つかりません！");
            return;
        }
        foreach(Transform child in mountPoint.transform)
        {
            Destroy(child.gameObject);
        }
        // 武器スロットを作成
        weaponSlots[slotName] = new WeaponSlot { mountPoint = mountPoint };
        // 既に存在する武器をセット
        Transform weaponTransform = FindChildInSlot(mountPoint);
        if (weaponTransform == null)
        {
            if (LoadoutManager.Instance.weapon[slotName] == null) return;
            if (LoadoutManager.Instance.weapon[slotName].weaponModel == null) return;
            Debug.Log(LoadoutManager.Instance.weapon[slotName].weaponName);
            Debug.Log(LoadoutManager.Instance.weapon[slotName].weaponModel);
            Addressables.InstantiateAsync(LoadoutManager.Instance.weapon[slotName].weaponModel, mountPoint.position, mountPoint.rotation)
            .Completed += (handle) =>
            {
                GameObject weaponObj = handle.Result;
                weaponObj.transform.SetParent(mountPoint);

                Weapon weaponComponent = weaponObj.GetComponent<Weapon>();
                if (weaponComponent != null)
                {
                    weaponSlots[slotName].equippedWeapon = weaponComponent;
                    weaponSlots[slotName].weaponModelPath = weaponComponent.weaponModel;
                    weaponSlots[slotName].weaponModelInstance = weaponObj;
                    weaponComponent.MountPoint(slotName);
                }
                else
                {
                    weaponComponent = weaponObj.AddComponent<Weapon>();
                    SetWeaponData(weaponComponent, slotName);
                    weaponSlots[slotName].equippedWeapon = weaponComponent;
                    weaponSlots[slotName].weaponModelPath = weaponComponent.weaponModel;
                    weaponSlots[slotName].weaponModelInstance = weaponObj;
                    weaponComponent.MountPoint(slotName);
                }
                weaponObj.transform.position = mountPoint.transform.position;
                weaponObj.transform.rotation = mountPoint.transform.rotation;
                weaponComponent.WeaponStart();

                UpdateWeaponVisibility(slotName, true);
                Material mat = weaponObj.GetComponentInChildren<Renderer>().material;
                if (mat != null)
                {
                    // 現在のプロパティ情報を取得
                    mat.SetColor("_MainColor1", LoadoutManager.Instance.equipColor[slotName]._MainColor1);
                    mat.SetColor("_MainColorEmi1", LoadoutManager.Instance.equipColor[slotName]._MainColor1);
                    mat.SetFloat("_MainSt1", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity1);
                    mat.SetColor("_MainColor2", LoadoutManager.Instance.equipColor[slotName]._MainColor2);
                    mat.SetColor("_MainColorEmi2", LoadoutManager.Instance.equipColor[slotName]._MainColor2);
                    mat.SetFloat("_MainSt2", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity2);
                    mat.SetColor("_MainColor3", LoadoutManager.Instance.equipColor[slotName]._MainColor3);
                    mat.SetColor("_MainColorEmi3", LoadoutManager.Instance.equipColor[slotName]._MainColor3);
                    mat.SetFloat("_MainSt3", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity3);
                    mat.SetColor("_SubColor1", LoadoutManager.Instance.equipColor[slotName]._SubColor1);
                    mat.SetColor("_SubColorEmi1", LoadoutManager.Instance.equipColor[slotName]._SubColor1);
                    mat.SetFloat("_SubSt1", LoadoutManager.Instance.equipIntencity[slotName].subIntencity1);
                    mat.SetColor("_SubColor2", LoadoutManager.Instance.equipColor[slotName]._SubColor2);
                    mat.SetColor("_SubColorEmi2", LoadoutManager.Instance.equipColor[slotName]._SubColor2);
                    mat.SetFloat("_SubSt2", LoadoutManager.Instance.equipIntencity[slotName].subIntencity2);
                    mat.SetColor("_EmiColor1", LoadoutManager.Instance.equipColor[slotName]._EmiColor1);
                    mat.SetFloat("_EmiSt1", LoadoutManager.Instance.equipIntencity[slotName].emiIntencity1);
                    mat.SetColor("_EmiColor2", LoadoutManager.Instance.equipColor[slotName]._EmiColor2);
                    mat.SetFloat("_EmiSt2", LoadoutManager.Instance.equipIntencity[slotName].emiIntencity2);
                }
            };

        }
    }

    // マウントポイントの直下にある最初の子オブジェクトを探す
    private Transform FindChildInSlot(Transform slotTransform)
    {
        return slotTransform.childCount > 0 ? slotTransform.GetChild(0) : null;
    }

    // 武器の表示を更新
    private void UpdateWeaponVisibility(string slotName, bool isMain)
    {
        Dictionary<string, string> weaponPairs = new Dictionary<string, string>
        {
            { "MainR", "SubR" },
            { "MainL", "SubL" },
            { "SubR", "MainR" },
            { "SubL", "MainL" }
        };
        string pairSlot = weaponPairs[slotName];
        //if (weaponSlots[pairSlot].equippedWeapon != null)
        //{
        if (isMain == true)
        {
            if (weaponSlots["MainR"].weaponModelInstance != null)
            {
                weaponSlots["MainL"].weaponModelInstance.SetActive(true);
            }
            if (weaponSlots["MainL"].weaponModelInstance != null)
            {
                weaponSlots["MainL"].weaponModelInstance.SetActive(true);
            }
            if (weaponSlots["SubR"].weaponModelInstance != null)
            {
                weaponSlots["SubR"].weaponModelInstance.SetActive(false);
            }
            if (weaponSlots["SubL"].weaponModelInstance != null)
            {
                weaponSlots["SubL"].weaponModelInstance.SetActive(false);
            }
        }
        else
        {
            if (slotName == "MainR" || slotName == "SubR") isUsingSubWeaponsR = !isUsingSubWeaponsR;
            if (slotName == "MainL" || slotName == "SubL") isUsingSubWeaponsL = !isUsingSubWeaponsL;
            // 現在の状態を反転させる（片方ONなら片方OFF）
            bool isSlotActive = (slotName == "MainR" || slotName == "SubR") ? isUsingSubWeaponsR : isUsingSubWeaponsL;
            bool newActiveState = !isSlotActive;

            if (weaponSlots[slotName].weaponModelInstance != null)
            {
                weaponSlots[slotName].weaponModelInstance.SetActive(newActiveState);
            }
            if (weaponSlots[pairSlot].weaponModelInstance != null)
            {
                weaponSlots[pairSlot].weaponModelInstance.SetActive(!newActiveState);
            }
        }
        //}

        _isWeaponChange = true;
    }

    // サブ武器の切り替え
    public void ToggleSubWeapons(string slotName)
    {
        Weapon Weapon = GetWeapon(slotName);
        if(Weapon != null)
        {
            Weapon.CancelLockOn();
        }
        UpdateWeaponVisibility(slotName, false);
    }


    // 現在使用しているメイン武器 or サブ武器を取得
    public Weapon GetWeapon(string slotName)
    {
        Dictionary<string, string> weaponPairs = new Dictionary<string, string>
        {
            { "MainR", "SubR" },
            { "MainL", "SubL" },
            { "SubR", "MainR" },
            { "SubL", "MainL" }
        };
        if (weaponPairs.ContainsKey(slotName))
        {
            bool isUsingSubWeapons = slotName == "MainR" || slotName == "SubR" ? isUsingSubWeaponsR : isUsingSubWeaponsL;
            string activeSlot = isUsingSubWeapons ? weaponPairs[slotName] : slotName;
            return weaponSlots.ContainsKey(activeSlot) ? weaponSlots[activeSlot].equippedWeapon : null;
        }
        return null;
    }
    //パージ
    public void PurgeWeaponSlot(string slotName)
    {
        Weapon weapon = null;
        if (slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            weapon = weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            weapon = GetWeapon(slotName);
        }
        if (weapon == null)
        {
            Debug.LogError($"スロット {slotName} が見つかりません！");
            return;
        }
        Vector3 worldPosition = transform.position;
        Quaternion worldRotation = transform.rotation;

        weapon.transform.SetParent(null);

        transform.position = worldPosition;
        transform.rotation = worldRotation;
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        BoxCollider boxCollider = weapon.GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            boxCollider.enabled = true;  // BoxCollider を有効化
        }
        // 武器スロットから削除
        foreach (var slot in weaponSlots)
        {
            if (slot.Value.equippedWeapon == weapon)
            {
                slot.Value.equippedWeapon = null;
                slot.Value.weaponModelInstance = null;
                slot.Value.weaponModelPath = null;
                Debug.Log($"スロット {slot.Key} から武器 {weapon.name} をパージしました。");
                return;
            }
        }
    }
    void SetWeaponData(Weapon weapon, string slotName)
    {
        weapon.weaponId = LoadoutManager.Instance.weapon[slotName].weaponId;
        weapon.weaponModel = LoadoutManager.Instance.weapon[slotName].weaponModel;
        weapon.weight = LoadoutManager.Instance.weapon[slotName].weight;
        weapon.modelNumber = LoadoutManager.Instance.weapon[slotName].modelNumber;
        weapon.weaponName = LoadoutManager.Instance.weapon[slotName].weaponName;
        weapon.ammoPrefabPath = LoadoutManager.Instance.weapon[slotName].ammo.ammoPrefabPath;
        weapon.ammoDamage = LoadoutManager.Instance.weapon[slotName].ammo.ammoDamage;
        weapon.ammoSize = LoadoutManager.Instance.weapon[slotName].ammo.ammoSize;
        weapon.ammoCount = LoadoutManager.Instance.weapon[slotName].ammo.ammoCount;
        weapon.bulletSpeed = LoadoutManager.Instance.weapon[slotName].ammo.bulletSpeed;
        weapon.gravityFactor = LoadoutManager.Instance.weapon[slotName].fireControl.gravityFactor;
        weapon.fireRate = LoadoutManager.Instance.weapon[slotName].fireControl.fireRate;
        weapon.reloadTime = LoadoutManager.Instance.weapon[slotName].fireControl.reloadTime;
        weapon.chargeReload = LoadoutManager.Instance.weapon[slotName].fireControl.chargeReload;
        weapon.lifeTime = LoadoutManager.Instance.weapon[slotName].ammo.lifeTime;
        weapon.fireBurst = LoadoutManager.Instance.weapon[slotName].fireControl.fireBurst;
        weapon.fireBustRate = LoadoutManager.Instance.weapon[slotName].fireControl.fireBustRate;
        weapon.boostDelay = LoadoutManager.Instance.weapon[slotName].fireControl.boostDelay;
        weapon.initialVeliocity = LoadoutManager.Instance.weapon[slotName].fireControl.initialVeliocity;
        weapon.initialGravity = LoadoutManager.Instance.weapon[slotName].fireControl.initialGravity;
        weapon.fireCount = LoadoutManager.Instance.weapon[slotName].fireControl.fireCount;
        weapon.spinUpType = LoadoutManager.Instance.weapon[slotName].fireControl.spinUpType;
        weapon.spinUp = LoadoutManager.Instance.weapon[slotName].fireControl.spinUp;
        weapon.spread = LoadoutManager.Instance.weapon[slotName].fireControl.spread;
        weapon.accuracy = LoadoutManager.Instance.weapon[slotName].fireControl.accuracy;
        weapon.explosion = LoadoutManager.Instance.weapon[slotName].fireControl.explosion;
        weapon.explosionType = LoadoutManager.Instance.weapon[slotName].fireControl.explosionType;
        weapon.lockOnTime = LoadoutManager.Instance.weapon[slotName].lockOn.lockOnTime;
        weapon.lockOnRange = LoadoutManager.Instance.weapon[slotName].lockOn.lockOnRange;
        weapon.lockOnSize.x = LoadoutManager.Instance.weapon[slotName].lockOn.lockOnSize_x;
        weapon.lockOnSize.y = LoadoutManager.Instance.weapon[slotName].lockOn.lockOnSize_y;
        weapon.homingAccuracy = LoadoutManager.Instance.weapon[slotName].lockOn.homingAccuracy;
        weapon.startHoming = LoadoutManager.Instance.weapon[slotName].lockOn.startHoming;
        weapon.allowMultiLock = LoadoutManager.Instance.weapon[slotName].lockOn.allowMultiLock;
        weapon.shootMuzzleRotate = LoadoutManager.Instance.weapon[slotName].lockOn.shootMuzzleRotate;
        weapon.recoilAmountX = LoadoutManager.Instance.weapon[slotName].recoil.recoilAmountX;
        weapon.recoilRecovery = LoadoutManager.Instance.weapon[slotName].recoil.recoilRecovery;
        weapon.recoilSpread = LoadoutManager.Instance.weapon[slotName].recoil.recoilSpread;
        weapon.maxRecoilRadius = LoadoutManager.Instance.weapon[slotName].recoil.maxRecoilRadius;
        weapon.cartridgePrefabPath = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgePrefabPath;
        weapon.cartridgeSize = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeSize;
        weapon.cartridgeSpeed = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeSpeed;
        weapon.cartridgeLifeTime = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeLifeTime;
        weapon.cartridgeDelay = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeDelay;
        weapon.cartridgeRotateSpeed = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeRotateSpeed;
        weapon.cartridgeRandomness = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeRandomness;
        weapon.cartridgeYAngle = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeYAngle;
        weapon.cartridgeRotate = LoadoutManager.Instance.weapon[slotName].cartridge.cartridgeRotate;
    }
}
