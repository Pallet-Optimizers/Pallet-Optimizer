using Pallet_Optimizer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Data
{
    public class InMemoryPalletRepository : IPalletRepository
    {
        private readonly List<Pallet> _store = new List<Pallet>();

        public InMemoryPalletRepository()
        {
            _store.Add(new Pallet { Id = "1", Name = "Pallet A", MaterialType = PALLET_MATERIAL_TYPE.Wood });
            _store.Add(new Pallet { Id = "2", Name = "Pallet B", MaterialType = PALLET_MATERIAL_TYPE.Plastic });
        }

        public Task AddPalletAsync(Pallet pallet)
        {
            _store.Add(pallet);
            return Task.CompletedTask;
        }

        public Task<Pallet?> GetPalletAsync(string index)
        {
            var p = _store.FirstOrDefault(x => x.Id == index);
            return Task.FromResult<Pallet?>(p);
        }

        public Task<PalletHolder> GetHolderAsync()
        {
            var holder = new PalletHolder
            {
                Pallets = _store.ToList(),
                CurrentPalletIndex = 0
            };
            return Task.FromResult(holder);
        }

        public Task UpdatePalletAsync(string index, Pallet updated)
        {
            var existing = _store.FirstOrDefault(p => p.Id == index);
            if (existing != null)
            {
                existing.Name = updated.Name;
                existing.MaterialType = updated.MaterialType;
                existing.Width = updated.Width;
                existing.Height = updated.Height;
                existing.Length = updated.Length;
                existing.MaxHeight = updated.MaxHeight;
                existing.MaxWeight = updated.MaxWeight;
                existing.IsSpecial = updated.IsSpecial;
                existing.Elements = updated.Elements;
            }
            return Task.CompletedTask;
        }

        public Task DeletePalletAsync(string index)
        {
            var existing = _store.FirstOrDefault(p => p.Id == index);
            if (existing != null) _store.Remove(existing);
            return Task.CompletedTask;
        }
    }
}
