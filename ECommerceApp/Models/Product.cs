using MongoDB.Bson; // MongoDB.Driver paketinden
using MongoDB.Bson.Serialization.Attributes; // MongoDB.Driver paketinden
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Product
    {
        // MongoDB için _id alanı (Otomatik oluşur ama maplemek iyidir)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; } // MongoDB'nin kendi ID'si (string olarak kullanmak daha kolay olabilir)

        // MSSQL için Id alanı (Primary Key)
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        // MongoDB'de olup MSSQL'de olmayan veya farklı olan alanlar buraya eklenebilir.
        // Bu basit örnekte aynı yapıyı kullanacağız.
    }
}