using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class OptimizationPlanDb
{
    public int PlanId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int PalletCount { get; set; }

    public decimal TotalWeight { get; set; }

    public decimal TotalHeight { get; set; }

    public virtual ICollection<ElementPlacementDb> ElementPlacements { get; set; } = new List<ElementPlacementDb>();

    public virtual ICollection<PackagePlanDb> PackagePlans { get; set; } = new List<PackagePlanDb>();

    public virtual ICollection<PlanPalletDb> PlanPallets { get; set; } = new List<PlanPalletDb>();
}
