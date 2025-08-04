using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    [Header("info")]
    public string armorModel;
    public string modelNumber;
    public string armorName;
    public float weight = 0f;
    private ArmorInfo armorInfo;
    public float health = 0f;
    public float defence = 0f;
    [Header("Head Property")]

    [Header("Body Property")]
    [Header("Arm Property")]
    [Header("Leg Property")]
    public float weightLimit = 0f;
    [Header("Backpack Property")]
    [Header("Genelator")]
    public float energy = 0f;
    public float energyRecoveryRate = 0f;
    public float energyBoostConsumption = 0f;
    public float energyHoverConsumption = 0f;
    [Header("Booster")]
    public float thrustPower = 0f;
    public float hoverPower = 0f;

    // Start is called before the first frame update
    void Start()
    {
        armorInfo = GetComponent<ArmorInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
