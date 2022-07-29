using Microsoft.AspNetCore.SignalR;

namespace PuppetMaster.WebApi.Providers
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.GetUserId().ToString();
        }
    }
}
