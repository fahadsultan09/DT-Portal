using Microsoft.AspNetCore.SignalR;
using Models.ViewModel;
using System.Threading.Tasks;

namespace DistributorPortal.SignalRNotification
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, SignalRResponse message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, message);
        }
    }
}
