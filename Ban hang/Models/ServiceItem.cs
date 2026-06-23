namespace Ban_hang.Models;

// One card in the "Dịch vụ" (Services) grid.
// IconKey picks which inline SVG the _ServiceCard partial draws —
// see Views/Shared/_Icons.cshtml for the icon-key switch.
public class ServiceItem
{
    public int Id { get; set; }
    public string IconKey { get; set; } = "spa";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Price { get; set; } = "";
}
