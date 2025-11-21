namespace Pallet_Optimizer.Models
{
    public class Measurements
    {
        public double? Width { get; set; }
        public double? Length { get; set; }
        public double? Height { get; set; } 

        public double? Weight { get; set; }
    }
    public class Element
    {
        public Measurements? Measurements { get; set; }

        public string? Name { get; set; }

        public int? Id { get; set; }
    }
}
