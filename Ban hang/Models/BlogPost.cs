namespace Ban_hang.Models;

public class BlogPost
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Tag { get; set; } = "";
    public string Title { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string ReadTime { get; set; } = "";
}
