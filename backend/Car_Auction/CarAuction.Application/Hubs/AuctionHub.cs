using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CarAuction.Application.Hubs
{
    [Authorize]
    public class AuctionHub : Hub
    {
        public async Task JoinVehicleGroup(string vehicleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"vehicle:{vehicleId}");
        }
        
        public async Task LeaveVehicleGroup(string vehicleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vehicle:{vehicleId}");
        }
        
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.Name;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var userId = Context.User.Identity.Name;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}