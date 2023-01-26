using Moq;

namespace PuppetMaster.WebApi.Tests
{
    public class MoqBase : IDisposable
    {
        public MoqBase()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        protected MockRepository MockRepository { get; }

        public void Dispose()
        {
            MockRepository.VerifyAll();
        }
    }
}