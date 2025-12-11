using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class ElementPlacementDb
{
    public int PlacementId { get; set; }

    public int PlanId { get; set; }

    public int PalletId { get; set; }

    public int ElementId { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public virtual ElementDb Element { get; set; } = null!;

    public virtual PalletDb Pallet { get; set; } = null!;

    public virtual OptimizationPlanDb Plan { get; set; } = null!;
}
