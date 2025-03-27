using UnityEngine;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parameter;

    private AudioManager AM => AudioManager.instance;

    public void SlideValue(float _value) => AM.mixer.SetFloat(parameter, Mathf.Log10(_value) * AM.multiplier);

    public void LoadSlider(float _value)
    {
        if (_value >= slider.minValue)
            slider.value = _value;
    }
}
