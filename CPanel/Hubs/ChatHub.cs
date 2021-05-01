using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using USQLCSharp.Models;


namespace CPanel.Hubs
{
    public class ChatHub : Hub
    {
        public HubConnection connection { get; set; } = new HubConnectionBuilder()
                    .WithUrl("http://176.36.127.144/Hub")
                    .WithAutomaticReconnect()
                   .Build();
        public async Task NewMessage(long username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);

        }
        public async Task Update(long id, string name)
        {
            await Clients.All.SendAsync("get", id, name);
        }
        public async Task Devices(string action, Device device)
        {
            await Clients.All.SendAsync("DevicesGet", action, device);
        }
        public async Task Parameters(string action, Parameter parameter)
        {
            await Clients.All.SendAsync("ParametersGet", action, parameter);
        }
    }
}
