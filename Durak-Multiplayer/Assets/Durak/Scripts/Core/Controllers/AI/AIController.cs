using System.Collections.Generic;

namespace Durak
{
    public abstract class AIController
    {
        protected DurakController _durakController;
        protected DeckController _deckController;
        protected DataHolderConroller _dataHolderController;
        protected SessionManager _sessionManager;

        public void Initialize()
        {
            _durakController = LazySingleton<DurakController>.Instance;
            _deckController = LazySingleton<DeckController>.Instance;
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
            _sessionManager = LazySingleton<SessionManager>.Instance;
        }

        /// <summary>
        /// Sort Cards by specific parameters of order.
        /// </summary>
        /// <param name="player">Cards owner</param>
        /// <param name="suit">Priority suit that will be compared.</param>
        /// <param name="minPriority">Min card priority</param>
        /// <returns></returns>
        public abstract List<CardComponent> GetCardsBySuitInOrder(PlayerController player, CardSuit suit, int minPriority = 6);

        public abstract CardComponent MakeMove(MoveData input);
    }

    public class MoveData
    {
        public PlayerController Attacker;
        public PlayerController Defender;
        public CardComponent AtkCard;
    }
}