using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;

namespace SocketSignalExample.Hub
{
   
     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public ChatHub()
        {
            
        }
      
        public async Task SendMessage(string user, string message)
        {
            
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
