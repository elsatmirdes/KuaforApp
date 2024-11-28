using Microsoft.AspNetCore.Mvc;

namespace KuaforApp.Models
{
    public class Salon
    {
        public int Id { get; set; }
        public string Name { get; set; } // Kuaför/Berber adı
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        //public List<Employee> Employees { get; set; } // Çalışanlar

    }
}
