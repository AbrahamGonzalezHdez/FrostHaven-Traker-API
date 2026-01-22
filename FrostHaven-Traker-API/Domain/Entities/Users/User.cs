using Domain.Entities.Roles;
using Domain.Enums;

namespace Domain.Entities.Users;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Active { get; private set; }
    
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
    
    public User(string email, string name, string surname, string displayName, string password,RoleName role, DateTime createdAt)
    {
        Email = email;
        Name = name;
        Surname = surname;
        DisplayName = displayName;
        Password = password;
        CreatedAt = createdAt;
        Active = true;
    }
    
    // Para EF
    private User()
    {
        
    }
    
    public void AddRole(Role role)
    {
        if (_userRoles.Any(x => x.RoleId == role.Id))
            return;

        _userRoles.Add(new UserRole(Id, role.Id));
    }
    
    public bool HasRole(RoleName roleName) =>
        _userRoles.Any(ur => ur.Role.Name == roleName);
    
    public void Deactivate() => Active = false;
}