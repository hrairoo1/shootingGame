using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static ColorManager;

public class ColorSelectWindow : MonoBehaviour
{
    [SerializeField] EquipSelectWindow selectWindow;
    [SerializeField] HSVColorPicker colorPicker;
    public string colorPoint;
    private bool _isColorSelect = false;
    public bool isColorSelect
    {
        get{ return _isColorSelect; }
        set { _isColorSelect = value; }
    }

    public GameObject EquipUI;
    public GameObject ui;
    public GameObject colorWindow;
    public GameObject saveColorWindow;
    public TextMeshProUGUI slotText;

    public Button AllButton;
    public Button MainRButton;
    public Button MainLButton;
    public Button SubRButton;
    public Button SubLButton;
    public Button ShoulderRButton;
    public Button ShoulderLButton;

    public Button HeadButton;
    public Button BodyButton;
    public Button ArmButton;
    public Button LegButton;
    public Button BackpackButton;

    public Button main1Button;
    public Button main2Button;
    public Button main3Button;
    public Button sub1Button;
    public Button sub2Button;
    public Button emi1Button;
    public Button emi2Button;

    public Button colorUIButton;
    public Button colorSliderButton;

    public Button saveButton;
    public Button notSaveButton;
    public Button cancelButton;

    [SerializeField] InputSystem_Actions controls;
    private InputAction nextWindow;
    private InputAction backWindow;

    public int slot = 0;
    public string slotname;
    public Dictionary<int, string> colorSlotName = new Dictionary<int, string>(){
        { 0, "MainR"},
        { 1, "MainL"},
        { 2, "SubR"},
        { 3, "SubL"},
        { 4, "ShoulderR"},
        { 5, "ShoulderL"},
        { 6, "Head"},
        { 7, "Body"},
        { 8, "Arm"},
        { 9, "Leg"},
        { 10, "Backpack"},
        { 11, "All"}
    };

