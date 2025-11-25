using Pallet_Optimizer.Models.Globals;

public static class GreedyPalletOptimizer
{
    //Returns a list of elements that are placed on the pallet
    public static List<Element> Optimize(List<Element> elements, Pallet pallet, OptimizationSettings settings)
    {
        Pallet optimizedPallet = pallet;
        List<Element> placedElements = new List<Element>();

        // Sort elements: special first, then by weight descending, then height descending
        var sorted = elements
            .OrderByDescending(e => e.MustBeAlone)
            .ThenByDescending(e => e.WeightKg)
            .ThenByDescending(e => e.Height)
            .ToList();

        foreach (var element in sorted)
        {

            if (CanPlace(element, optimizedPallet, settings))
            {
                Console.WriteLine($"Placing element {element.Name} on pallet {optimizedPallet.Id}" + optimizedPallet.CurrentWeight + " nigger " + element.WeightKg);
                optimizedPallet.Elements.Add(element);
                optimizedPallet.CurrentWeight += element.WeightKg;
                placedElements.Add(element);

            }
            
        }

        return placedElements;
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


        if (pallet.CurrentWeightKg + el.WeightKg > pallet.MaxWeight)
        {
            Console.WriteLine($"Element {el.Name} exceeds pallet max weight.");
            return false;
        }

        return true;
    }
}
