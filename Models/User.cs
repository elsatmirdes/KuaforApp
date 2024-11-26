using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KuaforApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad ve Soyad en fazla 100 karakter olabilir.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre doğrulama alanı zorunludur.")]
        [Compare("Password", ErrorMessage = "Şifre ve Şifre Doğrulama eşleşmiyor.")]
        public string ConfirmPassword { get; set; }


        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Role { get; set; } = "U";
        
    }
}
