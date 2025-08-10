using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.Shift;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using UnityEngine.Localization.Settings;

namespace _1.Scripts.UI.Setting
{
    public class SettingUI : MonoBehaviour
    {
        [Header("Language")]
        [SerializeField] private HorizontalSelector languageSelector;

        [Header("Audio Volume")]
        [SerializeField] private SliderManager masterVolumeSlider;
        [SerializeField] private SliderManager bgmVolumeSlider;
        [SerializeField] private SliderManager sfxVolumeSlider;

        [Header("Sensitivity")]
        [SerializeField] private SliderManager lookSensitivitySlider;
        [SerializeField] private SliderManager aimSensitivitySlider;

        [Header("Key Bindings")]
        // TODO: 키 바인딩 추가

        [Header("Graphics")]
        [SerializeField] private HorizontalSelector resolutionSelector;
        [SerializeField] private HorizontalSelector fullscreenModeSelector;

        private Resolution[] resolutions;
        private readonly List<string> fullscreenModes = new List<string> { "Fullscreen", "Borderless", "Windowed" };
        
        private void Start()
        {
            resolutions = Screen.resolutions;
            InitSensitivitySliders();
            InitResolutionSelector();
            InitFullscreenModeSelector();
            LoadSettings();
            AddListeners();
        }
        
        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            RegisterLanguageSelectorEvents();
            SyncLanguageSelector();
        }
        
        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
        {
            SyncLanguageSelector();
        }
        
        private void RegisterLanguageSelectorEvents()
        {
            if (!languageSelector || languageSelector.itemList == null) return;

            foreach (var item in languageSelector.itemList)
                item.onValueChanged.RemoveAllListeners();

            for (int i = 0; i < languageSelector.itemList.Count; i++)
            {
                int idx = i;
                languageSelector.itemList[i].onValueChanged.AddListener(() => OnLanguageSelectorChanged(idx));
            }
        }
        
        private void SyncLanguageSelector()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            var currentLocale = LocalizationSettings.SelectedLocale;
            int idx = locales.IndexOf(currentLocale);
            if (idx < 0) idx = 0;
            languageSelector.index = idx;
            languageSelector.UpdateUI();
        }
        
        private void OnLanguageSelectorChanged(int idx)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (idx < 0 || idx >= locales.Count) return;
            LocalizationSettings.SelectedLocale = locales[idx];
            PlayerPrefs.SetInt("LanguageIndex", idx);
        }
        
        private void InitSensitivitySliders()
        {
            //float lookDef = CoreManager.Instance.gameManager.LookSensitivity;
            //float aimDef  = CoreManager.Instance.gameManager.AimSensitivity;
            lookSensitivitySlider.enableSaving = false;
            aimSensitivitySlider.enableSaving  = false;
        }

        private void InitResolutionSelector()
        {
            if (!resolutionSelector) return;
            resolutionSelector.saveValue = false;
            resolutionSelector.loopSelection = false;
            resolutionSelector.itemList.Clear();

            foreach (var r in resolutions)
            {
                string option = $"{r.width}x{r.height} {r.refreshRateRatio}hz";
                resolutionSelector.CreateNewItem(option);
            }

            for (int i = 0; i < resolutionSelector.itemList.Count; i++)
            {
                int idx = i;
                resolutionSelector.itemList[i].onValueChanged.AddListener(() => OnResolutionChanged(idx));
            }
            
            int defaultIdx = PlayerPrefs.GetInt("ResolutionIndex", GetCurrentResolutionIndex());
            resolutionSelector.index = defaultIdx;
            resolutionSelector.UpdateUI();
        }

        private void InitFullscreenModeSelector()
        {
            if (!fullscreenModeSelector) return;
            fullscreenModeSelector.saveValue = false;
            fullscreenModeSelector.loopSelection = false;
            fullscreenModeSelector.itemList.Clear();

            foreach (var t in fullscreenModes)
            {
                fullscreenModeSelector.CreateNewItem(t);
            }

            for (int i = 0; i < fullscreenModes.Count; i++)
            {
                int idx = i;
                fullscreenModeSelector.itemList[i].onValueChanged.AddListener(() => OnFullscreenModeChanged(idx));
            }

            int defaultMode = PlayerPrefs.GetInt("FullscreenMode", Screen.fullScreen ? 0 : 2);
            fullscreenModeSelector.index = defaultMode;
            fullscreenModeSelector.UpdateUI();
        }

        private int GetCurrentResolutionIndex()
        {
            for (int i = 0; i < resolutions.Length; i++)
            {
                var r = resolutions[i];
                if (r.width == Screen.currentResolution.width &&
                    r.height == Screen.currentResolution.height &&
                    Math.Abs(r.refreshRateRatio.value - Screen.currentResolution.refreshRateRatio.value) < 0.5f)
                    return i;
            }
            return 0;
        }
        

        private void LoadSettings()
        {
            SoundManager sm = CoreManager.Instance.soundManager;
            sm.SetMasterVolume(PlayerPrefs.GetFloat(masterVolumeSlider.sliderTag + "SliderValue",
                masterVolumeSlider.defaultValue));
            sm.SetBGMVolume(PlayerPrefs.GetFloat(bgmVolumeSlider.sliderTag + "SliderValue",
                bgmVolumeSlider.defaultValue));
            sm.SetSFXVolume(PlayerPrefs.GetFloat(sfxVolumeSlider.sliderTag + "SliderValue",
                sfxVolumeSlider.defaultValue));

            //float lookVal = PlayerPrefs.GetFloat("LookSensitivity", CoreManager.Instance.gameManager.LookSensitivity);
            //float aimVal  = PlayerPrefs.GetFloat("AimSensitivity", CoreManager.Instance.gameManager.AimSensitivity);
            //lookSensitivitySlider.GetComponent<Slider>().value = lookVal;
            //aimSensitivitySlider.GetComponent<Slider>().value  = aimVal;
        }

        private void AddListeners()
        {

            
            masterVolumeSlider.GetComponent<Slider>().onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmVolumeSlider.GetComponent<Slider>().onValueChanged.AddListener(OnBGMVolumeChanged);
            sfxVolumeSlider.GetComponent<Slider>().onValueChanged.AddListener(OnSFXVolumeChanged);


            //lookSensitivitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnLookSensitivityChanged);
            //aimSensitivitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnAimSensitivityChanged);
        }

        private void OnMasterVolumeChanged(float vol)
        {
            CoreManager.Instance.soundManager.SetMasterVolume(vol);
        }

        private void OnBGMVolumeChanged(float vol)
        {
            CoreManager.Instance.soundManager.SetBGMVolume(vol);
        }

        private void OnSFXVolumeChanged(float vol)
        {
            CoreManager.Instance.soundManager.SetSFXVolume(vol);
        }

        /*private void OnLookSensitivityChanged(float sens)
        {
            CoreManager.Instance.gameManager.LookSensitivity = sens;
            PlayerPrefs.SetFloat("LookSensitivity", sens);
        }*/

        /*private void OnAimSensitivityChanged(float sens)
        {
            CoreManager.Instance.gameManager.AimSensitivity = sens;
            PlayerPrefs.SetFloat("AimSensitivity", sens);
        }*/

        private void OnResolutionChanged(int idx)
        {
            var r = resolutions[idx];
            Screen.SetResolution(r.width, r.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionIndex", idx);
        }

        private void OnFullscreenModeChanged(int idx)
        {
            switch (idx)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    Screen.fullScreen = true;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.fullScreen = true;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.fullScreen = false;
                    break;
            }
            PlayerPrefs.SetInt("FullscreenMode", idx);
        }
    }
}
