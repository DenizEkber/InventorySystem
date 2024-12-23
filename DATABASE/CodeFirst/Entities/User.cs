
namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class User
    {
        public int UserID { get; set; } // Benzersiz kullanıcı kimliği
        public string Name { get; set; } // Kullanıcı adı
        public string Email { get; set; } // Kullanıcı e-posta adresi
        public string Password { get; set; } // Şifre (hashlenmiş olmalı)
        public Role Role { get; set; } // "Supplier", "Warehouse" veya "Admin"
        public UserDetail UserDetail { get; set; }
    }
    public enum Role
    {
        Supplier,
        Warehouse,
        Admin
    }

}
