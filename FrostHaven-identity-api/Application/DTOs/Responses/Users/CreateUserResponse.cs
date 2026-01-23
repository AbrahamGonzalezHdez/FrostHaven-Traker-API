using Domain.Enums;

namespace Application.DTOs.Responses.Users;

public sealed class CreateUserResponse
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool Active { get; init; }
    public IReadOnlyList<RoleName> Roles { get; init; } = Array.Empty<RoleName>();
}