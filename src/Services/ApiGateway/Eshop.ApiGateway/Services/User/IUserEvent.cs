using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Common;
using MassTransit;

namespace Eshop.ApiGateway.Services.User
{
    public interface IUserEvent
    {
        Task PublishUpsertEventAsync(CreateUser request);
        // Task<Response<Result<UserCreated>>> ConsumeProductAsync(string userId);
        // Task PublishDeleteEventAsync(string userId);
    }
}