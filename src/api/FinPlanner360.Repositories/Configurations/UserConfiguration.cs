﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FinPlanner360.Entities.Users;

namespace FinPlanner360.Repositories.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Mapping columns
            builder.ToTable("TB_USER");

            builder.HasKey(x => x.UserId)
                .HasName("PK_TB_USER");

            builder.Property(x => x.UserId)
                .HasColumnName("USER_ID")
                .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("NAME")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.Email)
                .HasColumnName("EMAIL")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.AuthenticationId)
                .HasColumnName("AUTHENTICATION_ID")
                .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
                .IsRequired();
            #endregion

            #region Indexes
            #endregion

            #region Relationships
            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_TB_USER_01")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Categories)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_TB_USER_02")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Budgets)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_TB_USER_03")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.GeneralBudgets)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_TB_USER_04")
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
