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

        public async Task<Product> GetProductAsync(string mongoId)
        {
            return await _productsCollection.Find(p => p.MongoId == mongoId).FirstOrDefaultAsync();
        }

        public async Task UpdateProductAsync(string mongoId, Product updatedProduct)
        {
            try
            {
                var existingProduct = await _productsCollection.Find(p => p.MongoId == mongoId).FirstOrDefaultAsync();
                if (existingProduct == null)
                {
                    throw new Exception("Ürün bulunamadı");
                }

                var filter = Builders<Product>.Filter.Eq(p => p.MongoId, mongoId);
                var update = Builders<Product>.Update
                    .Set(p => p.Name, updatedProduct.Name)
                    .Set(p => p.Description, updatedProduct.Description)
                    .Set(p => p.Price, updatedProduct.Price);

                await _productsCollection.UpdateOneAsync(filter, update);

                var sqlProduct = await _sqlDbContext.Products.FindAsync(updatedProduct.Id);
                if (sqlProduct != null)
                {
                    sqlProduct.Name = updatedProduct.Name;
                    sqlProduct.Description = updatedProduct.Description;
                    sqlProduct.Price = updatedProduct.Price;
                    await _sqlDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ürün güncellenirken bir hata oluştu: {ex.Message}");
            }
        }


        public async Task DeleteProductAsync(string mongoId)
        {
           
            var product = await _productsCollection.Find(p => p.MongoId == mongoId).FirstOrDefaultAsync();

            if (product != null)
            {
                
                await _productsCollection.DeleteOneAsync(p => p.MongoId == mongoId);

                
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
