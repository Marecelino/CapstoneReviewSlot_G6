using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Persistence;
using Identity.Application.Abstractions.Security;
using System.Text;
using System.Globalization;

namespace Identity.Api.Extensions;

public static class DataSeeder
{
    public static void SeedLecturers(IdentityDbContext db, IPasswordHasherService passwordHasher)
    {
        var lecturers = new[]
        {
            "Nguyễn Trọng Tài", "Lâm Hữu Khánh Phương", "Phan Minh Tâm", "Nguyễn Minh Sang", 
            "Nguyễn Ngọc Lâm", "Lê Vũ Trường", "Đỗ Tấn Nhàn", "Tôn Thất Hoàng Minh", 
            "Bùi Thị Thu Thủy", "Trương Thị Mỹ Ngọc", "Ngô Đăng Hà An", "Vũ Thị Thùy Dương", 
            "Nguyễn Thị Cẩm Hương", "Trần Ngọc Như Quỳnh", "Phạm Thanh Trí", "Kiều Trọng Khánh", 
            "Lê Thị Quỳnh Chi", "Đỗ Phúc Thịnh", "Nguyễn Tấn Phúc", "Nguyễn Nguyên Bình", 
            "Lại Đức Hùng", "Trần Trọng Huỳnh", "Nguyễn Văn Chiến", "Võ Trần Duy", 
            "Mai Văn Duy", "Phan Thị Ngọc Hân", "Đặng Ngọc Minh Đức", "Thân Thị Ngọc Vân", 
            "Trương Long", "Lý Tuấn Anh"
        };

        foreach (var fullName in lecturers)
        {
            var email = GenerateEmail(fullName);
            var existingUser = db.Users.FirstOrDefault(u => u.Email == email);
            
            Guid userId;

            if (existingUser == null)
            {
                var user = User.Create(fullName, email, "", UserRole.Lecturer);
                user.PasswordHash = passwordHasher.HashPassword(user, "Lecturer@123");
                db.Users.Add(user);
                db.SaveChanges();
                userId = user.Id;
            }
            else
            {
                userId = existingUser.Id;
                // Update full name in case it was wrong
                existingUser.FullName = fullName;
                existingUser.Role = UserRole.Lecturer;
                db.SaveChanges();
            }

            // Ensure Lecturer profile exists
            if (!db.Lecturers.Any(l => l.UserId == userId))
            {
                var baseCode = GenerateLecturerCode(fullName);
                var lecturerCode = baseCode;
                int suffix = 1;
                
                while (db.Lecturers.Any(l => l.LecturerCode == lecturerCode))
                {
                    lecturerCode = $"{baseCode}{suffix}";
                    suffix++;
                }

                var lecturer = Lecturer.Create(userId, lecturerCode, "IT");
                db.Lecturers.Add(lecturer);
                db.SaveChanges();
            }
        }
    }

    private static string GenerateEmail(string fullName)
    {
        var normalized = RemoveDiacritics(fullName.ToLowerInvariant()).Replace(" ", "");
        return $"{normalized}@capstone.test";
    }

    private static string GenerateLecturerCode(string fullName)
    {
        var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return "LEC000";
        
        var code = RemoveDiacritics(words.Last()).ToUpperInvariant();
        for (int i = 0; i < words.Length - 1; i++)
        {
            if (words[i].Length > 0)
                code += char.ToUpperInvariant(RemoveDiacritics(words[i])[0]);
        }
        return code;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                // Convert typical Vietnamese chars like 'đ' to 'd'
                if (c == 'đ') stringBuilder.Append('d');
                else if (c == 'Đ') stringBuilder.Append('D');
                else stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
