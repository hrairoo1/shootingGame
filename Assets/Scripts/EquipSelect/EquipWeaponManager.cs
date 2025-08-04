using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.GraphicsBuffer;

public class EquipWeaponManager : MonoBehaviour
{
    public Dictionary<string, WeaponSlot> weaponSlots = new Dictionary<string, WeaponSlot>();
    public EquipWeaponManager()
    {
        // 初期化で事前定義したキーを使って weaponSlots にデータを追加
        weaponSlots["MainR"] = new WeaponSlot();
        weaponSlots["MainL"] = new WeaponSlot();
        weaponSlots["SubR"] = new WeaponSlot();
        weaponSlots["SubL"] = new WeaponSlot();
        weaponSlots["ShoulderR"] = new WeaponSlot();
        weaponSlots["ShoulderL"] = new WeaponSlot();
    }

    private bool isUsingSubWeapons = false;

    public class WeaponSlot
    {
        public Transform mountPoint; // 武器を装備する位置 (ボーンなど)
        public Weapon equippedWeapon; // 装備されている武器
        public string weaponModelPath; // 武器のプレハブのアドレス
        public GameObject weaponModelInstance; // 実際に生成された武器のモデル
    }

    void Start()
    {
        // 初期状態の武器表示を設定
        UpdateWeaponVisibility(false);
    }

    // 指定されたスロット名で武器スロットをセット
    public void SetWeaponSlot(string slotName, string wPath)
    {
        Transform mountPoint = transform.Find($"WeaponHolder/{slotName}");
        if (mountPoint == null)
        {
            Debug.LogError($"スロット {slotName} が見つかりません！");
            return;
        }
        foreach (Transform child in mountPoint.transform)
        {
            Destroy(child.gameObject);
        }
        // 武器スロットを作成
        weaponSlots[slotName] = new WeaponSlot { mountPoint = mountPoint };
        // 既に存在する武器をセット
        Transform weaponTransform = FindChildInSlot(mountPoint); string model;
        if (wPath != null) model = wPath;
        else if (LoadoutManager.Instance.weapon[slotName].weaponModel != null) model = LoadoutManager.Instance.weapon[slotName].weaponModel;
        else model = null;
        if (model != null)
        {
            Addressables.InstantiateAsync(wPath != null ? wPath : LoadoutManager.Instance.weapon[slotName].weaponModel, mountPoint.position, mountPoint.rotation)
            .Completed += (handle) =>
            {
                GameObject weaponObj = handle.Result;
                weaponSlots[slotName].weaponModelInstance = weaponObj;
                weaponObj.transform.SetParent(mountPoint);
                weaponObj.transform.position = mountPoint.transform.position;
                weaponObj.transform.rotation = mountPoint.transform.rotation;

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
    public void DestroyWeaponSlot(string slotName)
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
    }

    // マウントポイントの直下にある最初の子オブジェクトを探す
    private Transform FindChildInSlot(Transform slotTransform)
    {
        return slotTransform.childCount > 0 ? slotTransform.GetChild(0) : null;
    }

    // 武器の表示を更新
    public void UpdateWeaponVisibility(bool ActiveState)
    {
        if (weaponSlots["MainR"].weaponModelInstance != null) weaponSlots["MainR"].weaponModelInstance.SetActive(!ActiveState);
        if (weaponSlots["MainL"].weaponModelInstance != null) weaponSlots["MainL"].weaponModelInstance.SetActive(!ActiveState);
        if (weaponSlots["SubR"].weaponModelInstance != null) weaponSlots["SubR"].weaponModelInstance.SetActive(ActiveState);
        if (weaponSlots["SubL"].weaponModelInstance != null) weaponSlots["SubL"].weaponModelInstance.SetActive(ActiveState); 
    }
}
