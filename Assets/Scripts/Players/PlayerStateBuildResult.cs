using System.Collections.Generic;

namespace OFIS.Players
{
    public sealed class PlayerStateBuildResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }

        public IReadOnlyList<PlayerPublicState> PublicStates { get; }
        public IReadOnlyList<PlayerPrivateState> PrivateStates { get; }

        private PlayerStateBuildResult(
            bool success,
            string errorMessage,
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates)
        {
            Success = success;
            ErrorMessage = errorMessage;
            PublicStates = publicStates;
            PrivateStates = privateStates;
        }

        public static PlayerStateBuildResult Failed(string errorMessage)
        {
            return new PlayerStateBuildResult(
                false,
                errorMessage,
                new List<PlayerPublicState>(),
                new List<PlayerPrivateState>());
        }

        public static PlayerStateBuildResult Completed(
            IReadOnlyList<PlayerPublicState> publicStates,
            IReadOnlyList<PlayerPrivateState> privateStates)
        {
            return new PlayerStateBuildResult(
                true,
                string.Empty,
                publicStates,
                privateStates);
        }
    }
}