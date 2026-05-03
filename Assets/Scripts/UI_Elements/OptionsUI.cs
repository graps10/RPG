using System.Collections.Generic;
using Components.Audio;
using Core.Save_and_Load;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Elements
{
    public class OptionsUI : MonoBehaviour, ISaveManager
    {
        [Header("Audio Settings")]
        [SerializeField] private VolumeSlider[] volumeSettings;

        [Header("Display Settings")] 
        [SerializeField] private Toggle showHealthBarToggle;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private ArrowSelector resolutionSelector;
        [SerializeField] private ArrowSelector qualitySelector;
        
        [Header("Gameplay Settings")]
        [SerializeField] private Toggle showDamageNumbersToggle;
        [SerializeField] private Toggle cameraShakeToggle;

        public static bool ShowHealthBar { get; private set; } = true;
        public static bool ShowDamageNumbers { get; private set; } = true;
        public static bool CameraShake { get; private set; } = true;

        private Resolution[] _resolutions;
        
        private void Awake()
        {
            SetupResolutionSelector();
            SetupQualitySelector();
        }

        private void SetupResolutionSelector()
        {
            if (resolutionSelector == null) return;

            _resolutions = Screen.resolutions;

            var options = new List<string>();
            int currentIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                options.Add(
                    $"{_resolutions[i].width} x {_resolutions[i].height}  @{_resolutions[i].refreshRate} Hz");

                if (_resolutions[i].width  == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                    currentIndex = i;
            }

            resolutionSelector.ClearOptions();
            resolutionSelector.AddOptions(options);
            resolutionSelector.Value = currentIndex;
        }

        private void SetupQualitySelector()
        {
            if (qualitySelector == null) return;

            qualitySelector.ClearOptions();
            qualitySelector.AddOptions(new List<string>(QualitySettings.names));
            qualitySelector.Value = QualitySettings.GetQualityLevel();
        }

        private void OnEnable()
        {
            showHealthBarToggle?.onValueChanged.AddListener(OnHealthBarToggleChanged);
            fullscreenToggle?.onValueChanged.AddListener(OnFullscreenToggleChanged);
            resolutionSelector?.onValueChanged.AddListener(OnResolutionChanged);
            qualitySelector?.onValueChanged.AddListener(OnQualityChanged);
            showDamageNumbersToggle?.onValueChanged.AddListener(OnShowDamageNumbersChanged);
            cameraShakeToggle?.onValueChanged.AddListener(OnCameraShakeChanged);
        }

        private void OnDisable()
        {
            showHealthBarToggle?.onValueChanged.RemoveListener(OnHealthBarToggleChanged);
            fullscreenToggle?.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
            resolutionSelector?.onValueChanged.RemoveListener(OnResolutionChanged);
            qualitySelector?.onValueChanged.RemoveListener(OnQualityChanged);
            showDamageNumbersToggle?.onValueChanged.RemoveListener(OnShowDamageNumbersChanged);
            cameraShakeToggle?.onValueChanged.RemoveListener(OnCameraShakeChanged);
        }

        // ── Setting handlers ──────────────────────────────────────────────────
        private void OnHealthBarToggleChanged(bool isOn)
        {
            ShowHealthBar = isOn;

            if (PlayerManager.Instance?.PlayerGameObject != null)
            {
                HealthBar playerHealthBar =
                    PlayerManager.Instance.PlayerGameObject.GetComponentInChildren<HealthBar>(true);
                if (playerHealthBar != null)
                    playerHealthBar.gameObject.SetActive(isOn);
            }
        }

        private void OnFullscreenToggleChanged(bool isOn) => Screen.fullScreen = isOn;

        private void OnResolutionChanged(int index)
        {
            if (_resolutions == null || index < 0 || index >= _resolutions.Length) return;
            Resolution res = _resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        private void OnQualityChanged(int index) => QualitySettings.SetQualityLevel(index);

        private void OnShowDamageNumbersChanged(bool isOn) => ShowDamageNumbers = isOn;

        private void OnCameraShakeChanged(bool isOn) => CameraShake = isOn;

        // ── ISaveManager
        public void LoadData(GameData data)
        {
            // Audio
            foreach (var pair in data.GetVolumeSettings())
            {
                foreach (VolumeSlider item in volumeSettings)
                {
                    if (item.GetParameter() != pair.Key) continue;

                    item.LoadSlider(pair.Value);

                    if (item.GetParameter() == AudioManager.MIXER_BGM)
                        AudioManager.Instance.SetupBGMVolume(pair.Value);
                    else
                        AudioManager.Instance.SetupSFXVolume(pair.Value);
                }
            }

            // Health bar
            ShowHealthBar = data.ShowPlayerHealthBar;
            SetToggleSilently(showHealthBarToggle, ShowHealthBar, OnHealthBarToggleChanged);

            // Fullscreen
            bool isFullscreen = data.GetFullscreen();
            Screen.fullScreen = isFullscreen;
            SetToggleSilently(fullscreenToggle, isFullscreen, OnFullscreenToggleChanged);

            // Resolution
            int resIndex = data.GetResolutionIndex();
            if (_resolutions != null && resIndex >= 0 && resIndex < _resolutions.Length)
            {
                Resolution res = _resolutions[resIndex];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                SetSelectorSilently(resolutionSelector, resIndex, OnResolutionChanged);
            }

            // Graphics quality
            int quality = data.GetGraphicsQuality();
            QualitySettings.SetQualityLevel(quality);
            SetSelectorSilently(qualitySelector, quality, OnQualityChanged);

            // Show damage numbers
            ShowDamageNumbers = data.GetShowDamageNumbers();
            SetToggleSilently(showDamageNumbersToggle, ShowDamageNumbers, OnShowDamageNumbersChanged);

            // Camera shake
            CameraShake = data.GetCameraShake();
            SetToggleSilently(cameraShakeToggle, CameraShake, OnCameraShakeChanged);
        }

        public void SaveData(ref GameData data)
        {
            // Audio
            data.GetVolumeSettings().Clear();
            foreach (VolumeSlider item in volumeSettings)
                data.GetVolumeSettings().Add(item.GetParameter(), item.GetSlider().value);

            // Display + gameplay
            data.SetShowPlayerHealthBar(ShowHealthBar);
            data.SetFullscreen(Screen.fullScreen);
            data.SetResolutionIndex(resolutionSelector != null ? resolutionSelector.Value : -1);
            data.SetGraphicsQuality(QualitySettings.GetQualityLevel());
            data.SetShowDamageNumbers(ShowDamageNumbers);
            data.SetCameraShake(CameraShake);
        }
        
        /// Sets a Toggle without firing its listener, then re-subscribes.
        private static void SetToggleSilently(Toggle toggle, bool val,
            UnityEngine.Events.UnityAction<bool> listener)
        {
            if (toggle == null) return;
            toggle.onValueChanged.RemoveListener(listener);
            toggle.isOn = val;
            toggle.onValueChanged.AddListener(listener);
        }

        /// Sets an ArrowSelector without firing its listener, then re-subscribes.
        private static void SetSelectorSilently(ArrowSelector selector, int index,
            UnityEngine.Events.UnityAction<int> listener)
        {
            if (selector == null) return;
            selector.onValueChanged.RemoveListener(listener);
            selector.Value = index;
            selector.onValueChanged.AddListener(listener);
        }
    }
}
