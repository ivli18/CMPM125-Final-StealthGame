using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public Image fillImage;

    [Header("Colors")]
    public Color fullColor = new Color(1f, 0.85f, 0f);
    public Color lowColor  = new Color(0.9f, 0.1f, 0.1f);

    public void SetMax(float max) => slider.maxValue = max;

    public void SetValue(float value)
    {
        slider.value = value;
        fillImage.color = Color.Lerp(lowColor, fullColor, value / slider.maxValue);
    }
}