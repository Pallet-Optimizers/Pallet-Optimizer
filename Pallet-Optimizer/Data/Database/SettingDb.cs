using System;
using System.Collections.Generic;

namespace Pallet_Optimizer.Data.Database;

public partial class SettingDb
{
    public int SettingsId { get; set; }

    public int MaxLayers { get; set; }

    public decimal MaxWeightWhenRotated { get; set; }

    public decimal HeightWidthFactor { get; set; }

    public bool AllowStacking { get; set; }

    public decimal MaxTotalHeight { get; set; }
}
