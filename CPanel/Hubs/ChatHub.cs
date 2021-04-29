using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;
using Microsoft.AspNetCore.SignalR.Client;


namespace CPanel.Hubs
{
    public class ChatHub : Hub
    {
        public HubConnection connection { get; set; } = new HubConnectionBuilder()
                    .WithUrl("https://176.36.127.144:5001/Hub")
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
