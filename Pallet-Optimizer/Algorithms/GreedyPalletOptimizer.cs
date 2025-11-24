public class GreedyPalletOptimizer
{
    public List<Pallet> Optimize(List<Element> elements, Pallet defaultPallet, OptimizationSettings settings)
    {
        var pallets = new List<Pallet>();
        pallets.Add(new Pallet
        {
            Id = defaultPallet.Id,
            Width = defaultPallet.Width,
            Height = defaultPallet.Height,
            Length = defaultPallet.Length,
            MaxWeight = defaultPallet.MaxWeight,
            IsSpecial = defaultPallet.IsSpecial
        });

        // Sort elements: special first, then by weight descending, then height descending
        var sorted = elements
            .OrderByDescending(e => e.MustBeAlone)
            .ThenByDescending(e => e.WeightKg)
            .ThenByDescending(e => e.Height)
            .ToList();

        foreach (var element in sorted)
        {
            bool placed = false;

            foreach (var pallet in pallets)
            {
                if (CanPlace(element, pallet, settings))
                {
                    pallet.Elements.Add(element);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                var newPallet = new Pallet
                {
                    Id = Guid.NewGuid().ToString(),
                    Width = defaultPallet.Width,
                    Length = defaultPallet.Length,
                    Height = defaultPallet.Height,
                    MaxWeight = defaultPallet.MaxWeight,
                    IsSpecial = element.MustBeAlone
                };

                if (!CanPlace(element, newPallet, settings))
                    throw new InvalidOperationException($"Cannot place element {element.Id}");

                newPallet.Elements.Add(element);
                pallets.Add(newPallet);
            }
        }

        return pallets;
    }

    private bool CanPlace(Element el, Pallet pallet, OptimizationSettings settings)
    {
        if (el.MustBeAlone && pallet.Elements.Count > 0) return false;

        if (pallet.CurrentHeight + el.Height > settings.MaxPalletHeight) return false;

        if (el.WeightKg > settings.MaxWeightPerElement) return false;

        if (pallet.CurrentWeight + el.WeightKg > pallet.MaxWeight) return false;

        return true;
    }
}
