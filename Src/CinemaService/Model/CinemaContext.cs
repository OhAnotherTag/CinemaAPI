using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CinemaService.Model
{
    public partial class CinemaContext : DbContext
    {
        public CinemaContext()
        {
        }

        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CinemaModel> Cinemas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<CinemaModel>(entity =>
            {
                entity.ToTable("cinema");
                entity.HasKey(c => c.CinemaId);

                entity.Property(e => e.CinemaId)
                    .HasColumnName("cinema_id")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();

                entity.Property(e => e.Location)
                    .HasColumnType("character varying")
                    .HasColumnName("location")
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType("character varying")
                    .HasColumnName("name")
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}