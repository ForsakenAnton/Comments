using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .ToTable(t => t.HasCheckConstraint(
                name: "CK_User_UserName_OnlyLatinLettersAndDigits",
                sql: "UserName NOT LIKE '%[^a-zA-Z0-9 ]%'"));

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasIndex(u => u.Email)
            .IsUnique();

        builder
            .Property(u => u.HomePage)
            .HasMaxLength(300);
    }
}
