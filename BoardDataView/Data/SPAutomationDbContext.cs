using Microsoft.EntityFrameworkCore;
using BoardDataView.Models;

namespace BoardDataView.Data
{
    public class SPAutomationDbContext : DbContext
    {
        public SPAutomationDbContext(DbContextOptions<SPAutomationDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseInstance> CourseInstances { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for CourseInstance
            modelBuilder.Entity<CourseInstance>()
                .HasOne(ci => ci.Course)
                .WithMany()
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseInstance>()
                .HasOne(ci => ci.Instructor)
                .WithMany()
                .HasForeignKey(ci => ci.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CourseInstance>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CourseInstances)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CourseInstance>()
                .HasOne(ci => ci.Language)
                .WithMany(l => l.CourseInstances)
                .HasForeignKey(ci => ci.LanguageID)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure default values
            modelBuilder.Entity<CourseInstance>()
                .Property(ci => ci.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
} 