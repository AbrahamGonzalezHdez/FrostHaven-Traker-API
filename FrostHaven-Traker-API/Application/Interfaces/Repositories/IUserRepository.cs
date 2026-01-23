using Domain.Entities.Users;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<User> CreateAsync(string email, string name, string surname, string displayName, string passwordHash, RoleName role, CancellationToken ct = default);
}