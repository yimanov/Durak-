using DG.Tweening;
using Meta.Settings;
using System;
using UnityEngine;

namespace Durak
{
    public class TableData
    {
        public Transform AttackingRoot;
        public Transform DefendingRoot;
    }

    public class TableController
    {
        public event Action CardAddedToTableEvent;

        private Transform _attackingCardRoot;
        private Transform _defendingCardRoot;

        private TableBufferController _tableBufferController;
        private DurakController _durakController;
        private DataHolderConroller _dataHolderController;
        private AudioController _audioController;

        public CardComponent AttackCard;
        public CardComponent DefendCard;

        public void Initialize(TableData data)
        {
            _attackingCardRoot = data.AttackingRoot;
            _defendingCardRoot = data.DefendingRoot;

            _durakController = LazySingleton<DurakController>.Instance;
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _audioController = LazySingleton<AudioController>.Instance;
            _tableBufferController = LazySingleton<TableBufferController>.Instance;
        }

        /// <summary>
        /// Add card to table processs. Show animation of adding card to table.
        /// </summary>
        /// <param name="cardId"></param>
        public void AddCardToTable(int cardId)
        {
            var card = _dataHolderController.CardsDictionary[cardId];
            _tableBufferController.ToBuffer(card.transform);
            card.Lock();

            Sequence moveToDiscardPileAnim = DOTween.Sequence();
            var destinationRoot = _durakController.CurState == GameState.Attack ? _attackingCardRoot : _defendingCardRoot;
            var midPoint = card.transform.position + (destinationRoot.transform.position - card.transform.position) / 2;
            var endPoint = destinationRoot.transform.position;
            var animationTime = 0.3f;

            if (card.IsFaceUp())
            {
                moveToDiscardPileAnim.Append(card.transform.DOMove(endPoint, animationTime / 2));
            }
            else
            {
                moveToDiscardPileAnim.Append(card.transform.DORotate(new Vector3(0, 90, 0), animationTime / 2))
                                      .Join(card.transform.DOMove(midPoint, animationTime / 2))
                                      .Append(card.transform.DORotate(new Vector3(0, 0, 0), animationTime / 2).OnStart(card.FaceUp))
                                      .Join(card.transform.DOMove(endPoint, animationTime / 2));
            }
            moveToDiscardPileAnim.SetEase(Ease.Linear).SetUpdate(UpdateType.Normal, true)
                                 .OnComplete(() =>
                                 {
                                     if (_durakController.CurState == GameState.Attack)
                                     {
                                         AttackCard = card;
                                         AttackCard.transform.SetParent(_attackingCardRoot);
                                         CardAddedToTableEvent?.Invoke();
                                     }
                                     else if (_durakController.CurState == GameState.Defense)
                                     {
                                         DefendCard = card;
                                         DefendCard.transform.SetParent(_defendingCardRoot);
                                         CardAddedToTableEvent?.Invoke();
                                     }
                                 });

            _audioController.PlayShot(AudioSourceType.SFX, ClipType.Place);
        }

        /// <summary>
        /// Remove card from table.
        /// </summary>
        public void RemoveCardFromTable(int cardId)
        {
            var card = _dataHolderController.CardsDictionary[cardId];

            if (card.Equals(AttackCard))
                AttackCard = null;
            else if (card.Equals(DefendCard))
                DefendCard = null;
        }
    }
}