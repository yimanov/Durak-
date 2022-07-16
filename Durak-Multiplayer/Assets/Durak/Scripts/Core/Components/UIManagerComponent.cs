using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class UIManagerComponent : MonoBehaviour
    {
        public CanvasScaler CanvasScaler;
        public RectTransform MainRoot;
        public RectTransform ContentRoot;
        public RectTransform HudRect;

        [Header("Views:")]
        public EndGameView EndGameWindow;
        public SettingsView SettingsWindow;
        public GameFieldComponent GameField;
    }
}