using Microsoft.AspNetCore.Mvc;

namespace KuaforApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } // Çalışan adı
        public string Specialty { get; set; } // Uzmanlık alanı
        public bool IsAvailable { get; set; } // Müsaitlik
        public int SalonId { get; set; } // Hangi salona ait
        public string Role { get; set; } = "E";


    }
}
