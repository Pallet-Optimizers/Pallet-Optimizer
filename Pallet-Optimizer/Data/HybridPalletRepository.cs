using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Data
{
    // Hybrid repository: primary persistence via EF, runtime cache via in-memory store.
    // Keeps the in-memory store synchronized with the database on reads/writes.
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

                // Sync memory store to match EF store:
                var memHolder = await _mem.GetHolderAsync();

                // Remove mem pallets that are not present in EF
                foreach (var mp in memHolder.Pallets)
                {
                    if (!efHolder.Pallets.Any(p => p.Id == mp.Id))
                    {
                        await _mem.DeletePalletAsync(mp.Id);
                    }
                }

                // Add/replace mem pallets with EF pallets (ensure mem has up-to-date copy)
                foreach (var p in efHolder.Pallets)
                {
                    var existing = await _mem.GetPalletAsync(p.Id);
                    if (existing == null)
                    {
                        await _mem.AddPalletAsync(p);
                    }
                    else
                    {
                        await _mem.UpdatePalletAsync(p.Id, p);
                    }
                }

                return efHolder;
            }
            catch
            {
                // If EF fails, fall back to memory
                return await _mem.GetHolderAsync();
            }
        }

        public async Task<Pallet?> GetPalletAsync(string index)
        {
            try
            {
                var ef = await _ef.GetPalletAsync(index);
                if (ef != null)
                {
                    // keep memory in sync
                    var mem = await _mem.GetPalletAsync(index);
                    if (mem == null) await _mem.AddPalletAsync(ef);
                    else await _mem.UpdatePalletAsync(ef.Id, ef);
                    return ef;
                }
            }
            catch { /* ignore and fallback to memory */ }

            return await _mem.GetPalletAsync(index);
        }

        public async Task AddPalletAsync(Pallet pallet)
        {
            // Persist to EF (will set pallet.Id to DB id in modified EfPalletRepository)
            await _ef.AddPalletAsync(pallet);

            // ensure memory store mirrors the persisted object (pallet.Id updated)
            await _mem.AddPalletAsync(pallet);
        }

        public async Task UpdatePalletAsync(string index, Pallet updated)
        {
            try
            {
                await _ef.UpdatePalletAsync(index, updated);
            }
            catch { /* continue to attempt memory update */ }

            await _mem.UpdatePalletAsync(index, updated);
        }

        public async Task DeletePalletAsync(string index)
        {
            try
            {
                await _ef.DeletePalletAsync(index);
            }
            catch { /* ignore */ }

            await _mem.DeletePalletAsync(index);
        }
    }
}