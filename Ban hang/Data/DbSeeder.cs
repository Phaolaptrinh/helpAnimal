using Ban_hang.Models;
using Microsoft.AspNetCore.Identity;

namespace Ban_hang.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // ===== SEED ROLES =====
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // ===== SEED ADMIN USER =====
        string adminEmail = "admin@petlor.vn";
        string adminPassword = "Admin@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, adminPassword);
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ===== SEED DATA (giữ nguyên code cũ) =====
        if (!db.Services.Any())
        {
            db.Services.AddRange(
                new ServiceItem { IconKey = "spa", Title = "Spa & Cắt tỉa lông", Description = "Tắm gội, sấy khô, cắt tỉa lông và vệ sinh tai – móng bằng sản phẩm hữu cơ dịu nhẹ cho da.", Price = "Từ 150.000đ" },
                new ServiceItem { IconKey = "vet", Title = "Khám & Điều trị thú y", Description = "Khám tổng quát, tiêm phòng, xét nghiệm và điều trị bởi bác sĩ thú y giàu kinh nghiệm.", Price = "Từ 200.000đ" },
                new ServiceItem { IconKey = "hotel", Title = "Khách sạn thú cưng", Description = "Phòng lưu trú thoáng mát, camera theo dõi 24/7 và chế độ ăn riêng theo yêu cầu chủ nuôi.", Price = "Từ 120.000đ/đêm" },
                new ServiceItem { IconKey = "train", Title = "Huấn luyện hành vi", Description = "Khắc phục hành vi, dạy vâng lời cơ bản và nâng cao theo từng cá tính riêng của thú cưng.", Price = "Từ 350.000đ/buổi" },
                new ServiceItem { IconKey = "walk", Title = "Dắt thú cưng đi dạo", Description = "Lịch dạo bộ linh hoạt theo ngày hoặc theo tuần, có báo cáo lộ trình & hình ảnh sau buổi đi.", Price = "Từ 80.000đ/30 phút" },
                new ServiceItem { IconKey = "taxi", Title = "Đưa đón & Pet Taxi", Description = "Xe chuyên dụng đưa đón thú cưng đi khám, đi spa hoặc về nhà an toàn, đúng giờ hẹn.", Price = "Từ 50.000đ/km" }
            );
        }

        if (!db.TeamMembers.Any())
        {
            db.TeamMembers.AddRange(
                new TeamMember { Name = "BS. Trần Minh Anh", Role = "Trưởng khoa Thú y", Bio = "10 năm kinh nghiệm điều trị nội khoa & cấp cứu thú cưng.", AvatarSeed = "Tran Minh Anh", BgColor = "2F6F4E", TextColor = "ffffff" },
                new TeamMember { Name = "Nguyễn Hoài Thương", Role = "Chuyên viên Grooming", Bio = "Chuyên cắt tỉa tạo kiểu cho hơn 20 giống chó, mèo.", AvatarSeed = "Nguyen Hoai Thuong", BgColor = "F2A93B", TextColor = "1F2A24" },
                new TeamMember { Name = "Lê Quốc Bảo", Role = "Huấn luyện viên hành vi", Bio = "Chuyên khắc phục hành vi và huấn luyện vâng lời nâng cao.", AvatarSeed = "Le Quoc Bao", BgColor = "6E8FB3", TextColor = "ffffff" },
                new TeamMember { Name = "Phạm Thùy Linh", Role = "Quản lý chăm sóc khách hàng", Bio = "Đồng hành cùng bạn từ lúc đặt lịch đến khi bàn giao.", AvatarSeed = "Pham Thuy Linh", BgColor = "2F6F4E", TextColor = "ffffff" }
            );
        }

        if (!db.Testimonials.Any())
        {
            db.Testimonials.AddRange(
                new Testimonial { Quote = "Bé Mochi nhà mình rất nhút nhát nhưng các bạn ở Petlor rất kiên nhẫn.", Author = "Ngọc Mai", PetRelation = "Chủ nuôi bé Mochi", AvatarSeed = "Ngoc Mai", BgColor = "F2A93B", TextColor = "1F2A24" },
                new Testimonial { Quote = "Dịch vụ đón tận nhà siêu tiện, lại còn cập nhật hình ảnh liên tục qua Zalo.", Author = "Quang Huy", PetRelation = "Chủ nuôi bé Tom", AvatarSeed = "Quang Huy", BgColor = "6E8FB3", TextColor = "ffffff" },
                new Testimonial { Quote = "Bác sĩ tư vấn rất kỹ, giá cả minh bạch trước khi làm.", Author = "Thanh Tâm", PetRelation = "Chủ nuôi bé Sushi", AvatarSeed = "Thanh Tam", BgColor = "2F6F4E", TextColor = "ffffff" },
                new Testimonial { Quote = "Lớp huấn luyện hành vi giúp bé Lulu nhà mình bớt sủa khi có khách.", Author = "Bảo Trân", PetRelation = "Chủ nuôi bé Lulu", AvatarSeed = "Bao Tran", BgColor = "F2A93B", TextColor = "1F2A24" }
            );
        }

        if (!db.PriceTiers.Any())
        {
            db.PriceTiers.AddRange(
                new PriceTier { Name = "Cơ bản", Note = "Cho buổi chăm sóc hằng tuần", DogPrice = "150.000đ", CatPrice = "130.000đ", Featured = false },
                new PriceTier { Name = "Tiêu chuẩn", Note = "Lựa chọn của đa số khách hàng", DogPrice = "280.000đ", CatPrice = "250.000đ", Featured = true },
                new PriceTier { Name = "Cao cấp", Note = "Trải nghiệm spa trọn vẹn", DogPrice = "450.000đ", CatPrice = "400.000đ", Featured = false }
            );
        }

        if (!db.BlogPosts.Any())
        {
            db.BlogPosts.AddRange(
                new BlogPost { ImageUrl = "https://loremflickr.com/500/320/dog,sick/all?lock=401", Tag = "Sức khỏe", Title = "5 dấu hiệu cho thấy thú cưng cần đi khám ngay", Excerpt = "Nhận biết sớm các dấu hiệu bất thường.", ReadTime = "5 phút đọc" },
                new BlogPost { ImageUrl = "https://loremflickr.com/500/320/puppy,vaccine/all?lock=402", Tag = "Phòng bệnh", Title = "Lịch tiêm phòng cho chó mèo theo từng độ tuổi", Excerpt = "Tổng hợp đầy đủ các mốc tiêm phòng quan trọng.", ReadTime = "7 phút đọc" },
                new BlogPost { ImageUrl = "https://loremflickr.com/500/320/dog,food,bowl/all?lock=403", Tag = "Dinh dưỡng", Title = "Cách chọn thức ăn phù hợp theo cân nặng", Excerpt = "Hướng dẫn tính khẩu phần ăn hợp lý.", ReadTime = "4 phút đọc" }
            );
        }

        if (!db.FaqItems.Any())
        {
            db.FaqItems.AddRange(
                new FaqItem { Question = "Tôi cần đặt lịch trước bao lâu?", Answer = "Bạn nên đặt lịch trước ít nhất 2–4 giờ với dịch vụ spa." },
                new FaqItem { Question = "Petlor có dịch vụ đón tận nhà không?", Answer = "Có. Dịch vụ Pet Taxi đón và trả thú cưng tận nhà trong nội thành TP. Hồ Chí Minh." },
                new FaqItem { Question = "Thú cưng nhút nhát hoặc dữ có nhận chăm sóc không?", Answer = "Có. Đội ngũ Petlor được đào tạo để xử lý các bé nhút nhát." },
                new FaqItem { Question = "Có bác sĩ trực cấp cứu ngoài giờ không?", Answer = "Có, Petlor có bác sĩ trực cấp cứu 24/7 qua hotline 0901 234 567." },
                new FaqItem { Question = "Bảng giá đã bao gồm những gì?", Answer = "Giá niêm yết đã bao gồm nhân công và vật tư cơ bản." },
                new FaqItem { Question = "Tôi có thể theo dõi thú cưng khi gửi tại khách sạn không?", Answer = "Có, phòng lưu trú được trang bị camera và nhân viên gửi cập nhật qua Zalo mỗi ngày." }
            );
        }

        db.SaveChanges();
    }
}