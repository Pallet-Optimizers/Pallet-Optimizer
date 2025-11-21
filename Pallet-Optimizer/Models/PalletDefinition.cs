namespace Pallet_Optimizer.Models
{
    public class PalletDefinition
    {
        public double LengthMm { get; set; }
        public double WidthMm { get; set; }
        public double MaxHeightMm { get; set; }
        public double MaxWeightKg { get; set; }

        public double FootprintArea => LengthMm * WidthMm;
    }
}
