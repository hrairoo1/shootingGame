using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColorManager;

public class EquipColorButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        EquipButtonColorSet();
    }
    void EquipButtonColorSet()
    {
        Transform button = transform;
        Transform mainColor1Image = button.Find("ColorSet/Main1");
        Transform mainColor2Image = button.Find("ColorSet/Main2");
        Transform mainColor3Image = button.Find("ColorSet/Main3");
        Transform subColor1Image = button.Find("ColorSet/Sub1");
        Transform subColor2Image = button.Find("ColorSet/Sub2");
        Transform emiColor1Image = button.Find("ColorSet/Emi1");
        Transform emiColor2Image = button.Find("ColorSet/Emi2");
        Image mainColor1 = mainColor1Image.GetComponent<Image>();
        Image mainColor2 = mainColor2Image.GetComponent<Image>();
        Image mainColor3 = mainColor3Image.GetComponent<Image>();
        Image subColor1 = subColor1Image.GetComponent<Image>();
        Image subColor2 = subColor2Image.GetComponent<Image>();
        Image emiColor1 = emiColor1Image.GetComponent<Image>();
        Image emiColor2 = emiColor2Image.GetComponent<Image>();
        if (name != "All")
        {
            mainColor1.color = LoadoutManager.Instance.equipColor[name]._MainColor1;
            mainColor2.color = LoadoutManager.Instance.equipColor[name]._MainColor2;
            mainColor3.color = LoadoutManager.Instance.equipColor[name]._MainColor3;
            subColor1.color = LoadoutManager.Instance.equipColor[name]._SubColor1;
            subColor2.color = LoadoutManager.Instance.equipColor[name]._SubColor2;
            emiColor1.color = LoadoutManager.Instance.equipColor[name]._EmiColor1;
            emiColor2.color = LoadoutManager.Instance.equipColor[name]._EmiColor2;
        }
        else
        {
            // チェック対象の名前リスト
            List<string> equipNames = new List<string>()
            {
        "MainR", "MainL", "SubR", "SubL", "ShoulderR", "ShoulderL", "Head", "Body", "Arm", "Leg", "Backpack"
    };

            // MainColor1 ～ EmiColor2 のチェックと設定
            mainColor1.color = GetUniformColor(equipNames, "_MainColor1");
            mainColor2.color = GetUniformColor(equipNames, "_MainColor2");
            mainColor3.color = GetUniformColor(equipNames, "_MainColor3");
            subColor1.color = GetUniformColor(equipNames, "_SubColor1");
            subColor2.color = GetUniformColor(equipNames, "_SubColor2");
            emiColor1.color = GetUniformColor(equipNames, "_EmiColor1");
            emiColor2.color = GetUniformColor(equipNames, "_EmiColor2");
        }
    }// 色がすべて同じかチェックして色を返す。違う場合は黒
    private Color GetUniformColor(List<string> equipNames, string colorName)
    {
        if (equipNames.Count == 0) return Color.black;

        // 最初の色を取得
        Color firstColor = GetColorValue(equipNames[0], colorName);

        // すべてのパーツで同じ色かチェック
        foreach (string name in equipNames)
        {
            if (!LoadoutManager.Instance.equipColor.ContainsKey(name))
            {
                return Color.black;
            }

            // 現在の色を取得
            Color currentColor = GetColorValue(name, colorName);

            // 色が異なる場合は黒
            if (currentColor != firstColor)
            {
                return Color.black;
            }
        }

        // すべて同じ場合はその色を返す
        return firstColor;
    }

    // EquipColor から指定した colorName の Color を取得
    private Color GetColorValue(string name, string colorName)
    {
        EquipColor equip = LoadoutManager.Instance.equipColor[name];

        switch (colorName)
        {
            case "_MainColor1":
                return equip._MainColor1;
            case "_MainColor2":
                return equip._MainColor2;
            case "_MainColor3":
                return equip._MainColor3;
            case "_SubColor1":
                return equip._SubColor1;
            case "_SubColor2":
                return equip._SubColor2;
            case "_EmiColor1":
                return equip._EmiColor1;
            case "_EmiColor2":
                return equip._EmiColor2;
            default:
                return Color.black;
        }

    }
}
