using System.Threading.Tasks;

namespace Eshop.Infrastructure.Mongo.Interface
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}
