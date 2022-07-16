using Meta.Settings;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

namespace Durak
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleAudioClickComponent : MonoBehaviour
    {
        [SerializeField] private ClipType _type = ClipType.Click;
        private Toggle _uiElement;
        private AudioController _audioController;

        public void Start()
        {
            if (!_uiElement)
            {
                _uiElement = GetComponent<Toggle>();
            }

            _uiElement.onValueChanged.AddListener(OnClick);

            _audioController = LazySingleton<AudioController>.Instance;
        }

        public void Awake()
        {
            if (!_uiElement)
            {
                _uiElement = GetComponent<Toggle>();
            }

            _uiElement.onValueChanged.AddListener(OnClick);
        }

        private void OnClick(bool state)
        {
            _audioController.PlayShot(AudioSourceType.SFX, _type, 0.5f);
        }
    }
}