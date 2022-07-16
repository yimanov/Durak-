using System;
using System.Collections.Generic;

namespace Durak
{
    [Serializable]
    public class GameData
    {
        public Dictionary<int, CardComponent> DeckCards = new Dictionary<int, CardComponent>();
        public List<int> CardsDictionaryOrder = new List<int>();

        public List<CardComponent> Cards = new List<CardComponent>();
        public List<PlayerController> Players = new List<PlayerController>();
    }
}