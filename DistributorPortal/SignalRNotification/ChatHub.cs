using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Models.ViewModel;

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
