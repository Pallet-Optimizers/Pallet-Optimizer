using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class PlanPalletDb
{
    public int PlanPalletId { get; set; }

    public int PlanId { get; set; }

    public int PalletId { get; set; }

    public virtual PalletDb Pallet { get; set; } = null!;

    public virtual OptimizationPlanDb Plan { get; set; } = null!;
}
