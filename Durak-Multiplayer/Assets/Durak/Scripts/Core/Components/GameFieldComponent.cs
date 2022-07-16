using Meta.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class GameFieldComponent : PreInitedMonoBehaviour
    {
        private GameSettings _gameSettings;

#pragma warning disable 649
        [SerializeField] private UIManagerComponent _uiManagerComponent;
        [Space, SerializeField] private TableComponent _tableComponent;
        [SerializeField] private DeckComponent _deckComponent;
        [SerializeField] private GamePlacesComponent _gamePlacesComponent;
        [Space, SerializeField] private Button _giveUpBtn;
        [SerializeField] private Button _passBtn;
        [SerializeField] private Button _sortBtn;
#pragma warning restore 649

        private DataHolderConroller _dataHolderController;
        private DurakController _durakController;
        private SessionManager _sessionManager;
        private PlayersObserveController _playersObserveController;
//        private AdsController _adsController;
        private AudioController _audioController;
        private TableController _tableController;

        protected override void PreInitialize()
        {
            _durakController = LazySingleton<DurakController>.Instance;
            _sessionManager = LazySingleton<SessionManager>.Instance;
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _playersObserveController = LazySingleton<PlayersObserveController>.Instance;
          //  _adsController = LazySingleton<AdsController>.Instance;
            _audioController = LazySingleton<AudioController>.Instance;
            _tableController = LazySingleton<TableController>.Instance;

            _durakController.PreInitialize();
            _sessionManager.PreInitialize();
            _playersObserveController.RegisterActivePlayerEvent += OnPlayerChanged;
            _durakController.ChangeStateEvent += OnGameStateChanged;
            _tableController.CardAddedToTableEvent += OnCardAddedToTable;
            _giveUpBtn.DisableObj();

            _audioController.Play(AudioSourceType.Main, ClipType.BakgroundMusic, true, 0.1f);
        }

        /// <summary>
        /// Start game from settings panel.
        /// </summary>
        /// <param name="settings">Settimgs data.</param>
        public void StartGame(GameSettings settings)
        {
            _gameSettings = settings;
          //  _adsController.HideBanner();
            ResetGame();

            _dataHolderController.CardsDictionaryOrder.Clear();
            ExecuteGameStart();
        }

        /// <summary>
        /// Restart current game.
        /// </summary>
        public void RestartGame()
        {
            ResetGame();

            ExecuteGameStart();
        }

        /// <summary>
        /// Initialize all game parts.
        /// </summary>
        private void ExecuteGameStart()
        {
            _gamePlacesComponent.Initialize(new GamePlacesData() { PlayersCount = _gameSettings.PlayersCount, HideCards = _gameSettings.HideAiCards });
            _tableComponent.Initialize();
            _deckComponent.Generate();
            _sessionManager.Initialize(_gamePlacesComponent);
            _durakController.Initialize(new DurakGameData()
            {
                PlayersCount = _gameSettings.PlayersCount,
                CardsPerHand = _gameSettings.CardsPerHand,
                HideAiCards = _gameSettings.HideAiCards,
                PlacesComponent = _gamePlacesComponent,
                UiComponent = _uiManagerComponent,
                GameDifficulty = _gameSettings.GameDifficulty,
                GameTransferType = _gameSettings.GameTransferType,
                GameType = _gameSettings.GameType
            });
        }

        /// <summary>
        /// Called when click on give up button in game.
        /// </summary>
        public void GiveUp()
        {
            _durakController.GiveUp();
            _giveUpBtn.DisableObj();
        }

        /// <summary>
        /// Called when click on pass button in game.
        /// </summary>
        public void Pass()
        {
            _durakController.Pass();
            _passBtn.DisableObj();
        }


        /// <summary>
        /// Called when click on give up button in game.
        /// </summary>
        public void Sort()
        {
            _durakController.Sort();
        }

        /// <summary>
        /// Clear all game parts.
        /// </summary>
        private void ResetGame()
        {
            _dataHolderController.Clear();
            _deckComponent.Clear();
            _tableComponent.Clear();
            _gamePlacesComponent.ClearPlaces();
        }

        /// <summary>
        /// Callback of state changed event.
        /// </summary>
        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Start)
            {
                _sortBtn.EnableObj();
                return;
            }
            else if (state == GameState.End)
            {
                _giveUpBtn.DisableObj();
                _sortBtn.DisableObj();
                _passBtn.DisableObj();

                return;
            }
            else if (state == GameState.Defense)
            {
                var player = _dataHolderController.Players[_playersObserveController.ActivePlayerId];
                var giveUpBtnState = !player.IsAi && player.Id == _durakController.GetDefenderId();
                if (giveUpBtnState)
                {
                    _giveUpBtn.EnableObj();
                }
                else
                {
                    _giveUpBtn.DisableObj();
                }
            }
            else if (state == GameState.Attack)
            {
                var player = _dataHolderController.Players[_playersObserveController.ActivePlayerId];
                var passBtnState = !player.IsAi && _durakController.GameType == DurakType.ThrowIn
                                                && _sessionManager.HasCardsOnTable()
                                                && _playersObserveController.ActivePlayerId == GameConstants.UserPlayerId;
                if (passBtnState)
                {
                    _passBtn.EnableObj();
                }
            }
            else if (state == GameState.ResetHand)
            {
                _passBtn.DisableObj();
                _giveUpBtn.DisableObj();
            }
            else if (state == GameState.Draw)
            {
                _passBtn.DisableObj();
                _giveUpBtn.DisableObj();
            }
        }

        /// <summary>
        /// Callback of player changed event.
        /// </summary>
        private void OnPlayerChanged(int playerId)
        {
            
        }

        private void OnCardAddedToTable()
        {
            var player = _dataHolderController.Players[_playersObserveController.ActivePlayerId];
            _passBtn.DisableObj();

            var giveUpBtnState = !player.IsAi && _durakController.CurState == GameState.Defense;
            if (giveUpBtnState)
            {
                _giveUpBtn.DisableObj();
            }
        }

        protected override void Dispose()
        {
            if (_playersObserveController != null)
                _playersObserveController.RegisterActivePlayerEvent -= OnPlayerChanged;

            if (_durakController != null)
                _durakController.ChangeStateEvent -= OnGameStateChanged;

            if (_tableController != null)
                _tableController.CardAddedToTableEvent -= OnCardAddedToTable;
        }
    }
}