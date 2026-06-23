namespace Ban_hang.Models;

// The single object the HomeController hands to Views/Home/Index.cshtml.
// Everything that used to be hard-coded markup in the HTML is now data
// living on this view model — the view's job is only to loop and render it.
public class HomeViewModel
{
    public int Id { get; set; }
    public List<ServiceItem> Services { get; set; } = new();
    public List<TeamMember> Team { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = new();
    public List<PriceTier> PriceTiers { get; set; } = new();
    public List<GalleryItem> Gallery { get; set; } = new();
    public List<BlogPost> BlogPosts { get; set; } = new();
    public List<FaqItem> Faqs { get; set; } = new();

    // The booking form is rendered inside this same page, so its
    // (possibly invalid, re-displayed) state travels along on the model.
    public BookingViewModel Booking { get; set; } = new();
}
