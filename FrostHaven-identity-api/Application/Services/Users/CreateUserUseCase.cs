using Application.DTOs.Requests.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Domain.Enums;

namespace Application.Services.Users;

public class CreateUserUseCase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserUseCase(IUserRepository  users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<CreateUserResult> ExecuteAsync(CreateUserCommand userCommand, CancellationToken ct = default, RoleName role = RoleName.Player)
    {
        // Implementar fluent validation
        if (string.IsNullOrWhiteSpace(userCommand.Email)) throw new ArgumentException("Email requerido.");
        if (string.IsNullOrWhiteSpace(userCommand.Password)) throw new ArgumentException("Password requerido.");
        if (string.IsNullOrWhiteSpace(userCommand.DisplayName)) throw new ArgumentException("DisplayName requerido.");

        var passwordHash = _passwordHasher.Hash(userCommand.Password);

        var created = await _users.CreateAsync(
            email: userCommand.Email,
            name: userCommand.Name,
            surname: userCommand.Surname,
            displayName: userCommand.DisplayName,
            passwordHash: passwordHash,
            role: role,
            ct: ct);

        // Created debe venir recargado con roles (Include) según lo que acordamos
        var roles = created.UserRoles
            .Select(ur => ur.Role.RoleNumber)
            .Distinct()
            .ToList();

        return new CreateUserResult(
            Id: created.Id,
            Email: created.Email,
            DisplayName: created.DisplayName,
            Active: created.Active,
            Roles: roles);
    }
    
}