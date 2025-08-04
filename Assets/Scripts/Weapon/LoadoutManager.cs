using System.Collections.Generic;
using UnityEngine;
using static ColorManager;
using static PlayerWeaponManager;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance;

    public Dictionary<string, WeaponData> weapon = new Dictionary<string, WeaponData>();
    public LoadoutManager()
    {
        // 初期化で事前定義したキーを使って weaponSlots にデータを追加
        weapon["MainR"] = new WeaponData();
        weapon["MainL"] = new WeaponData();
        weapon["SubR"] = new WeaponData();
        weapon["SubL"] = new WeaponData();
        weapon["ShoulderR"] = new WeaponData();
        weapon["ShoulderL"] = new WeaponData();
        armor["Head"] = new ArmorData();
        armor["Body"] = new ArmorData();
        armor["Arm"] = new ArmorData();
        armor["Leg"] = new ArmorData();
        armor["Backpack"] = new ArmorData();

        equipColor["MainR"] = new EquipColor();
        equipColor["MainL"] = new EquipColor();
        equipColor["SubR"] = new EquipColor();
        equipColor["SubL"] = new EquipColor();
        equipColor["ShoulderR"] = new EquipColor();
        equipColor["ShoulderL"] = new EquipColor();
        equipColor["Head"] = new EquipColor();
        equipColor["Body"] = new EquipColor();
        equipColor["Arm"] = new EquipColor();
        equipColor["Leg"] = new EquipColor();
        equipColor["Backpack"] = new EquipColor();

        equipIntencity["MainR"] = new Intencity();
        equipIntencity["MainL"] = new Intencity();
        equipIntencity["SubR"] = new Intencity();
        equipIntencity["SubL"] = new Intencity();
        equipIntencity["ShoulderR"] = new Intencity();
        equipIntencity["ShoulderL"] = new Intencity();
        equipIntencity["Head"] = new Intencity();
        equipIntencity["Body"] = new Intencity();
        equipIntencity["Arm"] = new Intencity();
        equipIntencity["Leg"] = new Intencity();
        equipIntencity["Backpack"] = new Intencity();
    }
    public Dictionary<string, ArmorData> armor = new Dictionary<string, ArmorData>();
    public string costumeModel;
    public int gender;
    public Dictionary<string, EquipColor> equipColor = new Dictionary<string, EquipColor>();
    public Dictionary<string, Intencity> equipIntencity = new Dictionary<string, Intencity>();
    public string characterSkin;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 複製防止
        }
    }
}
