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

        public async Task<Pallet?> GetPalletAsync(string palletId) {
            if (!int.TryParse(palletId, out var pid)) return null;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);

            return db?.ToDomain();
        }

        public async Task AddPalletAsync(Pallet pallet) {
            if (pallet == null) throw new ArgumentNullException(nameof(pallet), "Pallet payload was null.");

            var db = pallet.ToDb();
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

        public async Task UpdatePalletAsync(string id, Pallet updated) {
            if (updated == null) throw new ArgumentNullException(nameof(updated), "Updated pallet payload was null.");
            if (!int.TryParse(id, out var pid)) return;

            var db = await _context.Set<PalletDb>()
                .Include(p => p.Elements)
                .FirstOrDefaultAsync(p => p.PalletId == pid);
            if (db == null) return;

            db.Type = updated.Name;
            db.Width = Convert.ToDecimal(updated.Width);
            db.Length = Convert.ToDecimal(updated.Length);
            db.Height = Convert.ToDecimal(updated.Height);
            db.MaxHeight = Convert.ToDecimal(updated.MaxHeight);
            db.MaxWeightKg = Convert.ToDecimal(updated.MaxWeightKg);
            db.Active = !updated.IsSpecial;
            db.MaterialId = (int)updated.MaterialType + 1;

            var existingElements = db.Elements?.ToList() ?? new List<ElementDb>();
            if (existingElements.Any()) {
                _context.Set<ElementDb>().RemoveRange(existingElements);
            }

            db.Elements = updated.Elements?.Select(e => e.ToDb()).ToList() ?? new List<ElementDb>();
            db.RecalculateWeightKg();

            await _context.SaveChangesAsync();
        }

        public async Task DeletePalletAsync(string id) {
            if (!int.TryParse(id, out var pid)) return;

            var db = await _context.Set<PalletDb>().FindAsync(pid);
            if (db == null) return;

            _context.Set<PalletDb>().Remove(db);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHolderAsync(PalletHolder holder) {
            if (holder == null)
                throw new ArgumentNullException(nameof(holder), "Holder payload was null.");
            if (holder.Pallets == null)
                throw new ArgumentException("Holder.Pallets collection was null.", nameof(holder));

            var dbSet = _context.Set<PalletDb>();
            var existing = await dbSet
                .Include(p => p.Elements)
                .ToListAsync();

            var incoming = holder.Pallets
                .Select(p => (Parsed: int.TryParse(p.Id, out var v), Value: v, Pallet: p))
                .ToList();

            var incomingIds = new HashSet<int>(incoming.Where(i => i.Parsed).Select(i => i.Value));
            var toRemove = existing.Where(e => !incomingIds.Contains(e.PalletId)).ToList();
            if (toRemove.Any()) {
                _context.RemoveRange(toRemove);
            }

            foreach (var inc in incoming) {
                var domainPallet = inc.Pallet;

                if (!inc.Parsed) {
                    var newDb = domainPallet.ToDb();
                    newDb.RecalculateWeightKg();
                    await dbSet.AddAsync(newDb);
                    await _context.SaveChangesAsync();
                    domainPallet.Id = newDb.PalletId.ToString();
                    continue;
                }

                var dbPallet = existing.FirstOrDefault(p => p.PalletId == inc.Value);
                if (dbPallet == null) {
                    var addDb = domainPallet.ToDb();
                    addDb.RecalculateWeightKg();
                    await dbSet.AddAsync(addDb);
                    await _context.SaveChangesAsync();
                    domainPallet.Id = addDb.PalletId.ToString();
                    continue;
                }

                dbPallet.Type = domainPallet.Name;
                dbPallet.Width = Convert.ToDecimal(domainPallet.Width);
                dbPallet.Length = Convert.ToDecimal(domainPallet.Length);
                dbPallet.Height = Convert.ToDecimal(domainPallet.Height);
                dbPallet.MaxHeight = Convert.ToDecimal(domainPallet.MaxHeight);
                dbPallet.MaxWeightKg = Convert.ToDecimal(domainPallet.MaxWeightKg);
                dbPallet.Active = !domainPallet.IsSpecial;
                dbPallet.MaterialId = (int)domainPallet.MaterialType + 1;

                var existingEls = dbPallet.Elements?.ToList() ?? new List<ElementDb>();
                if (existingEls.Any()) _context.RemoveRange(existingEls);
                dbPallet.Elements = domainPallet.Elements?.Select(e => e.ToDb()).ToList() ?? new List<ElementDb>();

                dbPallet.RecalculateWeightKg();
            }

            await _context.SaveChangesAsync();
        }
    }
}
