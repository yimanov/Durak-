using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "GameContent/Deck", order = 1)]
public class DeckData : ScriptableObject
{
    public List<CardData> Cards = new List<CardData>();
}
