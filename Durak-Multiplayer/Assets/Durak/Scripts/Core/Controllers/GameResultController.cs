using System;
using System.Linq;

namespace Durak
{
    public class ComparableResultInfo
    {
        public int CardsCount;
    }

    public class ComparedResultInfo
    {
        public int LosePlayerId;
        public GameResult Result;
    }

    public enum GameResult
    {
        None = 0,
        Lose,
        Win,
        Drawn
    }

    public class GameResultController
    {
        private DataHolderConroller _dataHolderController;

        public event Action<ComparedResultInfo> ResultEvent;

        public void Initialize()
        {
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
        }

        /// <summary>
        /// Calculate result of game.
        /// </summary>
        /// <param name="resultInfo">Input data.</param>
        public void CheckResult(ComparableResultInfo resultInfo)
        {
            ComparedResultInfo comapedInfo = new ComparedResultInfo();

            if (resultInfo.CardsCount <= 0)
            {
                var playersWithoutCards = _dataHolderController.Players.Count(x => x.CardsInHand.Count == 0);

                if (playersWithoutCards == _dataHolderController.Players.Count)
                {
                    comapedInfo.LosePlayerId = -1;
                    comapedInfo.Result = GameResult.Drawn;
                }
                else if (playersWithoutCards == _dataHolderController.Players.Count - 1)
                {
                    var loser = _dataHolderController.Players.First(x => x.CardsInHand.Count > 0);

                    comapedInfo.LosePlayerId = loser.Id;
                    comapedInfo.Result = loser.IsAi ? GameResult.Win : GameResult.Lose;
                }
                else
                {
                    comapedInfo.LosePlayerId = -1;
                    comapedInfo.Result = GameResult.None;
                }
            }

            ResultEvent?.Invoke(comapedInfo);
        }
    }
}