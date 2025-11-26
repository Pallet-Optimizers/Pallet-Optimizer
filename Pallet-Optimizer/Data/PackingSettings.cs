namespace Pallet_Optimizer.Data
{
    public class PackingSettings
    {
        public double Padding { get; set; } = 0.02; // meters between items
        public double MaxOverhangX { get; set; } = 0.0; // allowed overhang width
        public double MaxOverhangY { get; set; } = 0.0; // allowed overhang depth
        public bool AllowRotation { get; set; } = true;
        public bool RespectMustBeAlone { get; set; } = true;
        public double MaxPalletHeightAbsolute { get; set; } = 2.0; // safety
    }
}
