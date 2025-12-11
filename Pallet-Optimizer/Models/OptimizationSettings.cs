using Pallet_Optimizer.Models.Globals;
public class OptimizationSettings
{
    public double MaxPalletHeight { get; set; }
    public int MaxLayers { get; set; } = 3;
    public short MaxOverhangMM { get; set; } = 0;
}