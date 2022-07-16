using System.Collections.Generic;

namespace Durak
{
    public class DataHolderConroller
    {
        public Dictionary<int, CardComponent> CardsDictionary => DataConfig.DeckCards;
        public List<int> CardsDictionaryOrder => DataConfig.CardsDictionaryOrder;

        public List<CardComponent> Cards => DataConfig.Cards;
        public List<PlayerController> Players => DataConfig.Players;

        public GameData DataConfig = new GameData();

        /// <summary>
        /// Hold links to card components 
        /// </summary>
        public void SetCards(List<CardComponent> cards)
        {
            DataConfig.Cards = new List<CardComponent>(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                var controller = cards[i];

                DataConfig.DeckCards.Add(controller.CardId, controller);
            }
        }
        
        /// <summary>
        /// Hold last session cards order. 
        /// </summary>
        public void SetCardsOrder(List<int> order)
        {
            DataConfig.CardsDictionaryOrder = new List<int>(order);
        }

        /// <summary>
        /// Hold players.
        /// </summary>
        public void SetPlayers(List<PlayerController> players)
        {
            DataConfig.Players = new List<PlayerController>(players);
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void Clear()
        {
            CardsDictionary.Clear();
            Cards.Clear();
            Players.Clear();
        }
    }
}