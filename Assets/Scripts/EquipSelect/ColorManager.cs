using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public Dictionary<string, EquipColor> equipColor = new Dictionary<string, EquipColor>();
    public Dictionary<string, Intencity> equipIntencity = new Dictionary<string, Intencity>();

    public class EquipColor
    {
        public Color _MainColor1;
        public Color _MainColor2;
        public Color _MainColor3;
        public Color _SubColor1;
        public Color _SubColor2;
        public Color _EmiColor1;
        public Color _EmiColor2;
    }public class Intencity
    {
        public float mainIntencity1;
        public float mainIntencity2;
        public float mainIntencity3;
        public float subIntencity1;
        public float subIntencity2;
        public float emiIntencity1;
        public float emiIntencity2;
    }

    public void SetDefaltColor(string slotName)
    {
        equipColor[slotName]._MainColor1 = GameSettings.Instance._defaltMainColor1;
        equipColor[slotName]._MainColor2 = GameSettings.Instance._defaltMainColor2;
        equipColor[slotName]._MainColor3 = GameSettings.Instance._defaltMainColor3;
        equipColor[slotName]._SubColor1 = GameSettings.Instance._defaltSubColor1;
        equipColor[slotName]._SubColor2 = GameSettings.Instance._defaltSubColor2;
        equipColor[slotName]._EmiColor1 = GameSettings.Instance._defaltEmiColor1;
        equipColor[slotName]._EmiColor2 = GameSettings.Instance._defaltEmiColor2;
        equipIntencity[slotName].mainIntencity1 = 0;
        equipIntencity[slotName].mainIntencity2 = 0;
        equipIntencity[slotName].mainIntencity3 = 0;
        equipIntencity[slotName].subIntencity1 = 0;
        equipIntencity[slotName].subIntencity2 = 0;
        equipIntencity[slotName].emiIntencity1 = 1;
        equipIntencity[slotName].emiIntencity2 = 1;
    }
    
    public void SetColor(string slotName, string colorSlot, Color color)
    {
        if (equipColor.ContainsKey(slotName))
        {
            EquipColor equip = equipColor[slotName];

            // リフレクションでプロパティを設定
            FieldInfo field = typeof(EquipColor).GetField(colorSlot, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(Color))
            {
                field.SetValue(equip, color);
            }
            else
            {
                Debug.LogError($"指定されたプロパティ '{colorSlot}' は存在しないか、Color 型ではありません。");
            }
        }
        else
        {
            Debug.LogError($"指定された装備スロット '{slotName}' は存在しません。");
        }

    }
    public void ComfirmedColor()
    {
        LoadoutManager.Instance.equipColor = equipColor;
    }
    public void ReturnColor()
    {
        equipColor = LoadoutManager.Instance.equipColor;
    }
}
