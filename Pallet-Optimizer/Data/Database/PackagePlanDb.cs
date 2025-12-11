using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class PackagePlanDb
{
    public int PackagePlanId { get; set; }

    public int PlanId { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal? PackageWeight { get; set; }

    public decimal? PackageHeight { get; set; }

    public decimal? PackageWidth { get; set; }

    public decimal? PackageDepth { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PackageElementDb> PackageElements { get; set; } = new List<PackageElementDb>();

    public virtual ICollection<PalletPackageDb> PalletPackages { get; set; } = new List<PalletPackageDb>();

    public virtual OptimizationPlanDb Plan { get; set; } = null!;
}
