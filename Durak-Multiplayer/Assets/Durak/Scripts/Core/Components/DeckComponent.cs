using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class DeckComponent : PreInitedMonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private DeckData _data;
        [SerializeField]
        private CardsVisualContainer _visualContainer;

        [Space]
        [SerializeField]
        private Transform _trumpRoot;
        [SerializeField]
        private Transform _stackContainer;
        [SerializeField]
        private Transform _discardPileContainer;

        [Space]
        [SerializeField]
        private GameObject _cardPrefab;

        [Header("UI refs:")]
        [SerializeField]
        private Image _trumpIcon;
#pragma warning restore 649

        private DeckController _deckController;

        protected override void PreInitialize()
        {
            _deckController = LazySingleton<DeckController>.Instance;
        }

        /// <summary>
        /// Generate deck cards at the game start.
        /// </summary>
        public void Generate()
        {
            var cards = new List<CardComponent>();

            foreach (CardData data in _data.Cards)
            {
                GameObject cardObj = Instantiate(_cardPrefab, _stackContainer);
                CardComponent card = cardObj.GetComponent<CardComponent>();

                card.Data = data;
                card.VisualData = _visualContainer.GetVisualData(data.Suit, data.Priority);
                card.InitializeComponents();
                card.FaceDown();
                card.Lock();

                cards.Add(card);
            }

            DeckControllData deckData = new DeckControllData()
            {
                Cards = cards,
                DiscardPileContainer = _discardPileContainer,
                TrumpContainer = _trumpRoot,
                VisualContainer = _visualContainer,
                TrumpIcon = _trumpIcon,
            };
            _deckController.Initialize(deckData);
        }

        /// <summary>
        /// Remove gameObjects from containers.
        /// </summary>
        public void Clear()
        {
            foreach (Transform child in _stackContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _discardPileContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _trumpRoot)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void Dispose()
        {
        }
    }
}