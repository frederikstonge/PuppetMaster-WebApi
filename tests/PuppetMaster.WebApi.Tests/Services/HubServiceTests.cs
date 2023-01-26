using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PuppetMaster.WebApi.Hubs;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Tests.Services
{
    public class HubServiceTests : MoqBase
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHubContext<RoomHub>> _hubContextMock;
        private readonly Mock<IDelayedTasksService> _delayedTasksServiceMock;

        private readonly HubService _service;

        public HubServiceTests()
        {
            _mapperMock = MockRepository.Create<IMapper>();
            _hubContextMock = MockRepository.Create<IHubContext<RoomHub>>();
            _delayedTasksServiceMock = MockRepository.Create<IDelayedTasksService>();

            _service = new HubService(_mapperMock.Object, _hubContextMock.Object, _delayedTasksServiceMock.Object);
        }

        [Theory]
        [InlineData()]
        public async Task OnMatchChangedAsync()
        {
            // Test OnMatchChangedAsync
            await _service.OnMatchChangedAsync(match, room);
        }
    }
}
