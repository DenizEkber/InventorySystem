using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class UserDto
    {
        public int UserID { get; set; } // Benzersiz kullanıcı kimliği
        public string Name { get; set; } // Kullanıcı adı
        public string Email { get; set; } // Kullanıcı e-posta adresi
        public string Password { get; set; } // Şifre (hashlenmiş olmalı)
        public Role Role { get; set; } // "Supplier", "Warehouse" veya "Admin"
    }
}
