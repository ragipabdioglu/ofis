using System.Collections.Generic;

namespace OFIS.Roles
{
    public sealed class RoleAssignmentResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }

        public RoleDistribution Distribution { get; }
        public IReadOnlyList<PlayerRoleAssignment> Assignments { get; }

        private RoleAssignmentResult(
            bool success,
            string errorMessage,
            RoleDistribution distribution,
            IReadOnlyList<PlayerRoleAssignment> assignments)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Distribution = distribution;
            Assignments = assignments;
        }

        public static RoleAssignmentResult Failed(string errorMessage)
        {
            return new RoleAssignmentResult(
                false,
                errorMessage,
                default,
                new List<PlayerRoleAssignment>());
        }

        public static RoleAssignmentResult Completed(
            RoleDistribution distribution,
            IReadOnlyList<PlayerRoleAssignment> assignments)
        {
            return new RoleAssignmentResult(
                true,
                string.Empty,
                distribution,
                assignments);
        }
    }
}