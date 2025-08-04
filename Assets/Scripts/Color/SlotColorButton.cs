using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotColorButton : MonoBehaviour
{
    [SerializeField] ColorSelectWindow selectWindow;
    [SerializeField] HSVColorPicker colorPicker;

    private int slot = 0;
    private void Start()
    {
        // EventTrigger コンポーネントを取得
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        // PointerEnter イベントの設定
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) => { OnHovered(); });

        // EventTrigger に登録
        trigger.triggers.Add(entry);
    }
    void OnHovered()
    {
        if (colorPicker.isBoxUI || colorPicker.isSlider) return;
        if (colorPicker.setMat != null)
        {
            if (name == "Main1")
            {
                colorPicker.setMat.SetFloat("_Main1B", 1);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2b", 0);
            }
            if (name == "Main2")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 1);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
            if (name == "Main3")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 1);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
            if (name == "Sub1")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 1);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
            if (name == "Sub2")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 1);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
            if (name == "Emi1")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 1);
                colorPicker.setMat.SetFloat("_Emi2B", 0);
            }
            if (name == "Emi2")
            {
                colorPicker.setMat.SetFloat("_Main1B", 0);
                colorPicker.setMat.SetFloat("_Main2B", 0);
                colorPicker.setMat.SetFloat("_Main3B", 0);
                colorPicker.setMat.SetFloat("_Sub1B", 0);
                colorPicker.setMat.SetFloat("_Sub2B", 0);
                colorPicker.setMat.SetFloat("_Emi1B", 0);
                colorPicker.setMat.SetFloat("_Emi2B", 1);
            }
        }
        if(selectWindow.slot == 11)
        {
            foreach (Material mat in colorPicker.materials.Values)
            {
                if (mat != null)
                {
                    if (name == "Main1")
                    {
                        mat.SetFloat("_Main1B", 1);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2b", 0);
                    }
                    if (name == "Main2")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 1);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2B", 0);
                    }
                    if (name == "Main3")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 1);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2B", 0);
                    }
                    if (name == "Sub1")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 1);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2B", 0);
                    }
                    if (name == "Sub2")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 1);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2B", 0);
                    }
                    if (name == "Emi1")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 1);
                        mat.SetFloat("_Emi2B", 0);
                    }
                    if (name == "Emi2")
                    {
                        mat.SetFloat("_Main1B", 0);
                        mat.SetFloat("_Main2B", 0);
                        mat.SetFloat("_Main3B", 0);
                        mat.SetFloat("_Sub1B", 0);
                        mat.SetFloat("_Sub2B", 0);
                        mat.SetFloat("_Emi1B", 0);
                        mat.SetFloat("_Emi2B", 1);
                    }
                }
            }
        }
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (gameObject == EventSystem.current.currentSelectedGameObject)
            {
                OnHovered();
            }
        }
    }
}