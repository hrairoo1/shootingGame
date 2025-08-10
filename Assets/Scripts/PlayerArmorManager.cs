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
        public Transform armedPoint; // ����𑕔�����ʒu (�{�[���Ȃ�)
        public Armor equippedArmor; // ��������Ă��镐��
        public string armorModelPath; // ����̃v���n�u�̃A�h���X
        public GameObject armorModelInstance; // ���ۂɐ������ꂽ����̃��f��
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
        // slotName�ƑΉ����镔�ʂ��Ǘ�����Dictionary
    var slotMapping = new Dictionary<string, string>
    {
        { "Body", "spine" },
        { "Head", "neck" },
        { "Arm", "chest" },
        { "Leg", "hips" },
        { "Backpack", "chest" }
    };

        // Dictionary���g���đΉ����镔�ʂ��擾
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
            // ����X���b�g���쐬
            armorSlots[slotName] = new ArmorSlot { armedPoint = armedPoint };
            Transform weaponTransform = FindChildInSlot(armedPoint);
            Addressables.InstantiateAsync(LoadoutManager.Instance.armor[slotName].armorModel, ArmamentPoint.position, ArmamentPoint.rotation)
            .Completed += (handle) =>
            {
                GameObject armorObj = handle.Result;
                armorObj.transform.SetParent(armedPoint);
                ArmorInfo info = armorObj.GetComponent<ArmorInfo>();

                //Shoulder�|�C���g��ύX
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
                //�R���X�g���C���g

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

                        // �e�I�u�W�F�N�g���^�[�Q�b�g�ɒǉ�
                        ConstraintSource source = new ConstraintSource();
                        source.sourceTransform = pos.transform;
                        source.weight = 1.0f;

                        constraint.AddSource(source);

                        // �ʒu�Ɖ�]�̒Ǐ]��L����
                        constraint.constraintActive = true;
                        constraint.translationAtRest = Vector3.zero;
                        constraint.rotationAtRest = Vector3.zero;
                        constraint.locked = true;
                    }
                }

                Material mat = armorObj.GetComponentInChildren<Renderer>().material;
                if (mat != null)
                {
                    // ���݂̃v���p�e�B�����擾
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
        return null; // ������Ȃ������ꍇ
    }
    // �w�肵�����[�g���炷�ׂĂ̎q�{�[�����������i���O -> Transform�j
    private Dictionary<string, Transform> GetBoneDictionary(Transform root)
    {
        Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();
        foreach (Transform bone in root.GetComponentsInChildren<Transform>())
        {
            boneDict[bone.name] = bone;
        }
        return boneDict;
    }

    // RotateConstraint ��K�p����
    private void ApplyRotateConstraint(Transform target, Transform source)
    {
        RotationConstraint rotateConstraint = target.gameObject.AddComponent<RotationConstraint>();

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = source;
        constraintSource.weight = 1.0f;

        rotateConstraint.AddSource(constraintSource);
        rotateConstraint.weight = 1.0f;
        rotateConstraint.constraintActive = true;
    }// RotateConstraint ��K�p����
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
