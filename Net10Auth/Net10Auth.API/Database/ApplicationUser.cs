using Microsoft.AspNetCore.Identity;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Net10Auth.API.Database
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }

    
}