using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;
using CPanel.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using CPanel.MqttServer;
using CPanel.SignalR;

namespace CPanel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MqttController : ControllerBase
    {
        HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/hub")
                .WithAutomaticReconnect()
                .Build();
        private MqttServerClient mqttServerClient;
        public MqttController(MqttServerClient mqttServerClient)
        {
            this.mqttServerClient = mqttServerClient;
        }
        [HttpGet("Set")]
        public async Task Send(string topic, string value)
        {
            await mqttServerClient.Send(topic, value);
        }
        [HttpGet("Delete")]
        public async void Delete(int id)
        {
            await connection.StartAsync();

            using var db = new PeopleContext();
            var deleteOrderDetails =
                from details in db.Parameters
                where details.Id == id
                select details;
            foreach (var detail in deleteOrderDetails)
            {
                db.Parameters.Remove(detail);
                //await connection.SendAsync("MqttSync", "delete", detail.Id, detail.DeviseId , detail.Name, detail.Topic, detail.Data, detail.Type);
            }
            db.SaveChanges();
        }
        [HttpGet("GetParameters")]
        public List<Parameter> GetParameters()
        {
            using var db = new PeopleContext();
            var response = db.Parameters.Select(x => new Parameter
            {
                Id = x.Id,
                Data = x.Data,
                DeviseId = x.DeviseId,
                Name = x.Name,
                Topic = x.Topic,
                Type = x.Type
            }).ToList();
            return response;
        }
        [HttpGet("AddParameter")]
        public async void AddParameter(string name, string topic, string type)
        {
            while (connection.State != HubConnectionState.Connected) await connection.StartAsync();
            var mqttServerClient = new MqttServerClient();
            using var db = new PeopleContext();
            var parameter = new Parameter
            {
                Name = name,
                Topic = topic,
                Type = type
            };
            await db.AddAsync(parameter);
            await db.SaveChangesAsync();
            await mqttServerClient.AddTopicsForSubscribe();
            var item = db.Parameters.FirstOrDefault(x => x.Topic == topic);
            await connection.SendAsync("MqttSync", "add", item.Id, item.DeviseId, item.Name, item.Topic, item.Data, item.Type);
        }
        [HttpGet("UpdateParameter")]
        public async void UpdateParameter(int id, string name, string type)
        {
            while (connection.State != HubConnectionState.Connected) await connection.StartAsync();
            var mqttServerClient = new MqttServerClient();
            using var db = new PeopleContext();
            var item = db.Parameters.FirstOrDefault(x => x.Id == id);
            item.Name = name;
            item.Type = type;
            await db.SaveChangesAsync();
            await mqttServerClient.AddTopicsForSubscribe();
            await connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.Name, item.Topic, item.Data, item.Type);
        }
    }
}
