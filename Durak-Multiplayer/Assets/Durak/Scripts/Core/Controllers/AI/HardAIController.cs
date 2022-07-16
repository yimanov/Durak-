using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Durak
{
    public class HardAIController : AIController
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
                result = GetCardForAttack(input);
            }
            else if (_durakController.CurState == GameState.Defense)
            {
                result = GetCardForDefense(input);
            }

            return result;
        }

        private CardComponent GetCardForAttack(MoveData input)
        {
            CardComponent result = null;

            List<CardComponent> filteredCards = new List<CardComponent>();

            if (_durakController.GameType == DurakType.ThrowIn)
            {
                //if defender is AI
                if (input.Defender.IsAi)
                {
                    if (_sessionManager.HasCardsOnTable())
                    {
                        var isShouldMove = Utils.GetRandomBoolean();

                        if (!isShouldMove)
                        {

                            return result;
                        }

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
                else
                {
                    //if has any cards on table
                    if (_sessionManager.HasCardsOnTable())
                    {
                        //if card exist in hand
                        filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit && _sessionManager.CardsOnTable.Contains(c.Data.Priority))
                                                                .OrderBy(c => c.Data.Priority).ToList();

                        if (filteredCards != null && filteredCards.Count > 0)
                            result = filteredCards[0];
                        else
                        {
                            //get card from deck if exist
                            List<CardSuit> defenderSuits = input.Defender.CardsInHand.Select(x => x.Data.Suit).Distinct().ToList();
                            List<CardComponent> cards = new List<CardComponent>(input.Attacker.CardsInHand);
                            var deckCards = GetCardsForReplaceFromDeck();
                            cards.AddRange(deckCards);
                            filteredCards = cards.Where(c => _sessionManager.CardsOnTable.Contains(c.Data.Priority) && c.Data.Suit != _deckController.TrumpSuit).
                                                                        OrderBy(c => c.Data.Priority).
                                                                        OrderBy(c => !input.Attacker.CardsInHand.Contains(c)).ToList();

                            if (filteredCards != null && filteredCards.Count > 0)
                                result = filteredCards[0];

                            TryToReplaceCard(ref result, input.Attacker);
                        }
                    }
                    else
                    {
                        //get card that has no in user hand
                        List<CardSuit> defenderSuits = input.Defender.CardsInHand.Select(x => x.Data.Suit).Distinct().ToList();
                        List<CardComponent> cards = new List<CardComponent>(input.Attacker.CardsInHand);
                        var deckCards = GetCardsForReplaceFromDeck();
                        cards.AddRange(deckCards);
                        filteredCards = cards.Where(c => c.Data.Suit != _deckController.TrumpSuit && !defenderSuits.Contains(c.Data.Suit)).
                                                                    OrderBy(c => c.Data.Priority).
                                                                    OrderBy(c => !input.Attacker.CardsInHand.Contains(c)).ToList();

                        if (filteredCards != null && filteredCards.Count > 0)
                            result = filteredCards[0];
                        else
                        {
                            filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit).
                                                                         OrderBy(c => c.Data.Priority).ToList();
                            if (filteredCards != null && filteredCards.Count > 0)
                                result = filteredCards[0];
                            else
                            {
                                filteredCards = input.Attacker.CardsInHand.OrderBy(c => c.Data.Priority).ToList();
                                result = filteredCards[0];
                            }
                        }

                        TryToReplaceCard(ref result, input.Attacker);
                    }
                }
            }
            else
            {
                //if defender is AI
                if (input.Defender.IsAi)
                {
                    filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit).
                                                                                  OrderBy(c => c.Data.Priority).ToList();
                    if (filteredCards != null && filteredCards.Count > 0)
                        result = filteredCards[0];
                    else
                    {
                        filteredCards = input.Attacker.CardsInHand.OrderBy(c => c.Data.Priority).ToList();
                        result = filteredCards[0];
                    }
                }
                else
                {
                    //get card that has no in user hand
                    List<CardSuit> defenderSuits = input.Defender.CardsInHand.Select(x => x.Data.Suit).Distinct().ToList();
                    List<CardComponent> cards = new List<CardComponent>(input.Attacker.CardsInHand);
                    var deckCards = GetCardsForReplaceFromDeck();
                    cards.AddRange(deckCards);
                    filteredCards = cards.Where(c => c.Data.Suit != _deckController.TrumpSuit && !defenderSuits.Contains(c.Data.Suit)).
                                                                OrderBy(c => c.Data.Priority).
                                                                OrderBy(c => !input.Attacker.CardsInHand.Contains(c)).ToList();

                    if (filteredCards != null && filteredCards.Count > 0)
                        result = filteredCards[0];
                    else
                    {
                        filteredCards = input.Attacker.CardsInHand.Where(c => c.Data.Suit != _deckController.TrumpSuit).
                                                                     OrderBy(c => c.Data.Priority).ToList();
                        if (filteredCards != null && filteredCards.Count > 0)
                            result = filteredCards[0];
                        else
                        {
                            filteredCards = input.Attacker.CardsInHand.OrderBy(c => c.Data.Priority).ToList();
                            result = filteredCards[0];
                        }
                    }

                    TryToReplaceCard(ref result, input.Attacker);
                }
            }

            return result;
        }

        private CardComponent GetCardForDefense(MoveData input)
        {
            CardComponent result = null;

            if (input.Attacker.IsAi)
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
            else
            {
                if (input.AtkCard.Data.Suit == _deckController.TrumpSuit)
                {
                    List<CardComponent> trumps = GetCardsBySuitInOrder(input.Defender, _deckController.TrumpSuit, input.AtkCard.Data.Priority);

                    if (trumps != null && trumps.Count > 0)
                        result = trumps[0];
                    else
                    {
                        List<CardComponent> cards = new List<CardComponent>();
                        var deckCards = GetCardsForReplaceFromDeck();
                        cards.AddRange(deckCards);
                        trumps = cards.Where(c => c.Data.Suit == _deckController.TrumpSuit && c.Data.Priority > input.AtkCard.Data.Priority).
                                                                    OrderBy(c => c.Data.Priority).ToList();

                        if (trumps != null && trumps.Count > 0)
                        {
                            result = trumps[0];

                            TryToReplaceCard(ref result, input.Defender);
                        }
                    }
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
                        List<CardComponent> trumps = GetCardsBySuitInOrder(input.Defender, _deckController.TrumpSuit, input.AtkCard.Data.Priority);

                        if (trumps != null && trumps.Count > 0)
                            result = trumps[0];

                        else
                        {
                            List<CardComponent> cards = new List<CardComponent>();
                            var deckCards = GetCardsForReplaceFromDeck();
                            cards.AddRange(deckCards);
                            sameSuit = cards.Where(c => c.Data.Suit == input.AtkCard.Data.Suit && c.Data.Priority > input.AtkCard.Data.Priority).
                                                                        OrderBy(c => c.Data.Priority).ToList();
                            if (sameSuit != null && sameSuit.Count > 0)
                            {
                                result = sameSuit[0];

                                TryToReplaceCard(ref result, input.Defender);
                            }
                            else
                            {
                                trumps = GetCardsBySuitInOrder(input.Defender, _deckController.TrumpSuit);
                                if (trumps != null && trumps.Count > 0)
                                    result = trumps[0];
                                else
                                {
                                    List<CardComponent> trumpsCards = new List<CardComponent>();
                                    deckCards = GetCardsForReplaceFromDeck();
                                    trumpsCards.AddRange(deckCards);
                                    trumps = trumpsCards.Where(c => c.Data.Suit == _deckController.TrumpSuit).
                                                                                OrderBy(c => c.Data.Priority).ToList();
                                    if (trumps != null && trumps.Count > 0)
                                    {
                                        result = trumpsCards[0];

                                        TryToReplaceCard(ref result, input.Defender);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Cheat functionality for replace one card with other.
        /// </summary>
        /// <param name="resultedCard"></param>
        /// <param name="cardHolder"></param>
        private void TryToReplaceCard(ref CardComponent resultedCard, PlayerController cardHolder)
        {
            if (resultedCard != null && !cardHolder.CardsInHand.Contains(resultedCard))
            {
                CardExtensions.ReplaceCardData(resultedCard, cardHolder.CardsInHand[0], _deckController.GetVisual());

                if (!cardHolder.HideCards)
                {
                    resultedCard.UpdateView();
                    cardHolder.CardsInHand[0].UpdateView();
                }

                resultedCard = cardHolder.CardsInHand[0];
            }
        }

        /// <summary>
        /// Get available cards from deck.
        /// </summary>
        private List<CardComponent> GetCardsForReplaceFromDeck()
        {
            List<CardComponent> deckCards = new List<CardComponent>(_dataHolderController.Cards);

            if (deckCards.Count > 0)
            {
                deckCards.RemoveAt(0);
            }

            return deckCards;
        }
    }
}