using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pallet_Optimizer.Models
{
    public class Pallet
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Pallet";
        public PALLET_MATERIAL_TYPE MaterialType { get; set; } = PALLET_MATERIAL_TYPE.Wood;
        
        public bool Active { get; set; } = true;
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
