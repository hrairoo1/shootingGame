using System.Collections.Generic;
using UnityEngine;
using static PlayerWeaponManager;

public class LockOnUI : MonoBehaviour
{
    public static LockOnUI Instance;
    public PlayerWeaponManager weaponManager;

    [Header("UI Elements")]
    public RectTransform lockOnFrame;
    public GameObject lockOnIndicatorPrefab;
    public GameObject lockingIndicatorPrefab;

    private Dictionary<GameObject, GameObject> lockOnIndicatorsMainR = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockOnIndicatorsMainL = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockOnIndicatorsShoulderR = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockOnIndicatorsShoulderL = new Dictionary<GameObject, GameObject>();

    private Dictionary<GameObject, GameObject> lockingIndicatorsMainR = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockingIndicatorsMainL = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockingIndicatorsShoulderR = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> lockingIndicatorsShoulderL = new Dictionary<GameObject, GameObject>();

    private Dictionary<GameObject, GameObject> lockingIndicators = new Dictionary<GameObject, GameObject>();

    private Camera mainCamera;

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateIndicators(lockOnIndicatorsMainR);
        UpdateIndicators(lockOnIndicatorsMainL);
        UpdateIndicators(lockOnIndicatorsShoulderR);
        UpdateIndicators(lockOnIndicatorsShoulderL);
        UpdateIndicators(lockingIndicators);
    }

    public void ShowLockingIndicator(GameObject target)
    {
        if (!lockingIndicators.ContainsKey(target))
        {
            GameObject indicator = Instantiate(lockingIndicatorPrefab, lockOnFrame);
            lockingIndicators[target] = indicator;
        }
    }

    public void ConfirmLockOn(GameObject target, string weaponSlot)
    {
        if (lockingIndicators.ContainsKey(target))
        {
            Destroy(lockingIndicators[target]);
            lockingIndicators.Remove(target);
        }

        var indicators = GetLockOnIndicatorDictionary(weaponSlot);
        if (!indicators.ContainsKey(target))
        {
            GameObject indicator = Instantiate(lockOnIndicatorPrefab, lockOnFrame);
            indicators[target] = indicator;
        }
    }

    public void RemoveLockIndicator(GameObject target, string weaponSlot)
    {
        if (lockingIndicators.ContainsKey(target))
        {
            Destroy(lockingIndicators[target]);
            lockingIndicators.Remove(target);
        }
    }

    public void ClearLockIndicators()
    {
        ClearLockedIndicatorSet("MainR");
        ClearLockedIndicatorSet("MainL");
        ClearLockedIndicatorSet("ShoulderR");
        ClearLockedIndicatorSet("Shoulder");
    }

    public void ClearLockedIndicatorSet(string slotName)
    {
        var indicators = GetLockOnIndicatorDictionary(slotName);
        foreach (var indicator in indicators.Values)
        {
            Destroy(indicator);
        }
        indicators.Clear();
    }
    public void ClearLockingIndicatorSet(string slotName)
    {
        var indicators = GetLockingIndicatorDictionary(slotName);
        foreach (var indicator in indicators.Values)
        {
            Destroy(indicator);
        }
        indicators.Clear();
    }

    private void UpdateIndicators(Dictionary<GameObject, GameObject> indicators)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var entry in indicators)
        {
            if (entry.Key == null)
            {
                toRemove.Add(entry.Key);
                continue;
            }

            Vector3 screenPos = mainCamera.WorldToScreenPoint(entry.Key.transform.position);

            if (screenPos.z < 0)
            {
                entry.Value.SetActive(false);
                continue;
            }

            entry.Value.SetActive(true);
            entry.Value.transform.position = screenPos;
            entry.Value.transform.rotation = Quaternion.identity;
        }

        foreach (var target in toRemove)
        {
            Destroy(indicators[target]);
            indicators.Remove(target);
        }
    }

    private Dictionary<GameObject, GameObject> GetLockOnIndicatorDictionary(string weaponSlot)
    {
        switch (weaponSlot)
        {
            case "MainR": case "SubR": return lockOnIndicatorsMainR;
            case "MainL": case "SubL": return lockOnIndicatorsMainL;
            case "ShoulderR": return lockOnIndicatorsShoulderR;
            case "ShoulderL": return lockOnIndicatorsShoulderL;
            default: return lockOnIndicatorsMainR;
        }
    }


    private Dictionary<GameObject, GameObject> GetLockingIndicatorDictionary(string weaponSlot)
    {
        switch (weaponSlot)
        {
            case "MainR": case "SubR": return lockingIndicatorsMainR;
            case "MainL": case "SubL": return lockingIndicatorsMainL;
            case "ShoulderR": return lockingIndicatorsShoulderR;
            case "ShoulderL": return lockingIndicatorsShoulderL;
            default: return lockingIndicatorsMainR;
        };
    }
}
