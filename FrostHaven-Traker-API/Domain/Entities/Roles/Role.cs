using Domain.Enums;

namespace Domain.Entities.Roles;

public class Role
{
    public int Id { get; private set; }
    public RoleName RoleNumber {get; private set; }
    public string Description { get; private set; } = string.Empty;

    public Role(int id, RoleName roleNumber,string description)
    {
        Id = id;
        RoleNumber = roleNumber;
        Description = description;
    }

    // Para EF
    private Role() { }
}