public class OptimizationSettings
{
    public double MaxPalletHeight { get; set; }
    public int MaxLayers { get; set; } = 3;
    public double MaxWeightPerElement { get; set; } = 9999; // default unlimited
}