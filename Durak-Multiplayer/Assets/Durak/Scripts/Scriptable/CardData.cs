using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "GameContent/Card", order = 2)]
public class CardData : ScriptableObject
{
    public int Priority;
    public CardSuit Suit;
}

public enum CardSuit
{
    Clubs = 0,
    Diamonds = 14,
    Hearts = 28,
    Spades = 42
}
