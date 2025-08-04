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
    [SerializeField] private Transform categoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform mainCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform shoulderCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform shoulder2CategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform headCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform bodyCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform armCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform legCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)
    public Transform backpackCategoryContainer; // カテゴリボタンの親 (HorizontalLayoutGroup)

    public Transform mainListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform shoulderListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform shoulder2ListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform headListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform bodyListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform armListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform legListContainer; // 武器リストの親 (VerticalLayoutGroup)
    public Transform backpackListContainer; // 武器リストの親 (VerticalLayoutGroup)
    [SerializeField] private GameObject categoryButtonPrefab; // カテゴリボタンのプレハブ
    [SerializeField] private GameObject weaponListNoneButtonPrefab; // カテゴリボタンのプレハブ
    [SerializeField] private GameObject weaponButtonPrefab; // 武器ボタンのプレハブ
    [SerializeField] private TMP_Text modelNumberText; // 詳細表示エリア
    [SerializeField] private TMP_Text nameText; // 詳細表示エリア
    [SerializeField] private TMP_Text descriptionText; // 詳細表示エリア
    public GameObject detail;
    private Dictionary<string, Transform> mainCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> shoulderCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> shoulder2CategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> headCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> bodyCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> armCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> legCategoryContainers = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> backpackCategoryContainers = new Dictionary<string, Transform>();
    private List<string> categoryMainOrder = new List<string> { "Assault Rifle", "Rocket", "Sniper", "Missile", "Canon", "xxx", "a", "b", "c", "d" }; // カテゴリの並び順
    private List<string> categoryShoulderOrder = new List<string> { "shoulder Missile", "Huge Canon" }; // カテゴリの並び順
    private List<string> categoryShoulder2Order = new List<string> { "shoulder Missile", "Missile" }; // カテゴリの並び順
    private List<string> categoryHeadOrder = new List<string> { "Head_Light", "Head_Medium", "Head_Heavy" }; // カテゴリの並び順
    private List<string> categoryBodyOrder = new List<string> { "Body_Light", "Body_Medium", "Body_Heavy" }; // カテゴリの並び順
    private List<string> categoryArmOrder = new List<string> { "Arm_Light", "Arm_Medium", "Arm_Heavy" }; // カテゴリの並び順
    private List<string> categoryLegOrder = new List<string> { "Leg_Light", "Leg_Medium", "Leg_Heavy" }; // カテゴリの並び順
    private List<string> categoryBackpackOrder = new List<string> { "Backpack_Light", "Backpack_Medium", "Backpack_Heavy" }; // カテゴリの並び順
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
    public string targetWeaponName = "3"; // ここに検索したい武器名を設定
    private RectTransform categoryPanel;
    private Vector3 weaponPanel;
    public RectTransform content;  // ContentのRectTransform

    private Vector3 categoryTargetPosition; // 目標位置（スライドさせる位置）
    private Vector3 weaponTargetPosition; // 目標位置（スライドさせる位置）
    public GameObject DefaltfirstSelectedButton; // 最初に選択するボタン
    private GameObject firstSelectedButton;
    private GameObject currentSelectedButton;

    public GameObject statusTextPrefab;
    public Transform contentParent; // UI配置場所 (Vertical Layout Group)
    private List<GameObject> generatedUIElements = new List<GameObject>(); // 生成したUI要素を保持するリスト// 特定の変数名を指定（これらのみ表示）
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
        // カテゴリスライド
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

        //レイアウトを即時更新
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentSizeFitter.GetComponent<RectTransform>());

        //categoryTargetPosition = categoryPanel.position;
    }
    void CreateCategory(List<string> categoryOrder, Transform container, Transform listContainer, Dictionary<string, Transform> categoryWeaponContainers)
    {
        foreach (var category in categoryOrder)
        {
            // カテゴリごとの武器リスト用オブジェクト作成
            GameObject weaponContainer = new GameObject($"{category}_Weapons");
            // カテゴリボタン生成
            GameObject categoryButtonObj = Instantiate(categoryButtonPrefab, container);
            GameObject categoryButtonObjw = Instantiate(weaponListNoneButtonPrefab, weaponContainer.transform);
            TMP_Text categoryText = categoryButtonObj.GetComponentInChildren<TMP_Text>();

            // LocalizeStringEvent を追加して翻訳
            LocalizeStringEvent localizeEvent = categoryText.AddComponent<LocalizeStringEvent>();

            // テーブル名とキーを設定
            localizeEvent.StringReference.TableReference = "MainTable";
            localizeEvent.StringReference.TableEntryReference = category;

            // 翻訳した文字列を Text に適用
            localizeEvent.OnUpdateString.AddListener((translatedText) =>
            {
                categoryText.text = translatedText;
            });

            weaponContainer.transform.SetParent(listContainer);
            VerticalLayoutGroup verticalLayoutGroup = weaponContainer.AddComponent<VerticalLayoutGroup>(); // 縦並び// VerticalLayoutGroupの設定変更
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
        // 武器ボタン追加
        foreach (var weapon in GameSettings.Instance.weaponEntries)
        {
            if (categoryWeaponContainers.ContainsKey(weapon.category))
            {
                GameObject buttonObj = Instantiate(weaponButtonPrefab, categoryWeaponContainers[weapon.category]);
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    // LocalizeStringEvent を追加して翻訳
                    LocalizeStringEvent localizeEvent = buttonText.AddComponent<LocalizeStringEvent>();

                    localizeEvent.StringReference.TableReference = "Weapon Table";
                    localizeEvent.StringReference.TableEntryReference = weapon.id;

                    // 翻訳した文字列を Text に適用
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
                    // イベントトリガーの追加
                    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
                    Debug.Log(weapon.name);
                    // PointerEnterイベントの設定
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter; // マウスがボタンに乗ったとき
                    entry.callback.AddListener((data) =>
                    {
                        // マウスがボタンに乗った時の処理
                        OnButtonHovered(buttonEntry, buttonObj);
                    });
                    trigger.triggers.Add(entry);
                    button.onClick.AddListener(() => SetWeapon(weapon));
                }
                // 武器名をキーに辞書に登録
                buttonDictionary[weapon.id] = buttonObj;
                buttonsRect.Add(buttonObj.GetComponent<RectTransform>());
                if (DefaltfirstSelectedButton == null)
                {
                    DefaltfirstSelectedButton = buttonObj;
                }
            }
        }
        // 防具ボタン追加
        foreach (var armor in GameSettings.Instance.armorEntries)
        {
            if (categoryWeaponContainers.ContainsKey(armor.category))
            {
                GameObject buttonObj = Instantiate(weaponButtonPrefab, categoryWeaponContainers[armor.category]);
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                // LocalizeStringEvent を追加して翻訳
                LocalizeStringEvent localizeEvent = buttonText.AddComponent<LocalizeStringEvent>();

                localizeEvent.StringReference.TableReference = "Weapon Table";
                localizeEvent.StringReference.TableEntryReference = armor.id;

                // 翻訳した文字列を Text に適用
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
                    // イベントトリガーの追加
                    EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
                    Debug.Log(armor.name);
                    // PointerEnterイベントの設定
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter; // マウスがボタンに乗ったとき
                    entry.callback.AddListener((data) =>
                    {
                        // マウスがボタンに乗った時の処理
                        OnButtonHovered(buttonEntry, buttonObj);
                    });
                    trigger.triggers.Add(entry);
                    button.onClick.AddListener(() => SetWeapon(armor));
                }
                // 武器名をキーに辞書に登録
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

            // 翻訳した文字列を Text に適用
            nameTextlocalizeEvent.OnUpdateString.AddListener((translatedText) =>
            {
                nameText.text = translatedText;
            });
            // 即時更新
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
    // 古いUIを削除して新しく生成する
    public void DisplayNonEmptyValues(EquipSelectButton equip)
    {
        // 生成したUI要素を削除
        foreach (var uiElement in generatedUIElements)
        {
            Destroy(uiElement);
        }
        generatedUIElements.Clear(); // リストもクリア

        // Aスクリプト内のすべてのフィールドを取得
        var fields = equip.GetType().GetFields();

        int column = 0;
        GameObject row = CreateRow(); // 初回行生成

        foreach (var variableName in variablesToDisplay)
        {
            object value = GetVariableValue(equip, variableName);

            // null または 0 の場合はスキップ
            if (value == null ||
                (value is int intValue && intValue == 0) ||
                (value is float floatValue && floatValue == 0) ||  // 他の数値型もチェック
                (value is double doubleValue && doubleValue == 0) ||
                (value is string strValue && string.IsNullOrEmpty(strValue)))
            {
                continue;
            }

            // 変数名の表示
            GameObject statusText = Instantiate(statusTextPrefab, row.transform);
            GameObject verNameText = statusText.transform.Find("name").gameObject;
            GameObject valueText = statusText.transform.Find("var").gameObject;
            verNameText.GetComponent<TextMeshProUGUI>().text = variableName; // 変数名を表示
            Debug.Log(variableName);
            if (variableName == "lockOnSize_x")
            {
                object lockonsizeY = GetVariableValue(equip, "lockOnSize_y");
                valueText.GetComponent<TextMeshProUGUI>().text = value.ToString() + "," + lockonsizeY.ToString(); // 値を表示
            }
            else valueText.GetComponent<TextMeshProUGUI>().text = value.ToString(); // 値を表示

            // 横2列ごとに新しい行を作成
            column++;
            if (column >= 2)
            {
                row = CreateRow();
                column = 0;
            }

            // 生成したUI要素をリストに追加
            generatedUIElements.Add(statusText);
            generatedUIElements.Add(row);
        }
    }// 変数の値を取得し、内部の参照型も再帰的に探索
    private object GetVariableValue(object obj, string variableName, int depth = 0)
    {
        if (obj == null || depth > 5) // 無限ループ防止 (最大5階層まで)
            return null;

        var field = obj.GetType().GetField(variableName);
        if (field != null)
        {
            var value = field.GetValue(obj);

            // 参照型の中身も再帰的にチェック
            if (value != null && field.FieldType.IsClass && field.FieldType != typeof(string))
            {
                // 再帰探索でさらに中の変数も確認
                return SearchNestedFields(value, variableName, depth + 1);
            }
            return value;
        }

        // 参照型の場合、内部も確認
        var nestedFields = obj.GetType().GetFields();
        foreach (var nestedField in nestedFields)
        {
            var nestedValue = nestedField.GetValue(obj);

            // 参照型の中身も再帰的に探索
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

    // 内部クラスのフィールドも探索
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
                // さらに深い階層を探索
                var result = SearchNestedFields(nestedValue, variableName, depth + 1);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null;
    }

    // 新しい行を生成
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
    // 追加するマウスホバー時の処理
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

            // 上方向のボタンが未設定の場合のみセット
            if (nav.selectOnUp == null)
            {
                nav.selectOnUp = FindNearestButton(btn, Vector2.up, buttons);
            }

            // 下方向のボタンが未設定の場合のみセット
            if (nav.selectOnDown == null)
            {
                nav.selectOnDown = FindNearestButton(btn, Vector2.down, buttons);
            }

            // 左方向のボタンが未設定の場合のみセット
            if (nav.selectOnLeft == null)
            {
                nav.selectOnLeft = FindNearestButton(btn, Vector2.left, buttons);
            }

            // 右方向のボタンが未設定の場合のみセット
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

            // 指定方向にあるボタンのみ対象にする
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
