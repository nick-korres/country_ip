using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CountryIP.Entities
{
    public partial class test_dbContext : DbContext
    {
        public test_dbContext()
        {
        }

        public test_dbContext(DbContextOptions<test_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Ipaddress> Ipaddresses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=test_db;user=sa;password=R5gU85VTxQt6txN6");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ThreeLetterCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TwoLetterCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Ipaddress>(entity =>
            {
                entity.ToTable("IPAddresses");

                entity.HasIndex(e => e.Ip, "IX_IPAddresses")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Ip)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("IP");

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Ipaddresses)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IPAddresses_Countries");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
