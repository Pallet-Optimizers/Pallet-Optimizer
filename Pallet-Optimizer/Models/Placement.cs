namespace Pallet_Optimizer.Models
{
    public class Placement
    {
        public Guid ElementId { get; set; }
        public Guid PalletId { get; set; }
        public int RowIndex { get; set; } // logical row slot index
        public int ColumnIndex { get; set; }
        public int Layer { get; set; } // stacking layer
        public decimal X { get; set; } // mm offset
        public decimal Y { get; set; } // mm offset
        public decimal RotationDegrees { get; set; }
    }
}
