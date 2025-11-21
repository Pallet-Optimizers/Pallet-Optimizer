namespace Pallet_Optimizer.Data
{
    using Pallet_Optimizer.Models;

    public interface IRulesEngine
    {
        bool CanPlace(
            Element el,
            Pallet pallet,
            OptimizationSettings settings,
            out ElementPlacement placement);
    }
}
