using System.Collections.Generic;
using System.Linq;

namespace Durak
{
    public class EasyAIController : AIController
    {
        public override List<CardComponent> GetCardsBySuitInOrder(PlayerController player, CardSuit suit, int minPriority = 6)
        {
            return player.CardsInHand
                .Where(c => c.Data.Suit == suit && c.Data.Priority >= minPriority)
                .OrderBy(c => c.Data.Priority).ToList();
        }

        public override CardComponent MakeMove(MoveData input)
        {
            CardComponent result = null;

            if (_durakController.CurState == GameState.Attack)
            {
                List<CardComponent> filteredCards = new List<CardComponent>();
                if (_sessionManager.HasCardsOnTable() && _durakController.GameType == DurakType.ThrowIn)
                {
                    filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit && _sessionManager.CardsOnTable.Contains(c.Data.Priority))
                                                            .OrderBy(c => c.Data.Priority).ToList();

                    if (filteredCards != null && filteredCards.Count > 0)
                        result = filteredCards[0];
                }
                else
                {
                    filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit)
                                                            .OrderBy(c => c.Data.Priority).ToList();

                    if (filteredCards != null && filteredCards.Count > 0)
                        result = filteredCards[0];
                    else
                    {
                        filteredCards = input.Attacker.CardsInHand.OrderBy(c => c.Data.Priority).ToList();
                        result = filteredCards[0];
                    }
                }
            }
            else if (_durakController.CurState == GameState.Defense)
            {
                if (input.AtkCard.Data.Suit == _deckController.TrumpSuit)
                {
                    List<CardComponent> trumps = GetCardsBySuitInOrder(input.Defender, _deckController.TrumpSuit, input.AtkCard.Data.Priority);

                    if (trumps != null && trumps.Count > 0)
                        result = trumps[0];
                }
                else
                {
                    List<CardComponent> sameSuit = GetCardsBySuitInOrder(input.Defender, input.AtkCard.Data.Suit, input.AtkCard.Data.Priority);

                    if (sameSuit != null && sameSuit.Count > 0)
                    {
                        result = sameSuit[0];
                    }
                    else
                    {
                        List<CardComponent> trumps = GetCardsBySuitInOrder(input.Defender, _deckController.TrumpSuit);
                        if (trumps != null && trumps.Count > 0)
                            result = trumps[0];
                    }
                }
            }

            return result;
        }
    }
}