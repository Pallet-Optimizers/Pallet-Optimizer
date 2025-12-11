namespace Pallet_Optimizer.Rules
{
    using Pallet_Optimizer.Data;
    using Pallet_Optimizer.Models;

    public class SimpleRulesEngine : IRulesEngine
    {
        public bool CanPlace(
            Element el,
            Pallet pallet,
            OptimizationSettings settings,
            out ElementPlacement placement)
        {
            placement = null;

            // 1. Weight
            if (pallet.CurrentWeightKg + el.WeightKg > pallet.MaxWeightKg)
                return false;

            // 2. Height
            if (pallet.CurrentHeightMm + el.Size.Height > pallet.MaxHeight)
                return false;

            // 3. Area
            if (pallet.UsedArea + el.FootprintArea > pallet.FootprintArea)
                return false;

            // 4. Fixed placement (top-left)
            placement = new ElementPlacement
            {
                Element = el,
                X = 0,
                Y = 0,
                Rotation = Rotation.None
            };

            return true;
        }
    }
}
