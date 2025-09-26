using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Elements
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private string parameter;

        public void SlideValue(float value) 
            => AudioManager.Instance.GetMixer().
                SetFloat(parameter, Mathf.Log10(value) * AudioManager.Instance.GetVolumeScaleFactor());

        public void LoadSlider(float value)
        {
            if (value >= slider.minValue)
                slider.value = value;
        }
        
        public Slider GetSlider() => slider;
        public string GetParameter() => parameter;
    }
}
