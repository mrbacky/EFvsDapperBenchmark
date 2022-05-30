

using System;
using ConsoleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Persistence.EF.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        public ApplicationDbContext(DbContextOptions opt) : base(opt)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Constants.ConnectionStringEF).LogTo(Console.WriteLine);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");
                entity.Property(i => i.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(i => i.FirstName).HasColumnName("first_name");
                entity.Property(i => i.LastName).HasColumnName("last_name");
                entity.Property(i => i.BirthDate).HasColumnName("birth_date");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("course");
                entity.Property(i => i.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(i => i.Name).HasColumnName("name");
            });

            modelBuilder
                .Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students)
                .UsingEntity(b => b.ToTable("course_student"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
