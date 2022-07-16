using Meta.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Durak
{
    public enum DurakType
    {
        Simple,
        ThrowIn
    }

    public enum DurakTransferType
    {
        None,
        WithTransfer
    }

    public class DurakGameData
    {
        public int PlayersCount;
        public int CardsPerHand;
        public bool HideAiCards;
        public Difficulty GameDifficulty;
        public DurakType GameType;
        public DurakTransferType GameTransferType;

        public GamePlacesComponent PlacesComponent;
        public UIManagerComponent UiComponent;
    }

    public class DurakController
    {
        public event Action GameStartedEvent;
        public event Action<GameState> ChangeStateEvent;

        public GameState CurState { get; private set; }
        public DurakType GameType { get; private set; }
        public DurakTransferType GameTransferType { get; private set; }

        private AIController _oponentAI;

        private PlayerController _attackingHand;
        private PlayerController _defendingHand;

        private TableController _tableController;
        private DeckController _deckController;
        private DataHolderConroller _dataHolderController;
        private SessionManager _sessionManager;
        private GameResultController _gameResultController;
        private PlayersObserveController _playersObserveController;
        private GamePlacesComponent _gamePlacesComponent;
        private UIManagerComponent _uiComponent;
        private AudioController _audioController;
//        private AdsController _adsController;

        private bool _successfulDefense;
        private bool _firstMove;
        private int _cardsPerHand;
        private int _currentStartPlayerTurnId;
        private int _currentAttackPlayerAfterPass;
        private int _startAttackId;
        private int _currentDefenderPlayerId;

        public void PreInitialize()
        {
            _tableController = LazySingleton<TableController>.Instance;
            _deckController = LazySingleton<DeckController>.Instance;
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _sessionManager = LazySingleton<SessionManager>.Instance;
            _gameResultController = LazySingleton<GameResultController>.Instance;
            _playersObserveController = LazySingleton<PlayersObserveController>.Instance;
            _audioController = LazySingleton<AudioController>.Instance;
          //  _adsController = LazySingleton<AdsController>.Instance;

            _gameResultController.ResultEvent += OnResultGot;

            _sessionManager.EndSessionEvent += OnAllCardsBeat;
        }

        public async void Initialize(DurakGameData data)
        {
            UnsubscribeTemporaryEvents();

            ChangeState(GameState.Start);

            _gamePlacesComponent = data.PlacesComponent;
            _uiComponent = data.UiComponent;

            _oponentAI = AIResolver.ResolveAI(data.GameDifficulty);
            _oponentAI.Initialize();

            _gameResultController.Initialize();

            _cardsPerHand = data.CardsPerHand;

            GameType = data.GameType;
            GameTransferType = data.GameTransferType;

            if (GameType == DurakType.ThrowIn)
            {
                _sessionManager.PairFormedEvent += OnAttack;
                _sessionManager.PassedEvent += CalculateNextTurnAfterPass;
            }

            _sessionManager.PairNotFormedYetEvent += OnDefend;

            _successfulDefense = true;
            _firstMove = true;

            var players = _dataHolderController.Players;
            for (int j = 0; j < players.Count; j++)
            {
                var player = players[j];

                player.HideCards = player.IsAi && data.HideAiCards;
            }

            await Utils.WaitForEndOfFrame();

            Draw();

            while (CurState == GameState.Draw)
            {
                await Task.Yield();
            }
            _startAttackId = GetCurrentPlayerTurnId();
            _currentStartPlayerTurnId = _startAttackId;
            _currentDefenderPlayerId = GetNextPlayerId(_currentStartPlayerTurnId);

            _attackingHand = players[_currentStartPlayerTurnId];
            _defendingHand = players[_currentDefenderPlayerId];

            CalculateNextTurn();

            GameStartedEvent?.Invoke();
        }

        public void Sort()
        {
            var user = _dataHolderController.Players[GameConstants.UserPlayerId];
            user.CardsInHand = user.CardsInHand.OrderBy(x => x.Data.Suit).ThenBy(x => x.Data.Priority).ToList();
            for (int i = 0; i < user.CardsInHand.Count; i++)
            {
                user.CardsInHand[i].gameObject.transform.SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// Callback for ResultEvent.
        /// </summary>
        private void OnResultGot(ComparedResultInfo info)
        {

            int pushups = UnityEngine.Random.Range(5, 25);
            switch (info.Result)
            {
                case GameResult.Lose:
                    var loseData = new EndGameData() { Info = $"You Lost. You have to do "+ pushups + " push-ups"};
                    EndGameProcess(loseData, ClipType.LoseResult);
                //    _adsController.ShowRewardBasedVideo();
                    break;
                case GameResult.Win:
                    var winData = new EndGameData() { Info = $"You Won. Congrats" };
                    EndGameProcess(winData, ClipType.WinResult);
                    break;
                case GameResult.Drawn:
                    var drawData = new EndGameData() { Info = $"It is a Draw." };
                    EndGameProcess(drawData, ClipType.DrawnResult);
                //    _adsController.ShowInterstitial();
                    break;
            }
        }

        /// <summary>
        /// End game process for player.
        /// </summary>
        private void EndGameProcess(EndGameData endInfo, ClipType type)
        {
            Dispose();
            ChangeState(GameState.End);
            _uiComponent.EndGameWindow.Show(endInfo);
            _audioController.PlayShot(AudioSourceType.SFX, type);
        }

        /// <summary>
        /// Attack process for player.
        /// </summary>
        private void Attack()
        {
            var activePlayerId = _currentStartPlayerTurnId != GameConstants.UserPlayerId ? _currentStartPlayerTurnId : _attackingHand.Id;
            _playersObserveController.RegisterActivePlayer(activePlayerId);

            ChangeState(GameState.Attack);

            int attackPlayerId = _currentStartPlayerTurnId;

            if (attackPlayerId != GameConstants.UserPlayerId)
            {
                MoveData moveInput = new MoveData()
                {
                    Attacker = _dataHolderController.Players[attackPlayerId],
                    Defender = _dataHolderController.Players[GetNextPlayerId(attackPlayerId)],
                    AtkCard = null
                };
                CardComponent passedCard = _oponentAI.MakeMove(moveInput);

                if (passedCard == null)
                {
                    Pass();
                }
                else
                {
                    _sessionManager.RegisterTurn(passedCard.CardId);
                    _audioController.PlayShot(AudioSourceType.SFX, ClipType.Place);
                }
            }
        }

        /// <summary>
        /// Called when user should continue attack.
        /// </summary>
        private async void OnAttack()
        {
            await Utils.Wait(0.5f);

            Attack();
        }

        /// <summary>
        /// Called when user should continue defend.
        /// </summary>
        private async void OnDefend()
        {
            await Utils.Wait(0.5f);

            Defend();
        }

        /// <summary>
        /// Defend process for player.
        /// </summary>
        private void Defend()
        {
            var nextPlayerId = _currentDefenderPlayerId;
            var activePlayerId = nextPlayerId != GameConstants.UserPlayerId ? nextPlayerId : _defendingHand.Id;
            _playersObserveController.RegisterActivePlayer(activePlayerId);

            ChangeState(GameState.Defense);

            int defendPlayerId = nextPlayerId;
            if (nextPlayerId != GameConstants.UserPlayerId)
            {
                MoveData moveInput = new MoveData()
                {
                    Attacker = _dataHolderController.Players[GetNextPlayerId(defendPlayerId)],
                    Defender = _dataHolderController.Players[defendPlayerId],
                    AtkCard = _tableController.AttackCard
                };
                CardComponent passedCard = _oponentAI.MakeMove(moveInput);

                if (passedCard == null)
                {
                    GiveUp();
                }
                else
                {
                    _audioController.PlayShot(AudioSourceType.SFX, ClipType.Place);
                    _sessionManager.RegisterTurn(passedCard.CardId);
                }
            }
        }

        /// <summary>
        /// Give UP process for player.
        /// </summary>
        public async void GiveUp()
        {
            if (CurState != GameState.Defense)
            {
                return;
            }

            var resetedCardIds = _sessionManager.ResetSession();

            _deckController.GiveUp(_playersObserveController.ActivePlayerId, resetedCardIds);

            _successfulDefense = false;

            await Utils.Wait(0.5f);

            NewTurn();
        }

        /// <summary>
        /// Callback for EndSessionEvent.
        /// </summary>
        private async void OnAllCardsBeat()
        {
            await Utils.Wait(0.5f);

            ChangeState(GameState.ResetHand);

            for (int i = 0; i < _dataHolderController.Players.Count; i++)
            {
                _dataHolderController.Players[i].Lock();
            }

            var resetedCardIds = _sessionManager.ResetSession();

            _deckController.DiscardPile(resetedCardIds);

            _successfulDefense = true;

            await Utils.Wait(0.5f);

            NewTurn();
        }


        /// <summary>
        /// Called after resolve of last card hand.
        /// </summary>
        private async void NewTurn()
        {
            Draw();

            while (CurState == GameState.Draw)
            {
                await Task.Yield();
            }

            CalculateNextTurn();
        }

        /// <summary>
        /// Card drawwing process.
        /// </summary>
        private async void Draw()
        {
            ChangeState(GameState.Draw);

            var players = _dataHolderController.Players;

            var hasBeenDraw = false;

            for (int i = 0; i < players.Count; i++)
            {
                if (_dataHolderController.Cards.Count <= 0)
                {
                    break;
                }

                for (int j = 0; j < _cardsPerHand; j++)
                {
                    if (players[i].CardsInHand.Count >= _cardsPerHand)
                    {
                        break;
                    }

                    hasBeenDraw = true;

                    var drawCard = _deckController.Draw();
                    players[i].AddCard(drawCard);
                    _audioController.PlayShot(AudioSourceType.SFX, ClipType.Take);
                    await Utils.Wait(0.03f);

                }
            }

            if (hasBeenDraw)
                await Utils.Wait(0.3f);

            ChangeState(GameState.DrawFinished);

            ComparableResultInfo info = new ComparableResultInfo()
            {
                CardsCount = _dataHolderController.Cards.Count
            };

            _gameResultController.CheckResult(info);
        }

        /// <summary>
        /// Pass current turn available only for attack.
        /// </summary>
        public void Pass()
        {
            _sessionManager.Pass();
        }

        /// <summary>
        /// Steps that will be called after pass action.
        /// </summary>
        private async void CalculateNextTurnAfterPass()
        {
            if (CurState == GameState.End)
            {
                return;
            }

            var players = _dataHolderController.Players;

            _currentAttackPlayerAfterPass = GetNextPlayerIdWithIgnor(_currentStartPlayerTurnId, _currentDefenderPlayerId);
            _currentStartPlayerTurnId = _currentAttackPlayerAfterPass;
            int attackPlayerId = _currentStartPlayerTurnId;
            _attackingHand = players[attackPlayerId];

            _gamePlacesComponent.SetPlaceActive(_attackingHand.Id, _defendingHand.Id);

            await Utils.Wait(1f);

            Attack();
        }

        /// <summary>
        /// Calculated which players game in next hand.
        /// </summary>
        private async void CalculateNextTurn()
        {
            if (CurState == GameState.End)
            {
                return;
            }

            if (!_firstMove)
            {
                var players = _dataHolderController.Players;

                if (_successfulDefense)
                {
                    int attackPlayerId = GetNextPlayerId(_startAttackId);
                    _attackingHand = players[attackPlayerId];

                    int defendPlayerId = GetNextPlayerId(attackPlayerId);
                    _currentDefenderPlayerId = defendPlayerId;
                    _defendingHand = players[_currentDefenderPlayerId];
                    _currentStartPlayerTurnId = attackPlayerId;
                    _startAttackId = _currentStartPlayerTurnId;
                    _successfulDefense = false;
                }
                else
                {
                    int skippedTurnPlayerId = GetNextPlayerId(_startAttackId);
                    int attackPlayerId = GetNextPlayerId(skippedTurnPlayerId);
                    _attackingHand = players[attackPlayerId];

                    int defendPlayerId = GetNextPlayerId(attackPlayerId);
                    _currentDefenderPlayerId = defendPlayerId;
                    _defendingHand = players[_currentDefenderPlayerId];
                    _currentStartPlayerTurnId = attackPlayerId;
                    _startAttackId = _currentStartPlayerTurnId;
                }
            }
            else
            {
                _firstMove = false;
            }

            _gamePlacesComponent.SetPlaceActive(_attackingHand.Id, _defendingHand.Id);

            await Utils.Wait(1f);

            Attack();
        }

        /// <summary>
        /// Get current run player id.
        /// </summary>
        /// <returns></returns>
        private int GetCurrentPlayerTurnId()
        {
            int id = -1;
            var minTrumpCard = -1;

            for (int i = 0; i < _dataHolderController.Players.Count; i++)
            {
                var cardsArrayOfPlayer = _dataHolderController.Players[i].CardsInHand;
                for (int j = 0; j < cardsArrayOfPlayer.Count; j++)
                {
                    if (minTrumpCard != -1)
                    {
                        var trumps = _dataHolderController.Players[i].CardsInHand.Where(x => x.Data.Suit == _deckController.TrumpSuit);
                        var minTrumpCardOfCurrentPlayer = trumps.Any() ? trumps.Min(y => y.Data.Priority) : -1;

                        if (minTrumpCard > minTrumpCardOfCurrentPlayer)
                        {
                            minTrumpCard = minTrumpCardOfCurrentPlayer;
                            id = i;
                        }
                    }
                    else
                    {
                        var trumps = _dataHolderController.Players[i].CardsInHand.Where(x => x.Data.Suit == _deckController.TrumpSuit);
                        minTrumpCard = trumps.Any() ? trumps.Min(y => y.Data.Priority) : -1;

                        if (minTrumpCard != -1)
                        {
                            id = i;
                        }
                    }
                }
            }
            //if equals -1 need to write logic of getting then
            return id != -1 ? id : 0;
        }

        /// <summary>
        /// Get player after player with id
        /// </summary>
        private int GetNextPlayerId(int currentPlayeId)
        {
            int nextPlayerId = currentPlayeId == _dataHolderController.Players.Count - 1 ? 0 : currentPlayeId + 1;
            bool isNextPlayerFound = false;

            for (int i = 0; i < _dataHolderController.Players.Count; i++)
            {
                if (_dataHolderController.Players[nextPlayerId].HasCards)
                {
                    isNextPlayerFound = true;
                    break;
                }
                else
                {
                    nextPlayerId = nextPlayerId == _dataHolderController.Players.Count - 1 ? 0 : nextPlayerId + 1;
                }
            }
            return isNextPlayerFound ? nextPlayerId : -1;
        }

        /// <summary>
        /// Get next player igmnoring specific id.
        /// </summary>
        private int GetNextPlayerIdWithIgnor(int currentPlayeId, int ignorId)
        {
            int nextPlayerId = currentPlayeId == _dataHolderController.Players.Count - 1 ? 0 : currentPlayeId + 1;
            bool isNextPlayerFound = false;

            for (int i = 0; i < _dataHolderController.Players.Count; i++)
            {
                if (_dataHolderController.Players[nextPlayerId].HasCards && nextPlayerId != ignorId)
                {
                    isNextPlayerFound = true;
                    break;
                }
                else
                {
                    nextPlayerId = nextPlayerId == _dataHolderController.Players.Count - 1 ? 0 : nextPlayerId + 1;
                }
            }

            return isNextPlayerFound ? nextPlayerId : -1;
        }

        /// <summary>
        /// Get start attack id.
        /// </summary>
        public int GetStartAttackId()
        {
            return _startAttackId;
        }

        /// <summary>
        /// Get defend attack id.
        /// </summary>
        public int GetDefenderId()
        {
            return _currentDefenderPlayerId;
        }

        /// <summary>
        /// Check ability to add card to table.
        /// </summary>
        /// <param name="atkCard"></param>
        /// <param name="defCard"></param>
        /// <returns></returns>
        public bool IsCanCardAddToTable(CardComponent atkCard, CardComponent defCard)
        {
            if (atkCard == null)
            {
                return false;
            }

            if (atkCard != null && defCard == null && CurState == GameState.Attack)
            {
                if (_sessionManager.CardsOnTable.Count > 0)
                {
                    return GameType != DurakType.ThrowIn || _sessionManager.CardsOnTable.Contains(atkCard.Data.Priority);
                }
                else
                {
                    return true;
                }
            }

            if (atkCard && defCard)
            {
                if (defCard.Data.Suit == _deckController.TrumpSuit)
                {
                    return atkCard.Data.Suit != _deckController.TrumpSuit ||
                            defCard.Data.Priority > atkCard.Data.Priority;
                }
                else
                {
                    return defCard.Data.Suit == atkCard.Data.Suit &&
                            defCard.Data.Priority > atkCard.Data.Priority;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Changing game state process.
        /// </summary>
        /// <param name="state"></param>
        private void ChangeState(GameState state)
        {
            CurState = state;

            ChangeStateEvent?.Invoke(CurState);
        }

        private void UnsubscribeTemporaryEvents()
        {
            _sessionManager.PairFormedEvent -= OnAttack;
            _sessionManager.PairNotFormedYetEvent -= OnDefend;
            _sessionManager.PassedEvent -= CalculateNextTurnAfterPass;
        }

        private void Dispose()
        {
            _sessionManager.PairFormedEvent -= OnAttack;
            _sessionManager.PairNotFormedYetEvent -= OnDefend;
            _sessionManager.PassedEvent -= CalculateNextTurnAfterPass;
        }
    }
}
