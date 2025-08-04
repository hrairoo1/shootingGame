using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class WeaponListData
{
    public List<WeaponListEntry> weaponList;
}

[System.Serializable]
public class WeaponListEntry
{
    public string id;
    public string modelNumber;
    public string name;
    public int level;
    public string description;
    public string category;
    public string path; // 個別武器性能のJSONパス
    public WeaponData weapon;
}

[System.Serializable]
public class ArmorListData
{
    public List<ArmorListEntry> armorList;
}

[System.Serializable]
public class ArmorListEntry
{
    public string id;
    public string modelNumber;
    public string name;
    public int level;
    public string description;
    public string category;
    public string path; // 個別防具性能のJSONパス
    public ArmorData armor;
}
public class WeaponCatalog : MonoBehaviour
{
    public TextAsset weaponListJson; // 武器リストJSONをInspectorから設定
    public TextAsset armorListJson; // 防具リストJSONをInspectorから設定
    public WeaponSelector selector;

    public List<WeaponListEntry> weaponEntries;
    public List<ArmorListEntry> armorEntries;
    public string path;
    [SerializeField] EquipSelectionUI selectionUI;

    void Start()
    {
        WeaponListData weaponListData = JsonUtility.FromJson<WeaponListData>(weaponListJson.text);
        weaponEntries = weaponListData.weaponList;
        ArmorListData armorListData = JsonUtility.FromJson<ArmorListData>(armorListJson.text);
        armorEntries = armorListData.armorList;

        foreach (var weapon in weaponEntries)
        {
            SetWeaponDataList(weapon);
        }
        foreach (var armor in armorEntries)
        {
            SetArmorDataList(armor);
        }
        GameSettings.Instance.weaponEntries = weaponEntries;
        GameSettings.Instance.armorEntries = armorEntries;
        StartCoroutine(BBB());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) StartCoroutine(AAA());
    }
    public void SetWeaponDataList(WeaponListEntry weaponId)
    {

        string jsonPath = Path.Combine(Application.dataPath, weaponId.path);
        string jsonText = File.ReadAllText(jsonPath);
        WeaponDataWrapper data = JsonUtility.FromJson<WeaponDataWrapper>(jsonText);
        weaponId.weapon = data.WeaponData;
        weaponId.weapon.weaponId = weaponId.id;
    }
    public void SetArmorDataList(ArmorListEntry armorId)
    {
        string jsonPath = Path.Combine(Application.dataPath, armorId.path);
        string jsonText = File.ReadAllText(jsonPath);
        ArmorDataWrapper data = JsonUtility.FromJson<ArmorDataWrapper>(jsonText);
        armorId.armor = data.ArmorData;
        armorId.armor.armorId = armorId.id;
    }
        IEnumerator AAA()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(selector.LoadSceneDelayed("SampleScene"));
    }    IEnumerator BBB()
    {
        yield return new WaitForSeconds(1f);
        selectionUI.SetSelectUI();
    }

    public WeaponListEntry GetWeaponByName(string weaponId)
    {
        return weaponEntries.Find(w => w.id == weaponId);
    }
    public ArmorListEntry GetArmorByName(string armorId)
    {
        return armorEntries.Find(w => w.id == armorId);
    }
}
[System.Serializable]
public class ArmorDataWrapper
{
    public ArmorData ArmorData;
}
[System.Serializable]
public class ArmorData
{
    public string armorId;
    public string armorModel;
    public float weight;
    public string level;
    public string modelNumber;
    public string armorName;
    public float health;
    public float defence;
    public HeadData head;
    public BodyData body;
    public ArmData arm;
    public LegData leg;
    public BackpackData backpack;
    public OtherData other;
}
[System.Serializable]
public class HeadData
{

}
[System.Serializable]
public class BodyData
{

}
[System.Serializable]
public class ArmData
{

}
[System.Serializable]
public class LegData
{
    public float weightLimit;
}
[System.Serializable]
public class BackpackData
{

}
[System.Serializable]
public class OtherData
{
    public float energy;
    public float energyRecoveryRate;
    public float energyBoostConsumption;
    public float energyHoverConsumption;
    public float thrustPower;
    public float hoverPower;
}

[System.Serializable]
public class WeaponDataWrapper
{
    public WeaponData WeaponData;
}

[System.Serializable]
public class WeaponData
{
    public string weaponId;
    public string weaponModel;
    public float weight;
    public string level;
    public string modelNumber;
    public string weaponName;
    public AmmoData ammo;
    public FireControlData fireControl;
    public LockOnData lockOn;
    public RecoilData recoil;
    public CartridgeData cartridge;
}
[System.Serializable]
public class AmmoData
{
    public string ammoPrefabPath;
    public float ammoDamage;
    public float ammoSize;
    public int ammoCount;
    public float bulletSpeed;
    public float lifeTime;
}
[System.Serializable]
public class FireControlData
{
    public float fireRate;
    public float reloadTime;
    public float chargeReload;
    public float gravityFactor;
    public int fireBurst;
    public float fireBustRate;
    public float boostDelay;
    public float initialVeliocity;
    public float initialGravity;
    public int fireCount;
    public int spinUpType;
    public float spinUp;
    public float spread;
    public float accuracy;
    public float explosion;
    public int explosionType;
}
[System.Serializable]
public class LockOnData
{
    public float lockOnTime;
    public float lockOnRange;
    public float lockOnSize_x;
    public float lockOnSize_y;
    public float homingAccuracy;
    public float startHoming;
    public bool allowMultiLock;
    public bool shootMuzzleRotate;
}
[System.Serializable]
public class RecoilData
{
    public float recoilAmountX;
    public float recoilRecovery;
    public float recoilSpread;
    public float maxRecoilRadius;
}
[System.Serializable]
public class CartridgeData
{
    public string cartridgePrefabPath;
    public float cartridgeSize;
    public float cartridgeSpeed;
    public float cartridgeLifeTime;
    public float cartridgeDelay;
    public float cartridgeRotateSpeed;
    public float cartridgeRandomness;
    public float cartridgeYAngle;
    public float cartridgeRotate;
}

