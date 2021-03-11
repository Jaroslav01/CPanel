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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;

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
            Task.Run(Initialize);
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

        }
        public async Task Initialize()
        {
            var ip = "176.36.127.144";
            var port = "1883";
            var login = "yaroslav";
            var password = "220977qQ";
            var client = new MqttFactory().CreateMqttClient();
            var connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/Hub")
                    .WithAutomaticReconnect()
                   .Build();
            string topic = "yaroslav/Kitchen/output0";
            string type = "slider";
            string name = "Kitchen/output0";
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithTcpServer(ip, int.Parse(port))
                .WithCredentials(login, password)
                .WithProtocolVersion(MqttProtocolVersion.V311)
                .Build();
           var Auth = await client.ConnectAsync(options);
            client.SubscribeAsync();
            client.UseApplicationMessageReceivedHandler(async me =>
            {
                using var db = new PeopleContext();
                var msg = me.ApplicationMessage;
                var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);
                await connection.StartAsync();
                if (item != null)
                {
                    if (name == null) name = item.Name;
                    item.Name = name;
                    item.Type = type;
                    item.Data = Encoding.UTF8.GetString(msg.Payload);
                    await connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.Name, item.Topic, item.Data, item.Type);
                }
                else
                {
                    if (name == null) name = "Lamp";
                    var parameter = new Parameter
                    {
                        Name = name,
                        Type = type,
                        Data = Encoding.UTF8.GetString(msg.Payload),
                        Topic = msg.Topic
                    };
                    db.Add(parameter);
                    var item2 =  db.Parameters.Select(x => new Parameter
                    {
                        Id = x.Id,
                        Data = x.Data,
                        DeviseId = x.DeviseId,
                        Name = x.Name,
                        Topic = x.Topic,
                        Type = x.Type
                    }).ToList();
                    if (item2 != null)
                    {
                        for (int i = 0; i < item2.Count; i++)
                        {
                            if (item2[i].Topic == topic)
                            {
                                await connection.SendAsync("MqttSync", "add", item2[i].Id, item2[i].DeviseId, item2[i].Name, item2[i].Topic, item2[i].Data, item2[i].Type);
                            }
                        }
                    }
                }
                db.SaveChanges();
                //await client.DisconnectAsync();
                //await connection.StopAsync();
            });
        }
    }
}
