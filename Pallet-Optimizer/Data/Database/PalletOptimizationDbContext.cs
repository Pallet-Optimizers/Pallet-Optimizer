using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pallet_Optimizer.Data.Database;

public partial class PalletOptimizationDbContext : DbContext
{
    public PalletOptimizationDbContext()
    {
    }

    public PalletOptimizationDbContext(DbContextOptions<PalletOptimizationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ElementDb> Elements { get; set; }

    public virtual DbSet<ElementPlacementDb> ElementPlacements { get; set; }

    public virtual DbSet<OptimizationPlanDb> OptimizationPlans { get; set; }

    public virtual DbSet<PackageElementDb> PackageElements { get; set; }

    public virtual DbSet<PackagePlanDb> PackagePlans { get; set; }

    public virtual DbSet<PalletDb> Pallets { get; set; }

    public virtual DbSet<PalletPackageDb> PalletPackages { get; set; }

    public virtual DbSet<PlanPalletDb> PlanPallets { get; set; }

    public virtual DbSet<SettingDb> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Server=MAGNUS\\SQLEXPRESS;Database=PalletOptimizationDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ElementDb>(entity =>
        {
            entity.HasKey(e => e.ElementId).HasName("PK__Element__A429723A936DE604");

            entity.ToTable("Element");

            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Depth).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PalletType).HasMaxLength(50);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Pallet).WithMany(p => p.Elements)
                .HasForeignKey(d => d.PalletId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Element_Pallet");
        });

        modelBuilder.Entity<ElementPlacementDb>(entity =>
        {
            entity.HasKey(e => e.PlacementId).HasName("PK__ElementP__2E328C45BA541727");

            entity.ToTable("ElementPlacement");

            entity.Property(e => e.PlacementId).HasColumnName("PlacementID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Element).WithMany(p => p.ElementPlacements)
                .HasForeignKey(d => d.ElementId)
                .HasConstraintName("FK_ElementPlacement_Element");

            entity.HasOne(d => d.Pallet).WithMany(p => p.ElementPlacements)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK_ElementPlacement_Pallet");

            entity.HasOne(d => d.Plan).WithMany(p => p.ElementPlacements)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_ElementPlacement_Plan");
        });

        modelBuilder.Entity<OptimizationPlanDb>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Optimiza__755C22D759A68C25");

            entity.ToTable("OptimizationPlan");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalHeight).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TotalWeight).HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<PackageElementDb>(entity =>
        {
            entity.HasKey(e => e.PackageElementId).HasName("PK__PackageE__29D7BF4AB603A657");

            entity.ToTable("PackageElement");

            entity.Property(e => e.PackageElementId).HasColumnName("PackageElementID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.PackagePlanId).HasColumnName("PackagePlanID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Element).WithMany(p => p.PackageElements)
                .HasForeignKey(d => d.ElementId)
                .HasConstraintName("FK_PackageElement_Element");

            entity.HasOne(d => d.PackagePlan).WithMany(p => p.PackageElements)
                .HasForeignKey(d => d.PackagePlanId)
                .HasConstraintName("FK_PackageElement_PackagePlan");
        });

        modelBuilder.Entity<PackagePlanDb>(entity =>
        {
            entity.HasKey(e => e.PackagePlanId).HasName("PK__PackageP__13788863F3BBD5BC");

            entity.ToTable("PackagePlan");

            entity.Property(e => e.PackagePlanId).HasColumnName("PackagePlanID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PackageDepth).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PackageHeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.PackageWeight).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PackageWidth).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            entity.HasOne(d => d.Plan).WithMany(p => p.PackagePlans)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_PackagePlan_OptimizationPlan");
        });

        modelBuilder.Entity<PalletDb>(entity =>
        {
            entity.HasKey(e => e.PalletId).HasName("PK__Pallet__C049FE5C77E639A5");

            entity.ToTable("Pallet");

            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxHeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxWeightKg).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PalletSize).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PalletPackageDb>(entity =>
        {
            entity.HasKey(e => e.PalletPackageId).HasName("PK__PalletPa__561CF12F16023726");

            entity.ToTable("PalletPackage");

            entity.Property(e => e.PalletPackageId).HasColumnName("PalletPackageID");
            entity.Property(e => e.PackagePlanId).HasColumnName("PackagePlanID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.PackagePlan).WithMany(p => p.PalletPackages)
                .HasForeignKey(d => d.PackagePlanId)
                .HasConstraintName("FK_PalletPackage_PackagePlan");

            entity.HasOne(d => d.Pallet).WithMany(p => p.PalletPackages)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK_PalletPackage_Pallet");
        });

        modelBuilder.Entity<PlanPalletDb>(entity =>
        {
            entity.HasKey(e => e.PlanPalletId).HasName("PK__PlanPall__103FFB6063AFF040");

            entity.ToTable("PlanPallet");

            entity.Property(e => e.PlanPalletId).HasColumnName("PlanPalletID");
            entity.Property(e => e.PalletId).HasColumnName("PalletID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            entity.HasOne(d => d.Pallet).WithMany(p => p.PlanPallets)
                .HasForeignKey(d => d.PalletId)
                .HasConstraintName("FK_PlanPallet_Pallet");

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanPallets)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_PlanPallet_OptimizationPlan");
        });

        modelBuilder.Entity<SettingDb>(entity =>
        {
            entity.HasKey(e => e.SettingsId).HasName("PK__Settings__991B19DCEA1F5068");

            entity.Property(e => e.SettingsId).HasColumnName("SettingsID");
            entity.Property(e => e.AllowStacking).HasDefaultValue(true);
            entity.Property(e => e.HeightWidthFactor).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxTotalHeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxWeightWhenRotated).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
