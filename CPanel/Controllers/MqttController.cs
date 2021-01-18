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

namespace CPanel.Controllers
{
    public class MqttController : Controller
    {

        public IMqttClient Client { get; private set; }
        public MqttClientAuthenticateResult Auth { get; private set; }

        public async Task Connect(string ip, string port, string login, string password)
        {
            IMqttClient client = new MqttFactory().CreateMqttClient();
            Client = client;
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithTcpServer(ip, int.Parse(port))
                .WithCredentials(login, password)
                .WithProtocolVersion(MqttProtocolVersion.V311)
                .Build();

            Auth = await client.ConnectAsync(options);
        }

        public async Task Send(string topic, string data)
        {
            await Client.PublishAsync(topic, data);
        }
        [HttpGet]
        public async Task<IEnumerable<Mqtt>> Subscribe(string topic)
        {

            MqttClientSubscribeResultItem result = (await Client.SubscribeAsync(
                        new TopicFilterBuilder()
                        .WithTopic(topic)
                        .Build()
                    )).Items[0];
            switch (result.ResultCode)
            {
                case MqttClientSubscribeResultCode.GrantedQoS0:
                case MqttClientSubscribeResultCode.GrantedQoS1:
                case MqttClientSubscribeResultCode.GrantedQoS2:
                    IMqttClient mqttClient = Client.UseApplicationMessageReceivedHandler(me => { var msg = me.ApplicationMessage; });
                    return Enumerable.Range(1, 1).Select(index => new Mqtt
                    {
                        Msg = Client.Options.WillMessage.Topic,
                        Data = Encoding.UTF8.GetString(Client.Options.WillMessage.Payload)
                    }).ToArray();
            
                default:
                    throw new Exception(result.ResultCode.ToString());
            }
        }
    }
}
