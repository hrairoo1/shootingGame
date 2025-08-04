using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EquipSelectWindow : MonoBehaviour
{
    public GameObject Player;
    public string selectSlotName;
    private GameObject currentSelectedButton;
    [SerializeField] EquipSelectionUI EquipSelectionUI;
    [SerializeField] ColorSelectWindow colorSelectWindow;
    [SerializeField] HSVColorPicker colorPicker;
    [SerializeField] EquipWeaponManager weaponManager;
    [SerializeField] EquipArmorManager armorManager;
    public GameObject MainSelect;
    public GameObject weaponSelectUI;
    public GameObject armorSelectUI;
    public GameObject colorSelectUI;
    public GameObject equipListUI;
    public GameObject removeEquipUI;

    public Button weaponButton;
    public Button armorButton;
    public Button colorButton;
    public Button ExitButton;

    public Button MainRButton;
    public Button MainLButton;
    public Button SubRButton;
    public Button SubLButton;
    public Button ShoulderRButton;
    public Button ShoulderLButton;
    public Button weaponBackButton;

    public Button HeadButton;
    public Button BodyButton;
    public Button ArmButton;
    public Button LegButton;
    public Button BackpackButton;
    public Button armorBackButton;

    public Button Main1Button;
    public Button Main2Button;
    public Button Main3Button;
    public Button Sub1Button;
    public Button Sub2Button;
    public Button Emi1Button;
    public Button Emi2Button;
    public Button ColorBackButton;

    public Button removYes;
    public Button removNo;

    [SerializeField] InputSystem_Actions controls;
    private InputAction cancel;
    private InputAction unloadEquip;

    public Vector3 lastMousePosition;
    public bool isMouseVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(weaponButton.gameObject);
        MainSelect.SetActive(true);
        weaponSelectUI.SetActive(false);
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(false);
        colorSelectUI.SetActive(false);
        colorSelectWindow.ui.SetActive(false);
        removeEquipUI.SetActive(false);
        weaponButton.onClick.AddListener(WeaponClicked);
        armorButton.onClick.AddListener(ArmorClicked);
        colorButton.onClick.AddListener(ColorClicked);

        MainRButton.onClick.AddListener(MainRClicked);
        MainLButton.onClick.AddListener(MainLClicked);
        SubRButton.onClick.AddListener(SubRClicked);
        SubLButton.onClick.AddListener(SubLClicked);
        ShoulderRButton.onClick.AddListener(ShoulderRClicked);
        ShoulderLButton.onClick.AddListener(ShoulderLClicked);
        weaponBackButton.onClick.AddListener(WeaponBackClicked);

        HeadButton.onClick.AddListener(HeadClicked);
        BodyButton.onClick.AddListener(BodyClicked);
        ArmButton.onClick.AddListener(ArmClicked);
        LegButton.onClick.AddListener(LegClicked);
        BackpackButton.onClick.AddListener(BackpackClicked);
        armorBackButton.onClick.AddListener(WeaponBackClicked);

        removYes.onClick.AddListener(WeaponRemoveYes);
        removNo.onClick.AddListener(WeaponRemoveNo);

        controls = new InputSystem_Actions();
        cancel = controls.UI.Cancel;
        cancel.started += ctx => Cancel();
        unloadEquip = controls.UI.UnloadEquip;
        unloadEquip.started += ctx => UnloadEquip();
        controls.Enable();
    }
    private void Update()
    {
        // マウスが動いた場合、マウスカーソルを表示
        if (Input.mousePosition != lastMousePosition)
        {
            Cursor.visible = true;
            isMouseVisible = true;
            lastMousePosition = Input.mousePosition;

        }
        // キーボードの入力があった場合、マウスカーソルを非表示にする
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            Cursor.visible = false;
            isMouseVisible = false;

        }
        if (Input.GetKeyDown(KeyCode.Escape)) controls.Disable();
    }
    void Cancel()
    {
        if ((weaponSelectUI.activeSelf == true || armorSelectUI.activeSelf == true || colorSelectUI.activeSelf == true) && equipListUI.activeSelf == false)
        {
            WeaponBackClicked();
        }
        else if (equipListUI.activeSelf == true)
        {
            EquipSelectionUI.BackWindow();
        }
        else if (colorSelectWindow.ui.activeSelf)
        {
            foreach (Material mat in colorPicker.materials.Values)
            {
                if (mat != null)
                {
                    mat.SetFloat("_Main1B", 0);
                    mat.SetFloat("_Main2B", 0);
                    mat.SetFloat("_Main3B", 0);
                    mat.SetFloat("_Sub1B", 0);
                    mat.SetFloat("_Sub2B", 0);
                    mat.SetFloat("_Emi1B", 0);
                    mat.SetFloat("_Emi2B", 0);
                }
            }
            colorSelectWindow.ui.SetActive(false);
            colorSelectWindow.EquipUI.SetActive(true);
        }
        else if (colorSelectWindow.colorWindow.activeSelf == true && !(colorPicker.isBoxUI || colorPicker.isSlider))
        {
            colorSelectWindow.SaveChangeColor();
        }
        else if (colorPicker.isBoxUI || colorPicker.isSlider)
        {
            if (colorPicker.isBoxUI) EventSystem.current.SetSelectedGameObject(colorSelectWindow.colorUIButton.gameObject);
            if (colorPicker.isSlider) EventSystem.current.SetSelectedGameObject(colorSelectWindow.colorSliderButton.gameObject);
            colorPicker.isBoxUI = false;
            colorPicker.isSlider = false;
        }
    }
    void UnloadEquip()
    {
        if (weaponSelectUI.activeSelf || armorSelectUI.activeSelf)
        {
            //武装を外す
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == MainRButton) selectSlotName = "MainR";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == MainLButton) selectSlotName = "MainL";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == SubRButton) selectSlotName = "SubR";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == SubLButton) selectSlotName = "SubL";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == ShoulderRButton) selectSlotName = "ShoulderR";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == ShoulderLButton) selectSlotName = "ShoulderL";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == HeadButton) selectSlotName = "Head";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == BodyButton) selectSlotName = "Body";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == ArmButton) selectSlotName = "Arm";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == LegButton) selectSlotName = "Leg";
            if (EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>() == BackpackButton) selectSlotName = "Backpack";
            if (LoadoutManager.Instance.weapon.ContainsKey(selectSlotName) && LoadoutManager.Instance.weapon[selectSlotName].weaponName != null)
            {
                removeEquipUI.SetActive(true);
                EventSystem.current.SetSelectedGameObject(removNo.gameObject);
            }
            else if ((LoadoutManager.Instance.armor.ContainsKey(selectSlotName) && LoadoutManager.Instance.armor[selectSlotName].armorName != null))
            {
                removeEquipUI.SetActive(true);
                EventSystem.current.SetSelectedGameObject(removNo.gameObject);
            }
            else
            {
                selectSlotName = null;
            }
        }
    }
    public void ActiveSubShoulderButton()
    {
        //メイン武器がなければサブ武器選択ボタンが押せない
        if (LoadoutManager.Instance.weapon["MainR"] == null || LoadoutManager.Instance.weapon["MainR"].weaponName == null) SubRButton.interactable = false;
        else SubRButton.interactable = true;
        if (LoadoutManager.Instance.weapon["MainL"] == null || LoadoutManager.Instance.weapon["MainL"].weaponName == null) SubLButton.interactable = false;
        else SubLButton.interactable = true;

        //バックパックに肩武器積載場所がなければボタンが使えないようにしたい。今は仮。
        Transform Backpack = Player.transform.Find("ArmorHolder/Backpack");
        // 初期状態でボタン無効
        ShoulderRButton.interactable = false;
        ShoulderLButton.interactable = false;
        // Backpackが存在する場合のみArmorInfo取得
        if (Backpack != null)
        {
            ArmorInfo BackpackInfo = Backpack.GetComponentInChildren<ArmorInfo>();

            // ArmorInfoが存在する場合のみ肩武器チェック
            if (BackpackInfo != null)
            {
                if (BackpackInfo.ShoulderR != null)
                    ShoulderRButton.interactable = true;

                if (BackpackInfo.ShoulderL != null)
                    ShoulderLButton.interactable = true;
            }
        }
    }

    private void WeaponClicked()
    {
        ActiveSubShoulderButton();
        MainSelect.SetActive(false);
        weaponSelectUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(MainRButton.gameObject);
    }
    private void ArmorClicked()
    {
        MainSelect.SetActive(false);
        armorSelectUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(HeadButton.gameObject);
    }
    private void ColorClicked()
    {
        MainSelect.SetActive(false);
        colorSelectUI.SetActive(true);
        colorSelectWindow.isColorSelect = true;
        EventSystem.current.SetSelectedGameObject(colorSelectWindow.MainRButton.gameObject);
    }
    private void MainRClicked()
    {
        selectSlotName = "MainR";
        MainClicked();
        weaponManager.UpdateWeaponVisibility(false);
    }
    private void MainLClicked()
    {
        selectSlotName = "MainL";
        MainClicked();
        weaponManager.UpdateWeaponVisibility(false);
    }
    private void SubRClicked()
    {
        selectSlotName = "SubR";
        weaponManager.UpdateWeaponVisibility(true);
        MainClicked();
    }
    private void SubLClicked()
    {
        selectSlotName = "SubL";
        weaponManager.UpdateWeaponVisibility(true);
        MainClicked();
    }
    private void ShoulderRClicked()
    {
        selectSlotName = "ShoulderR";
        ShoulderClicked();
    }
    private void ShoulderLClicked()
    {
        selectSlotName = "ShoulderL";
        ShoulderClicked();
    }
    private void MainClicked()
    {
        weaponSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.mainCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.mainListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void ShoulderClicked()
    {
        weaponSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.shoulderCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.shoulderListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    public void WeaponBackClicked()
    {
        if(weaponSelectUI.activeSelf) EventSystem.current.SetSelectedGameObject(weaponButton.gameObject);
        if(armorSelectUI.activeSelf) EventSystem.current.SetSelectedGameObject(armorButton.gameObject);
        if(colorSelectUI.activeSelf) EventSystem.current.SetSelectedGameObject(colorButton.gameObject);
        MainSelect.SetActive(true);
        weaponSelectUI.SetActive(false);
        armorSelectUI.SetActive(false);
        colorSelectWindow.EquipUI.SetActive(false);
    }
    private void HeadClicked()
    {
        selectSlotName = "Head";
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.headCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.headListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void BodyClicked()
    {
        selectSlotName = "Body";
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.bodyCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.bodyListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void ArmClicked()
    {
        selectSlotName = "Arm";
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.armCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.armListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void LegClicked()
    {
        selectSlotName = "Leg";
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.legCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.legListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void BackpackClicked()
    {
        selectSlotName = "Backpack";
        armorSelectUI.SetActive(false);
        equipListUI.SetActive(true);
        EquipSelectionUI.backpackCategoryContainer.gameObject.SetActive(true);
        EquipSelectionUI.backpackListContainer.gameObject.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(EquipSelectionUI.FindButton("DMG1"));
    }
    private void WeaponRemoveYes()
    {
        removeEquipUI.SetActive(false);
        weaponManager.DestroyWeaponSlot(selectSlotName);
        Button returnButton = null;
        if (selectSlotName == "MainR") {
            returnButton = MainRButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
            weaponManager.DestroyWeaponSlot("SubR");
        }
        if (selectSlotName == "MainL") { 
            returnButton = MainLButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
            weaponManager.DestroyWeaponSlot("SubL");
        }

        if (selectSlotName == "SubR")
        {
            returnButton = SubRButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
        }
        if (selectSlotName == "SubL")
        {
            returnButton = SubLButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
        }
        if (selectSlotName == "ShoulderR")
        {
            returnButton = ShoulderRButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
        }
        if (selectSlotName == "ShoulderL")
        {
            returnButton = ShoulderLButton;
            weaponManager.DestroyWeaponSlot(selectSlotName);
        }
        if (selectSlotName == "Head")
        {
            returnButton = HeadButton;
            armorManager.DestroyArmorSlot(selectSlotName);
        }
        if (selectSlotName == "Body")
        {
            returnButton = BodyButton;
            armorManager.DestroyArmorSlot(selectSlotName);
        }
        if (selectSlotName == "Arm")
        {
            returnButton = armorButton;
            armorManager.DestroyArmorSlot(selectSlotName);
        }
        if (selectSlotName == "Leg")
        {
            returnButton = LegButton;
            armorManager.DestroyArmorSlot(selectSlotName);
        }
        if (selectSlotName == "Backpack")
        {
            returnButton = BackpackButton;
            armorManager.DestroyArmorSlot(selectSlotName);
            weaponManager.DestroyWeaponSlot("ShoulderR");
            weaponManager.DestroyWeaponSlot("ShoulderL");
            LoadoutManager.Instance.weapon["ShoulderR"] = new WeaponData();
            LoadoutManager.Instance.weapon["ShoulderL"] = new WeaponData();

        }
        if (LoadoutManager.Instance.weapon.ContainsKey(selectSlotName))
        {
            LoadoutManager.Instance.weapon[selectSlotName] = new WeaponData();
            if (selectSlotName == "MainR") LoadoutManager.Instance.weapon["SubR"] = new WeaponData();
            if (selectSlotName == "MainL") LoadoutManager.Instance.weapon["SubL"] = new WeaponData();
        }
        if (LoadoutManager.Instance.armor.ContainsKey(selectSlotName))
        {
            LoadoutManager.Instance.armor[selectSlotName] = new ArmorData();
        }
        selectSlotName = null;
        ActiveSubShoulderButton();
        EventSystem.current.SetSelectedGameObject(returnButton.gameObject);
    }
    private void WeaponRemoveNo()
    {
        ActiveSubShoulderButton();
        removeEquipUI.SetActive(false);
        Button returnButton = MainRButton;
        if (selectSlotName == "MainR") returnButton = MainRButton;
        if (selectSlotName == "MainL") returnButton = MainLButton;
        if (selectSlotName == "SubR") returnButton = SubRButton;
        if (selectSlotName == "SubL") returnButton = SubLButton;
        if (selectSlotName == "ShoulderR") returnButton = ShoulderRButton;
        if (selectSlotName == "ShoulderL") returnButton = ShoulderLButton;
        EventSystem.current.SetSelectedGameObject(returnButton.gameObject);

        selectSlotName = null;
    }
}
