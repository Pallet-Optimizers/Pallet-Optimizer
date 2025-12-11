using System.Linq;

namespace Pallet_Optimizer.Data.Database;

public partial class PalletDb
{
    /// <summary>
    /// Recalculates only the persisted WeightKg (sum of element weights).
    /// Does NOT modify Width, Length or Height so the pallet's dimensions remain unchanged.
    /// Assigns the value only when it actually differs (rounded to 2 decimals) to avoid
    /// causing EF Core to mark the pallet as modified unnecessarily.
    /// </summary>
    public void RecalculateWeightKg()
    {
        // Treat empty collection as zero weight.
        if (Elements == null || !Elements.Any())
        {
            SetWeightIfDifferent(0m);
            return;
        }

        // Round to 2 decimals to match DB precision and avoid tiny differences.
        decimal newWeight = decimal.Round(Elements.Sum(e => e.Weight), 2, MidpointRounding.ToEven);
        SetWeightIfDifferent(newWeight);
    }

    private void SetWeightIfDifferent(decimal newValue)
    {
        if (WeightKg != newValue)
        {
            WeightKg = newValue;
        }
    }
}