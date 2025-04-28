using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string PasswordHash { get; set; } = string.Empty; 
    }
}
