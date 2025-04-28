using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ECommerceApp.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly ApplicationDbContext _sqlDbContext;

        public ProductService(IOptions<MongoDbSettings> mongoDbSettings, ApplicationDbContext sqlDbContext)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _productsCollection = mongoDatabase.GetCollection<Product>(mongoDbSettings.Value.ProductsCollectionName);
            _sqlDbContext = sqlDbContext;
        }

        public async Task<List<Product>> GetProductsAsync() =>
            await _productsCollection.Find(_ => true).ToListAsync();

        public async Task AddProductAsync(Product newProduct)
        {
            _sqlDbContext.Products.Add(newProduct);
            await _sqlDbContext.SaveChangesAsync();
            await _productsCollection.InsertOneAsync(newProduct);
        }

        public async Task AddProductToMongoOnlyAsync(Product newProduct)
        {
            await _productsCollection.InsertOneAsync(newProduct);
        }

        // Yeni eklenen metodlar
        public async Task<Product> GetProductAsync(string mongoId)
        {
            return await _productsCollection.Find(p => p.MongoId == mongoId).FirstOrDefaultAsync();
        }

        public async Task UpdateProductAsync(string mongoId, Product updatedProduct)
        {
            // MongoDB'de güncelle
            await _productsCollection.ReplaceOneAsync(
                p => p.MongoId == mongoId,
                updatedProduct);

            // SQL'de güncelle
            var sqlProduct = await _sqlDbContext.Products.FindAsync(updatedProduct.Id);
            if (sqlProduct != null)
            {
                sqlProduct.Name = updatedProduct.Name;
                sqlProduct.Description = updatedProduct.Description;
                sqlProduct.Price = updatedProduct.Price;
                await _sqlDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(string mongoId)
        {
            // Önce ürünü bul
            var product = await _productsCollection.Find(p => p.MongoId == mongoId).FirstOrDefaultAsync();

            if (product != null)
            {
                // MongoDB'den sil
                await _productsCollection.DeleteOneAsync(p => p.MongoId == mongoId);

                // SQL'den sil
                var sqlProduct = await _sqlDbContext.Products.FindAsync(product.Id);
                if (sqlProduct != null)
                {
                    _sqlDbContext.Products.Remove(sqlProduct);
                    await _sqlDbContext.SaveChangesAsync();
                }
            }
        }
    }
}
