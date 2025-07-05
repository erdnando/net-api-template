using Microsoft.EntityFrameworkCore;
using netapi_template.Models;

namespace netapi_template.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // PasswordResetToken Configuration
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.UsedAt).HasColumnType("datetime");
            
            // Relationships
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            
            // Enum configuration
            entity.Property(e => e.Status)
                .HasConversion<string>();
            
            // Soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // Relationships
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Role Configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            
            // Soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        
        // Module Configuration
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            
            // Soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        
        // UserPermission Configuration
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ModuleId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            
            // Enum configuration
            entity.Property(e => e.PermissionType)
                .HasConversion<string>();
            
            // Relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Module)
                .WithMany(m => m.UserPermissions)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TaskItem Configuration (updated for new User structure)
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });
        
        // Catalog Configuration (existing)
        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use a fixed date for all seed data to avoid migration issues with dynamic dates
        var now = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Administrador", Description = "Administrator role with full access", IsSystemRole = true, CreatedAt = now, IsDeleted = false },
            new Role { Id = 2, Name = "Sin asignar", Description = "Unassigned role", IsSystemRole = true, CreatedAt = now, IsDeleted = false },
            new Role { Id = 3, Name = "Analista", Description = "Analyst role", IsSystemRole = false, CreatedAt = now, IsDeleted = false },
            new Role { Id = 4, Name = "Reportes", Description = "Reports role", IsSystemRole = false, CreatedAt = now, IsDeleted = false },
            new Role { Id = 5, Name = "Soporte", Description = "Support role", IsSystemRole = false, CreatedAt = now, IsDeleted = false }
//          new Role { Id = 6, Name = "Invitado", Description = "Guest role", IsSystemRole = false, CreatedAt = now, IsDeleted = false }
        );
        
        // Seed Modules
        modelBuilder.Entity<Module>().HasData(
            new Module { Id = 1, Name = "Home", Path = "/", Icon = "HomeIcon", Order = 1, Description = "Home dashboard", Code = "HOME", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 2, Name = "Tasks", Path = "/tasks", Icon = "AssignmentIcon", Order = 2, Description = "Task management", Code = "TASKS", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 3, Name = "Users", Path = "/users", Icon = "PeopleIcon", Order = 3, Description = "User management", Code = "USERS", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 4, Name = "Roles", Path = "/roles", Icon = "SecurityIcon", Order = 4, Description = "Role management", Code = "ROLES", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 5, Name = "Catalogs", Path = "/catalogs", Icon = "CategoryIcon", Order = 5, Description = "Catalog management", Code = "CATALOGS", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 6, Name = "Permisos", Path = "/permissions", Icon = "AssignmentIcon", Order = 6, Description = "Permission management", Code = "PERMISSIONS", IsActive = true, CreatedAt = now, IsDeleted = false },
            new Module { Id = 7, Name = "Admin Utilities", Path = "/admin/utils", Icon = "SecurityIcon", Order = 7, Description = "Admin utilities", Code = "ADMIN_UTILS", IsActive = true, CreatedAt = now, IsDeleted = false }
        );
        
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "sistema",
                Email = "admin@sistema.com",
                RoleId = 1, // Admin role
                Status = UserStatus.Active,
                CreatedAt = now,
                IsDeleted = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            },
            new User
            {
                Id = 2,
                FirstName = "Erdnando",
                LastName = "User",
                Email = "erdnando@gmail.com",
                RoleId = 3, // Analista role
                Status = UserStatus.Active,
                CreatedAt = now,
                IsDeleted = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123")
            }
        );
        
        // Seed User Permissions
        modelBuilder.Entity<UserPermission>().HasData(
            // Standard permissions for Erdnando
            new UserPermission { Id = 6, UserId = 2, ModuleId = 1, PermissionType = PermissionType.Read, CreatedAt = now },
            new UserPermission { Id = 7, UserId = 2, ModuleId = 2, PermissionType = PermissionType.Edit, CreatedAt = now },
            new UserPermission { Id = 8, UserId = 2, ModuleId = 3, PermissionType = PermissionType.Read, CreatedAt = now },
            new UserPermission { Id = 9, UserId = 2, ModuleId = 4, PermissionType = PermissionType.Write, CreatedAt = now },
            new UserPermission { Id = 10, UserId = 2, ModuleId = 5, PermissionType = PermissionType.None, CreatedAt = now }
        );
        
        // Seed Catalogs (existing)
        modelBuilder.Entity<Catalog>().HasData(
            new Catalog
            {
                Id = 1,
                Title = "Premium Headphones",
                Description = "High-quality wireless headphones with noise cancellation",
                Category = "Electronics",
                Rating = 4.5,
                Price = 299.99m,
                InStock = true,
                CreatedAt = now
            },
            new Catalog
            {
                Id = 2,
                Title = "Gaming Laptop",
                Description = "High-performance laptop for gaming and professional work",
                Category = "Electronics",
                Rating = 4.8,
                Price = 1299.99m,
                InStock = true,
                CreatedAt = now
            }
        );
    }
}
