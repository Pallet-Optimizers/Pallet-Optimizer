using System.Collections.Generic;

namespace Pallet_Optimizer.Models
{
    // Aggregates multiple pallets and tracks the current index
    public class PalletHolder
    {
        public List<Pallet> Pallets { get; set; } = new List<Pallet>();
        public int CurrentPalletIndex { get; set; } = 0;

        public IEnumerable<Element> GetAllElements()
            => Pallets.SelectMany(p => p.Elements ?? new List<Element>());
    }
}