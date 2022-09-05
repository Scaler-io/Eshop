using AutoMapper;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Constants;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Eshop.User.DataAccess.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<CreateUser> _users;

        public UserRepository(IMongoDatabase database, IMapper mapper)
        {
            _database = database;
            _users = _database.GetCollection<CreateUser>(MongoDatabases.Users);
            _mapper = mapper;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            FilterDefinition<CreateUser> filter = Builders<CreateUser>.Filter.Eq(u => u.UserId, userId);
            var deleteResult = await _users.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<UserCreated> GetUser(string userId)
        {
            var user = await _users.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            if (user == null) return null;
            var userCreated = _mapper.Map<UserCreated>(user);
            return userCreated;
        }

        public async Task<UserCreated> UpsertUser(CreateUser user)
        {
            if (!string.IsNullOrEmpty(user.UserId) && await IsDocumentUpdateRequest(user.UserId))
                await _users.ReplaceOneAsync(filter: p => p.UserId == user.UserId, replacement: user);
            else
                await _users.InsertOneAsync(user);
            var productCreated = _mapper.Map<UserCreated>(user);
            return productCreated;
        }

        private async Task<bool> IsDocumentUpdateRequest(string id)
        {
            var filter = Builders<CreateUser>.Filter.Eq(p => p.UserId, id);
            var product = await _users.Find(filter).FirstOrDefaultAsync();
            if (product == null) { return false; }
            return true;
        }
    }
}
