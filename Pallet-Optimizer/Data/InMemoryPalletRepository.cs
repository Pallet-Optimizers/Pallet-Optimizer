using Pallet_Optimizer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Data
{
    public class InMemoryPalletRepository : IPalletRepository
    {
        private readonly Dictionary<string, PackagePlan> _packagePlans;
        private readonly PalletHolder _holder;

        public InMemoryPalletRepository()
        {
            _packagePlans = new Dictionary<string, PackagePlan>();
            
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

        public Task<List<PackagePlanViewModel>> GetAllPackagePlansAsync()
        {
            var viewModels = _packagePlans.Select(kvp => new PackagePlanViewModel
            {
                Id = kvp.Key,
                Name = kvp.Value.Name,
                CreatedDate = kvp.Value.CreatedDate,
                LastModified = kvp.Value.LastModified,
                PalletCount = kvp.Value.Holder.Pallets.Count,
                TotalElements = kvp.Value.Holder.Pallets.Sum(p => p.Elements.Count),
                TotalWeight = kvp.Value.Holder.Pallets.Sum(p => p.CurrentWeightKg)
            }).ToList();

            return Task.FromResult(viewModels);
        }

        public Task<PalletHolder> GetPackagePlanByIdAsync(string id)
        {
            if (_packagePlans.TryGetValue(id, out var plan))
            {
                return Task.FromResult(plan.Holder);
            }
            return Task.FromResult<PalletHolder>(null);
        }

        public Task<string> CreatePackagePlanAsync(string name)
        {
            var id = Guid.NewGuid().ToString();
            var plan = new PackagePlan
            {
                Id = id,
                Name = name,
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                Holder = new PalletHolder
                {
                    Pallets = new List<Pallet>(),
                    CurrentPalletIndex = 0
                }
            };

            _packagePlans[id] = plan;
            return Task.FromResult(id);
        }

        public Task<bool> DeletePackagePlanAsync(string id)
        {
            var removed = _packagePlans.Remove(id);
            return Task.FromResult(removed);
        }

        public Task<bool> UpdatePackagePlanAsync(string id, PalletHolder holder)
        {
            if (_packagePlans.TryGetValue(id, out var plan))
            {
                plan.Holder = holder;
                plan.LastModified = DateTime.Now;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        // Internal class to hold package plan data
        private class PackagePlan
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime LastModified { get; set; }
            public PalletHolder Holder { get; set; }
        }
    }
}
