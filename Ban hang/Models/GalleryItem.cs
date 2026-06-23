namespace Ban_hang.Models;

public class GalleryItem
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Alt { get; set; } = "";
    public string Caption { get; set; } = "";

    // "spa" | "vet" | "hotel" | "train" — used by the client-side filter buttons
    public string Category { get; set; } = "";

    // extra CSS classes for the grid: "" | "wide" | "tall" | "wide tall"
    public string LayoutClass { get; set; } = "";
}
