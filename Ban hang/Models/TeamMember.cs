namespace Ban_hang.Models;

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Bio { get; set; } = "";

    // Reuses the same DiceBear avatar trick as the static page.
    public string AvatarSeed { get; set; } = "";
    public string BgColor { get; set; } = "2F6F4E";
    public string TextColor { get; set; } = "ffffff";

    public string AvatarUrl =>
        $"https://api.dicebear.com/7.x/initials/svg?seed={Uri.EscapeDataString(AvatarSeed)}&backgroundColor={BgColor}&textColor={TextColor}";
}
