
using UnityEngine;
using System.Collections.Generic;
using Meta.Settings;

namespace Durak
{
    public class SettingsManagerComponent : PreInitedMonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _mainFirstSource;
        [SerializeField] private AudioSource _mainSecondSource;

        [Header("Content data:")]
        [SerializeField] private AudioClipsContainer _audioClipsContainer;
#pragma warning restore 649

        private AudioController _audioController;
        private VibrationController _vibrationController;

        public override void Awake()
        {
            base.Awake();
            Init();
        }

        protected override void PreInitialize()
        {
            _audioController =  LazySingleton<AudioController>.Instance;
            _vibrationController =  LazySingleton<VibrationController>.Instance;
        }

        public void Init()
        {
            Dictionary<AudioSourceType, AudioSource> sourcesDict = new Dictionary<AudioSourceType, AudioSource>
        {
            { AudioSourceType.Main, _mainFirstSource },
            { AudioSourceType.MainAdditional, _mainSecondSource },
            { AudioSourceType.SFX, _sfxSource }
        };

            _audioController.Init(_audioClipsContainer.Clips, sourcesDict);
            _vibrationController.Init();

            _mainFirstSource.mute = _audioController.IsMainMute;
            _mainSecondSource.mute = _audioController.IsMainMute;
            _sfxSource.mute = _audioController.IsSfxMute;
        }

        /// <summary>
        /// Change Mute state of main audio source.
        /// </summary>
        public void ChangeMainMuteState()
        {
            bool state = _mainFirstSource.mute;
            _audioController.SetMuteSource(AudioSourceType.Main, !state);
            _audioController.SetMuteSource(AudioSourceType.MainAdditional, !state);
        }

        /// <summary>
        /// Change Mute state of sfx audio source.
        /// </summary>
        public void ChangeSfxMuteState()
        {
            bool state = _sfxSource.mute;
            _audioController.SetMuteSource(AudioSourceType.SFX, !state);
        }
    }
}