using CPanel.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;
using CPanel.MqttServer;
using System;
using MQTTnet.Client.Connecting;
using CPanel.Controllers;

namespace CPanel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddDbContext<PeopleContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/hub");

            });
            
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            Task.Run(StartMqtt);
            Task.Run(StartSignalR);
        }
        private async Task StartSignalR() {

            var chatHub = new ChatHub();
            do
            {
                await chatHub.connection.StartAsync();
            } while (chatHub.connection.State != HubConnectionState.Connected);
            

        }
        private async Task StartMqtt()
        {
            var mqttController = new MqttController();
            var mqttServerClient = new MqttServerClient();
            var ParametersList = mqttController.GetParameters();
            var topicList = new List<string>();
            await mqttServerClient.Connect(
                Configuration["Mqtt:ip"],
                Configuration["Mqtt:port"],
                Configuration["Mqtt:login"],
                Configuration["Mqtt:password"]);
            foreach (var parameter in ParametersList)
            {
                topicList.Add(parameter.Topic);
            }
            if (mqttServerClient.Auth.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new Exception(mqttServerClient.Auth.ResultCode.ToString());
            }
            else
            {
                await mqttServerClient.Subscribe(topicList);
            }
            await Task.Run(mqttServerClient.WaitForReciveMessage);
        }
    }
}
