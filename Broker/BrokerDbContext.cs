using Microsoft.EntityFrameworkCore;

namespace Broker;

public class BrokerDbContext : DbContext
{
    public BrokerDbContext() : base(new DbContextOptions<BrokerDbContext>()) { }

    public DbSet<Message> Messages { get; set; }
    public DbSet<Subscriber> Subscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>();
        modelBuilder.Entity<Subscriber>();
        // Configure additional properties and relationships if necessary
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost; Database=broker; Username=your_user; Password=your_password");
        }
    }
}