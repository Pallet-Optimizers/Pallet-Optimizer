using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Data
{
    // Hybrid repository: EF persistence + in-memory cache.
    public class HybridPalletRepository : IPalletRepository
    {
        private readonly EfPalletRepository _ef;
        private readonly InMemoryPalletRepository _mem;

        public HybridPalletRepository(EfPalletRepository ef, InMemoryPalletRepository mem)
        {
            _ef = ef;
            _mem = mem;
        }

        public async Task<PalletHolder> GetHolderAsync()
        {
            try
            {
                var efHolder = await _ef.GetHolderAsync();

                var memHolder = await _mem.GetHolderAsync();

                foreach (var mp in memHolder.Pallets.ToList())
                {
                    if (!efHolder.Pallets.Any(p => p.Id == mp.Id))
                    {
                        await _mem.DeletePalletAsync(mp.Id);
                    }
                }

                foreach (var p in efHolder.Pallets)
                {
                    var existing = await _mem.GetPalletAsync(p.Id);
                    if (existing == null)
                        await _mem.AddPalletAsync(p);
                    else
                        await _mem.UpdatePalletAsync(p.Id, p);
                }

                return efHolder;
            }
            catch
            {
                return await _mem.GetHolderAsync();
            }
        }

        public async Task<Pallet?> GetPalletAsync(string palletId)
        {
            try
            {
                var ef = await _ef.GetPalletAsync(palletId);
                if (ef != null)
                {
                    var mem = await _mem.GetPalletAsync(palletId);
                    if (mem == null) await _mem.AddPalletAsync(ef);
                    else await _mem.UpdatePalletAsync(ef.Id, ef);
                    return ef;
                }
            }
            catch { }

            return await _mem.GetPalletAsync(palletId);
        }

        public async Task AddPalletAsync(Pallet pallet)
        {
            await _ef.AddPalletAsync(pallet);
            await _mem.AddPalletAsync(pallet);
        }

        public async Task UpdatePalletAsync(string id, Pallet updated)
        {
            try { await _ef.UpdatePalletAsync(id, updated); } catch { }
            await _mem.UpdatePalletAsync(id, updated);
        }

        public async Task DeletePalletAsync(string id)
        {
            try { await _ef.DeletePalletAsync(id); } catch { }
            await _mem.DeletePalletAsync(id);
        }

        public async Task UpdateHolderAsync(PalletHolder holder)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder), "Holder payload was null.");
            if (holder.Pallets == null) throw new ArgumentException("Holder.Pallets collection was null.", nameof(holder));

            // Persist full holder via EF
            await _ef.UpdateHolderAsync(holder);

            // Sync memory copy
            var refreshed = await _ef.GetHolderAsync();
            await _mem.UpdateHolderAsync(refreshed);
        }
    }
}