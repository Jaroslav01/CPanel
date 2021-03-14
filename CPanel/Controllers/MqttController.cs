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

namespace CPanel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MqttController : ControllerBase
    {
        private ChatHub chatHub = new ChatHub();
        private MqttServerClient mqttServerClient = new MqttServerClient();
        [HttpGet("Set")]
        public async Task Send(string topic, string value)
        {
            await mqttServerClient.Send(topic, value);
        }
        [HttpGet("Delete")]
        public async void Delete(int? id)
        {
            await chatHub.connection.StartAsync();

            using var db = new PeopleContext();
            var deleteOrderDetails =
                from details in db.Parameters
                where details.Id == id
                select details;
            foreach (var detail in deleteOrderDetails)
            {
                db.Parameters.Remove(detail);
            }
            db.SaveChanges();
            await chatHub.connection.SendAsync("MqttSync", "delete", id, null, "null", "null", "null");
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
        [HttpGet("GetDevices")]
        public List<Device> GetDevices()
        {
            using var db = new PeopleContext();
            return db.Devices.Select(x => new Device
            {
                Id = x.Id,
                Name = x.Name,
                Topic = x.Topic,
                Ip = x.Ip,
                Mac = x.Mac,
                Rssi = x.Rssi,
                Uptime = x.Uptime,
                State = x.State
            }).ToList();
        }
    }
}
