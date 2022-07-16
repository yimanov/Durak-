using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class EndGameView : WindowView
    {
#pragma warning disable 649
        [SerializeField]        private UIManagerComponent _uiManagerComponent;
        [Space, SerializeField] private Text _resultText;
#pragma warning restore 649

        public override void Show(WindowData data)
        {
            base.Show(data);

            var endGameInfo = data as EndGameData;

            if (endGameInfo != null)
            {
                _resultText.text = endGameInfo.Info;
            }
        }

        public void Restart()
        {
            Hide();
            _uiManagerComponent.GameField.RestartGame();
        }

        public void NewGame()
        {
            Hide();
            _uiManagerComponent.SettingsWindow.Show(null);
        }
    }
}

public class EndGameData : WindowData
{
    public string Info;
}