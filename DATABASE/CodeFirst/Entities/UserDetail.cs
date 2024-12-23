using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class UserDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProfileImageUrl { get; set; }
        public User User { get; set; }
    }
}
