using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Shared.Constants;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Eshop.Product.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<CreateProduct> _products;

        public ProductRepository(IMongoDatabase database)
        {
            _database = database;
            _products = _database.GetCollection<CreateProduct>(MongoDatabases.Products);
        }

        public async Task<ProductCreated> Upsert(CreateProduct product)
        {
            if(!string.IsNullOrEmpty(product.ProductId) && await IsDocumentUpdateRequest(product.ProductId))
            {
               await _products.ReplaceOneAsync(filter: p => p.ProductId == product.ProductId, replacement: product);
            }
            else
            {
                await _products.InsertOneAsync(product);
            }     
            return new ProductCreated
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,  
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<ProductCreated> GetProduct(string productId)
        {
            var product = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
            if(product == null) { return null;}
            return new ProductCreated
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<ProductCreated>> GetProductByPredicate(Expression<Func<CreateProduct, bool>> predicate)
        {
            FilterDefinition<CreateProduct> filter = Builders<CreateProduct>.Filter.Where(predicate);
            var products = await _products.Find(filter).ToListAsync();
            return products.Select(s => new ProductCreated
            {
                ProductId   = s.ProductId,
                ProductName = s.ProductName,
                CreatedAt   = DateTime.UtcNow
            });
        }
        public async Task<bool> DeleteProduct(string productId)
        {
            FilterDefinition<CreateProduct> filter = Builders<CreateProduct>.Filter.Eq(p => p.ProductId, productId);
            var deleteResult = await _products.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        private async Task<bool> IsDocumentUpdateRequest(string id)
        {
            var filter = Builders<CreateProduct>.Filter.Eq(p => p.ProductId, id);
            var product = await _products.Find(filter).FirstOrDefaultAsync();
            if(product == null) { return false;}
            return true;
        }
    }
}
