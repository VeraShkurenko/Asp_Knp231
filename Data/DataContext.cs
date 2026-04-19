using Microsoft.EntityFrameworkCore;

namespace AspKnP231.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.UserAccess> UserAccesses { get; set; }

        public DbSet<Entities.UserData> UsersData { get; set; }

        public DbSet<Entities.UserRole> UserRoles { get; set; }


        public DataContext(DbContextOptions options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // налаштування моделі БД: а) відношення між сутностями

            modelBuilder.Entity<Entities.UserAccess>()
                .HasIndex(a => a.Login)
                .IsUnique();

            modelBuilder.Entity<Entities.UserAccess>()  // відношення може встановлюватись з автоматичним
                .HasOne(a => a.UserData)                // визначенням ключів поєднання якщо: один з них Id або TableId
                .WithMany(d => d.UserAccesses)          // інший - TableId (UserDataId), оскільки останнє порушується
                .HasForeignKey(a => a.UserId);          // необхідно прямо зазначити ключ
                // .HasPrincipalKey(d => d.Id)          // інший - за правилом, можна не зазначати

            modelBuilder.Entity<Entities.UserAccess>()  // Іменування усіх ключів - за правилом, можна не зазначати,
                .HasOne(a => a.UserRole)                // За відсутності інверсної властивості WithMany()
                .WithMany();                            // лишається порожнім
                                                        // .HasForeignKey(a => a.UserRoleId)
                                                        // .HasPrincipalKey(r => r.Id);

            // б) початкові дані
            modelBuilder.Entity<Entities.UserRole>().HasData(
                new Entities.UserRole
                {
                    Id = Guid.Parse("28578D03-1E83-4607-9FB0-631FED207DCF"),
                    Name = "Self Registered",
                    Description = "Користувачі, що самі зареєструвались на сайті. Мінімальні права доступу",
                    CreateLevel = 0,
                    ReadLevel = 0,
                    UpdateLevel = 0,
                    DeleteLevel = 0,
                },
                new Entities.UserRole
                {
                    Id = Guid.Parse("250FA2D3-0818-42D6-A1ED-112F115407D6"),
                    Name = "Root Administrator",
                    Description = "Користувач з максимальним доступом, через якого вводяться інші ролі та доступи",
                    CreateLevel = -1,
                    ReadLevel = -1,
                    UpdateLevel = -1,
                    DeleteLevel = -1,
                }
            );

            modelBuilder.Entity<Entities.UserData>().HasData(
                new Entities.UserData
                {
                    Id = Guid.Parse("AE3A3665-8F44-4A2C-95A4-8BBCCBA7C80D"),
                    Name = "Default Administrator",
                    Email = "admin@change.me",
                    Birthdate = new DateTime(2026, 3, 12),
                });

            modelBuilder.Entity<Entities.UserAccess>().HasData(
                new Entities.UserAccess
                {
                    Id = Guid.Parse("F749F994-AF12-4EA1-8BEF-829EF751FC4A"),
                    UserId = Guid.Parse("AE3A3665-8F44-4A2C-95A4-8BBCCBA7C80D"),  // !! з UserData
                    UserRoleId = Guid.Parse("250FA2D3-0818-42D6-A1ED-112F115407D6"),  // !! Role
                    Login = "DefaultAdministrator",
                    Salt = "380B5CB5-1578-49A2-BCAF-4A1CA8AA9BC2",
                    CreatedAt = new DateTime(2026, 3, 12),
                    Dk = "4E455301C8628F110EADAB21A780FF766CFE0B95"
                });
        }

    }
}
