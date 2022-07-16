using Meta.Settings;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Durak
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudioClickComponent : MonoBehaviour
    {
        [SerializeField] private ClipType _type = ClipType.Click;
        private Button _uiElement;
        private AudioController _audioController;

        public void Start()
        {
            if (!_uiElement)
            {
                _uiElement = GetComponent<Button>();
            }

            _uiElement.onClick.AddListener(OnClick);

            _audioController = LazySingleton<AudioController>.Instance;
        }

        private void OnClick()
        {
            _audioController.PlayShot(AudioSourceType.SFX, _type, 0.5f);
        }
    }
}