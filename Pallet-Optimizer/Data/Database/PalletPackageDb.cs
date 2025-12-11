using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class PalletPackageDb
{
    public int PalletPackageId { get; set; }

    public int PalletId { get; set; }

    public int PackagePlanId { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public virtual PackagePlanDb PackagePlan { get; set; } = null!;

    public virtual PalletDb Pallet { get; set; } = null!;
}
