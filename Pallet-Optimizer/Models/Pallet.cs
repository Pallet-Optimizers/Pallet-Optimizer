using Pallet_Optimizer.Models;
using Pallet_Optimizer.Models.Globals;

public class Pallet
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double CurrentHeight { get; set; }
    public double CurrentWeight{ get; set; } 
    public double Length { get; set; }
    public double MaxHeight { get; set; }
    public double MaxWeightKg { get; set; } = Globals.MAX_WEIGHTKg; // KG
    public double WeightKg { get; set; }
    public PALLET_MATERIAL_TYPE MaterialType { get; set; } = 0; // 0=Wood, 1=Plastic, etc.
    public bool IsSpecial { get; set; } = false;

    public List<Element> Elements { get; set; } = new List<Element>();

    public double CurrentHeightMm => Elements.Sum(e => e.Height);
    public double CurrentWeightKg => Elements.Sum(e => e.WeightKg);
    public double UsedArea => Elements.Sum(e => e.FootprintArea);
    public double FootprintArea => Length * Width;


}
