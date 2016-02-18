﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.WebApi;
using Toolbox.ServiceAgents;
using SampleApi1.ServiceAgents;
using Toolbox.ServiceAgents.Settings;
using Toolbox.WebApi.CorrelationId;

namespace SampleApi1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build().ReloadOnChanged("appsettings.json");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationId();

            services.AddSingleServiceAgent<SampleApi2Agent>(settings =>
            {
                settings.Scheme = HttpSchema.Http;
                settings.Host = "localhost.fiddler";
                settings.Port = "5001";
                settings.Path = "api/";
            }, (serviceProvider, client) => client.SetCorrelationValues(serviceProvider));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseCorrelationId("SampleApi1");

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
