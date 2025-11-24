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
            // index is a string id in domain; DB uses integer key
            if (!int.TryParse(index, out var pid)) return null;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);

            return db?.ToDomain();
        }

        public async Task AddPalletAsync(Pallet pallet) {
            var db = pallet.ToDb();
            await _context.Set<PalletDb>().AddAsync(db);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePalletAsync(string index, Pallet updated) {
            if (!int.TryParse(index, out var pid)) return;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);
            if (db == null) return;

            // update simple properties
            db.Type = updated.Name;
            db.Width = Convert.ToDecimal(updated.Width);
            db.Length = Convert.ToDecimal(updated.Length);
            db.Height = Convert.ToDecimal(updated.Height);
            db.MaxHeight = Convert.ToDecimal(updated.MaxHeight);
            db.MaxWeight = Convert.ToDecimal(updated.MaxWeight);
            db.Active = !updated.IsSpecial;

            // Persist material selection to the DB FK column (enum -> DB id mapping).
            // Matches PalletMappers.ToDb which does: MaterialId = (int)domain.MaterialType + 1
            db.MaterialId = (int)updated.MaterialType + 1;

            // replace elements: remove existing placements/elements as appropriate then add new
            // simple approach: remove existing Element rows that belong to this pallet and re-add
            var existingElements = db.Elements?.ToList() ?? new List<ElementDb>();
            if (existingElements.Any()) {
                _context.Set<ElementDb>().RemoveRange(existingElements);
            }

            db.Elements = updated.Elements?.Select(e => e.ToDb()).ToList() ?? new List<ElementDb>();

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
