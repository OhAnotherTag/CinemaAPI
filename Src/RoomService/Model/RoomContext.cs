using System;
using System.Collections.Generic;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace RoomService.Model
{
    public partial class RoomContext : DbContext
    {
        public RoomContext()
        {
        }

        static RoomContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<Format>();

        public RoomContext(DbContextOptions<RoomContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoomModel> Rooms { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Format>()
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<RoomModel>(entity =>
            {
                entity.ToTable("room");

                entity.HasKey(e => e.RoomId);

                entity.Property(e => e.RoomId)
                    .HasColumnName("room_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CinemaId)
                    .HasColumnName("cinema_id")
                    .IsRequired();

                entity.Property(e => e.Seats)
                    .HasColumnName("seats")
                    .IsRequired();
                
                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .IsRequired();

                entity.Property(e => e.Format)
                    .HasColumnName("format")
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}