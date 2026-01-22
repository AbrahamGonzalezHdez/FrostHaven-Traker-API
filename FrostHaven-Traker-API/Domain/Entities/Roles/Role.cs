using Domain.Enums;

namespace Domain.Entities.Roles;

public class Role
{
    public int Id { get; private set; }
    public RoleName Name { get; private set; }

    public Role(int id, RoleName name)
    {
        Id = id;
        Name = name;
    }

    // Para EF
    private Role() { }
}