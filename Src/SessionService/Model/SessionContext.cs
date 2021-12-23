using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SessionService.Model
{
    public partial class SessionContext : DbContext
    {
        public SessionContext()
        {
        }

        public SessionContext(DbContextOptions<SessionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SessionModel> Sessions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<SessionModel>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("session_id");

                entity.ToTable("session");

                entity.Property(e => e.MovieId)
                    .ValueGeneratedNever()
                    .HasColumnName("movie_id")
                    .IsRequired();

                entity.Property(e => e.EndingTime)
                    .HasColumnName("ending_time")
                    .IsRequired();

                entity.Property(e => e.RoomId)
                    .HasColumnName("room_id")
                    .IsRequired();

                entity.Property(e => e.ScreeningDate)
                    .HasColumnName("screening_date")
                    .IsRequired();

                entity.Property(e => e.SessionId)
                    .HasColumnName("session_id")
                    .HasDefaultValueSql("uuid_generate_v4()").IsRequired();

                entity.Property(e => e.StartingTime)
                    .HasColumnName("starting_time")
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}