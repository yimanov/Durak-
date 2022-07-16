using DG.Tweening;
using Meta.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class DeckControllData
    {
        public List<CardComponent> Cards;
        public Transform TrumpContainer;
        public Transform DiscardPileContainer;
        public CardsVisualContainer VisualContainer;
        public Image TrumpIcon;
    }

    public class DeckController
    {
        public CardSuit TrumpSuit;

        private DataHolderConroller _dataHolderController;
        private AudioController _audioController;
        private Transform _trumpContainer;
        private Transform _discardPileContainer;
        private CardsVisualContainer _visualContainer;
        private Image _trumpIcon;

        public void Initialize(DeckControllData data)
        {
            _audioController = LazySingleton<AudioController>.Instance;
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _dataHolderController.SetCards(data.Cards);
            _trumpContainer = data.TrumpContainer;
            _discardPileContainer = data.DiscardPileContainer;
            _visualContainer = data.VisualContainer;
            _trumpIcon = data.TrumpIcon;

            //Set last cards oredr. Using for restart game.
            if (_dataHolderController.CardsDictionaryOrder != null && _dataHolderController.CardsDictionaryOrder.Count > 0)
            {
                for (int i = 0; i < _dataHolderController.CardsDictionaryOrder.Count; i++)
                {
                    _dataHolderController.Cards[i] = _dataHolderController.CardsDictionary[_dataHolderController.CardsDictionaryOrder[i]];
                }
            }
            //Shuffle cards. Using for new game.
            else
            {
                Shuffle();
            }

            PickTrump();
        }

        /// <summary>
        /// Calculate which card will be a trump card.
        /// </summary>
        private void PickTrump()
        {
            CardComponent trumpCard = _dataHolderController.Cards[0];

            trumpCard.transform.SetParentWithSiblingIndex(_trumpContainer, 0);
            trumpCard.transform.ResetTransformLocal();
            trumpCard.FaceUp();

            TrumpSuit = trumpCard.Data.Suit;

            _trumpIcon.sprite = _visualContainer.GetSuitIcon(TrumpSuit);
            _trumpIcon.enabled = true;
        }

        /// <summary>
        /// Cards shuffle process.
        /// </summary>
        public void Shuffle()
        {
            _dataHolderController.Cards.Shufle();

            List<int> lastOrder = new List<int>();
            for (int i = 0; i < _dataHolderController.Cards.Count; i++)
            {
                lastOrder.Add(_dataHolderController.Cards[i].CardId);
            }
            _dataHolderController.SetCardsOrder(lastOrder);
        }

        /// <summary>
        /// Draw card  from cardsprocess.
        /// </summary>
        /// <returns></returns>
        public CardComponent Draw()
        {
            if (_dataHolderController.Cards.Count > 0)
            {
                CardComponent drawnCard = _dataHolderController.Cards[_dataHolderController.Cards.Count - 1];
                _dataHolderController.Cards.Remove(drawnCard);
                return drawnCard;
            }

            return null;
        }

        /// <summary>
        /// Discard pile process.
        /// </summary>
        /// <param name="cardIds"></param>
        public void DiscardPile(List<int> cardIds)
        {
            for (int i = 0; i < cardIds.Count; i++)
            {
                var card = _dataHolderController.CardsDictionary[cardIds[i]];

                card.Lock();

                card.transform.SetParentWithLastSiblingIndex(_discardPileContainer);

                Sequence moveToDiscardPileAnim = DOTween.Sequence();
                var midPoint = card.transform.position + (_discardPileContainer.transform.position - card.transform.position) / 2;
                var endPoint = _discardPileContainer.transform.position;
                var animationTime = 0.3f;

                moveToDiscardPileAnim.Append(card.transform.DORotate(new Vector3(0, 90, 0), animationTime / 2))
                                      .Join(card.transform.DOMove(midPoint, animationTime / 2))
                                      .Append(card.transform.DORotate(new Vector3(0, 0, 0), animationTime / 2).OnStart(card.FaceDown))
                                      .Join(card.transform.DOMove(endPoint, animationTime / 2))
                                      .SetEase(Ease.Linear)
                                      .OnComplete(() =>
                                        {
                                            card.transform.ResetTransformLocal();
                                        });
            }
            _audioController.PlayShot(AudioSourceType.SFX, ClipType.DiscardPile);
        }

        /// <summary>
        /// Give up cards process
        /// </summary>
        /// <param name="playerId">Player that make give up.</param>
        /// <param name="cardIds">Id of cards</param>
        public void GiveUp(int playerId, List<int> cardIds)
        {
            var player = _dataHolderController.Players[playerId];
            player.Lock();

            for (int i = 0; i < cardIds.Count; i++)
            {
                var card = _dataHolderController.CardsDictionary[cardIds[i]];
                player.AddCard(card);
            }
            _audioController.PlayShot(AudioSourceType.SFX, ClipType.GiveUp);
        }

        public CardsVisualContainer GetVisual()
        {
            return _visualContainer;
        }
    }
}