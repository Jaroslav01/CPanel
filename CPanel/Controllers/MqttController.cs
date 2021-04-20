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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace CPanel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class MqttController : ControllerBase
    {
        public IConfiguration _configuration { get; }
        private MqttServerClient _mqttServerClient;
        private readonly SignalRClient _signalRClient;

        public MqttController(MqttServerClient mqttServerClient, IConfiguration configuration, SignalRClient signalRClient)
        {
            _mqttServerClient = mqttServerClient;
            _configuration = configuration;
            _signalRClient = signalRClient;
        }
        [Authorize(Roles = "user")]
        [HttpPost("Set")]
        public async Task<IActionResult> Send(string topic, string value)
        {
            await _mqttServerClient.Send(topic, value);
            return Ok();
        }
        [Authorize(Roles = "user")]
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = new PeopleContext();
            var deleteOrderDetails = db.Parameters.Where(x => x.Id == id);
            foreach (var detail in deleteOrderDetails)
            {
                db.Parameters.Remove(detail);
                await _mqttServerClient.Unsubscribe(detail.Topic);
                await _signalRClient.connection.SendAsync("MqttSync", "delete", detail.Id, detail.DeviseId, detail.UserId, detail.Name, detail.Topic, detail.Data, detail.Type);
            }
            db.SaveChanges();
            return Ok();
        }
        [Authorize(Roles = "user")]
        [HttpPost("GetParameters")]
        public List<Parameter> GetParameters()
        {
            using var db = new PeopleContext();
            var user = db.Person.FirstOrDefault(x => x.Login == User.Identity.Name);
            var response = db.Parameters.Where(x => x.UserId == user.Id).ToList();
            return response;
        }
        [Authorize(Roles = "user")]
        [HttpPost("AddParameter")]
        public async Task<IActionResult> AddParameter(string name, string topic, string type)
        {
//            while (_signalRClient.connection.State != HubConnectionState.Connected) await _signalRClient.connection.StartAsync();
            using var db = new PeopleContext();
            var user = db.Person.FirstOrDefault(x => x.Login == User.Identity.Name);
            var parameter = new Parameter
            {
                Name = name,
                Topic = topic,
                Type = type, 
                UserId = user.Id
            };
            await db.AddAsync(parameter);
            await db.SaveChangesAsync();
            await _mqttServerClient.AddTopicsForSubscribe();
            var item = db.Parameters.FirstOrDefault(x => x.Topic == topic);
            await _signalRClient.connection.SendAsync("MqttSync", "add", item.Id, item.DeviseId, item.UserId, item.Name, item.Topic, item.Data, item.Type);
            return Ok();
        }
        [Authorize(Roles = "user")]
        [HttpPost("UpdateParameter")]
        public async Task<IActionResult> UpdateParameter(int id, string name, string type)
        {
            while (_signalRClient.connection.State != HubConnectionState.Connected) await _signalRClient.connection.StartAsync();
            using var db = new PeopleContext();
            var item = db.Parameters.FirstOrDefault(x => x.Id == id);
            item.Name = name;
            item.Type = type;
            await db.SaveChangesAsync();
            var topicList = new List<string>();
            topicList.Add(item.Topic);
            await _mqttServerClient.Subscribe(topicList);
            await _signalRClient.connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.UserId, item.Name, item.Topic, item.Data, item.Type);
            return Ok();
        }
    }
}
