using Microsoft.AspNetCore.SignalR;
using PuppetMaster.WebApi.Attributes;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Messages;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Hubs
{
    [CustomAuthorize]
    public class RoomHub : Hub
    {
        private readonly IAccountService _accountService;

        public RoomHub(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _accountService.GetUserAsync(Context.User!);
            var roomId = user!.RoomUser!.RoomId;
            var groupId = roomId!.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            var message = new ChatMessage()
            {
                From = "Console",
                Message = $"{user.UserName} has joined the room."
            };

            await SendChatMessage(groupId, message);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _accountService.GetUserAsync(Context.User!);
            var roomId = user!.RoomUser!.RoomId;
            var groupId = roomId!.ToString();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            var message = new ChatMessage()
            {
                From = "Console",
                Message = $"{user.UserName} has left the room."
            };

            await SendChatMessage(groupId, message);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task ChatMessage(ChatMessage message)
        {
            var user = await _accountService.GetUserAsync(Context.User!);
            var roomId = user!.RoomUser!.RoomId;
            var groupId = roomId!.ToString();
            message.From = user!.UserName;
            await SendChatMessage(groupId, message);
        }

        private Task SendChatMessage(string groupId, ChatMessage message)
        {
            return Clients.Group(groupId).SendAsync(SignalRMethods.ChatMessage, message);
        }
    }
}
