namespace Pallet_Optimizer.Models
{
    public class PackagePlanViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public int PalletCount { get; set; }
        public int TotalElements { get; set; }
        public double TotalWeight { get; set; }
    }
}