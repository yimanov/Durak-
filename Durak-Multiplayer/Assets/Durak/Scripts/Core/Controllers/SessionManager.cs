using Meta.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Durak
{
    public class SessionCardsPair
    {
        public CardComponent AttackCard;
        public CardComponent DefendCard;

        public bool IsPairFormed
        {
            get
            {
                return AttackCard != null && DefendCard != null;
            }
        }
    }

    public class SessionManager
    {
        /// <summary>
        /// Called when attacking and Defender players maked their turns
        /// </summary>
        public event Action PairNotFormedYetEvent;
        public event Action PassedEvent;
        public event Action PairFormedEvent;
        public event Action EndSessionEvent;

        /// <summary>
        /// playerId, cardId
        /// </summary>
        public Dictionary<int, List<int>> CardsDict = new Dictionary<int, List<int>>();
        public List<int> CardsOnTable = new List<int>();

        public List<SessionCardsPair> Pairs;

        private int _passCounter;

        private DataHolderConroller _dataHolderController;
        private PlayersObserveController _playersObserveController;
        private DurakController _durakController;
        private GamePlacesComponent _gamePlacesComponent;
        private TableController _tableController;
        private VibrationController _vibrationController;
        private AudioController _audioController;

        internal void PreInitialize()
        {
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _playersObserveController = LazySingleton<PlayersObserveController>.Instance;
            _durakController = LazySingleton<DurakController>.Instance;
            _tableController = LazySingleton<TableController>.Instance;
            _vibrationController = LazySingleton<VibrationController>.Instance;
            _audioController = LazySingleton<AudioController>.Instance;

            _tableController.CardAddedToTableEvent += CheckPairStatus;
        }

        public void Initialize(GamePlacesComponent gamePlacesComponent)
        {
            _gamePlacesComponent = gamePlacesComponent;
            Pairs = new List<SessionCardsPair>()
            {
                new SessionCardsPair(){ AttackCard = null, DefendCard = null }
            };

            CardsDict.Clear();
            CardsOnTable.Clear();
            _passCounter = 0;
        }

        /// <summary>
        /// Validating of card using.
        /// </summary>
        public void OnValidateCard(CardComponent card)
        {
            float distanceToStartPos = card.transform.localPosition.y - card.StartPosY;

            if (distanceToStartPos > 75)
            {
                RegisterTurn(card.CardId);
            }
            else
            {
                card.transform.ResetTransformLocal();
                _gamePlacesComponent.Rebuild(_playersObserveController.ActivePlayerId);
                _vibrationController.Vibrate();
                _audioController.PlayShot(AudioSourceType.SFX, ClipType.Cancel);
            }
        }

        /// <summary>
        /// Start handling card on current session.
        /// </summary>
        /// <param name="cardId">Id of card</param>
        public void RegisterTurn(int cardId)
        {
            var card = cardId.ToCard();
            var player = _dataHolderController.Players[_playersObserveController.ActivePlayerId];

            if (card == null || player == null)
            {
                return;
            }

            var currentPair = GetCurrentPair();

            if (currentPair != null)
            {
                if (_durakController.CurState == GameState.Attack)
                {
                    currentPair.AttackCard = card;
                }
                else if (_durakController.CurState == GameState.Defense)
                {
                    currentPair.DefendCard = card;
                }
            }

            if (_durakController.IsCanCardAddToTable(currentPair.AttackCard, currentPair.DefendCard))
            {
                player.CardsInHand.Remove(card);

                if (!CardsDict.ContainsKey(_playersObserveController.ActivePlayerId))
                {
                    CardsDict.Add(_playersObserveController.ActivePlayerId, new List<int>() { cardId });
                }
                else
                {
                    CardsDict[_playersObserveController.ActivePlayerId].Add(cardId);
                }

                CardsOnTable.Add(card.Data.Priority);
                _tableController.AddCardToTable(cardId);
            }
            else
            {
                if (currentPair != null)
                {
                    if (_durakController.CurState == GameState.Attack)
                    {
                        currentPair.AttackCard = null;
                    }
                    else if (_durakController.CurState == GameState.Defense)
                    {
                        currentPair.DefendCard = null;
                    }
                }
                card.transform.ResetTransformLocal();
                _vibrationController.Vibrate();
            }

            _gamePlacesComponent.Rebuild(_playersObserveController.ActivePlayerId);
        }

        /// <summary>
        /// Check status of current pair on table.
        /// </summary>
        private void CheckPairStatus()
        {
            var currentPair = GetCurrentPair();

            if (currentPair.IsPairFormed)
            {
                switch (_durakController.GameType)
                {
                    case DurakType.Simple:
                        EndSessionEvent?.Invoke();
                        return;
                    case DurakType.ThrowIn:
                        var defenderCardsInHandCount = _dataHolderController.Players[_durakController.GetDefenderId()].CardsInHand.Count;
                        var playersWithCards = _dataHolderController.Players.Count(x => x.CardsInHand.Count > 0) - 1;
                        if (Pairs.Count == 6 || _passCounter >= playersWithCards || defenderCardsInHandCount == 0)
                        {
                            EndSessionEvent?.Invoke();
                            return;
                        }
                        break;
                }
                AddNewPairTemplate();
                PairFormedEvent?.Invoke();
            }
            else
            {
                PairNotFormedYetEvent?.Invoke();
            }
        }

        public void Pass()
        {
            _passCounter++;

            if (_durakController.GameType == DurakType.ThrowIn)
            {
                var activePlayers = _dataHolderController.Players.Count(x => x.HasCards);
                if (_passCounter == activePlayers - 1)
                {
                    EndSessionEvent?.Invoke();
                    return;
                }
                else
                {
                    PassedEvent?.Invoke();
                }
            }
        }

        /// <summary>
        /// Reset all card at the current session.
        /// </summary>
        public List<int> ResetSession()
        {
            var resetedCardIds = new List<int>();

            for (int i = 0; i < CardsDict.Count; i++)
            {
                var element = CardsDict.ElementAt(i);

                for (int j = 0; j < element.Value.Count; j++)
                {
                    var card = _dataHolderController.CardsDictionary[element.Value[j]];

                    var player = _dataHolderController.Players[element.Key];

                    player.RemoveCard(card);

                    _tableController.RemoveCardFromTable(card.CardId);

                    resetedCardIds.Add(card.CardId);
                }
            }

            CreateNewSession();
            CardsDict.Clear();
            CardsOnTable.Clear();
            _passCounter = 0;
            return resetedCardIds;
        }


        /// <summary>
        /// Get curren pair of session.
        /// </summary>
        private SessionCardsPair GetCurrentPair()
        {
            return Pairs[Pairs.Count - 1];
        }

        /// <summary>
        /// Start new session.
        /// </summary>
        private void CreateNewSession()
        {
            Pairs = new List<SessionCardsPair>()
            {
                new SessionCardsPair(){ AttackCard = null, DefendCard = null }
            };
        }

        /// <summary>
        /// Add clear pair.
        /// </summary>
        private void AddNewPairTemplate()
        {
            Pairs.Add(new SessionCardsPair() { AttackCard = null, DefendCard = null });
        }

        /// <summary>
        /// If cards exists on table.
        /// </summary>
        /// <returns></returns>
        public bool HasCardsOnTable()
        {
            return CardsOnTable.Count > 0;
        }
    }
}