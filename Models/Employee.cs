using Microsoft.AspNetCore.Mvc;

namespace KuaforApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
#pragma warning disable CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public string Name { get; set; } // Çalışan adı
#pragma warning restore CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
#pragma warning disable CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public string Specialty { get; set; } // Uzmanlık alanı
#pragma warning restore CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public bool IsAvailable { get; set; } // Müsaitlik
        public TimeOnly start_available { get; set; } // Müsaitlik
        public TimeOnly finish_available { get; set; } // Müsaitlik
        public int SalonId { get; set; } // Hangi salona ait
        public string Role { get; set; } = "E";
        public int userID { get; set; } = -1; 
    }
}
