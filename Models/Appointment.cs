using Microsoft.AspNetCore.Mvc;

namespace KuaforApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int SalonId { get; set; } // Kuaför/Berber
        public int EmployeeId { get; set; } // Çalışan
#pragma warning disable CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public string Service { get; set; } // İşlem
#pragma warning restore CS8618 // Null atanamaz alan, oluşturucudan çıkış yaparken null olmayan bir değer içermelidir. Alanı null atanabilir olarak bildirmeyi düşünün.
        public DateTime Date { get; set; }// Tarih
        public TimeSpan Time { get; set; } // Saat

        public decimal Price { get; set; } // Ücret

        //onaylandımı onaylanmadımı -1 reddedildi 0 bekliyor 1 kabul edildi
        public int acceptAppointment { get; set; } = 0;

        // seçen user ıd sini tutmak için randevu için user ıd ekliyoruz
        public int UserId { get; set; } = -1;

    }
}
