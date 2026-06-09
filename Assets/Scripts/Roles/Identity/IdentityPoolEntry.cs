using OFIS.Roles.Departments;

namespace OFIS.Roles.Identity
{
    public sealed class IdentityPoolEntry
    {
        public string CharacterName { get; }
        public DepartmentType Department { get; }
        public string DepartmentDisplayName { get; }
        public string JobTitle { get; }

        public IdentityPoolEntry(
            string characterName,
            DepartmentType department,
            string departmentDisplayName,
            string jobTitle)
        {
            CharacterName = characterName;
            Department = department;
            DepartmentDisplayName = departmentDisplayName;
            JobTitle = jobTitle;
        }
    }
}