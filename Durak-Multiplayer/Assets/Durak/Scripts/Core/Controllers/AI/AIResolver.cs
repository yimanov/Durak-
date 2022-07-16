using UnityEngine;

namespace Durak
{
    public static class AIResolver
    {
        /// <summary>
        /// Get AI logic based on difficulty.
        /// </summary>
        public static AIController ResolveAI(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Hard:
                    return new HardAIController();
                case Difficulty.Easy:
                default:
                    return new EasyAIController();
            }
        }
    }
}