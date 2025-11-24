using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class PalletDb
{
    public int PalletId { get; set; }

    // Added to match the database column that is non-nullable.
    public int MaterialId { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal MaxHeight { get; set; }

    public decimal MaxWeight { get; set; }

    public string? PalletSize { get; set; }

    public string? Type { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<ElementPlacementDb> ElementPlacements { get; set; } = new List<ElementPlacementDb>();

    public virtual ICollection<ElementDb> Elements { get; set; } = new List<ElementDb>();

    public virtual ICollection<PalletPackageDb> PalletPackages { get; set; } = new List<PalletPackageDb>();

    public virtual ICollection<PlanPalletDb> PlanPallets { get; set; } = new List<PlanPalletDb>();
}
