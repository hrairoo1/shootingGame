using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations;
using static PlayerArmorManager;
using static PlayerWeaponManager;

public class PlayerArmorManager : MonoBehaviour
{
    public Dictionary<string, ArmorSlot> armorSlots = new Dictionary<string, ArmorSlot>();
    public string bodyArmorPath;
    public string armArmorPath;
    public string legArmorPath;
    public List<GameObject> boosterEffect;
    public GameObject obj;
    public class ArmorSlot
    {
        public Transform armedPoint; // 武器を装備する位置 (ボーンなど)
        public Armor equippedArmor; // 装備されている武器
        public string armorModelPath; // 武器のプレハブのアドレス
        public GameObject armorModelInstance; // 実際に生成された武器のモデル
    }

    // Start is called before the first frame update
    void Awake()
    {
        SetArmorSlot("Head");
        SetArmorSlot("Body");
        SetArmorSlot("Arm");
        SetArmorSlot("Leg");
        SetArmorSlot("Backpack");

    }

    public void SetArmorSlot(string slotName)
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
            SetArmor(slotName, slotMapping[slotName]);
        }
    }
    void SetArmor(string slotName, string boneName)
    {
        if (LoadoutManager.Instance.armor[slotName] != null)
        {
            if (LoadoutManager.Instance.armor[slotName].armorModel == null) return;
            Transform armedPoint = transform.Find($"ArmorHolder/{slotName}");
            PlayerInfo pInfo = GetComponent<PlayerInfo>();
            Transform ArmamentPoint = FindChildRecursively(pInfo.armature.transform, boneName);
            // 武器スロットを作成
            armorSlots[slotName] = new ArmorSlot { armedPoint = armedPoint };
            Transform weaponTransform = FindChildInSlot(armedPoint);
            Addressables.InstantiateAsync(LoadoutManager.Instance.armor[slotName].armorModel, ArmamentPoint.position, ArmamentPoint.rotation)
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
                    Transform shoulderLPoint = transform.Find("WeaponHolder/ShoulderL");
                    if (info.ShoulderL != null)
                    {
                        shoulderLPoint.transform.position = info.ShoulderL.transform.position;
                    }
                }
                //コンストレイント

                Utility.SetBoneConstraint(pInfo.armature.transform, info.armature.transform);

                Armor armorComponent = armorObj.GetComponent<Armor>();
                if (armorComponent != null)
                {
                    SetArmorData(armorComponent, slotName);
                    armorSlots[slotName].equippedArmor = armorComponent;
                    armorSlots[slotName].armorModelPath = armorComponent.armorModel;
                    armorSlots[slotName].armorModelInstance = armorObj;
                }
                else
                {
                    armorComponent = armorObj.AddComponent<Armor>();
                    SetArmorData(armorComponent, slotName);
                    armorSlots[slotName].equippedArmor = armorComponent;
                    armorSlots[slotName].armorModelPath = armorComponent.armorModel;
                    armorSlots[slotName].armorModelInstance = armorObj;
                }
                if (info.nozzle.Count != 0)
                {
                    foreach (Transform pos in info.nozzle)
                    {
                        GameObject booster = Instantiate(obj, pos.position, Quaternion.AngleAxis(180f, Vector3.right) * pos.rotation);
                        booster.transform.SetParent(pos);
                        boosterEffect.Add(booster);

                        //ApplyRotateConstraint(booster.transform, pos);
                        //ApplyPositionConstraint(booster.transform, pos);
                        ParentConstraint constraint = booster.AddComponent<ParentConstraint>();

                        // 親オブジェクトをターゲットに追加
                        ConstraintSource source = new ConstraintSource();
                        source.sourceTransform = pos.transform;
                        source.weight = 1.0f;

                        constraint.AddSource(source);

                        // 位置と回転の追従を有効化
                        constraint.constraintActive = true;
                        constraint.translationAtRest = Vector3.zero;
                        constraint.rotationAtRest = Vector3.zero;
                        constraint.locked = true;
                    }
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
    // 指定したルートからすべての子ボーンを辞書化（名前 -> Transform）
    private Dictionary<string, Transform> GetBoneDictionary(Transform root)
    {
        Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();
        foreach (Transform bone in root.GetComponentsInChildren<Transform>())
        {
            boneDict[bone.name] = bone;
        }
        return boneDict;
    }

    // RotateConstraint を適用する
    private void ApplyRotateConstraint(Transform target, Transform source)
    {
        RotationConstraint rotateConstraint = target.gameObject.AddComponent<RotationConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = source;
        constraintSource.weight = 1.0f;

        rotateConstraint.AddSource(constraintSource);
        rotateConstraint.weight = 1.0f;
        rotateConstraint.constraintActive = true;
    }// RotateConstraint を適用する
    private void ApplyPositionConstraint(Transform target, Transform source)
    {
        PositionConstraint positionConstraint = target.gameObject.AddComponent<PositionConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = source;
        constraintSource.weight = 1.0f;

        positionConstraint.AddSource(constraintSource);
        positionConstraint.weight = 1.0f;
        positionConstraint.constraintActive = true;
    }
    void SetArmorData(Armor armor, string slotName)
    {
        armor.armorModel = LoadoutManager.Instance.armor[slotName].armorModel;
        armor.weight = LoadoutManager.Instance.armor[slotName].weight;
        armor.modelNumber = LoadoutManager.Instance.armor[slotName].modelNumber;
        armor.armorName = LoadoutManager.Instance.armor[slotName].armorName;
        armor.health = LoadoutManager.Instance.armor[slotName].health;
        armor.defence = LoadoutManager.Instance.armor[slotName].defence;

        armor.weightLimit = LoadoutManager.Instance.armor[slotName].leg.weightLimit;

        armor.energy = LoadoutManager.Instance.armor[slotName].energy;
        armor.energyRecoveryRate = LoadoutManager.Instance.armor[slotName].backpack.energyRecoveryRate;
        armor.energyBoostConsumption = LoadoutManager.Instance.armor[slotName].booster.energyBoostConsumption;
        armor.energyHoverConsumption = LoadoutManager.Instance.armor[slotName].booster.energyHoverConsumption;

        armor.thrustPower = LoadoutManager.Instance.armor[slotName].booster.thrustPower;
        armor.hoverPower = LoadoutManager.Instance.armor[slotName].booster.hoverPower;
    }
}
