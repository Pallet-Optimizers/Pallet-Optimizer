namespace Pallet_Optimizer.Models
{
    public class ElementPlacement
    {
        public Element Element { get; set; }

        public double X { get; set; }   // mm (position on pallet)
        public double Y { get; set; }   // mm
        public Rotation Rotation { get; set; }
    }

    public enum Rotation
    {
        None,
        Rotated90
    }
}
