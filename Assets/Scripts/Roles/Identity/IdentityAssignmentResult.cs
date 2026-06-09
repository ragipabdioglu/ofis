using System.Collections.Generic;

namespace OFIS.Roles.Identity
{
    public sealed class IdentityAssignmentResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        public IReadOnlyList<PlayerPublicIdentity> Identities { get; }

        private IdentityAssignmentResult(
            bool success,
            string errorMessage,
            IReadOnlyList<PlayerPublicIdentity> identities)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Identities = identities;
        }

        public static IdentityAssignmentResult Failed(string errorMessage)
        {
            return new IdentityAssignmentResult(
                false,
                errorMessage,
                new List<PlayerPublicIdentity>());
        }

        public static IdentityAssignmentResult Completed(IReadOnlyList<PlayerPublicIdentity> identities)
        {
            return new IdentityAssignmentResult(
                true,
                string.Empty,
                identities);
        }
    }
}