using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace CPanel.SignalR
{
    public class SignalRClient
    {
        public readonly HubConnection connection = new HubConnectionBuilder()
                      .WithUrl("https://localhost:5001/Hub")
                      .WithAutomaticReconnect()
                     .Build();
        public async Task Start()
        {
            await connection.StartAsync();
        }
    }
}
