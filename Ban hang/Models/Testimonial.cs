namespace Ban_hang.Models;

public class Testimonial
{
    public int Id { get; set; }
    public string Quote { get; set; } = "";
    public string Author { get; set; } = "";
    public string PetRelation { get; set; } = "";
    public string AvatarSeed { get; set; } = "";
    public string BgColor { get; set; } = "F2A93B";
    public string TextColor { get; set; } = "1F2A24";

    public string AvatarUrl =>
        $"https://api.dicebear.com/7.x/initials/svg?seed={Uri.EscapeDataString(AvatarSeed)}&backgroundColor={BgColor}&textColor={TextColor}";
}
