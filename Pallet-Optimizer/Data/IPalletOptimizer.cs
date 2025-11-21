namespace Pallet_Optimizer.Algorithms
{
    using Pallet_Optimizer.Data;
    using Pallet_Optimizer.Models;

    public interface IPalletOptimizer
    {
        OptimizationResult Optimize(
            IEnumerable<Element> elements,
            PalletDefinition defaultDefinition,
            OptimizationSettings settings);
    }
}
