using CPanel.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MqttController mqttController = new MqttController();
            Task.Run(mqttController.UpdateDataAsync);

            CreateHostBuilder(args).Build().Run();
            
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
