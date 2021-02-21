using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPanel.SignalR
{
    public class Client
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
