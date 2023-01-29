using Moq;
using PuppetMaster.WebApi.Repositories;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Tests.Services
{
    public class MatchesServiceTests : MoqBase
    {
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<IHubService> _hubServiceMock;

        private readonly MatchesService _service;

        public MatchesServiceTests()
        {
            _dbContextMock = MockRepository.Create<ApplicationDbContext>();
            _hubServiceMock = MockRepository.Create<IHubService>();

            _service = new MatchesService(_dbContextMock.Object, _hubServiceMock.Object);
        }
    }
}
