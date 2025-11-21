namespace Pallet_Optimizer.Data
{
    using Pallet_Optimizer.Models;

    public class OptimizationResult
    {
        public List<PalletDefinition> UsedPallets { get; } = new();
        public List<ElementPlacement> Placements { get; } = new();
    }
}
