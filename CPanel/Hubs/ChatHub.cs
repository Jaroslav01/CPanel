using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;


namespace CPanel.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewMessage(long username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);

        }
        public async Task Update(long id, string name)
        {
            await Clients.All.SendAsync("get", id, name);

        }
        public async Task MqttSync(List<Parameter> parameters)
        {
            if (parameters != null && Clients != null )
            {
                await Clients.All.SendAsync("mqttsyncres", parameters);
            }
        }
    }
}
