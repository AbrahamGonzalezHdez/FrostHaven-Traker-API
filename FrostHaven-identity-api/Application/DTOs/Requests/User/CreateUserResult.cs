using Domain.Enums;

namespace Application.DTOs.Requests.User;

public sealed record CreateUserResult
(
    int Id,
    string Email,
    string DisplayName,
    bool Active,
    IReadOnlyList<RoleName> Roles
);