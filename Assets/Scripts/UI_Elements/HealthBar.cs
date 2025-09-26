using Core.Utilities;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Elements
{
    public class HealthBar : MonoBehaviour 
    {
        private Entity.Entity _entity;
        private CharacterStats _myStats;
        
        private RectTransform _myTransform;
        private Slider _slider;

        private void Awake() 
        {
            _entity = GetComponentInParent<Entity.Entity>();
            _myTransform = GetComponent<RectTransform>();
            _slider = GetComponentInChildren<Slider>();
            _myStats = GetComponentInParent<CharacterStats>();
        }

        private void OnEnable()
        {
            _entity.OnFlipped += FlipUI;
            _myStats.OnHealthChanged += UpdateHealthUI;
        }
        private void OnDisable()
        {
            _entity.OnFlipped -= FlipUI;
            _myStats.OnHealthChanged -= UpdateHealthUI;
        }

        private void Start() => UpdateHealthUI();

        private void UpdateHealthUI()
        {
            _slider.maxValue = _myStats.GetMaxHealthValue();
            _slider.value = _myStats.CurrentHealth;
        }

        private void FlipUI() => _myTransform.Rotate(TransformUtils.FlipAngle);
    }
}