    private void Start()
    {
        AllButton.onClick.AddListener(AllClicked);
        MainRButton.onClick.AddListener(MainRClicked);
        MainLButton.onClick.AddListener(MainLClicked);
        SubRButton.onClick.AddListener(SubRClicked);
        SubLButton.onClick.AddListener(SubLClicked);
        ShoulderRButton.onClick.AddListener(ShoulderRClicked);
        ShoulderLButton.onClick.AddListener(ShoulderLClicked);

        HeadButton.onClick.AddListener(HeadClicked);
        BodyButton.onClick.AddListener(BodyClicked);
        ArmButton.onClick.AddListener(ArmClicked);
        LegButton.onClick.AddListener(LegClicked);
        BackpackButton.onClick.AddListener(BackpackClicked);

        main1Button.onClick.AddListener(Main1Clicked);
        main2Button.onClick.AddListener(Main2Clicked);
        main3Button.onClick.AddListener(Main3Clicked);
        sub1Button.onClick.AddListener(Sub1Clicked);
        sub2Button.onClick.AddListener(Sub2Clicked);
        emi1Button.onClick.AddListener(Emi1Clicked);
        emi2Button.onClick.AddListener(Emi2Clicked);

        colorUIButton.onClick.AddListener(ColorUIClicked);
        colorSliderButton.onClick.AddListener(ColorSliderClicked);

        saveButton.onClick.AddListener(SaveClicked);
        notSaveButton.onClick.AddListener(NotSaveClicked);
        cancelButton.onClick.AddListener(CancelClicked);
        controls = new InputSystem_Actions();
        nextWindow = controls.UI.NextWindow;
        nextWindow.started += ctx => NextWindow();
        backWindow = controls.UI.BackWindow;
        backWindow.started += ctx => BackWindow();
        controls.Enable();
    }
    private void Update()
    {
        SetMaterial();
        slotText.text = colorSlotName[slot];
        colorPicker.setMat = colorPicker.materials[colorSlotName[slot]];
        if (colorPicker.isBoxUI || colorPicker.isSlider)
        {
        }
        if(colorPicker.setMat != null || slot == 11)
        {
            main1Button.interactable = true;
            main2Button.interactable = true;
            main3Button.interactable = true;
            sub1Button.interactable = true;
            sub2Button.interactable = true;
            emi1Button.interactable = true;
            emi2Button.interactable = true;
        }
        else
        {
            main1Button.interactable = true;
            main2Button.interactable = true;
            main3Button.interactable = true;
            sub1Button.interactable = true;
            sub2Button.interactable = true;
            emi1Button.interactable = true;
            emi2Button.interactable = true;
        }
        SlotButtonColorSet(main1Button);
        SlotButtonColorSet(main2Button);
        SlotButtonColorSet(main3Button);
        SlotButtonColorSet(sub1Button);
        SlotButtonColorSet(sub2Button);
        SlotButtonColorSet(emi1Button);
        SlotButtonColorSet(emi2Button);
    }
    private void AllClicked()
    {
        slot = 11;
        OpenSlot();
    }
    private void MainRClicked()
    {
        slot = 0;
        OpenSlot();
    }
    private void MainLClicked()
    {
        slot = 1;
        OpenSlot();
    }
    private void SubRClicked()
    {
        slot = 2;
        OpenSlot();
    }
    private void SubLClicked()
    {
        slot = 3;
        OpenSlot();
    }
    private void ShoulderRClicked()
    {
        slot = 4;
        OpenSlot();
    }
    private void ShoulderLClicked()
    {
        slot = 5;
        OpenSlot();
    }
    private void HeadClicked()
    {
        slot = 6;
        OpenSlot();
    }
    private void BodyClicked()
    {
        slot = 7;
        OpenSlot();
    }
    private void ArmClicked()
    {
        slot = 8;
        OpenSlot();
    }
    private void LegClicked()
    {
        slot = 9;
        OpenSlot();
    }
    private void BackpackClicked()
    {
        slot = 10;
        OpenSlot();
    }
    void OpenSlot()
    {
        EquipUI.SetActive(false);
        ui.SetActive(true);
        isColorSelect = true;
        EventSystem.current.SetSelectedGameObject(main1Button.gameObject);
    }
    void SetMaterial()
    {
        foreach (string slot in colorSlotName.Values)
        {
            if (LoadoutManager.Instance.weapon.ContainsKey(slot))
            {
                Transform mountPoint = selectWindow.Player.transform.Find($"WeaponHolder/{slot}");
                if (mountPoint.childCount > 0)
                {
                    Transform weapon = mountPoint.GetChild(0);
                    if (weapon != null)
                    {
                        colorPicker.materials[slot] = weapon.GetComponentInChildren<Renderer>().material;
                    }
                    else colorPicker.materials[slot] = null;
                }
                else colorPicker.materials[slot] = null;
            }
            else if (LoadoutManager.Instance.armor.ContainsKey(slot))
            {
                Transform mountPoint = selectWindow.Player.transform.Find($"ArmorHolder/{slot}");
                if (mountPoint.childCount > 0)
                {
                    Transform armor = mountPoint.GetChild(0);
                    if (armor != null)
                    {
                        colorPicker.materials[slot] = armor.GetComponentInChildren<Renderer>().material;
                    }
                    else colorPicker.materials[slot] = null;
                }
                else colorPicker.materials[slot] = null;
            }
            else colorPicker.materials[slot] = null;
        }
    }

