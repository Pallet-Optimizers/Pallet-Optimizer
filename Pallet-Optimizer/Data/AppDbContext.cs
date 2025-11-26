using Microsoft.EntityFrameworkCore;
using Pallet_Optimizer.Data.Database;

namespace Pallet_Optimizer.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Use the scaffolded DB entity types that match your existing database.
        public DbSet<PalletDb> PalletDb { get; set; }
        public DbSet<ElementDb> ElementDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicit primary keys to remove ambiguity from convention-based mapping.
            modelBuilder.Entity<PalletDb>().HasKey(p => p.PalletId);
            modelBuilder.Entity<ElementDb>().HasKey(e => e.ElementId);
            modelBuilder.Entity<ElementPlacementDb>().HasKey(ep => ep.PlacementId);

            // Keys for additional scaffolded types that do not follow EF naming conventions.
            modelBuilder.Entity<OptimizationPlanDb>().HasKey(op => op.PlanId);
            modelBuilder.Entity<PlanPalletDb>().HasKey(pp => pp.PlanPalletId);
            modelBuilder.Entity<PackagePlanDb>().HasKey(pp => pp.PackagePlanId);
            modelBuilder.Entity<PalletPackageDb>().HasKey(pp => pp.PalletPackageId);
            modelBuilder.Entity<PackageElementDb>().HasKey(pe => pe.PackageElementId);

            // Explicit relationships matching scaffolded navigation properties.
            modelBuilder.Entity<PalletDb>()
                .HasMany(p => p.Elements)
                .WithOne(e => e.Pallet)
                .HasForeignKey(e => e.PalletId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PalletDb>()
                .HasMany(p => p.ElementPlacements)
                .WithOne(ep => ep.Pallet)
                .HasForeignKey(ep => ep.PalletId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ElementDb>()
                .HasMany(e => e.ElementPlacements)
                .WithOne(ep => ep.Element)
                .HasForeignKey(ep => ep.ElementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Explicitly configure decimal column types/precision to avoid truncation warnings.
            modelBuilder.Entity<ElementDb>(entity =>
            {
                entity.Property(e => e.Depth).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ElementPlacementDb>(entity =>
            {
                entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<OptimizationPlanDb>(entity =>
            {
                entity.Property(e => e.TotalHeight).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.TotalWeight).HasColumnType("decimal(12, 2)");
            });

            modelBuilder.Entity<PackagePlanDb>(entity =>
            {
                entity.Property(e => e.PackageDepth).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PackageHeight).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PackageWeight).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.PackageWidth).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<PalletDb>(entity =>
            {
                entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxHeight).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxWeightKg).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WeightKg).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<PalletPackageDb>(entity =>
            {
                entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}