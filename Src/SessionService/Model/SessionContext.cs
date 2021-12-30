using System;
using System.Collections.Generic;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace SessionService.Model
{
    public partial class SessionContext : DbContext
    {
        public SessionContext()
        {
        }

        static SessionContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<Format>();
        
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
            modelBuilder.HasPostgresEnum<Format>().HasPostgresExtension("uuid-ossp");

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
                
                entity.Property(e => e.CinemaId)
                    .HasColumnName("cinema_id")
                    .IsRequired();
                
                entity.Property(e => e.MovieFormat)
                    .HasColumnName("movie_format")
                    .IsRequired();
                
                entity.Property(e => e.MovieRuntime)
                    .HasColumnName("movie_runtime")
                    .IsRequired();
                
                entity.Property(e => e.RoomFormat)
                    .HasColumnName("room_format")
                    .IsRequired();

                entity.Property(e => e.ScreeningDate)
                    .HasColumnName("screening_date")
                    .IsRequired();
                
                entity.Property(e => e.MovieReleaseDate)
                    .HasColumnName("movie_release_date")
                    .IsRequired();

                entity.Property(e => e.SessionId)
                    .HasColumnName("session_id")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();

                entity.Property(e => e.StartingTime)
                    .HasColumnName("starting_time")
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}