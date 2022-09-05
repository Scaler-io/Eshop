using System;
using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using Eshop.Shared.Extensions;
using MassTransit;
using Serilog;

namespace Eshop.ApiGateway.Services.User
{
    public class UserEvent : IUserEvent
    {
        private readonly ILogger _logger;
        private readonly IBusControl _bus;

        public UserEvent(ILogger logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        // public Task<Response<Result<UserCreated>>> ConsumeProductAsync(string userId)
        // {
        //     throw new System.NotImplementedException();
        // }

        // public Task PublishDeleteEventAsync(string userId)
        // {
        //     throw new System.NotImplementedException();
        // }

        public async Task PublishUpsertEventAsync(CreateUser request)
        {
            _logger.Here().MethodEnterd();
            var endpoint = await GetEventBusURI(EventBusQueueNames.UserQueueNames.CreateOrUpdate);
            await endpoint.Send(request);
            _logger.Here().MethodExited();
        }
        private async Task<ISendEndpoint> GetEventBusURI(string queue)
        {
            var uri = new Uri($"rabbitmq://localhost/{queue}");
            return await _bus.GetSendEndpoint(uri);
        }
    }
}