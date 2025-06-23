using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GearUp.Models
{
    public class NotificationHub : Hub
    {
        public async Task SendNewCarNotification(string category, object carHtml)
        {
            await Clients.All.SendAsync("ReceiveCarNotification", category, carHtml);
        }
    }
}
