using MongoDB.Bson; 
using MongoDB.Bson.Serialization.Attributes; 
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün alanı zorunludur.")]
        [Display(Name = "Ürün")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }
    }

}