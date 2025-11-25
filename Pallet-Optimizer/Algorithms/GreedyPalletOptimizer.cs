using Pallet_Optimizer.Models.Globals;

public static class GreedyPalletOptimizer
{
    public static List<Pallet> Optimize(List<Element> elements, Pallet defaultPallet, OptimizationSettings settings)
    {
        var pallets = new List<Pallet>();
        pallets.Add(new Pallet
        {
            Id = defaultPallet.Id,
            Width = defaultPallet.Width,
            Height = defaultPallet.Height,
            Length = defaultPallet.Length,
            CurrentWeight = defaultPallet.CurrentWeight,
            MaxWeight = Globals.MAX_WEIGHT,
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
                    Console.WriteLine($"Placing element {element.Name} on pallet {pallet.Id}" + pallet.CurrentWeight + " nigger " + element.WeightKg);
                    pallet.Elements.Add(element);
                    pallet.CurrentWeight += element.WeightKg;
                    
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                var newPallet = new Pallet
                {
                    Id = Guid.NewGuid().ToString(),
                    Width = 80,
                    Length = 210,
                    Height = 30,
                    MaxWeight = Globals.MAX_WEIGHT,
                    IsSpecial = element.MustBeAlone
                };

                try
                {
                    if (!CanPlace(element, newPallet, settings))
                        {
                        throw new InvalidOperationException($"Element {element.Name} cannot be placed on a new pallet due to size or weight constraints.");
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Element {element.Name} cannot be placed on a new pallet.", ex);
                }

                newPallet.Elements.Add(element);
                pallets.Add(newPallet);
            }
        }

        return pallets;
    }

    private static bool CanPlace(Element el, Pallet pallet, OptimizationSettings settings)
    {
        if (el.MustBeAlone && pallet.Elements.Count > 0)
        {
            Console.WriteLine($"Element {el.Name} must be alone on the pallet.");
            return false;
        }
        
         
        if (pallet.CurrentHeight + el.Height > Globals.MAX_HEIGHT)
        {
            Console.WriteLine($"Element {el.Name} exceeds max pallet height.");
            return false;
        }


        if (pallet.CurrentWeight + el.WeightKg > pallet.MaxWeight)
        {
            Console.WriteLine($"Element {el.Name} exceeds pallet max weight.");
            return false;
        }

        return true;
    }
}
