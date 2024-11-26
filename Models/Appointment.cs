using Microsoft.AspNetCore.Mvc;

namespace KuaforApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int SalonId { get; set; } // Kuaför/Berber
        public int EmployeeId { get; set; } // Çalışan
        public string Service { get; set; } // İşlem
        public DateTime Date { get; set; } // Tarih
        public TimeSpan Time { get; set; } // Saat

        public decimal Price { get; set; } // Ücret

        // seçen user ıd sini tutmak için randevu için user ıd ekliyoruz
        public int UserId { get; set; }

    }
}
