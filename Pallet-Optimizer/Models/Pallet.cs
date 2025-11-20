using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pallet_Optimizer.Models
{
    public class PalletHolder : Model
    {
        public List<Pallet> Pallets { get; set; } = new List<Pallet>()
        { new Pallet() };
        public int currentPalletIndex { get; set; } = 0;

        public PalletHolder()
        {
        }
    }
    public enum PALLET_MATERIAL_TYPE : short
    {
        WOOD,
        TEST
    }

    public class Measurements
    {
        public double Length { get; set; }// In centimeters
        public double Width { get; set; }// In centimeters
        public double Height { get; set; }// In centimeters
        public short Weight { get; set; } // In kilograms

        public Measurements(double length, double width, double height, short weight)
        {
            Length = length;
            Width = width;
            Height = height;
            Weight = weight;
        }
    }

    public class Pallet
    {
        public string? Description { get; set; } = string.Empty;

        public Measurements? Measurements { get; set; } = new Measurements(0,0,0,0);
        public short Overhang { get; set; } // In MM

        public short PalletGroup { get; set; }

        public PALLET_MATERIAL_TYPE? MaterialType { get; set; } = PALLET_MATERIAL_TYPE.WOOD;

        public bool Active { get; set; } = true;

        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
