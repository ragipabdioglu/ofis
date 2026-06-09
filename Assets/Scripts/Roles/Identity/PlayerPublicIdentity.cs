using OFIS.Core.Ids;
using OFIS.Roles.Departments;

namespace OFIS.Roles.Identity
{
    public sealed class PlayerPublicIdentity
    {
        public PlayerId PlayerId { get; }
        public string CharacterName { get; }
        public DepartmentType Department { get; }
        public string DepartmentDisplayName { get; }
        public string JobTitle { get; }

        public PlayerPublicIdentity(
            PlayerId playerId,
            string characterName,
            DepartmentType department,
            string departmentDisplayName,
            string jobTitle)
        {
            PlayerId = playerId;
            CharacterName = characterName;
            Department = department;
            DepartmentDisplayName = departmentDisplayName;
            JobTitle = jobTitle;
        }

        public override string ToString()
        {
            return $"{CharacterName} / {DepartmentDisplayName} / {JobTitle}";
        }
    }
}