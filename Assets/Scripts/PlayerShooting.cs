using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public PlayerWeaponManager weaponManager;
    private bool canToggleWeapon = false;
    private bool canPurgeWeapon = false;
    private bool canToggleMainL = false;
    private bool canToggleMainR = false;
    private bool canFireMainL = false;
    private bool canFireMainR = false;
    private bool canFireShoulderL = false;
    private bool canFireShoulderR = false;
    private bool purgeMR = false;
    private bool purgeML = false;
    private bool purgeSR = false;
    private bool purgeSL = false;
    [SerializeField] InputSystem_Actions controls;
    private InputAction MainRShoot;
    private InputAction MainLShoot;
    private InputAction ShoulderRShoot;
    private InputAction ShoulderLShoot;
    private InputAction Reload;
    private InputAction ToggleWeapon;
    private InputAction purgeWeapon;
    void Start()
    {

        controls = new InputSystem_Actions();
        //actionイベント
        MainRShoot = controls.Player.MainRShoot;
        MainRShoot.started += ctx => { if (!canToggleMainR && !canPurgeWeapon && !canToggleWeapon) FireWeapon("MainR"); if (canPurgeWeapon && !canToggleWeapon) purgeMR = true; if (canToggleWeapon == true) canToggleMainR = true; };
        MainRShoot.canceled += ctx => ButtonUp("MainR");
        MainLShoot = controls.Player.MainLShoot;
        MainLShoot.started += ctx => { if (!canToggleMainL && !canPurgeWeapon && !canToggleWeapon) FireWeapon("MainL"); if (canPurgeWeapon && !canToggleWeapon) purgeML = true; if (canToggleWeapon == true) canToggleMainL = true; };
        MainLShoot.canceled += ctx => ButtonUp("MainL");
        ShoulderRShoot = controls.Player.ShoulderRShoot;
        ShoulderRShoot.started += ctx => { if (!canPurgeWeapon && !canToggleWeapon) FireWeapon("ShoulderR"); if (canPurgeWeapon && !canToggleWeapon) purgeSR = true; };
        ShoulderRShoot.canceled += ctx => ButtonUp("ShoulderR");
        ShoulderLShoot = controls.Player.ShoulderLShoot;
        ShoulderLShoot.started += ctx => { if (!canPurgeWeapon && !canToggleWeapon) FireWeapon("ShoulderL"); if (canPurgeWeapon && !canToggleWeapon) purgeSL = true; };
        ShoulderLShoot.canceled += ctx => ButtonUp("ShoulderL");
        Reload = controls.Player.Reload;
        Reload.started += ctx =>
        {
            ReloadWeapon("MainR");
            ReloadWeapon("MainL");
            ReloadWeapon("ShoulderR");
            ReloadWeapon("ShoulderL");
        };
        ToggleWeapon = controls.Player.ToggleWeapon;
        ToggleWeapon.started += ctx => canToggleWeapon = true;
        ToggleWeapon.canceled += ctx => canToggleWeapon = false;
        purgeWeapon = controls.Player.PurgeWeapon;
        purgeWeapon.started += ctx => canPurgeWeapon = true;
        purgeWeapon.canceled += ctx => canPurgeWeapon = false;
        controls.Enable();
    }
    void Update()
    {

        //パージ武器
        if (canPurgeWeapon)
        {
            if (purgeML)
            {
                purgeML = false;
                weaponManager.PurgeWeaponSlot("MainL");
            }
            if (purgeMR)
            {
                purgeMR = false;
                weaponManager.PurgeWeaponSlot("MainR");
            }
            if (purgeSL)
            {
                purgeSL = false;
                weaponManager.PurgeWeaponSlot("ShoulderL");
            }
            if (purgeSR)
            {
                purgeSR = false;
                weaponManager.PurgeWeaponSlot("ShoulderR");
            }
        }
        if (canToggleWeapon) ToggleWeapons();
        if (canFireMainR) FireWeapon("MainR");
        if (canFireMainL) FireWeapon("MainL");
        if (canFireShoulderR) FireWeapon("ShoulderR");
        if (canFireShoulderL) FireWeapon("ShoulderL");
    }

    void FireWeapon(string slotName)
    {
        Weapon weapon = null;
        if(slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            if (slotName == "ShoulderR") canFireShoulderR = true;
            if (slotName == "ShoulderL") canFireShoulderL = true;
            weapon = weaponManager.weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            if (slotName == "MainR") canFireMainR = true;
            if (slotName == "MainL") canFireMainL = true;
            weapon = weaponManager.GetWeapon(slotName);
        }
        if (weapon != null)
        {
            StartCoroutine(weapon.Shoot());
            if(weapon.fireBurst != 0)
            {
                weapon.ToggleSemiAuto(true);
            }
        }
    }
    void ButtonUp(string slotName)
    {
        Weapon weapon = null;
        if (slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            if (slotName == "ShoulderR") canFireShoulderR = false;
            if (slotName == "ShoulderL") canFireShoulderL = false;
            weapon = weaponManager.weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            if (slotName == "MainR") canFireMainR = false;
            if (slotName == "MainL") canFireMainL = false;
            weapon = weaponManager.GetWeapon(slotName);
        }
        if (weapon != null)
        {
            weapon.ButtonUp();
            weapon.ToggleSemiAuto(false);
        }
    }
    void ReloadWeapon(string slotName)
    {
        Weapon weapon = null;
        if (slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            weapon = weaponManager.weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            weapon = weaponManager.GetWeapon(slotName);
        }
        if (weapon != null && weapon.currentAmmoCount != weapon.ammoCount)
        weapon.Reload();
    }
    void ToggleWeapons()
    {
        if (canToggleMainL)
        {
            weaponManager.ToggleSubWeapons("MainL");
            canToggleMainL = false;
        }
        if (canToggleMainR)
        {
            weaponManager.ToggleSubWeapons("MainR");
            canToggleMainR = false;
        }
    }
}
