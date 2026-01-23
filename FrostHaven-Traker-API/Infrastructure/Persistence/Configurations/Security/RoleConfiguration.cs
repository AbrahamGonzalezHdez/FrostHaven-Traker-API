using Domain.Entities.Roles;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Security;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles","security");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // porque tú los fijas: Admin=1, Player=2

        builder.Property(x => x.RoleNumber)
            .HasColumnName("role_number")
            .HasConversion<int>() // RoleName enum <-> int
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasColumnName("role_description")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.RoleNumber).IsUnique();

        // Seed recomendado para no depender de inserts manuales
        builder.HasData(
            new Role(1, RoleName.Admin,"Admin"),
            new Role(2, RoleName.Player,"Player")
        );
    }
}