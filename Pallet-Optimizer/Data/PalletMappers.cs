using System;
using System.Collections.Generic;
using System.Linq;
using Pallet_Optimizer.Models;
using Pallet_Optimizer.Data.Database;

namespace Pallet_Optimizer.Data
{
    internal static class PalletMappers
    {
        // Map DB PalletDb -> domain Pallet
        public static Pallet ToDomain(this PalletDb db)
        {
            if (db == null) return null!;

            PALLET_MATERIAL_TYPE material = PALLET_MATERIAL_TYPE.Wood;

            if (db.MaterialId > 0)
            {
                var enumIndex = db.MaterialId - 1;
                if (Enum.IsDefined(typeof(PALLET_MATERIAL_TYPE), enumIndex))
                {
                    material = (PALLET_MATERIAL_TYPE)enumIndex;
                }
                else
                {
                    material = ParseMaterialType(db.Type);
                }
            }
            else
            {
                material = ParseMaterialType(db.Type);
            }

            var pallet = new Pallet
            {
                Id = db.PalletId.ToString(),
                Name = db.Type ?? db.PalletSize ?? $"Pallet-{db.PalletId}",
                Width = (double)db.Width,
                Length = (double)db.Length,
                Height = (double)db.Height,
                MaxHeight = (double)db.MaxHeight,
                MaxWeightKg = (double)db.MaxWeightKg,
                CurrentHeight = 0,
                CurrentWeight = 0,
                MaterialType = material,
                IsSpecial = !db.Active,
                Elements = db.Elements?.Select(e => e.ToDomain()).ToList() ?? new List<Element>()
            };

            return pallet;
        }

        // Map DB ElementDb -> domain Element
        public static Element ToDomain(this ElementDb db)
        {
            if (db == null) return null!;

            return new Element
            {
                Id = db.ElementId.ToString(),
                Name = db.Brand ?? db.PalletType ?? $"Element-{db.ElementId}",
                Width = (double)db.Width,
                Height = (double)db.Height,
                Depth = (double)db.Depth,
                WeightKg = (double)db.Weight,
                CanRotate = db.CanRotate,
                MustBeAlone = false,
                PalletId = db.PalletId?.ToString() ?? string.Empty
            };
        }

        // Map domain Pallet -> DB PalletDb (do not set identity)
        public static PalletDb ToDb(this Pallet domain)
        {
            if (domain == null) return null!;

            var db = new PalletDb
            {
                Length = Convert.ToDecimal(domain.Length),
                Width = Convert.ToDecimal(domain.Width),
                Height = Convert.ToDecimal(domain.Height),
                MaxHeight = Convert.ToDecimal(domain.MaxHeight),
                MaxWeightKg = Convert.ToDecimal(domain.MaxWeightKg),
                Type = domain.Name,
                MaterialId = (int)domain.MaterialType + 1,
                Active = !domain.IsSpecial,
                // Elements will also avoid setting identity
                Elements = domain.Elements?.Select(e => e.ToDb()).ToList() ?? new List<ElementDb>()
            };

            return db;
        }

        // Map domain Element -> DB ElementDb (do not set identity)
        public static ElementDb ToDb(this Element domain)
        {
            if (domain == null) return null!;

            return new ElementDb
            {
                // ElementId intentionally not set; let DB generate it
                Width = Convert.ToDecimal(domain.Width),
                Height = Convert.ToDecimal(domain.Height),
                Depth = Convert.ToDecimal(domain.Depth),
                Weight = Convert.ToDecimal(domain.WeightKg),
                CanRotate = domain.CanRotate,
                Brand = domain.Name,
                PalletId = TryParseNullableInt(domain.PalletId)
            };
        }

        // Helpers
        private static int TryParseInt(string? s)
        {
            if (int.TryParse(s, out var v)) return v;
            return 0;
        }

        private static int? TryParseNullableInt(string? s)
        {
            if (int.TryParse(s, out var v)) return v;
            return null;
        }

        private static PALLET_MATERIAL_TYPE ParseMaterialType(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return PALLET_MATERIAL_TYPE.Wood;
            if (Enum.TryParse<PALLET_MATERIAL_TYPE>(s, true, out var parsed)) return parsed;
            if (s.Contains("plastic", StringComparison.OrdinalIgnoreCase)) return PALLET_MATERIAL_TYPE.Plastic;
            if (s.Contains("metal", StringComparison.OrdinalIgnoreCase)) return PALLET_MATERIAL_TYPE.Metal;
            return PALLET_MATERIAL_TYPE.Wood;
        }
    }
}