using System.IO;
using UnityEngine;
using static PlayerWeaponManager;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using static PlayerArmorManager;

public class WeaponSelector : MonoBehaviour
{
    public EquipWeaponManager weaponManager;
    public EquipArmorManager armorManager;
    public WeaponCatalog weaponCatalog;
    public GameObject Player;

    public void SelectWeapon(string weaponId, string slotName)
    {
        WeaponListEntry foundWeapon = weaponCatalog.weaponEntries.Find(weapon => weapon.id == weaponId);
        // 武器を生成して性能を適用
        LoadoutManager.Instance.weapon[slotName] = foundWeapon.weapon;//武器データをloadoutManagerに
        weaponManager.SetWeaponSlot(slotName, null);
    }
    public void SelectArmor(string armorId, string slotName)
    {
        ArmorListEntry foundArmor = weaponCatalog.armorEntries.Find(armor => armor.id == armorId);
        LoadoutManager.Instance.armor[slotName] = foundArmor.armor;
        armorManager.SetArmorSlot(slotName, null);
        if(slotName == "Backpack")
        {

        }
    }

    public IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSeconds(2f); // 少し待ってからシーン変更
        SceneManager.LoadScene(sceneName);
    }
}
