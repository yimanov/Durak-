using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsVisualContainer", menuName = "Scriptable/VisualContainer")]
public class CardsVisualContainer : ScriptableObject
{
#pragma warning disable 649
    [SerializeField]
    private Sprite _backIcon;
#pragma warning disable 649

    [SerializeField]
    private List<SuitInfo> _suitsIcons = new List<SuitInfo>();

    [SerializeField]
    private List<CardSuitInfo> _clubs = new List<CardSuitInfo>();
    [SerializeField]
    private List<CardSuitInfo> _diamonds = new List<CardSuitInfo>();
    [SerializeField]
    private List<CardSuitInfo> _hearts = new List<CardSuitInfo>();
    [SerializeField]
    private List<CardSuitInfo> _spades = new List<CardSuitInfo>();

    public VisualData GetVisualData(CardSuit suit, int priority)
    {
        var list = GetListBySuit(suit);

        VisualData data = new VisualData
        {
            Back = _backIcon,
            Face = list != null && list.Count > 0 ? list.FirstOrDefault(x => x.Card.Priority == priority)?.Icon : null
        };
        if (data.Face == null)
        {
            Debug.LogError($"suit {suit} priority {priority} ");
        }
        return data;
    }

    public Sprite GetSuitIcon(CardSuit suit)
    {
        return _suitsIcons.First(x => x.Suit == suit)?.Icon;
    }

    private List<CardSuitInfo> GetListBySuit(CardSuit suit)
    {
        switch (suit)
        {
            case CardSuit.Clubs:
                return _clubs;
            case CardSuit.Diamonds:
                return _diamonds;
            case CardSuit.Hearts:
                return _hearts;
            case CardSuit.Spades:
                return _spades;
            default:
                return null;
        }
    }
}

public class VisualData
{
    public Sprite Face;
    public Sprite Back;
}

[System.Serializable]
public class SuitInfo
{
    public CardSuit Suit;
    public Sprite Icon;
}

[System.Serializable]
public class CardSuitInfo
{
    public CardData Card;
    public int Priority;
    public Sprite Icon;
}
