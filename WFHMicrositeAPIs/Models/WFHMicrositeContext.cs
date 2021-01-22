using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WFHMicrositeAPIs.Models
{
    public partial class WFHMicrositeContext : DbContext
    {
        public WFHMicrositeContext()
        {
        }

        public WFHMicrositeContext(DbContextOptions<WFHMicrositeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AlternatePonumber> AlternatePonumbers { get; set; }
        public virtual DbSet<MasterPostalZipP2> MasterPostalZipP2s { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductOption> ProductOptions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<UserNote> UserNotes { get; set; }
        public virtual DbSet<UserSelection> UserSelections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AlternatePonumber>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AlternatePONumbers");

                entity.Property(e => e.AlternatePonumber1)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("AlternatePONumber")
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<MasterPostalZipP2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MasterPostalZipP2");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Chair)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Config).HasMaxLength(100);

                entity.Property(e => e.DealerCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.InstallGuide).HasMaxLength(50);

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.LogoFile).HasMaxLength(100);

                entity.Property(e => e.LogoFile2).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Ponumber)
                    .HasMaxLength(30)
                    .HasColumnName("PONumber");

                entity.Property(e => e.Shipper).HasMaxLength(10);

                entity.Property(e => e.SitFitGuide).HasMaxLength(50);

                entity.Property(e => e.UserGuide).HasMaxLength(50);

                entity.Property(e => e.VideoUrl).HasMaxLength(50);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");

                entity.Property(e => e.FileName).HasMaxLength(100);
            });

            modelBuilder.Entity<ProductOption>(entity =>
            {
                entity.ToTable("ProductOption");

                entity.Property(e => e.ProductOptionId).HasColumnName("ProductOptionID");

                entity.Property(e => e.FileName).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.StockCode).HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address1).HasMaxLength(100);

                entity.Property(e => e.Address2).HasMaxLength(100);

                entity.Property(e => e.AttnName).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Completed).HasColumnType("datetime");

                entity.Property(e => e.Country).HasMaxLength(10);

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Emailed).HasColumnType("datetime");

                entity.Property(e => e.InProduction).HasColumnType("datetime");

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.Pin)
                    .HasMaxLength(10)
                    .HasColumnName("PIN");

                entity.Property(e => e.PostalZip).HasMaxLength(15);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.ProvinceState).HasMaxLength(50);

                entity.Property(e => e.Shipped).HasColumnType("datetime");

                entity.Property(e => e.SpecialInstructions).HasMaxLength(1000);

                entity.Property(e => e.Submitted).HasColumnType("datetime");

                entity.Property(e => e.TrackingNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.ToTable("UserLog");

                entity.Property(e => e.UserLogId).HasColumnName("UserLogID");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserNote>(entity =>
            {
                entity.ToTable("UserNote");

                entity.Property(e => e.UserNoteId).HasColumnName("UserNoteID");

                entity.Property(e => e.Csuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("CSUser");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Note).IsRequired();

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserSelection>(entity =>
            {
                entity.ToTable("UserSelection");

                entity.Property(e => e.UserSelectionId).HasColumnName("UserSelectionID");

                entity.Property(e => e.ProductOptionId).HasColumnName("ProductOptionID");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