    void NextWindow()
    {
        if (!ui.activeSelf) return;
        if (colorPicker.materials != null )
        {
            foreach (Material mat in colorPicker.materials.Values)
            {
                if(mat != null)
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
        }
        slot = (slot + 1) % colorSlotName.Count;
    }
    void BackWindow()
    {
        if (!ui.activeSelf) return;
        if (colorPicker.materials != null)
        {
            foreach (Material mat in colorPicker.materials.Values)
            {
                if (mat != null)
                {
                    Debug.Log(mat.name);
                    mat.SetFloat("_Main1B", 0);
                    mat.SetFloat("_Main2B", 0);
                    mat.SetFloat("_Main3B", 0);
                    mat.SetFloat("_Sub1B", 0);
                    mat.SetFloat("_Sub2B", 0);
                    mat.SetFloat("_Emi1B", 0);
                    mat.SetFloat("_Emi2B", 0);
                }
            }
        }
        slot = (slot - 1 + colorSlotName.Count) % colorSlotName.Count;
    }

    void Main1Clicked()
    {
        colorPoint = "Main1";
        OpenColorSelect();
    }
    void Main2Clicked()
    {
        colorPoint = "Main2";
        OpenColorSelect();
    }
    void Main3Clicked()
    {
        colorPoint = "Main3";
        OpenColorSelect();
    }
    void Sub1Clicked()
    {
        colorPoint = "Sub1";
        OpenColorSelect();
    }
    void Sub2Clicked()
    {
        colorPoint = "Sub2";
        OpenColorSelect();
    }
    void Emi1Clicked()
    {
        colorPoint = "Emi1";
        OpenColorSelect();
    }
    void Emi2Clicked()
    {
        colorPoint = "Emi2";
        OpenColorSelect();
    }
    void SaveClicked()
    {
        if (slot != 11)
        {
            if (colorPoint == "Main1")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor1 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity1 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Main2")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor2 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity2 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Main3")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor3 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity3 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Sub1")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor1 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].subIntencity1 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Sub2")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor2 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].subIntencity2 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Emi1")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor1 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].emiIntencity1 = colorPicker.GetIntencity();
            }
            if (colorPoint == "Emi2")
            {
                LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor2 = colorPicker.GetColor();
                LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].emiIntencity2 = colorPicker.GetIntencity();
            }
        }
        else
        {
            Debug.Log("else");
            foreach(string slotName in colorSlotName.Values)
            {
                if (slotName == "All") break;
                if (colorPoint == "Main1")
                {
                    LoadoutManager.Instance.equipColor[slotName]._MainColor1 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].mainIntencity1 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Main2")
                {
                    LoadoutManager.Instance.equipColor[slotName]._MainColor2 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].mainIntencity2 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Main3")
                {
                    LoadoutManager.Instance.equipColor[slotName]._MainColor3 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].mainIntencity3 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Sub1")
                {
                    LoadoutManager.Instance.equipColor[slotName]._SubColor1 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].subIntencity1 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Sub2")
                {
                    LoadoutManager.Instance.equipColor[slotName]._SubColor2 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].subIntencity2 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Emi1")
                {
                    LoadoutManager.Instance.equipColor[slotName]._EmiColor1 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].emiIntencity1 = colorPicker.GetIntencity();
                }
                if (colorPoint == "Emi2")
                {
                    LoadoutManager.Instance.equipColor[slotName]._EmiColor2 = colorPicker.GetColor();
                    LoadoutManager.Instance.equipIntencity[slotName].emiIntencity2 = colorPicker.GetIntencity();
                }
            }
        }
        saveColorWindow.SetActive(false);
        CloseColorSelect();
        Debug.Log("End");
        //Material mat = GetComponent<Material>();
        //mat.SetColor("color", color);
    }
    void NotSaveClicked()
    {
        saveColorWindow.SetActive(false);
        if (slot != 11)
        {

            if (colorPicker.setMat != null)
            {
                if (colorPoint == "Main1")
                {
                    colorPicker.setMat.SetColor("_MainColor1", colorPicker.currentColor);
                    colorPicker.setMat.SetColor("_MainColorEmi1", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_MainSt1", colorPicker.currentIntencity);
                }
                if (colorPoint == "Main2")
                {
                    colorPicker.setMat.SetColor("_MainColor2", colorPicker.currentColor);
                    colorPicker.setMat.SetColor("_MainColorEmi2", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_MainSt2", colorPicker.currentIntencity);
                }
                if (colorPoint == "Main3")
                {
                    colorPicker.setMat.SetColor("_MainColor3", colorPicker.currentColor);
                    colorPicker.setMat.SetColor("_MainColorEmi3", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_MainSt3", colorPicker.currentIntencity);
                }
                if (colorPoint == "Sub1")
                {
                    colorPicker.setMat.SetColor("_SubColor1", colorPicker.currentColor);
                    colorPicker.setMat.SetColor("_SubColorEmi1", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_SubSt1", colorPicker.currentIntencity);
                }
                if (colorPoint == "Sub2")
                {
                    colorPicker.setMat.SetColor("_SubColor2", colorPicker.currentColor);
                    colorPicker.setMat.SetColor("_SubColorEmi2", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_SubSt2", colorPicker.currentIntencity);
                }
                if (colorPoint == "Emi1")
                {
                    colorPicker.setMat.SetColor("_EmiColor1", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_EmiSt1", colorPicker.currentIntencity);
                }
                if (colorPoint == "Emi2")
                {
                    colorPicker.setMat.SetColor("_EmiColor2", colorPicker.currentColor);
                    colorPicker.setMat.SetFloat("_EmiStr2", colorPicker.currentIntencity);
                }
            }
        }
        else
        {
            foreach(string slotName in colorSlotName.Values)
            {
                if (slotName == "All") break;
                if (colorPicker.materials[slotName] != null)
                {
                    if (colorPoint == "Main1")
                    {
                        colorPicker.materials[slotName].SetColor("_MainColor1", LoadoutManager.Instance.equipColor[slotName]._MainColor1);
                        colorPicker.materials[slotName].SetColor("_MainColorEmi1", LoadoutManager.Instance.equipColor[slotName]._MainColor1);
                        colorPicker.materials[slotName].SetFloat("_MainSt1", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity1);
                    }
                    if (colorPoint == "Main2")
                    {
                        colorPicker.materials[slotName].SetColor("_MainColor2", LoadoutManager.Instance.equipColor[slotName]._MainColor2);
                        colorPicker.materials[slotName].SetColor("_MainColorEmi2", LoadoutManager.Instance.equipColor[slotName]._MainColor2);
                        colorPicker.materials[slotName].SetFloat("_MainSt2", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity2);
                    }
                    if (colorPoint == "Main3")
                    {
                        colorPicker.materials[slotName].SetColor("_MainColor3", LoadoutManager.Instance.equipColor[slotName]._MainColor3);
                        colorPicker.materials[slotName].SetColor("_MainColorEmi3", LoadoutManager.Instance.equipColor[slotName]._MainColor3);
                        colorPicker.materials[slotName].SetFloat("_MainSt3", LoadoutManager.Instance.equipIntencity[slotName].mainIntencity3);
                    }
                    if (colorPoint == "Sub1")
                    {
                        colorPicker.materials[slotName].SetColor("_SubColor1", LoadoutManager.Instance.equipColor[slotName]._SubColor1);
                        colorPicker.materials[slotName].SetColor("_SubColorEmi1", LoadoutManager.Instance.equipColor[slotName]._SubColor1);
                        colorPicker.materials[slotName].SetFloat("_SubSt1", LoadoutManager.Instance.equipIntencity[slotName].emiIntencity1);
                    }
                    if (colorPoint == "Sub2")
                    {
                        colorPicker.materials[slotName].SetColor("_SubColor2", LoadoutManager.Instance.equipColor[slotName]._SubColor2);
                        colorPicker.materials[slotName].SetColor("_SubColorEmi2", LoadoutManager.Instance.equipColor[slotName]._SubColor2);
                        colorPicker.materials[slotName].SetFloat("_SubSt2", LoadoutManager.Instance.equipIntencity[slotName].subIntencity2);
                    }
                    if (colorPoint == "Emi1")
                    {
                        colorPicker.materials[slotName].SetColor("_EmiColor1", LoadoutManager.Instance.equipColor[slotName]._EmiColor1);
                        colorPicker.materials[slotName].SetFloat("_EmiSt1", LoadoutManager.Instance.equipIntencity[slotName].emiIntencity1);
                    }
                    if (colorPoint == "Emi2")
                    {
                        colorPicker.materials[slotName].SetColor("_EmiColor2", LoadoutManager.Instance.equipColor[slotName]._EmiColor2);
                        colorPicker.materials[slotName].SetFloat("_EmiStr2", LoadoutManager.Instance.equipIntencity[slotName].emiIntencity2);
                    }
                }
            }
        }
        CloseColorSelect();
    }
    void CancelClicked()
    {
        saveColorWindow.SetActive(false);
    }

    void OpenColorSelect()
    {
        EventSystem.current.SetSelectedGameObject(colorUIButton.gameObject);
        Color color = new Color();
        float intencity = new float();
        if (slot != 11)
        {
            if (colorPoint == "Main1")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor1;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity1;
            }
            if (colorPoint == "Main2")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor2;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity2;
            }
            if (colorPoint == "Main3")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor3;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].mainIntencity3;
            }
            if (colorPoint == "Sub1")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor1;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].subIntencity1;
            }
            if (colorPoint == "Sub2")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor2;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].subIntencity2;
            }
            if (colorPoint == "Emi1")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor1;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].emiIntencity1;
            }
            if (colorPoint == "Emi2")
            {
                color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor2;
                intencity = LoadoutManager.Instance.equipIntencity[colorSlotName[slot]].emiIntencity2;
            }
            if(colorPicker.setMat != null)
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
        }
        else
        {
            if (colorPoint == "Main1")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._MainColor1;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].mainIntencity1;
            }
            if (colorPoint == "Main2")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._MainColor2;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].mainIntencity2;
            }
            if (colorPoint == "Main3")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._MainColor3;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].mainIntencity3;
            }
            if (colorPoint == "Sub1")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._SubColor1;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].subIntencity1;
            }
            if (colorPoint == "Sub2")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._SubColor2;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].subIntencity2;
            }
            if (colorPoint == "Emi1")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._EmiColor1;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].emiIntencity1;
            }
            if (colorPoint == "Emi2")
            {
                color = LoadoutManager.Instance.equipColor["Head"]._EmiColor2;
                intencity = LoadoutManager.Instance.equipIntencity["Head"].emiIntencity2;
            }
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
        }
        if (color != null)
        {
            colorPicker.currentColor = color;
            colorPicker.currentIntencity = intencity;
            colorPicker.SetHSVColor(color, intencity);
        }
        colorWindow.SetActive(true);
        ui.SetActive(false);
    }

    public void SaveChangeColor()
    {
        if (colorPicker.GetColor() != colorPicker.currentColor || colorPicker.GetIntencity() != colorPicker.currentIntencity)
        {
            EventSystem.current.SetSelectedGameObject(cancelButton.gameObject);
            saveColorWindow.SetActive(true);
        }
        else CloseColorSelect();
    }
    public void CloseColorSelect()
    {
        GameObject btn = new GameObject();
        if (colorPoint == "Main1") btn = main1Button.gameObject;
        if (colorPoint == "Main2") btn = main2Button.gameObject;
        if (colorPoint == "Main3") btn = main3Button.gameObject;
        if (colorPoint == "Sub1") btn = sub1Button.gameObject;
        if (colorPoint == "Sub2") btn = sub2Button.gameObject;
        if (colorPoint == "Emi1") btn = emi1Button.gameObject;
        if (colorPoint == "Emi2") btn = emi2Button.gameObject;
        EventSystem.current.SetSelectedGameObject(btn);
        colorWindow.SetActive(false);
        ui.SetActive(true);
        colorPicker.isBoxUI = false;
        colorPicker.isSlider = false;
    }


    void ColorUIClicked()
    {
        colorPicker.isBoxUI = true;
        colorPicker.isSlider = false;
        EventSystem.current.SetSelectedGameObject(null);
    }
    void ColorSliderClicked()
    {
        colorPicker.isBoxUI = false;
        colorPicker.isSlider = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    //スロットボタンのカラーパレット変更
    void SlotButtonColorSet(Button btn)
    {
        Transform button = btn.transform;
        Transform mainColor1Image = button.Find("ColorSet");
        Image color = mainColor1Image.GetComponent<Image>();
        if (slot != 11)
        {
            if (btn.name == "Main1") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor1;
            if (btn.name == "Main2") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor2;
            if (btn.name == "Main3") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor3;
            if (btn.name == "Sub1") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor1;
            if (btn.name == "Sub2") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor2;
            if (btn.name == "Emi1") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor1;
            if (btn.name == "Emi2") color.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor2;
        }
        else
        {
            if (btn.name == "Main1") color.color = LoadoutManager.Instance.equipColor["Head"]._MainColor1;
            if (btn.name == "Main2") color.color = LoadoutManager.Instance.equipColor["Head"]._MainColor2;
            if (btn.name == "Main3") color.color = LoadoutManager.Instance.equipColor["Head"]._MainColor3;
            if (btn.name == "Sub1") color.color = LoadoutManager.Instance.equipColor["Head"]._SubColor1;
            if (btn.name == "Sub2") color.color = LoadoutManager.Instance.equipColor["Head"]._SubColor2;
            if (btn.name == "Emi1") color.color = LoadoutManager.Instance.equipColor["Head"]._EmiColor1;
            if (btn.name == "Emi2") color.color = LoadoutManager.Instance.equipColor["Head"]._EmiColor2;
        }
    }
    void EquipButtonColorSet(Button btn)
    {
        Transform button = btn.transform;
        Transform mainColor1Image = button.Find("ColorSet/Main1");
        Transform mainColor2Image = button.Find("ColorSet/Main2");
        Transform mainColor3Image = button.Find("ColorSet/Main3");
        Transform subColor1Image = button.Find("ColorSet/MSub1");
        Transform subColor2Image = button.Find("ColorSet/Sub2");
        Transform emiColor1Image = button.Find("ColorSet/Emi1");
        Transform emiColor2Image = button.Find("ColorSet/Emi2");
        Image mainColor1 = mainColor1Image.GetComponent<Image>();
        mainColor1.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor1;
        Image mainColor2 = mainColor2Image.GetComponent<Image>();
        mainColor2.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor2;
        Image mainColor3 = mainColor3Image.GetComponent<Image>();
        mainColor3.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._MainColor3;
        Image subColor1 = subColor1Image.GetComponent<Image>();
        subColor1.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor1;
        Image subColor2 = subColor2Image.GetComponent<Image>();
        subColor2.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._SubColor2;
        Image emiColor1 = emiColor1Image.GetComponent<Image>();
        subColor1.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor1;
        Image emiColor2 = emiColor2Image.GetComponent<Image>();
        emiColor2.color = LoadoutManager.Instance.equipColor[colorSlotName[slot]]._EmiColor2;
    }
}

