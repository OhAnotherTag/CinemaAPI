using System;
using System.Collections.Generic;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace MovieService.Model
{
    public partial class MovieContext : DbContext
    {
        public MovieContext()
        {
        }
        
        static MovieContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<Format>();

        public MovieContext(DbContextOptions<MovieContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MovieModel> Movies { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Format>()
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<MovieModel>(entity =>
            {
                entity.ToTable("movie");

                entity.HasKey(e => e.MovieId);

                entity.Property(e => e.MovieId)
                    .HasColumnName("movie_id")
                    .HasDefaultValueSql("uuid_generate_v4()").IsRequired();

                entity.Property(e => e.Plot).HasColumnName("plot").IsRequired();

                entity.Property(e => e.ReleaseDate).HasColumnName("release_date").IsRequired();

                entity.Property(e => e.Runtime).HasColumnName("runtime").IsRequired();

                entity.Property(e => e.Title)
                    .HasColumnType("character varying")
                    .HasColumnName("title").IsRequired();
                
                entity.Property(e => e.Format)
                    .HasColumnName("format")
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
