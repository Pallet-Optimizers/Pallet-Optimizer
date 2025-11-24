using Pallet_Optimizer.Models;

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
    public double MaxWeight { get; set; }
    public PALLET_MATERIAL_TYPE MaterialType { get; set; } = 0; // 0=Wood, 1=Plastic, etc.
    public bool IsSpecial { get; set; } = false;

    public List<Element> Elements { get; set; } = new List<Element>();

    public double CurrentHeightMm => Elements.Sum(e => e.Height);
    public double CurrentWeightKg => Elements.Sum(e => e.Weight);
    public double UsedArea => Elements.Sum(e => e.FootprintArea);
    public double FootprintArea => Length * Width;


}
