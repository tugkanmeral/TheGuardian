using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        SeedData();
    }

    private void SeedData()
    {
        if (!Users.Any())
        {
            Users.Add(new User()
            {
                Id = Guid.Parse("96c86995-434a-42c3-a84b-37c1ebdb3461"),
                UserName = "tugkan.meral",
                PasswordHash = "8f69bcd3193467cfae9b0130ffa9a26dd384a19cd118e52f56a6b908db694ac1", // 123456
                CreatedDate = DateTimeOffset.Now
            });
            SaveChanges();
        }
    }
}