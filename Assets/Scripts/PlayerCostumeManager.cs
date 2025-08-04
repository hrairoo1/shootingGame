using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerCostumeManager : MonoBehaviour
{
    [SerializeField] PlayerInfo info;
    private void Start()
    {
        //SetArmature();
    }
    public void SetArmature()
    {
        Transform armaturePosition = transform.Find("ArmatureHolder");
        if (armaturePosition == null)
        {
            Debug.LogError($"Armatureスロット が見つかりません！");
            return;
        }
        foreach (Transform child in armaturePosition.transform)
        {
            Destroy(child.gameObject);
        }
        info.armature = info.bone[1];
        GameObject costumeObj = info.armature;// Instantiate(info.armature, armaturePosition.position, armaturePosition.rotation);
        //SetCostume();
    }
    public void SetCostume()
    {
        Transform costumePosition = transform.Find("CostumeHolder");
        if (costumePosition == null)
        {
            Debug.LogError($"Costumeスロット が見つかりません！");
            return;
        }
        foreach (Transform child in costumePosition.transform)
        {
            Destroy(child.gameObject);
        }
        if(LoadoutManager.Instance.costumeModel != null)
        {
            Addressables.InstantiateAsync(LoadoutManager.Instance.costumeModel, costumePosition.position, costumePosition.rotation)
            .Completed += (handle) =>
            {
                GameObject costumeObj = handle.Result;
                CostumeInfo costumeInfo = costumeObj.GetComponent<CostumeInfo>();
                Utility.SetBoneConstraint(info.armature.transform, costumeInfo.armature.transform);

            };
        }
    }
}
