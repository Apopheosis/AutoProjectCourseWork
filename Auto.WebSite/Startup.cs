using Auto.Data;
using System.Reflection;
using System.Reflection.Metadata;
using Auto.API.Hubs;
using EasyNetQ;
using GraphQL;
using GraphQL.Types;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auto.Website {
    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddTransient<IAutoStorage, AutoCsvFileStorage>();
            services.AddSignalR();

            services.AddGraphQL(builder => builder
                    .AddNewtonsoftJson()                
                    .AddAutoSchema<AutoSchema<ISchema>>()        
                    .AddSchema<AutoSchema<ISchema>>()           
            );
            
            var bus = RabbitHutch.CreateBus(Configuration.GetConnectionString("AutoRabbitMQ"));
            services.AddSingleton<IBus>(bus);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseGraphQL<AutoSchema<ISchema>>();
            app.UseGraphQLGraphiQL("/graphiql");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AutoHub>("/hub");
                endpoints.MapControllerRoute(
                    name: "website",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}