using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ColorManager;
using System.Collections.Generic;

public class HSVColorPicker : MonoBehaviour
{
    [Header("UI Elements")]
    public ColorSelectWindow selectWindow;
    public HueCircle hueCircle;
    public SaturationBrightnessBox satBrightBox;
    public Slider hueSlider;
    public Slider satSlider;
    public Slider brightSlider;
    public Slider intensitySlider;
    public Image hueSliderFill;
    public Image satSliderFill;
    public Image brightSliderFill;
    public Image previewColor;
    public TMP_Text hueText;
    public TMP_Text satText;
    public TMP_Text valText;
    public TMP_Text intText;
    public Button uiModeButton;
    public Button sliderModeButton;

    [Header("Settings")]
    public Material setMat;
    public Dictionary<string, Material> materials = new Dictionary<string, Material>();
    public HSVColorPicker()
    {
        materials["MainR"] = null;
        materials["MainL"] = null;
        materials["SubR"] = null;
        materials["SubL"] = null;
        materials["ShoulderR"] = null;
        materials["ShoulderL"] = null;
        materials["Head"] = null;
        materials["Body"] = null;
        materials["Arm"] = null;
        materials["Leg"] = null;
        materials["Backpack"] = null;
    }
    private float hue = 0f;        // 色相 0-1
    private float saturation = 1f; // 彩度 0-1
    private float brightness = 1f; // 明度 0-1
    private float intencity = 1f; // 明度 0-1
    private int currentSlider = 0; // 0: Hue, 1: Sat, 2: Bright
    public bool isBoxUI = false; // 現在UIモードかどうか
    public bool isSlider = false; // 現在UIモードかどうか
    private Color _currentColor;
    public Color currentColor
    {
        get { return _currentColor; }
        set { _currentColor = value; }
    }
    private float _currentIntencity;
    public float currentIntencity
    {
        get { return _currentIntencity; }
        set { _currentIntencity = value; }
    }

    public Color GetColor()
    {
        return Color.HSVToRGB(hue, saturation, brightness);
    }
    public float GetIntencity()
    {
        return intencity;
    }
    public void SetHSVColor(Color color, float inte)
    {
        Color.RGBToHSV(color, out hue, out saturation, out brightness);
        intencity = inte;
        hueSlider.value = hue;
        satSlider.value = saturation;
        brightSlider.value = brightness;
        intensitySlider.value = intencity;
        hueText.text = string.Format("{0:F0}", hue * 255);
        satText.text = string.Format("{0:F0}", saturation * 100);
        valText.text = string.Format("{0:F0}", brightness * 100);
        intText.text = string.Format("{0:F0}", intencity);

        //サークルマーカー回転
        hueCircle.hueMarker.localEulerAngles = new Vector3(0, 0, -hue * 360f);

        //明度彩度矩形色変更
        satBrightBox.satBrightIndicator.color = Color.HSVToRGB(hue, 1, 1);
        float x = saturation * satBrightBox.satBrightIndicator.rectTransform.rect.width;
        float y = brightness * satBrightBox.satBrightIndicator.rectTransform.rect.height;

        satBrightBox.satBrightMarker.anchoredPosition = new Vector2(x, y);

        previewColor.color = color;
        UpdateAllSliders();
    }
    private void Start()
    {
        // 初期設定
        UpdateColor(); // 初期色設定
        UpdateSliderUI();
        UpdateAllSliders(); // スライダーの色を初期化
    }

