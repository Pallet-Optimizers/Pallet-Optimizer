using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class ElementDb
{
    public int ElementId { get; set; }

    public decimal Height { get; set; }

    public decimal Width { get; set; }

    public decimal Depth { get; set; }

    public decimal Weight { get; set; }

    public bool CanRotate { get; set; }

    public string? PalletType { get; set; }

    public string? Brand { get; set; }

    public int? PalletId { get; set; }

    public virtual ICollection<ElementPlacementDb> ElementPlacements { get; set; } = new List<ElementPlacementDb>();

    public virtual ICollection<PackageElementDb> PackageElements { get; set; } = new List<PackageElementDb>();

    public virtual PalletDb? Pallet { get; set; }
}
