using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASVS_Security_Auditor.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AsvsRequirement> AsvsRequirements { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<AssessmentItem> AssessmentItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        
        // MySQL key length fix
        builder.Entity<IdentityUser>(entity => 
        {
            entity.Property(m => m.Id).HasMaxLength(75);
            entity.Property(m => m.UserName).HasMaxLength(128);
            entity.Property(m => m.NormalizedUserName).HasMaxLength(128);
            entity.Property(m => m.Email).HasMaxLength(128);
            entity.Property(m => m.NormalizedEmail).HasMaxLength(128);
        });
        builder.Entity<IdentityRole>(entity => 
        {
            entity.Property(m => m.Id).HasMaxLength(75);
            entity.Property(m => m.Name).HasMaxLength(128);
            entity.Property(m => m.NormalizedName).HasMaxLength(128);
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.Property(m => m.LoginProvider).HasMaxLength(75);
            entity.Property(m => m.ProviderKey).HasMaxLength(75);
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(75);
            entity.Property(m => m.RoleId).HasMaxLength(75);
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(75);
            entity.Property(m => m.LoginProvider).HasMaxLength(75);
            entity.Property(m => m.Name).HasMaxLength(75);
        });
    }
}


