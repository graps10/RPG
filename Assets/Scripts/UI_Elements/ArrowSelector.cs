using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI_Elements
{
    public class ArrowSelector : MonoBehaviour
    {
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TextMeshProUGUI valueText;

        [Tooltip("Optional: pre-fill options in Inspector (can also be set at runtime via SetOptions)")]
        [SerializeField] private List<string> options = new();

        // Mirrors Dropdown.onValueChanged so OptionsUI can subscribe the same way
        public UnityEvent<int> onValueChanged = new();

        private int _currentIndex;

        public int Value
        {
            get => _currentIndex;
            set => SetIndex(value, notify: false);
        }
        
        private void Awake()
        {
            leftButton?.onClick.AddListener(Previous);
            rightButton?.onClick.AddListener(Next);

            RefreshShownValue();
        }

        private void OnDestroy()
        {
            leftButton?.onClick.RemoveListener(Previous);
            rightButton?.onClick.RemoveListener(Next);
        }

        /// <summary>Replaces the current option list and resets selection to 0.</summary>
        public void ClearOptions()
        {
            options.Clear();
            _currentIndex = 0;
            RefreshShownValue();
        }

        /// <summary>Appends a list of option strings.</summary>
        public void AddOptions(List<string> newOptions)
        {
            options.AddRange(newOptions);
            RefreshShownValue();
        }

        /// <summary>Convenience: replaces options and selects the given index silently.</summary>
        public void SetOptions(List<string> newOptions, int startIndex = 0)
        {
            options = new List<string>(newOptions);
            SetIndex(startIndex, notify: false);
        }

        /// <summary>Updates the displayed text to match the current index.</summary>
        public void RefreshShownValue()
        {
            if (valueText == null) return;

            if (options == null || options.Count == 0)
            {
                valueText.text = "-";
                return;
            }

            valueText.text = options[Mathf.Clamp(_currentIndex, 0, options.Count - 1)];
        }

        private void Next()
        {
            if (options == null || options.Count == 0) return;
            SetIndex((_currentIndex + 1) % options.Count, notify: true);
        }

        private void Previous()
        {
            if (options == null || options.Count == 0) return;
            SetIndex((_currentIndex - 1 + options.Count) % options.Count, notify: true);
        }

        private void SetIndex(int index, bool notify)
        {
            if (options == null || options.Count == 0)
            {
                _currentIndex = 0;
                RefreshShownValue();
                return;
            }

            _currentIndex = Mathf.Clamp(index, 0, options.Count - 1);
            RefreshShownValue();

            if (notify)
                onValueChanged.Invoke(_currentIndex);
        }
    }
}
