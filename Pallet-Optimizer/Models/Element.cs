public class Element
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Depth { get; set; }
    public double WeightKg { get; set; }
    public bool CanRotate { get; set; } = true;
    public bool MustBeAlone { get; set; } = false;

    // for compatibility with your old optimizer code
    public (double Width, double Height, double Depth) Size => (Width, Height, Depth);
    public double FootprintArea => Width * Depth;
}