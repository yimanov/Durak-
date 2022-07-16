using UnityEngine;

namespace Durak
{
    [System.Serializable]
    public class GameSettings
    {
        /// <summary>
        /// The value which means count of players in game.
        /// </summary>
        [Range(2, 6)]
        public int PlayersCount = 2;

        /// <summary>
        /// The value which means count of cards per each player in game.
        /// </summary>
        [Range(2, 6)]
        public int CardsPerHand = 6;

        /// <summary>
        /// Show back side of caards for AI players if TRUE
        /// Show face side of cards for AI players if FALSE
        /// </summary>
        public bool HideAiCards;

        /// <summary>
        /// How strong AI
        /// </summary>
        public Difficulty GameDifficulty;

        /// <summary>
        /// 
        /// </summary>
        public DurakType GameType;
        
        /// <summary>
        /// 
        /// </summary>
        public DurakTransferType GameTransferType;
    }
}