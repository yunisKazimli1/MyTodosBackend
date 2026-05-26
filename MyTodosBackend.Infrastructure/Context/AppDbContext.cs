using Microsoft.EntityFrameworkCore;
using MyTodosBackend.Domain.Entities;

namespace MyTodosBackend.Infrastructure.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(x => x.Title)
                      .IsRequired()
                      .HasMaxLength(200);
            });
        }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
