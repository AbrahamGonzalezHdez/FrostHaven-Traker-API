using Domain.Entities.Users;

namespace Domain.Entities.Roles;

public class UserRole
{
    public int UserId { get; private set; }

    public int RoleId { get; private set; }
    public Role Role { get; private set; } = default!;

    public UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    // Para EF
    private UserRole() { }
}