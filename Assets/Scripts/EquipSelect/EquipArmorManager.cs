using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations;
using static PlayerArmorManager;
using static PlayerWeaponManager;

public class EquipArmorManager : MonoBehaviour
{
    public Dictionary<string, ArmorSlot> armorSlots = new Dictionary<string, ArmorSlot>();
    public EquipArmorManager()
    {
        // 初期化で事前定義したキーを使って weaponSlots にデータを追加
        armorSlots["Head"] = new ArmorSlot();
        armorSlots["Body"] = new ArmorSlot();
        armorSlots["Arm"] = new ArmorSlot();
        armorSlots["Leg"] = new ArmorSlot();
        armorSlots["Backpack"] = new ArmorSlot();
    }
    public class ArmorSlot
    {
        public Transform armedPoint; // 武器を装備する位置 (ボーンなど)
        public Armor equippedArmor; // 装備されている武器
        public string armorModelPath; // 武器のプレハブのアドレス
        public GameObject armorModelInstance; // 実際に生成された武器のモデル
    }
    [SerializeField] EquipWeaponManager weaponManager;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    public void SetArmorSlot(string slotName, string aPath)
    {
        // slotNameと対応する部位を管理するDictionary
        var slotMapping = new Dictionary<string, string>
        {
            { "Body", "spine" },
            { "Head", "neck" },
            { "Arm", "chest" },
            { "Leg", "hips" },
            { "Backpack", "chest" }
        };

        // Dictionaryを使って対応する部位を取得
        if (slotMapping.ContainsKey(slotName))
        {
            string boneName = slotMapping[slotName];
            PlayerInfo pInfo = GetComponent<PlayerInfo>(); 
            Transform armedPoint = transform.Find($"ArmorHolder/{slotName}");
            Transform ArmamentPoint = FindChildRecursively(pInfo.armature.transform, boneName);
            foreach (Transform child in armedPoint.transform)
            {
                Destroy(child.gameObject);
            }
            // 武器スロットを作成
            armorSlots[slotName] = new ArmorSlot { armedPoint = armedPoint };
            Transform weaponTransform = FindChildInSlot(armedPoint);

            string model;
            if (aPath != null) model = aPath;
            else if (LoadoutManager.Instance.armor[slotName].armorModel != null) model = LoadoutManager.Instance.armor[slotName].armorModel;
            else model = null;
            if (model != null)
            {
                Addressables.InstantiateAsync(aPath != null ? aPath : LoadoutManager.Instance.armor[slotName].armorModel, ArmamentPoint.position, ArmamentPoint.rotation)
                .Completed += (handle) =>
                {
                    GameObject armorObj = handle.Result;
                    armorObj.transform.SetParent(armedPoint);
                    ArmorInfo info = armorObj.GetComponent<ArmorInfo>();

                    //Shoulderポイントを変更
                    if (slotName == "Backpack")
                    {

                        Transform shoulderRPoint = transform.Find("WeaponHolder/ShoulderR");
                        if (info.ShoulderR != null)
                        {
                            shoulderRPoint.transform.position = info.ShoulderR.transform.position;
                        }
                        else
                        {
                            weaponManager.DestroyWeaponSlot("ShoulderR");
                            LoadoutManager.Instance.weapon["ShoulderR"] = new WeaponData();
                        }
                        Transform shoulderLPoint = transform.Find("WeaponHolder/ShoulderL");
                        if (info.ShoulderL != null)
                        {
                            shoulderLPoint.transform.position = info.ShoulderL.transform.position;
                        }
                        else
                        {
                            weaponManager.DestroyWeaponSlot("ShoulderL");
                            LoadoutManager.Instance.weapon["ShoulderL"] = new WeaponData();
                        }
                    }
                    Utility.SetBoneConstraint(pInfo.armature.transform, info.armature.transform);

                    Armor armorComponent = armorObj.GetComponent<Armor>();
                    if (armorComponent != null)
                    {
                        armorSlots[slotName].equippedArmor = armorComponent;
                        armorSlots[slotName].armorModelPath = armorComponent.armorModel;
                        armorSlots[slotName].armorModelInstance = armorObj;
                    }
                    else
                    {
                        armorComponent = armorObj.AddComponent<Armor>();
                        armorSlots[slotName].equippedArmor = armorComponent;
                        armorSlots[slotName].armorModelPath = armorComponent.armorModel;
                        armorSlots[slotName].armorModelInstance = armorObj;
                    }

                    Material mat = armorObj.GetComponentInChildren<Renderer>().material;
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
    }
    public void DestroyArmorSlot(string slotName)
    {
        Transform mountPoint = transform.Find($"ArmorHolder/{slotName}");
        if (mountPoint == null)
        {
            Debug.LogError($"スロット {slotName} が見つかりません！");
            return;
        }
        foreach (Transform child in mountPoint.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private Transform FindChildInSlot(Transform slotTransform)
    {
        return slotTransform.childCount > 0 ? slotTransform.GetChild(0) : null;
    }
    Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform found = FindChildRecursively(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null; // 見つからなかった場合
    }
}
