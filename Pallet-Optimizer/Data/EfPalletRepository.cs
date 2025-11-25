using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pallet_Optimizer.Models;
using Pallet_Optimizer.Data.Database;

namespace Pallet_Optimizer.Data {
    public class EfPalletRepository : IPalletRepository {
        private readonly AppDbContext _context;

        public EfPalletRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<PalletHolder> GetHolderAsync() {
            var dbPallets = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .ToListAsync();

            var pallets = dbPallets.Select(p => p.ToDomain()).ToList();

            return new PalletHolder {
                Pallets = pallets ?? new List<Pallet>(),
                CurrentPalletIndex = 0
            };
        }

        public async Task<Pallet?> GetPalletAsync(string index) {
            if (!int.TryParse(index, out var pid)) return null;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);

            return db?.ToDomain();
        }

        public async Task AddPalletAsync(Pallet pallet) {
            var db = pallet.ToDb();

            // Only update persisted WeightKg — pallet dimensions must remain as provided.
            db.RecalculateWeightKg();

            await _context.Set<PalletDb>().AddAsync(db);
            await _context.SaveChangesAsync();

            pallet.Id = db.PalletId.ToString();

            if (pallet.Elements != null) {
                foreach (var el in pallet.Elements) {
                    el.PalletId = pallet.Id;
                }
            }
        }

        public async Task UpdatePalletAsync(string index, Pallet updated) {
            if (!int.TryParse(index, out var pid)) return;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);
            if (db == null) return;

            db.Type = updated.Name;
            db.Width = Convert.ToDecimal(updated.Width);
            db.Length = Convert.ToDecimal(updated.Length);
            db.Height = Convert.ToDecimal(updated.Height);
            db.MaxHeight = Convert.ToDecimal(updated.MaxHeight);
            db.MaxWeight = Convert.ToDecimal(updated.MaxWeight);
            db.Active = !updated.IsSpecial;
            db.MaterialId = (int)updated.MaterialType + 1;

            var existingElements = db.Elements?.ToList() ?? new List<ElementDb>();
            if (existingElements.Any()) {
                _context.Set<ElementDb>().RemoveRange(existingElements);
            }

            db.Elements = updated.Elements?.Select(e => e.ToDb()).ToList() ?? new List<ElementDb>();

            // Only recalculate WeightKg here so pallet dimensions remain unchanged.
            db.RecalculateWeightKg();

            await _context.SaveChangesAsync();
        }

        public async Task DeletePalletAsync(string index) {
            if (!int.TryParse(index, out var pid)) return;

            var db = await _context.Set<PalletDb>().FindAsync(pid);
            if (db == null) return;

            _context.Set<PalletDb>().Remove(db);
            await _context.SaveChangesAsync();
        }
    }
}
