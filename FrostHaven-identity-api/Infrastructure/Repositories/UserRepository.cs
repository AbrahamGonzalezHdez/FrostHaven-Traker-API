using Application.Interfaces.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SecurityPostgresDbContext _dbSecurity;
    
    public UserRepository(SecurityPostgresDbContext dbSecurity)
    {
        _dbSecurity = dbSecurity;
    }
    
    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _dbSecurity.Users.AnyAsync(u => u.Email.ToLower() == normalized, ct);
    }

    public async Task<User> CreateAsync(string email, string name, string surname, string displayName, string passwordHash, RoleName role, CancellationToken ct = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        
        if (await _dbSecurity.Users.AnyAsync(u => u.Email == normalizedEmail, ct))
            throw new InvalidOperationException("Ya existe un usuario con ese email.");

        //-------Buscar rol (en tu diseño, Role.Id = (int)RoleName)
        var roleEntity = await _dbSecurity.Roles
            .FirstOrDefaultAsync(r => r.RoleNumber == role, ct);

        if (roleEntity is null)
            throw new InvalidOperationException($"No existe el rol '{role}' en la base de datos.");

        //-------Crear usuario
        var user = new User(
            email: normalizedEmail,
            name: name.Trim(),
            surname: surname.Trim(),
            displayName: displayName.Trim(),
            password: passwordHash,
            createdAt: DateTime.UtcNow);

        //-------Guardar usuario para obtener Id
        _dbSecurity.Users.Add(user);
        await _dbSecurity.SaveChangesAsync(ct);

        //-------Crear relación user_roles (ahora sí existe user.Id)
        _dbSecurity.UserRoles.Add(new UserRole(user.Id, roleEntity.Id));
        await _dbSecurity.SaveChangesAsync(ct);

        var createdUser = await _dbSecurity.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == user.Id, ct);
        return createdUser;
    }
    
    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}