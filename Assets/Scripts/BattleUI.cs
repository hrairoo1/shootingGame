using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public PlayerWeaponManager weaponManager;
    public PlayerMovement movement;
    public GameObject UI;
    public Transform weaponHolder;
    public GameObject Player;
    //セリフ
    [SerializeField] DialogueManager dialogueManager;
    public GameObject dialogue;
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI speakerText;

    // Start is called before the first frame update
    void Awake()
    {

        movement = Player.GetComponent<PlayerMovement>();
        SetDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponInfo("MainR");
        WeaponInfo("MainL");
        WeaponInfo("ShoulderR");
        WeaponInfo("ShoulderL");
        Unit character = Player.GetComponent<Unit>();
        Transform HoverMode = UI.transform.Find("HoverMode");
        Transform Enelgy = UI.transform.Find("EnergySlider");
        Slider EnelguSl = Enelgy.GetComponent<Slider>();
        Transform HP = UI.transform.Find("HPSlider");
        Slider HPSl = HP.GetComponent<Slider>();
        EnelguSl.maxValue = movement.energyMax;
        EnelguSl.value = movement.energy;
        HPSl.maxValue = character.health;
        HPSl.value = character.currentHealth;

        if (movement.isHovering)
        {
            HoverMode.gameObject.SetActive(true);
        }
        else
        {
            HoverMode.gameObject.SetActive(false);
        }
        //ダイアログ表示
        if (dialogueManager.IsPlaying == true)
        {
            dialogue.SetActive(true);
        }
        else
        {
            dialogue.SetActive(false);
        }
    }
    void WeaponInfo(string slotName)
    {
        if (!weaponManager.weaponSlots.ContainsKey(slotName)) return;
        if (weaponManager.weaponSlots[slotName] == null) return;
        Weapon Weapon = null;
        if (slotName == "ShoulderR" || slotName == "ShoulderL")
        {
            Weapon = weaponManager.weaponSlots[slotName].equippedWeapon;
        }
        else
        {
            Weapon = weaponManager.GetWeapon(slotName);
        }

        Transform WeaponTGT = UI.transform.Find(slotName);
        Transform WeaponName = UI.transform.Find($"{slotName}/Name");
        Transform WeaponAmmo = UI.transform.Find($"{slotName}/Ammo");
        Transform WeaponReloadTime = UI.transform.Find($"{slotName}/ReloadTime");

        TextMeshProUGUI WeaponNameTMP = WeaponName.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI WeaponAmmoTMP = WeaponAmmo.GetComponent<TextMeshProUGUI>();
        Slider WeaponReloadTimeSl = WeaponReloadTime.GetComponent<Slider>();
        if (Weapon != null)
        {
            if (Weapon.weaponId != null)
            {
                if (weaponManager.isWeaponChange)
                {
                    // LocalizeStringEvent をセットまたは取得
                    LocalizeStringEvent localizeEvent = WeaponName.gameObject.GetComponent<LocalizeStringEvent>();
                    if (localizeEvent == null)
                    {
                        localizeEvent = WeaponName.gameObject.AddComponent<LocalizeStringEvent>();
                    }

                    // 武器変更時のみ翻訳キー更新
                    localizeEvent.StringReference.TableReference = "Weapon Table";
                    localizeEvent.StringReference.TableEntryReference = Weapon.weaponId;
                        localizeEvent.OnUpdateString.AddListener((translatedText) =>
                        {
                            WeaponNameTMP.text = translatedText;
                        });
                    localizeEvent.RefreshString(); // 翻訳を強制更新
                    weaponManager.isWeaponChange = false; // 変更完了
                }
            }
            else
            {
                WeaponNameTMP.text = "none";
            }
            WeaponAmmoTMP.text = Weapon.currentAmmoCount + "/" + Weapon.ammoCount;

            WeaponReloadTimeSl.maxValue = Weapon.reloadTime;
            WeaponReloadTimeSl.value = Weapon.currentReloadTime;
            if (Weapon.currentReloadTime > 0)
            {
                WeaponReloadTime.gameObject.SetActive(true);
            }
            else
            {
                WeaponReloadTime.gameObject.SetActive(false);
            }
        }
        else
        {
            WeaponNameTMP.text = "none";
            WeaponAmmoTMP.text = "";
            WeaponReloadTime.gameObject.SetActive(false);
        }
    }
    void SetDialogue()
    {
    }
}
