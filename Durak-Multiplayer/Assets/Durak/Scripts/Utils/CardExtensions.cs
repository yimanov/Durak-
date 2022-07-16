namespace Durak
{
    public static class CardExtensions
    {
        public static CardComponent ToCard(this int cardId)
        {
            var dataHolderController = LazySingleton<DataHolderConroller>.Instance;

            return dataHolderController.CardsDictionary.ContainsKey(cardId) ? dataHolderController.CardsDictionary[cardId] : null;
        }

        /// <summary>
        /// Repalce card data in components.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="visualContainer"></param>
        public static void ReplaceCardData( CardComponent first, CardComponent second, CardsVisualContainer visualContainer)
        {
            var dataHolderController = LazySingleton<DataHolderConroller>.Instance;

            CardData firstData = first.Data;
            CardData secondData = second.Data;

            //UnityEngine.Debug.LogError($"REPLACE - First: {firstData.Suit} Priority: {firstData.Priority} \nSecond: {secondData.Suit} Priority: {secondData.Priority}");

            int firstCardId = first.CardId;
            int secondCardId = second.CardId;

            first.Data = secondData;
            second.Data = firstData;

            first.VisualData = visualContainer.GetVisualData(secondData.Suit, secondData.Priority);
            second.VisualData = visualContainer.GetVisualData(firstData.Suit, firstData.Priority);

            first.View.Initialize(first.VisualData);
            second.View.Initialize(second.VisualData);

            dataHolderController.CardsDictionary[firstCardId] = second;
            dataHolderController.CardsDictionary[secondCardId] = first;
        }
    }
}
