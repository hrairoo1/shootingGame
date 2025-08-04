using UnityEngine;
using UnityEngine.UI;

public class ColorSlider : MonoBehaviour
{
    public Slider slider;

    public void SetValue(float value, Color color)
    {
        slider.value = value;
    }
}