    private void Update()
    {
        if (isBoxUI)
        {
            HandleUIInput();
        }
        if(isSlider)
        {
            HandleSliderInput();
        }
        if (selectWindow.slot != 11)
        {
            if (setMat != null)
            {
                if (selectWindow.colorPoint == "Main1")
                {
                    // 現在のプロパティ情報を取得
                    setMat.SetColor("_MainColor1", GetColor());
                    setMat.SetColor("_MainColorEmi1", GetColor());
                    setMat.SetFloat("_MainSt1", GetIntencity());
                }
                if (selectWindow.colorPoint == "Main2")
                {
                    setMat.SetColor("_MainColor2", GetColor());
                    setMat.SetColor("_MainColorEmi2", GetColor());
                    setMat.SetFloat("_MainSt2", GetIntencity());
                }
                if (selectWindow.colorPoint == "Main3")
                {
                    setMat.SetColor("_MainColor3", GetColor());
                    setMat.SetColor("_MainColorEmi3", GetColor());
                    setMat.SetFloat("_MainSt3", GetIntencity());
                }
                if (selectWindow.colorPoint == "Sub1")
                {
                    setMat.SetColor("_SubColor1", GetColor());
                    setMat.SetColor("_SubColorEmi1", GetColor());
                    setMat.SetFloat("_SubSt1", GetIntencity());
                }
                if (selectWindow.colorPoint == "Sub2")
                {
                    setMat.SetColor("_SubColor2", GetColor());
                    setMat.SetColor("_SubColorEmi2", GetColor());
                    setMat.SetFloat("_SubSt2", GetIntencity());
                }
                if (selectWindow.colorPoint == "Emi1")
                {
                    setMat.SetColor("_EmiColor1", GetColor());
                    setMat.SetFloat("_EmiSt1", GetIntencity());
                }
                if (selectWindow.colorPoint == "Emi2")
                {
                    setMat.SetColor("_EmiColor2", GetColor());
                    setMat.SetFloat("_EmiSt2", GetIntencity());
                }
            }
        }
        else
        {
            Debug.Log("Start");
            foreach(Material mat in materials.Values)
            {
                if (mat != null)
                {
                    Debug.Log("foreach");
                    if (selectWindow.colorPoint == "Main1")
                    {
                        // 現在のプロパティ情報を取得
                        mat.SetColor("_MainColor1", GetColor());
                        mat.SetColor("_MainColorEmi1", GetColor());
                        mat.SetFloat("_MainSt1", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Main2")
                    {
                        mat.SetColor("_MainColor2", GetColor());
                        mat.SetColor("_MainColorEmi2", GetColor());
                        mat.SetFloat("_MainSt2", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Main3")
                    {
                        mat.SetColor("_MainColor3", GetColor());
                        mat.SetColor("_MainColorEmi3", GetColor());
                        mat.SetFloat("_MainSt3", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Sub1")
                    {
                        mat.SetColor("_SubColor1", GetColor());
                        mat.SetColor("_SubColorEmi1", GetColor());
                        mat.SetFloat("_SubSt1", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Sub2")
                    {
                        mat.SetColor("_SubColor2", GetColor());
                        mat.SetColor("_SubColorEmi2", GetColor());
                        mat.SetFloat("_SubSt2", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Emi1")
                    {
                        mat.SetColor("_EmiColor1", GetColor());
                        mat.SetFloat("_EmiSt1", GetIntencity());
                    }
                    if (selectWindow.colorPoint == "Emi2")
                    {
                        mat.SetColor("_EmiColor2", GetColor());
                        mat.SetFloat("_EmiSt2", GetIntencity());
                    }
                }
            }
        }
    }

    private void HandleUIInput()
    {
        // 色相、彩度、明度を変更するためのキー入力
        float sliderStep = 0.0005f;

        // Q / E -> 色相変更
        if (Input.GetKey(KeyCode.Q)) ChangeHue(-sliderStep); // 色相を左に
        if (Input.GetKey(KeyCode.E)) ChangeHue(sliderStep); // 色相を右に

        // W / S -> 明度変更
        if (Input.GetKey(KeyCode.W)) ChangeBrightness(0.001f); // 明度を上げる
        if (Input.GetKey(KeyCode.S)) ChangeBrightness(-0.001f); // 明度を下げる

        // A / D -> 彩度変更
        if (Input.GetKey(KeyCode.A)) ChangeSaturation(-0.001f); // 彩度を下げる
        if (Input.GetKey(KeyCode.D)) ChangeSaturation(0.001f); // 彩度を上げる
    }

    private void HandleSliderInput()
    {
        // A / D -> スライダー値変更
        float sliderStep = 0.001f;
        if (Input.GetKey(KeyCode.A)) AdjustSliderValue(-sliderStep);
        if (Input.GetKey(KeyCode.D)) AdjustSliderValue(sliderStep);

        // W / S -> スライダー切り替え
        if (Input.GetKeyDown(KeyCode.W)) SelectPreviousSlider();
        if (Input.GetKeyDown(KeyCode.S)) SelectNextSlider();
    }

    private void SelectPreviousSlider()
    {
        currentSlider = (currentSlider - 1 + 4) % 4;
        UpdateSliderUI();
    }

    private void SelectNextSlider()
    {
        currentSlider = (currentSlider + 1) % 4;
        UpdateSliderUI();
    }

    private void AdjustSliderValue(float amount)
    {
        switch (currentSlider)
        {
            case 0: ChangeHue(amount); break;
            case 1: ChangeSaturation(amount); break;
            case 2: ChangeBrightness(amount); break;
            case 3: ChangeIntencity(amount); break;
        }
    }

    public void ChangeHue(float amount)
    {
        hue = Mathf.Repeat(hue + amount, 1f);
        hueCircle.SetHue(hue);

        // 色相を変更した場合、四角形の彩度と明度はそのままにして色を更新
        UpdateColor();
        UpdateAllSliders();
    }

    public void ChangeSaturation(float amount)
    {
        saturation = Mathf.Clamp01(saturation + amount);
        satBrightBox.SetSaturationBrightness(hue, saturation, brightness);
        UpdateColor();
        UpdateAllSliders();
    }

    public void ChangeBrightness(float amount)
    {
        brightness = Mathf.Clamp01(brightness + amount);
        satBrightBox.SetSaturationBrightness(hue, saturation, brightness);
        UpdateColor();
        UpdateAllSliders();
    }

    public void ChangeIntencity(float amount)
    {
        intencity = Mathf.Clamp(intencity + amount * 100f, 0 ,100f);
        UpdateColor();
        UpdateAllSliders();
    }

    private void UpdateColor()
    {
        Color newColor = Color.HSVToRGB(hue, saturation, brightness);
        previewColor.color = newColor;
        hueSlider.value = hue;
        satSlider.value = saturation;
        brightSlider.value = brightness;
        intensitySlider.value = intencity;
        hueText.text = string.Format("{0:F0}", hue * 255);
        satText.text = string.Format("{0:F0}", saturation * 100);
        valText.text = string.Format("{0:F0}", brightness * 100);
        intText.text = string.Format("{0:F0}", intencity);

        // 色相変更時に四角形の色を更新（彩度と明度はそのまま）
        satBrightBox.SetSaturationBrightness(hue, saturation, brightness);
    }

    private void UpdateSliderUI()
    {
        hueSlider.interactable = (currentSlider == 0 && isSlider);
        satSlider.interactable = (currentSlider == 1 && isSlider);
        brightSlider.interactable = (currentSlider == 2 && isSlider);
        intensitySlider.interactable = (currentSlider == 3 && isSlider);
    }

    //  スライダーの色を更新
    private void UpdateAllSliders()
    {
        UpdateHueSlider();
        UpdateSaturationSlider();
        UpdateBrightnessSlider();
    }

    private void UpdateHueSlider()
    {
        Texture2D hueTexture = new Texture2D(256, 1);
        for (int i = 0; i < 256; i++)
        {
            float h = i / 255f;
            hueTexture.SetPixel(i, 0, Color.HSVToRGB(h, 1f, 1f));
        }
        hueTexture.Apply();
        hueSliderFill.sprite = Sprite.Create(hueTexture, new Rect(0, 0, 256, 1), new Vector2(0.5f, 0.5f));
    }

    private void UpdateSaturationSlider()
    {
        Texture2D satTexture = new Texture2D(256, 1);
        for (int i = 0; i < 256; i++)
        {
            float s = i / 255f;
            satTexture.SetPixel(i, 0, Color.HSVToRGB(hue, s, brightness));
        }
        satTexture.Apply();
        satSliderFill.sprite = Sprite.Create(satTexture, new Rect(0, 0, 256, 1), new Vector2(0.5f, 0.5f));
    }

    private void UpdateBrightnessSlider()
    {
        Texture2D brightTexture = new Texture2D(256, 1);
        for (int i = 0; i < 256; i++)
        {
            float b = i / 255f;
            brightTexture.SetPixel(i, 0, Color.HSVToRGB(hue, saturation, b));
        }
        brightTexture.Apply();
        brightSliderFill.sprite = Sprite.Create(brightTexture, new Rect(0, 0, 256, 1), new Vector2(0.5f, 0.5f));
    }
}
