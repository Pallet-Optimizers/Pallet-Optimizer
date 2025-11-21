using Pallet_Optimizer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Data
{
    public class InMemoryPalletRepository : IPalletRepository
    {
        private readonly PalletHolder _holder;

        public InMemoryPalletRepository()
        {
            // sample seed data
            _holder = new PalletHolder
            {
                Pallets = new List<Pallet>
                {
                    new Pallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Pallet A",
                        MaterialType = 0,
                        Elements = new List<Element>
                        {
                            new Element { Id = Guid.NewGuid().ToString(), Name = "Box 1"},
                            new Element { Id = Guid.NewGuid().ToString(), Name = "Box 2"},
                            new Element { Id = Guid.NewGuid().ToString(), Name = "Box 3"}
                        }
                    },
                    new Pallet
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Pallet B",
                        MaterialType = PALLET_MATERIAL_TYPE.Plastic,
                        Elements = new List<Element>
                        {
                            new Element { Id = Guid.NewGuid().ToString(), Name = "Box 3"}
                        }
                    }
                },
                CurrentPalletIndex = 0
            };
        }

        public Task AddPalletAsync(Pallet pallet)
        {
            pallet.Id = _holder.Pallets.Any() ? _holder.Pallets.Max(p => p.Id) + 1 : 0.ToString();
            _holder.Pallets.Add(pallet);
            return Task.CompletedTask;
        }

        public Task<Pallet> GetPalletAsync(int index)
        {
            var pallet = _holder.Pallets.ElementAtOrDefault(index);
            return Task.FromResult(pallet);
        }

  


        public Task<PalletHolder> GetHolderAsync()
        {
            return Task.FromResult(_holder);
        }

        public Task UpdatePalletAsync(int index, Pallet updated)
        {
            var existing = _holder.Pallets.ElementAtOrDefault(index);
            if (existing != null)
            {
                existing.Name = updated.Name;
                existing.MaterialType = updated.MaterialType;
                existing.Elements = updated.Elements ?? new List<Element>();
            }
            return Task.CompletedTask;
        }
    }
}
