using Domain.Enums;

namespace Application.DTOs.Requests.User;

public sealed record CreateUserCommand
(
    string Email,
    string Name,
    string Surname,
    string DisplayName,
    string Password,
    RoleName Role
);