using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnSite : MonoBehaviour
{
    public GameObject UI;
    public PlayerWeaponManager weaponManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LockSite("MainR");
        LockSite("MainL");
        LockSite("ShoulderR");
        LockSite("ShoulderL");
    }
    void LockSite(string slotName)
    {
        if (!weaponManager.weaponSlots.ContainsKey(slotName)) return;
        if (weaponManager.weaponSlots[slotName] == null) return;
        Weapon weapon = null;
        Transform site = UI.transform.Find(slotName);
        RectTransform siteRect = site.GetComponent<RectTransform>();
        if (slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            weapon = weaponManager.weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            weapon = weaponManager.GetWeapon(slotName);
        }
        if (weapon == null)
        {

            site.gameObject.SetActive(false);
            return;
        }
        siteRect.sizeDelta = new Vector2(weapon.lockOnSize.x, weapon.lockOnSize.y);
        if (weapon.semiAuto)
        {
            site.gameObject.SetActive(true);
        }
        else
        {
            site.gameObject.SetActive(false);
        }
    }
}
