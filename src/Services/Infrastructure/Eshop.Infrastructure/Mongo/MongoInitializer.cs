using Eshop.Infrastructure.Mongo.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Eshop.Infrastructure.Mongo
{
    public class MongoInitializer: IDatabaseInitializer
    {
        private bool _initilized;
        private readonly IMongoDatabase _database;  
        public MongoInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task InitializeAsync()
        {
            if (_initilized)
                return;
            var conventionPack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("Eshop", conventionPack , c => true);
            _initilized = true;
            await Task.CompletedTask;
        }
    }
}
