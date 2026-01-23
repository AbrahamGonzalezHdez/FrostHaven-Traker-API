using Domain.Enums;

namespace Application.DTOs.Requests.User;

public sealed class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RoleName Role { get; set; } = RoleName.Player;
}