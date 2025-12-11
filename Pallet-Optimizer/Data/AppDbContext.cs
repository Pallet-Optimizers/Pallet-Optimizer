using Microsoft.EntityFrameworkCore;
using Pallet_Optimizer.Data.Database;

namespace Pallet_Optimizer.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PalletDb> PalletDb { get; set; }
        public DbSet<ElementDb> ElementDb { get; set; }
        public DbSet<ElementPlacementDb> ElementPlacementDb { get; set; }
        public DbSet<OptimizationPlanDb> OptimizationPlanDb { get; set; }
        public DbSet<PackagePlanDb> PackagePlanDb { get; set; }
        public DbSet<PalletPackageDb> PalletPackageDb { get; set; }
        public DbSet<PlanPalletDb> PlanPalletDb { get; set; }
        public DbSet<PackageElementDb> PackageElementDb { get; set; } // add this

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Keys
            modelBuilder.Entity<PalletDb>().HasKey(p => p.PalletId);
            modelBuilder.Entity<ElementDb>().HasKey(e => e.ElementId);
            modelBuilder.Entity<ElementPlacementDb>().HasKey(ep => ep.PlacementId);

            modelBuilder.Entity<OptimizationPlanDb>().HasKey(p => p.PlanId);
            modelBuilder.Entity<PackagePlanDb>().HasKey(p => p.PackagePlanId);
            modelBuilder.Entity<PalletPackageDb>().HasKey(p => p.PalletPackageId);
            modelBuilder.Entity<PlanPalletDb>().HasKey(pp => pp.PlanPalletId);

            // Identity generation for PKs
            modelBuilder.Entity<PalletDb>().Property(p => p.PalletId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ElementDb>().Property(e => e.ElementId).ValueGeneratedOnAdd();
            modelBuilder.Entity<OptimizationPlanDb>().Property(p => p.PlanId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PackagePlanDb>().Property(p => p.PackagePlanId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PalletPackageDb>().Property(p => p.PalletPackageId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PlanPalletDb>().Property(p => p.PlanPalletId).ValueGeneratedOnAdd();

            // Relationships
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

            modelBuilder.Entity<PlanPalletDb>()
                .HasOne(pp => pp.Plan)
                .WithMany(p => p.PlanPallets)
                .HasForeignKey(pp => pp.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanPalletDb>()
                .HasOne(pp => pp.Pallet)
                .WithMany(p => p.PlanPallets)
                .HasForeignKey(pp => pp.PalletId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PackagePlanDb>()
                .HasOne(p => p.Plan)
                .WithMany(o => o.PackagePlans)
                .HasForeignKey(p => p.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PalletPackageDb>()
                .HasOne(p => p.Pallet)
                .WithMany(x => x.PalletPackages)
                .HasForeignKey(p => p.PalletId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PalletPackageDb>()
                .HasOne(p => p.PackagePlan)
                .WithMany(x => x.PalletPackages)
                .HasForeignKey(p => p.PackagePlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ElementPlacementDb>()
                .HasOne(ep => ep.Plan)
                .WithMany(p => p.ElementPlacements)
                .HasForeignKey(ep => ep.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // ADD: PackageElementDb mapping so EF knows its PK and relationships
            modelBuilder.Entity<PackageElementDb>(entity =>
            {
                entity.HasKey(e => e.PackageElementId);
                entity.Property(e => e.Quantity).HasDefaultValue(1);

                entity.HasOne(e => e.Element)
                      .WithMany(e => e.PackageElements)
                      .HasForeignKey(e => e.ElementId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PackagePlan)
                      .WithMany(p => p.PackageElements)
                      .HasForeignKey(e => e.PackagePlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Precision/column types
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