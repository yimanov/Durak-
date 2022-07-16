using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Settings
{
    [Serializable]
    public class VibrationSet
    {
        public BaseParams Params;

        public VibrationSet()
        {
            Params = new VibrationParams();
        }

        [Serializable]
        public class VibrationParams : BaseParams
        {
            public VibrationParams()
            {
                Values = new Dictionary<string, object>()
                {
                    {"Enabled", true }
                };
            }
        }
    }


    public class VibrationController : Settings
    {
        private SaveUtility _saveUtility;

        private VibrationSet _settings;

        private const string _saveDirectory = "VibrationSave";
        private const string _saveFileName = "VibrationSettings";

        private bool _isEnabled;

        public bool IsEnabled => (bool)_settings.Params.Values["Enabled"];

        protected override string SaveDirectory => "VibrationSave";
        protected override string SaveFileName => "VibrationSettings";

        public long DebugMilliseconds = -1;

        public void Init()
        {
            _saveUtility = LazySingleton<SaveUtility>.Instance;

            LoadSettings();
        }

        protected override void LoadSettings()
        {
            string settingsData = _saveUtility.ReadFile(SaveDirectory, SaveFileName);

            if (string.IsNullOrEmpty(settingsData))
            {
                CreateNewSettings();
                return;
            }

            _settings = JsonConvert.DeserializeObject<VibrationSet>(settingsData);

            _isEnabled = IsEnabled;
        }

        protected override void CreateNewSettings()
        {
            _settings = new VibrationSet();

            _isEnabled = IsEnabled;

            string saveAudioData = JsonConvert.SerializeObject(_settings);
            _saveUtility.WriteFile(saveAudioData, SaveDirectory, SaveFileName);
        }

        public void SwitchVibration()
        {
            _isEnabled = !_isEnabled;
            SaveVibrationSettings("Enabled", _isEnabled);
        }

        public void SetEnableVibration(bool value)
        {
            _isEnabled = value;

            SaveVibrationSettings("Enabled", value);
        }

        private void SaveVibrationSettings(string key, object value)
        {
            if (_settings == null)
            {
                _settings = new VibrationSet();
            }

            _settings.Params.Values[key] = value;

            string saveAudioData = JsonConvert.SerializeObject(_settings);
            _saveUtility.WriteFile(saveAudioData, _saveDirectory, _saveFileName);
        }

        public void Vibrate()
        {
            if (!_isEnabled)
            {
                return;
            }

            //If you want to add custom vibration - you sould write code here instead of default Unity vibration.
            Handheld.Vibrate();
        }
    }
}