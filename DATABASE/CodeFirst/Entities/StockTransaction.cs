using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class StockTransaction
    {
        public int ID { get; set; } // Benzersiz ID

        // Foreign Key: Hangi stok üzerinde işlem yapıldı
        public int StockID { get; set; }
        public Stock Stock { get; set; }

        // Kimden geldi / Kime gidiyor
        public int From { get; set; } // Supplier veya Warehouse ID olabilir
        public int To { get; set; }   // Supplier veya Warehouse ID olabilir

        public int Quantity { get; set; } // Taşınan miktar

        public DateTime TransactionDate { get; set; } // İşlem tarihi

        // Transaction Type: İşlem türünü belirler
        public TransactionType TransactionType { get; set; }
    }

    public enum TransactionType
    {
        /*Entry,
        Exit,
        Return*/
        Purchase,    // Satın alma işlemi
        Sale,        // Satış işlemi
        TransferIn,  // Depoya giriş
        TransferOut, // Depodan çıkış
        Adjustment   // Envanter ayarı
    }

}
