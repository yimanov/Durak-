using UnityEngine;
using System;
using System.Collections.Generic;
using Durak;
using Newtonsoft.Json;

namespace Meta.Settings
{
    public class FadeSetings
    {
        public float StartValue;
        public float EndValue;
    }

    public enum AudioSourceType
    {
        Null = 0,
        Main,
        MainAdditional,
        SFX
    }

    [Serializable]
    public class AudioSet
    {
        public AudioSourceParams SfxAudioParams;
        public AudioSourceParams MainAudioParams;

        public AudioSet()
        {
            SfxAudioParams = new AudioSourceParams();
            MainAudioParams = new AudioSourceParams();
        }

        [Serializable]
        public class AudioSourceParams : BaseParams
        {
            public AudioSourceParams()
            {
                Values = new Dictionary<string, object>()
                {
                    {"Volume", 1f },
                    {"Enabled", true },
                    {"Mute", false }
                };
            }
        }
    }

    public class AudioController : Settings
    {
        private SaveUtility _saveUtility;

        private Dictionary<ClipType, AudioClip> _clipsDict;
        private Dictionary<AudioSourceType, AudioSource> _sourcesDict;

        private AudioSet _settings;
        
        protected override string SaveDirectory => "AudioSave";
        protected override string SaveFileName => "AudioSettings";

        public bool IsSfxMute => (bool)_settings.SfxAudioParams.Values["Mute"];
        public bool IsMainMute => (bool)_settings.MainAudioParams.Values["Mute"];

        private AudioSourceType _activeMainSource = AudioSourceType.Null;

        protected override void LoadSettings()
        {
            string settingsData = _saveUtility.ReadFile(SaveDirectory, SaveFileName);

            if (string.IsNullOrEmpty(settingsData))
            {
                CreateNewSettings();
                return;
            }

            _settings = JsonConvert.DeserializeObject<AudioSet>(settingsData);
        }

        protected override void CreateNewSettings()
        {
            _settings = new AudioSet();

            string saveAudioData = JsonConvert.SerializeObject(_settings);
            _saveUtility.WriteFile(saveAudioData, SaveDirectory, SaveFileName);
        }

        public void Init(List<AudioClipData> clips, Dictionary<AudioSourceType, AudioSource> sources)
        {
            _saveUtility = LazySingleton<SaveUtility>.Instance;

            LoadSettings();

            _clipsDict = new Dictionary<ClipType, AudioClip>();
            _sourcesDict = new Dictionary<AudioSourceType, AudioSource>(sources);

            for (int i = 0; i < clips.Count; i++)
            {
                AudioClipData item = clips[i];

                if (_clipsDict.ContainsKey(item.Type))
                {
                    continue;
                }

                _clipsDict.Add(item.Type, item.Clip);
            }
        }

        public void SwitchSourceState(AudioSourceType type)
        {
            AudioSource source = _sourcesDict[type];

            if (!CheckSource(source))
            {
                return;
            }

            source.enabled = !source.enabled;

            SaveSettingsByType(type, "Enabled", source.enabled);
        }

        public void SwitchSourceMuteState(AudioSourceType type)
        {
            AudioSource source = _sourcesDict[type];

            if (!CheckSource(source))
            {
                return;
            }

            source.mute = !source.mute;

            SaveSettingsByType(type, "Mute", source.mute);
        }

        public void SetEnableSource(AudioSourceType type, bool value)
        {
            AudioSource source = _sourcesDict[type];

            if (!CheckSource(source))
            {
                return;
            }

            source.enabled = value;

            SaveSettingsByType(type, "Enabled", value);
        }

        public void SetMuteSource(AudioSourceType type, bool value)
        {
            AudioSource source = _sourcesDict[type];

            if (!CheckSource(source))
            {
                return;
            }

            source.mute = value;

            SaveSettingsByType(type, "Mute", value);
        }

        public void SetVolumeSource(AudioSourceType type, float value)
        {
            AudioSource source = _sourcesDict[type];

            if (!CheckSource(source))
            {
                return;
            }

            source.volume = value;

            SaveSettingsByType(type, "Volume", value);
        }

        public float GetActiveAudioSourceVolume()
        {
            AudioSource source = _sourcesDict[_activeMainSource];

            if (!CheckSource(source))
            {
                return 0;
            }

            return source.volume;
        }

        public void Stop(AudioSourceType sourceType)
        {
            AudioSource source = _sourcesDict[sourceType];

            if (!CheckSource(source))
            {
                return;
            }

            source.Stop();
        }

        public void Pause()
        {
            AudioSource source = _sourcesDict[_activeMainSource];

            if (!CheckSource(source))
            {
                return;
            }

            source.Pause();
        }

        public void Unpause()
        {
            AudioSource source = _sourcesDict[_activeMainSource];

            if (!CheckSource(source))
            {
                return;
            }

            source.UnPause();
        }

        public void PlayShot(AudioSourceType sourceType, ClipType clip, float sourceVolume = 1f)
        {
            AudioSource source = _sourcesDict[sourceType];

            if (!CheckSource(source))
            {
                return;
            }

            AudioClip audio = _clipsDict != null && _clipsDict.Count > 0 ? _clipsDict[clip] : null;
            if (!audio)
            {
                return;
            }

            source.PlayOneShot(audio, sourceVolume);
        }

        public void Play(AudioSourceType sourceType, ClipType clip, bool loop, float sourceVolume = 1f)
        {
            AudioSource source = _sourcesDict[sourceType];

            if (!CheckSource(source))
            {
                return;
            }

            AudioClip audio = _clipsDict != null && _clipsDict.Count > 0 ? _clipsDict[clip] : null;
            if (!audio)
            {
                return;
            }

            source.loop = loop;
            source.clip = audio;
            source.volume = sourceVolume;
            source.Play();
        }

        public void Play(AudioSourceType sourceType, AudioClip clip, bool loop, float sourceVolume = 1f)
        {
            AudioSource source = _sourcesDict[sourceType];

            if (!CheckSource(source))
            {
                return;
            }

            AudioClip audio = clip;
            if (!audio)
            {
                return;
            }

            source.loop = loop;
            source.clip = audio;
            source.volume = sourceVolume;
            source.Play();
        }

        private void SaveSettingsByType(AudioSourceType type, string key, object value)
        {
            if (_settings == null)
            {
                _settings = new AudioSet();
            }

            switch (type)
            {
                case AudioSourceType.Main:
                case AudioSourceType.MainAdditional:
                    _settings.MainAudioParams.Values[key] = value;
                    break;
                case AudioSourceType.SFX:
                    _settings.SfxAudioParams.Values[key] = value;
                    break;
            }

            string saveAudioData = JsonConvert.SerializeObject(_settings);
            _saveUtility.WriteFile(saveAudioData, SaveDirectory, SaveFileName);
        }

        private bool CheckSource(AudioSource source)
        {
            bool exist = true;

            if (!source)
            {
                exist = false;
            }

            return exist;
        }
    }
}