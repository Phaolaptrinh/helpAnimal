namespace Ban_hang.Models;

public class PriceTier
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Note { get; set; } = "";
    public string DogPrice { get; set; } = "";
    public string CatPrice { get; set; } = "";
    public List<string> Features { get; set; } = new();
    public bool Featured { get; set; }
}
