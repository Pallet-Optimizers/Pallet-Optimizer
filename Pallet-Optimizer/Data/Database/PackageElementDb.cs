using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class PackageElementDb
{
    public int PackageElementId { get; set; }

    public int PackagePlanId { get; set; }

    public int ElementId { get; set; }

    public int Quantity { get; set; }

    public virtual ElementDb Element { get; set; } = null!;

    public virtual PackagePlanDb PackagePlan { get; set; } = null!;
}
