using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Durak
{
    public class PlayerGameData
    {
        public int Id;
        public bool IsAi;
        public bool HideCards;
        public RectTransform Place;
        public GamePlacesComponent GamePlacesComponent;
    }

    public class PlayerController
    {
        public int Id;
        public bool IsAi;
        public bool HideCards;

        public bool HasCards
        {
            get
            {
                return CardsInHand.Count > 0;
            }
        }
        public List<CardComponent> Cards;
        public List<CardComponent> CardsInHand;

        private bool _isLocked;
        private Transform _root;
        private GamePlacesComponent _gamePlacesComponent;
        private TableController _tableController;
        private TableBufferController _tableBufferController;
        private PlayersObserveController _playersObserveController;

        public void Initialize(PlayerGameData data)
        {
            Id = data.Id;
            _root = data.Place;
            IsAi = data.IsAi;
            HideCards = data.HideCards;
            _isLocked = true;
            _gamePlacesComponent = data.GamePlacesComponent;

            Cards = new List<CardComponent>();
            CardsInHand = new List<CardComponent>();
            _tableController = LazySingleton<TableController>.Instance;
            _tableBufferController = LazySingleton<TableBufferController>.Instance;
            _playersObserveController = LazySingleton<PlayersObserveController>.Instance;

            _tableController.CardAddedToTableEvent += OnCardAdded;
            _playersObserveController.RegisterActivePlayerEvent += OnPlayerActive;
        }

        public void AddCard(CardComponent card, bool isAnimated = true)
        {
            if (card != null)
            {
                _tableBufferController.ToBuffer(card.transform);
                Cards.Add(card);
                CardsInHand.Add(card);

                if (_isLocked)
                    card.Lock();

                if (!isAnimated)
                {
                    return;
                }

                Sequence moveToPlayerAnim = DOTween.Sequence();
                var midPoint = card.transform.position + (_root.transform.position - card.transform.position) / 2;
                var endPoint = _root.transform.position;
                var animationTime = 0.3f;

                //If we want to hide card
                if (HideCards)
                {
                    //If current side of card is face
                    if (card.IsFaceUp())
                    {

                        moveToPlayerAnim.Append(card.transform.DORotate(new Vector3(0, -90, 0), animationTime / 2))
                                         .Join(card.transform.DOMove(midPoint, animationTime / 2))
                                         .Append(card.transform.DORotate(new Vector3(0, 0, 0), animationTime / 2).OnStart(card.FaceDown))
                                         .Join(card.transform.DOMove(endPoint, animationTime / 2));
                    }
                    else
                    {
                        moveToPlayerAnim.Append(card.transform.DOMove(_root.transform.position, animationTime));
                    }
                }
                else //If we don't want to hide card
                {
                    if (card.IsFaceUp())
                    {
                        moveToPlayerAnim.Append(card.transform.DOMove(_root.transform.position, animationTime))
                                        .Join(card.transform.DORotate(new Vector3(0, 0, 0), animationTime));
                    }
                    else
                    {
                        moveToPlayerAnim.Append(card.transform.DORotate(new Vector3(0, -90, 0), animationTime / 2))
                                             .Join(card.transform.DOMove(midPoint, animationTime / 2))
                                             .Append(card.transform.DORotate(new Vector3(0, 0, 0), animationTime / 2).OnStart(card.FaceUp))
                                             .Join(card.transform.DOMove(endPoint, animationTime / 2));

                    }
                }

                moveToPlayerAnim.SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, false)
                                 .OnComplete(() =>
                                 {
                                     card.transform.SetParent(_root);
                                     card.transform.ResetTransformLocal();
                                     _gamePlacesComponent.Rebuild(Id);
                                 });
            }
        }

        /// <summary>
        /// Remove ccard from player.
        /// </summary>
        public void RemoveCard(CardComponent card)
        {
            if (card != null)
            {
                Cards.Remove(card);
            }
        }

        /// <summary>
        /// Lock card access.
        /// </summary>
        public void Lock()
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                var card = Cards[i];
                card.Lock();
            }

            _isLocked = true;
        }

        /// <summary>
        /// Unlock card access
        /// </summary>
        public void Unlock()
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                var card = Cards[i];
                card.Unlock();
            }

            _isLocked = false;
        }

        private void OnCardAdded()
        {
            Lock();
        }

        private void OnPlayerActive(int activePlayerId)
        {
            if (activePlayerId == Id)
            {
                Unlock();
            }
            else
            {
                Lock();
            }
        }

        public void Dispose()
        { 
            if (_tableController != null)
                _tableController.CardAddedToTableEvent -= OnCardAdded;

            if (_playersObserveController != null)
                _playersObserveController.RegisterActivePlayerEvent -= OnPlayerActive;
        }
    }
}