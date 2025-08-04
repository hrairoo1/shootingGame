using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting;
using UnityEngine.Localization.Components;

public class EquipSelectionUI : MonoBehaviour
{
    [SerializeField] EquipSelectWindow equipSelectWindow;
    [SerializeField] EquipWeaponManager weaponManager;
    [SerializeField] EquipArmorManager armorManager;
    [SerializeField] WeaponSelector selector;
    [SerializeField] ScrollToSelected scrollToSelected;
    [SerializeField] private Transform categoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform mainCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform shoulderCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform shoulder2CategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform headCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform bodyCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform armCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform legCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)
    public Transform backpackCategoryContainer; // �J�e�S���{�^���̐e (HorizontalLayoutGroup)

    public Transform mainListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform shoulderListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform shoulder2ListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform headListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform bodyListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform armListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform legListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    public Transform backpackListContainer; // ���탊�X�g�̐e (VerticalLayoutGroup)
    [SerializeField] private GameObject categoryButtonPrefab; // �J�e�S���{�^���̃v���n�u
    [SerializeField] private GameObject weaponListNoneButtonPrefab; // �J�e�S���{�^���̃v���n�u
    [SerializeField] private GameObject weaponButtonPrefab; // ����{�^���̃v���n�u
    [SerializeField] private TMP_Text modelNumberText; // �ڍו\���G���A
    [SerializeField] private TMP_Text nameText; // �ڍו\���G���A
    [SerializeField] private TMP_Text descriptionText; // �ڍו\���G���A
    public GameObject detail;
    private Dictionary<string, Transform> mainCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> shoulderCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> shoulder2CategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> headCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> bodyCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> armCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> legCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> backpackCategoryContainers = new Dictionary<string, Transform>();
    private List<string> categoryMainOrder = new List<string> { "Assault Rifle", "Rocket", "Sniper", "Missile", "Canon", "xxx", "a", "b", "c", "d" }; // �J�e�S���̕��я�
    private List<string> categoryShoulderOrder = new List<string> { "shoulder Missile", "Huge Canon" }; // �J�e�S���̕��я�
    private List<string> categoryShoulder2Order = new List<string> { "shoulder Missile", "Missile" }; // �J�e�S���̕��я�
    private List<string> categoryHeadOrder = new List<string> { "Head_Light", "Head_Medium", "Head_Heavy" }; // �J�e�S���̕��я�
    private List<string> categoryBodyOrder = new List<string> { "Body_Light", "Body_Medium", "Body_Heavy" }; // �J�e�S���̕��я�
    private List<string> categoryArmOrder = new List<string> { "Arm_Light", "Arm_Medium", "Arm_Heavy" }; // �J�e�S���̕��я�
    private List<string> categoryLegOrder = new List<string> { "Leg_Light", "Leg_Medium", "Leg_Heavy" }; // �J�e�S���̕��я�
    private List<string> categoryBackpackOrder = new List<string> { "Backpack_Light", "Backpack_Medium", "Backpack_Heavy" }; // �J�e�S���̕��я�
    private Dictionary<string, GameObject> buttonDictionary = new Dictionary<string, GameObject>();
    private List<GameObject> mainButtons = new List<GameObject>();
    private List<GameObject> shoulderButtons = new List<GameObject>();
    private List<GameObject> shoulder2Buttons = new List<GameObject>();
    private List<GameObject> headButtons = new List<GameObject>();
    private List<GameObject> bodyButtons = new List<GameObject>();
    private List<GameObject> armButtons = new List<GameObject>();
    private List<GameObject> legButtons = new List<GameObject>();
    private List<GameObject> backpackButtons = new List<GameObject>();
    private List<RectTransform> buttonsRect = new List<RectTransform>();
    public string targetWeaponName = "3"; // �����Ɍ������������햼��ݒ�
    private RectTransform categoryPanel;
    private Vector3 weaponPanel;
    public RectTransform content;  // Content��RectTransform

    private Vector3 categoryTargetPosition; // �ڕW�ʒu�i�X���C�h������ʒu�j
    private Vector3 weaponTargetPosition; // �ڕW�ʒu�i�X���C�h������ʒu�j
    public GameObject DefaltfirstSelectedButton; // �ŏ��ɑI������{�^��
    private GameObject firstSelectedButton;
    private GameObject currentSelectedButton;

    public GameObject statusTextPrefab;
    public Transform contentParent; // UI�z�u�ꏊ (Vertical Layout Group)
    private List<GameObject> generatedUIElements = new List<GameObject>(); // ��������UI�v�f��ێ����郊�X�g// ����̕ϐ������w��i�����̂ݕ\���j
    private List<string> variablesToDisplay = new List<string> { "level", "equipName","lockOnSize_x","id", "modelNumber", "weight", "ammoCount"};


    public void SetSelectUI()
    {
        equipSelectWindow.lastMousePosition = Input.mousePosition;
        GenerateCategoryButtons("MainWeapon");
        mainCategoryContainer.gameObject.SetActive(false);
        shoulderCategoryContainer.gameObject.SetActive(false);
        headCategoryContainer.gameObject.SetActive(false);
        bodyCategoryContainer.gameObject.SetActive(false);
        armCategoryContainer.gameObject.SetActive(false);
        legCategoryContainer.gameObject.SetActive(false);
        backpackCategoryContainer.gameObject.SetActive(false);
        mainListContainer.gameObject.SetActive(false);
        shoulderListContainer.gameObject.SetActive(false);
        headListContainer.gameObject.SetActive(false);
        bodyListContainer.gameObject.SetActive(false);
        armListContainer.gameObject.SetActive(false);
        legListContainer.gameObject.SetActive(false);
        backpackListContainer.gameObject.SetActive(false);
    }
    private void Start()
    {
        categoryPanel = categoryContainer.GetComponent<RectTransform>();
        categoryTargetPosition = categoryPanel.position;
        //ChangeCategory("Sniper");
    }

    void Update()
    {
        AdjustNavigation();
        // �J�e�S���X���C�h
        categoryPanel.position = new Vector2(content.position.x, categoryTargetPosition.y);

        /*if (weaponListContainer.position != weaponTargetPosition)
        {
            weaponListContainer.position = Vector3.Lerp(weaponListContainer.position, weaponTargetPosition, categorySlideSpeed * Time.deltaTime);
        }*/
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (currentSelectedButton != EventSystem.current.currentSelectedGameObject) {
                scrollToSelected.ScrollToButton(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
            }
            currentSelectedButton = EventSystem.current.currentSelectedGameObject;
            ShowWeaponDetails(currentSelectedButton.GetComponent<EquipSelectButton>());
        }
        EventSystem.current.SetSelectedGameObject(currentSelectedButton);
    }

    void GenerateCategoryButtons(string equipType)
    {
        //ClearButton();

        CreateCategory(categoryMainOrder, mainCategoryContainer, mainListContainer, mainCategoryContainers);
        CreateCategory(categoryShoulderOrder, shoulderCategoryContainer, shoulderListContainer, shoulderCategoryContainers);
        //CreateCategory(categoryShoulder2Order, shoulder2CategoryContainer, shoulder2ListContainer, shoulder2CategoryContainers);
        CreateCategory(categoryHeadOrder, headCategoryContainer, headListContainer, headCategoryContainers);
        CreateCategory(categoryBodyOrder, bodyCategoryContainer, bodyListContainer, bodyCategoryContainers);
        CreateCategory(categoryArmOrder, armCategoryContainer, armListContainer, armCategoryContainers);
        CreateCategory(categoryLegOrder, legCategoryContainer, legListContainer, legCategoryContainers);
        CreateCategory(categoryBackpackOrder, backpackCategoryContainer, backpackListContainer, backpackCategoryContainers);
        CreateButtons(mainCategoryContainers);
        CreateButtons(shoulderCategoryContainers);
        //CreateButtons(shoulder2CategoryContainers);
        CreateButtons(headCategoryContainers);
        CreateButtons(bodyCategoryContainers);
        CreateButtons(armCategoryContainers);
        CreateButtons(legCategoryContainers);
        CreateButtons(backpackCategoryContainers);

        if (buttonDictionary.TryGetValue(targetWeaponName, out GameObject targetButton))
        {
            //firstSelectedButton = targetButton;
        }
        //EventSystem.current.SetSelectedGameObject((firstSelectedButton != null) ? firstSelectedButton : DefaltfirstSelectedButton);
        ContentSizeFitter _contentSizeFitter = content.GetComponent<ContentSizeFitter>();
        _contentSizeFitter.SetLayoutHorizontal();
        _contentSizeFitter.SetLayoutVertical();

        //���C�A�E�g�𑦎��X�V
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentSizeFitter.GetComponent<RectTransform>());

        //categoryTargetPosition = categoryPanel.position;
    }
    void CreateCategory(List<string> categoryOrder, Transform container, Transform listContainer, Dictionary<string, Transform> categoryWeaponContainers)
    {
        foreach (var category in categoryOrder)
        {
            // �J�e�S�����Ƃ̕��탊�X�g�p�I�u�W�F�N�g�쐬
            GameObject weaponContainer = new GameObject($"{category}_Weapons");
            // �J�e�S���{�^������
            GameObject categoryButtonObj = Instantiate(categoryButtonPrefab, container);
            GameObject categoryButtonObjw = Instantiate(weaponListNoneButtonPrefab, weaponContainer.transform);
            TMP_Text categoryText = categoryButtonObj.GetComponentInChildren<TMP_Text>();

            // LocalizeStringEvent ��ǉ����Ė|��
            LocalizeStringEvent localizeEvent = categoryText.AddComponent<LocalizeStringEvent>();

            // �e�[�u�����ƃL�[��ݒ�
            localizeEvent.StringReference.TableReference = "MainTable";
            localizeEvent.StringReference.TableEntryReference = category;

            // �|�󂵂�������� Text �ɓK�p
            localizeEvent.OnUpdateString.AddListener((translatedText) =>
            {
                categoryText.text = translatedText;
            });

            weaponContainer.transform.SetParent(listContainer);
            VerticalLayoutGroup verticalLayoutGroup = weaponContainer.AddComponent<VerticalLayoutGroup>(); // �c����// VerticalLayoutGroup�̐ݒ�ύX
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;
            //weaponContainer.SetActive(false);
            categoryWeaponContainers[category] = weaponContainer.transform;
        }
    }
    void CreateButtons(Dictionary<string, Transform> categoryWeaponContainers)
    {
        // ����{�^���ǉ�
        foreach (var weapon in GameSettings.Instance.weaponEntries)
        {
            if (categoryWeaponContainers.ContainsKey(weapon.category))
            {
                GameObject buttonObj = Instantiate(weaponButtonPrefab, categoryWeaponContainers[weapon.category]);
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    // LocalizeStringEvent ��ǉ����Ė|��
                    LocalizeStringEvent localizeEvent = buttonText.AddComponent<LocalizeStringEvent>();

                    localizeEvent.StringReference.TableReference = "Weapon Table";
                    localizeEvent.StringReference.TableEntryReference = weapon.id;

                    // �|�󂵂�������� Text �ɓK�p
                    localizeEvent.OnUpdateString.AddListener((translatedText) =>
                    {
                        buttonText.text = translatedText + " " + weapon.level;
                        ;
                    });
                }
                EquipSelectButton buttonEntry = buttonObj.GetComponent<EquipSelectButton>();
                buttonObj.name = weapon.name;
                buttonEntry.id = weapon.id;
                buttonEntry.modelNumber = weapon.weapon.modelNumber;
                buttonEntry.equipName = weapon.name;
                buttonEntry.level = weapon.level;
                buttonEntry.description = weapon.description;
                buttonEntry.category = weapon.category;
                buttonEntry.path = weapon.path;
                buttonEntry.weapon = weapon.weapon;
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    if (categoryWeaponContainers == mainCategoryContainers)
                    {
                        mainButtons.Add(buttonObj);
                    }
                    else if (categoryWeaponContainers == shoulderCategoryContainers)
                    {
                        shoulderButtons.Add(buttonObj);
                    }
                    // �C�x���g�g���K�[�̒ǉ�
                    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
                    Debug.Log(weapon.name);
                    // PointerEnter�C�x���g�̐ݒ�
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter; // �}�E�X���{�^���ɏ�����Ƃ�
                    entry.callback.AddListener((data) =>
                    {
                        // �}�E�X���{�^���ɏ�������̏���
                        OnButtonHovered(buttonEntry, buttonObj);
                    });
                    trigger.triggers.Add(entry);
                    button.onClick.AddListener(() => SetWeapon(weapon));
                }
                // ���햼���L�[�Ɏ����ɓo�^
                buttonDictionary[weapon.id] = buttonObj;
                buttonsRect.Add(buttonObj.GetComponent<RectTransform>());
                if (DefaltfirstSelectedButton == null)
                {
                    DefaltfirstSelectedButton = buttonObj;
                }
            }
        }
        // �h��{�^���ǉ�
        foreach (var armor in GameSettings.Instance.armorEntries)
        {
            if (categoryWeaponContainers.ContainsKey(armor.category))
            {
                GameObject buttonObj = Instantiate(weaponButtonPrefab, categoryWeaponContainers[armor.category]);
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                // LocalizeStringEvent ��ǉ����Ė|��
                LocalizeStringEvent localizeEvent = buttonText.AddComponent<LocalizeStringEvent>();

                localizeEvent.StringReference.TableReference = "Weapon Table";
                localizeEvent.StringReference.TableEntryReference = armor.id;

                // �|�󂵂�������� Text �ɓK�p
                localizeEvent.OnUpdateString.AddListener((translatedText) =>
                {
                    buttonText.text = translatedText + " " + armor.level;
                    ;
                });
                EquipSelectButton buttonEntry = buttonObj.GetComponent<EquipSelectButton>();
                buttonObj.name = armor.name;
                buttonEntry.id = armor.id;
                buttonEntry.modelNumber = armor.armor.modelNumber;
                buttonEntry.equipName = armor.name;
                buttonEntry.level = armor.level;
                buttonEntry.description = armor.description;
                buttonEntry.category = armor.category;
                buttonEntry.path = armor.path;
                buttonEntry.armor = armor.armor;
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    if (categoryWeaponContainers == headCategoryContainers)
                    {
                        headButtons.Add(buttonObj);
                    }
                    else if (categoryWeaponContainers == bodyCategoryContainers)
                    {
                        bodyButtons.Add(buttonObj);
                    }
                    else if (categoryWeaponContainers == armCategoryContainers)
                    {
                        armButtons.Add(buttonObj);
                    }
                    else if (categoryWeaponContainers == legCategoryContainers)
                    {
                        legButtons.Add(buttonObj);
                    }
                    else if (categoryWeaponContainers == backpackCategoryContainers)
                    {
                        backpackButtons.Add(buttonObj);
                    }
                    // �C�x���g�g���K�[�̒ǉ�
                    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
                    Debug.Log(armor.name);
                    // PointerEnter�C�x���g�̐ݒ�
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter; // �}�E�X���{�^���ɏ�����Ƃ�
                    entry.callback.AddListener((data) =>
                    {
                        // �}�E�X���{�^���ɏ�������̏���
                        OnButtonHovered(buttonEntry, buttonObj);
                    });
                    trigger.triggers.Add(entry);
                    button.onClick.AddListener(() => SetWeapon(armor));
                }
                // ���햼���L�[�Ɏ����ɓo�^
                buttonDictionary[armor.id] = buttonObj;
                buttonsRect.Add(buttonObj.GetComponent<RectTransform>());
                if (DefaltfirstSelectedButton == null)
                {
                    DefaltfirstSelectedButton = buttonObj;
                }
            }
        }
    }

    public void BackWindow()
    {
        if (weaponManager.weaponSlots.ContainsKey(equipSelectWindow.selectSlotName))
        {
            weaponManager.SetWeaponSlot(equipSelectWindow.selectSlotName, null);
        }
        if (armorManager.armorSlots.ContainsKey(equipSelectWindow.selectSlotName))
        {
            armorManager.SetArmorSlot(equipSelectWindow.selectSlotName, null);
        }
        equipSelectWindow.ActiveSubShoulderButton();
        gameObject.SetActive(false);
        equipSelectWindow.weaponSelectUI.SetActive(false);
        equipSelectWindow.armorSelectUI.SetActive(false);
        mainCategoryContainer.gameObject.SetActive(false);
        shoulderCategoryContainer.gameObject.SetActive(false);
        headCategoryContainer.gameObject.SetActive(false);
        bodyCategoryContainer.gameObject.SetActive(false);
        armCategoryContainer.gameObject.SetActive(false);
        legCategoryContainer.gameObject.SetActive(false);
        backpackCategoryContainer.gameObject.SetActive(false);
        mainListContainer.gameObject.SetActive(false);
        shoulderListContainer.gameObject.SetActive(false);
        headListContainer.gameObject.SetActive(false);
        bodyListContainer.gameObject.SetActive(false);
        armListContainer.gameObject.SetActive(false);
        legListContainer.gameObject.SetActive(false);
        backpackListContainer.gameObject.SetActive(false);
        if (equipSelectWindow.selectSlotName == "MainR" ||
        equipSelectWindow.selectSlotName == "MainL" ||
        equipSelectWindow.selectSlotName == "SubR" ||
        equipSelectWindow.selectSlotName == "SubL" ||
        equipSelectWindow.selectSlotName == "ShoulderR" ||
        equipSelectWindow.selectSlotName == "ShoulderL") equipSelectWindow.weaponSelectUI.SetActive(true);
        else equipSelectWindow.armorSelectUI.SetActive(true);
        equipSelectWindow.selectSlotName = null;
        if (equipSelectWindow.selectSlotName == "MainR") EventSystem.current.SetSelectedGameObject(equipSelectWindow.MainRButton.gameObject);
        if (equipSelectWindow.selectSlotName == "MainL") EventSystem.current.SetSelectedGameObject(equipSelectWindow.MainLButton.gameObject);
        if (equipSelectWindow.selectSlotName == "SubR")
        {
            EventSystem.current.SetSelectedGameObject(equipSelectWindow.SubRButton.gameObject);
            weaponManager.UpdateWeaponVisibility(false);
        }
        if (equipSelectWindow.selectSlotName == "SubL")
        {
            EventSystem.current.SetSelectedGameObject(equipSelectWindow.SubLButton.gameObject);
            weaponManager.UpdateWeaponVisibility(false);
        }
        if (equipSelectWindow.selectSlotName == "ShoulderR") EventSystem.current.SetSelectedGameObject(equipSelectWindow.ShoulderRButton.gameObject);
        if (equipSelectWindow.selectSlotName == "ShoulderL") EventSystem.current.SetSelectedGameObject(equipSelectWindow.ShoulderLButton.gameObject);
        if (equipSelectWindow.selectSlotName == "Head") EventSystem.current.SetSelectedGameObject(equipSelectWindow.HeadButton.gameObject);
        if (equipSelectWindow.selectSlotName == "Body") EventSystem.current.SetSelectedGameObject(equipSelectWindow.BodyButton.gameObject);
        if (equipSelectWindow.selectSlotName == "Arm") EventSystem.current.SetSelectedGameObject(equipSelectWindow.ArmButton.gameObject);
        if (equipSelectWindow.selectSlotName == "Leg") EventSystem.current.SetSelectedGameObject(equipSelectWindow.LegButton.gameObject);
        if (equipSelectWindow.selectSlotName == "Backpack") EventSystem.current.SetSelectedGameObject(equipSelectWindow.BackpackButton.gameObject);
    }

    void SetWeapon<T>(T entry)
    {
        if (entry is WeaponListEntry weapon)
        {
            selector.SelectWeapon(weapon.id, equipSelectWindow.selectSlotName);
        }
        else if (entry is ArmorListEntry armor)
        {
            selector.SelectArmor(armor.id, equipSelectWindow.selectSlotName);
        }
    }
    void ShowWeaponDetails(EquipSelectButton equip)
    {
        if (equip == null) return; 
        LocalizeStringEvent nameTextlocalizeEvent = nameText.gameObject.GetComponent<LocalizeStringEvent>();
        if(nameTextlocalizeEvent == null) nameTextlocalizeEvent = nameText.gameObject.AddComponent<LocalizeStringEvent>();

        if (nameText.name != equip.equipName)
        {
            modelNumberText.text = equip.modelNumber;
            DisplayNonEmptyValues(equip);
            nameText.name = equip.equipName;
            descriptionText.text = equip.description;
            nameTextlocalizeEvent.StringReference.TableReference = "Weapon Table";
            nameTextlocalizeEvent.StringReference.TableEntryReference = equip.id;

            // �|�󂵂�������� Text �ɓK�p
            nameTextlocalizeEvent.OnUpdateString.AddListener((translatedText) =>
            {
                nameText.text = translatedText;
            });
            // �����X�V
            nameTextlocalizeEvent.RefreshString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(detail.GetComponent<RectTransform>());
            if (weaponManager.weaponSlots.ContainsKey(equipSelectWindow.selectSlotName))
            {
                weaponManager.SetWeaponSlot(equipSelectWindow.selectSlotName, equip.weapon.weaponModel);
            }
            if (armorManager.armorSlots.ContainsKey(equipSelectWindow.selectSlotName))
            {
                armorManager.SetArmorSlot(equipSelectWindow.selectSlotName, equip.armor.armorModel);
            }
        }
    }
    // �Â�UI���폜���ĐV������������
    public void DisplayNonEmptyValues(EquipSelectButton equip)
    {
        // ��������UI�v�f���폜
        foreach (var uiElement in generatedUIElements)
        {
            Destroy(uiElement);
        }
        generatedUIElements.Clear(); // ���X�g���N���A

        // A�X�N���v�g���̂��ׂẴt�B�[���h���擾
        var fields = equip.GetType().GetFields();

        int column = 0;
        GameObject row = CreateRow(); // ����s����

        foreach (var variableName in variablesToDisplay)
        {
            object value = GetVariableValue(equip, variableName);

            // null �܂��� 0 �̏ꍇ�̓X�L�b�v
            if (value == null ||
                (value is int intValue && intValue == 0) ||
                (value is float floatValue && floatValue == 0) ||  // ���̐��l�^���`�F�b�N
                (value is double doubleValue && doubleValue == 0) ||
                (value is string strValue && string.IsNullOrEmpty(strValue)))
            {
                continue;
            }

            // �ϐ����̕\��
            GameObject statusText = Instantiate(statusTextPrefab, row.transform);
            GameObject verNameText = statusText.transform.Find("name").gameObject;
            GameObject valueText = statusText.transform.Find("var").gameObject;
            verNameText.GetComponent<TextMeshProUGUI>().text = variableName; // �ϐ�����\��
            Debug.Log(variableName);
            if (variableName == "lockOnSize_x")
            {
                object lockonsizeY = GetVariableValue(equip, "lockOnSize_y");
                valueText.GetComponent<TextMeshProUGUI>().text = value.ToString() + "," + lockonsizeY.ToString(); // �l��\��
            }
            else valueText.GetComponent<TextMeshProUGUI>().text = value.ToString(); // �l��\��

            // ��2�񂲂ƂɐV�����s���쐬
            column++;
            if (column >= 2)
            {
                row = CreateRow();
                column = 0;
            }

            // ��������UI�v�f�����X�g�ɒǉ�
            generatedUIElements.Add(statusText);
            generatedUIElements.Add(row);
        }
    }// �ϐ��̒l���擾���A�����̎Q�ƌ^���ċA�I�ɒT��
    private object GetVariableValue(object obj, string variableName, int depth = 0)
    {
        if (obj == null || depth > 5) // �������[�v�h�~ (�ő�5�K�w�܂�)
            return null;

        var field = obj.GetType().GetField(variableName);
        if (field != null)
        {
            var value = field.GetValue(obj);

            // �Q�ƌ^�̒��g���ċA�I�Ƀ`�F�b�N
            if (value != null && field.FieldType.IsClass && field.FieldType != typeof(string))
            {
                // �ċA�T���ł���ɒ��̕ϐ����m�F
                return SearchNestedFields(value, variableName, depth + 1);
            }
            return value;
        }

        // �Q�ƌ^�̏ꍇ�A�������m�F
        var nestedFields = obj.GetType().GetFields();
        foreach (var nestedField in nestedFields)
        {
            var nestedValue = nestedField.GetValue(obj);

            // �Q�ƌ^�̒��g���ċA�I�ɒT��
            if (nestedValue != null && nestedField.FieldType.IsClass && nestedField.FieldType != typeof(string))
            {
                var result = GetVariableValue(nestedValue, variableName, depth + 1);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null;
    }

    // �����N���X�̃t�B�[���h���T��
    private object SearchNestedFields(object obj, string variableName, int depth)
    {
        if (obj == null || depth > 5)
            return null;

        var nestedFields = obj.GetType().GetFields();
        foreach (var nestedField in nestedFields)
        {
            if (nestedField.Name == variableName)
            {
                return nestedField.GetValue(obj);
            }

            var nestedValue = nestedField.GetValue(obj);
            if (nestedValue != null && nestedField.FieldType.IsClass && nestedField.FieldType != typeof(string))
            {
                // ����ɐ[���K�w��T��
                var result = SearchNestedFields(nestedValue, variableName, depth + 1);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null;
    }

    // �V�����s�𐶐�
    GameObject CreateRow()
    {
        GameObject newRow = new GameObject("Row", typeof(HorizontalLayoutGroup));
        newRow.transform.SetParent(contentParent);
        newRow.transform.localScale = Vector3.one;

        HorizontalLayoutGroup layout = newRow.GetComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childScaleWidth = true;
        layout.childScaleHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        return newRow;
    }
    // �ǉ�����}�E�X�z�o�[���̏���
    private void OnButtonHovered(EquipSelectButton equip, GameObject button)
    {
        ShowWeaponDetails(equip);
        EventSystem.current.SetSelectedGameObject(button);
    }
    public void AdjustNavigation()
    {
        List<GameObject> buttons = new List<GameObject>();
        if(equipSelectWindow.selectSlotName == "MainR" || equipSelectWindow.selectSlotName == "MainL" || equipSelectWindow.selectSlotName == "SubR" || equipSelectWindow.selectSlotName == "SubL")
        {
            buttons = mainButtons;
        }
        foreach (GameObject btn in buttons)
            {
                Button btnbtn = btn.GetComponent<Button>();
                Navigation nav = btnbtn.navigation;
                nav.mode = Navigation.Mode.Explicit;

            // ������̃{�^�������ݒ�̏ꍇ�̂݃Z�b�g
            if (nav.selectOnUp == null)
            {
                nav.selectOnUp = FindNearestButton(btn, Vector2.up, buttons);
            }

            // �������̃{�^�������ݒ�̏ꍇ�̂݃Z�b�g
            if (nav.selectOnDown == null)
            {
                nav.selectOnDown = FindNearestButton(btn, Vector2.down, buttons);
            }

            // �������̃{�^�������ݒ�̏ꍇ�̂݃Z�b�g
            if (nav.selectOnLeft == null)
            {
                nav.selectOnLeft = FindNearestButton(btn, Vector2.left, buttons);
            }

            // �E�����̃{�^�������ݒ�̏ꍇ�̂݃Z�b�g
            if (nav.selectOnRight == null)
            {
                nav.selectOnRight = FindNearestButton(btn, Vector2.right, buttons);
            }

            btnbtn.navigation = nav;
            }
    }
    Button FindNearestButton(GameObject current, Vector2 direction, List<GameObject> buttons)
    {
        RectTransform currentRect = current.GetComponent<RectTransform>();
        Vector2 currentPos = currentRect.anchoredPosition;
        RectTransform currentParentRect = current.transform.parent.GetComponent<RectTransform>();
        Vector2 currentParentPos = currentParentRect.anchoredPosition;
        currentPos = currentPos + currentParentPos;
        Button bestMatch = null;
        float minDistance = float.MaxValue;
        foreach (GameObject other in buttons)
        {
            Button otherButton = other.GetComponent<Button>();
            if (otherButton == current) continue;
            
            RectTransform otherRect = other.GetComponent<RectTransform>();
            Vector2 otherPos = otherRect.anchoredPosition;
            RectTransform otherParentRect = other.transform.parent.GetComponent<RectTransform>();
            Vector2 otherParentPos = otherParentRect.anchoredPosition;
            otherPos = otherPos + otherParentPos;
            Vector2 diff = otherPos - currentPos;

            // �w������ɂ���{�^���̂ݑΏۂɂ���
            bool isValid = false;

            if (direction == Vector2.up && diff.y > 0 && Mathf.Abs(diff.x) < 0.05) isValid = true;
            if (direction == Vector2.down && diff.y < 0 && Mathf.Abs(diff.x) < 0.05) isValid = true;
            if (direction == Vector2.left && diff.x < 0 ) isValid = true;
            if (direction == Vector2.right && diff.x > 0 ) isValid = true;

            if (isValid)
            {
                float distance = diff.magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestMatch = otherButton;
                }
            }
        }

        return bestMatch;
    }
    public GameObject FindButton(string buttonId)
    {
        return buttonDictionary[buttonId];
    }
}
