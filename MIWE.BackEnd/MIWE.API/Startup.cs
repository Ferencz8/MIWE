using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MIWE.Core;
using MIWE.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProtoBuf.Grpc.Server;
using Serilog;
using MIWE.Data.Services;
using Microsoft.EntityFrameworkCore;
using MIWE.Data;
using MIWE.Data.Services.Interfaces;
using MIWE.API.HostedServices;
using MIWE.Core.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System.IO;
using System.Reflection;
using System.Threading;
using MIWE.Common;

namespace MIWE.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        private Microsoft.Extensions.Logging.ILogger _logger;

        public IConfiguration Configuration { get; }

        //https://thinkrethink.net/2017/03/09/application-shutdown-in-asp-net-core/
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            //for code 1st
            services.AddCodeFirstGrpc(options => options.EnableDetailedErrors = true);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(n => n.EnableEndpointRouting = false);

            var connString = Configuration.GetSection("DbConn")?.Value;
            services.AddDbContext<WorkerContext>(options => options.UseSqlServer(connString));
            services.AddHealthChecks().AddSqlServer(connString);
            services.AddHealthChecksUI().AddInMemoryStorage();

            services.AddSwaggerGen(config =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });

            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobExecuter, JobExecuter>();
            services.AddScoped<IInstanceService, InstanceService>();
            services.AddScoped<IPluginRunner, PluginRunner>();
            services.AddScoped<IWorkloadAnalyzer, WorkloadAnalyzer>();

            services.AddScoped<IAzureBlobRepository, AzureBlobRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobSessionRepository, JobSessionRepository>();
            services.AddScoped<IJobScheduleRepository, JobScheduleRepository>();
            services.AddScoped<IInstanceRepository, InstanceRepository>();
            services.AddSingleton<TaskSettings, TaskSettings>();
            services.AddSingleton<JobRunnerBackgroundTask, JobRunnerBackgroundTask>();
            

            Initialize(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MIWE API V1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            SetupLogging(loggerFactory);

            var instance = serviceProvider.GetService<IInstanceService>();
            var jobService = serviceProvider.GetService<IJobService>();

            applicationLifetime.ApplicationStopping.Register(() => OnShutdown(instance, jobService));

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseMvc();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<JobReceiver>();
            });
            app.UseHealthChecks("/hc", new HealthCheckOptions() {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/hc-ui";
            });
        }

        private void SetupLogging(ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            loggerFactory.AddSerilog();

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        private void Initialize(IServiceCollection services)
        {
            DecideMasterInstance(services);
        }

        private void DecideMasterInstance(IServiceCollection services)
        {
            var instanceService = services.BuildServiceProvider().GetService<IInstanceService>();
            var instanceRepo = services.BuildServiceProvider().GetService<IInstanceRepository>();
            bool isMasterRegistered = instanceService.IsMasterRegistered(); 
            
            if (isMasterRegistered)
            {
                var masterInstance = instanceService.GetMasterInstance();
                string currentIp = HttpHelper.GetCurrentExternalIP();
                if (masterInstance.IP == currentIp)
                {
                    Console.WriteLine("This instance will run as a Master");
                    instanceService.RegisterInstance(true);
                    services.AddHostedService<JobRunnerBackgroundTask>();
                }
                else
                {
                    Console.WriteLine("This instance will run as Slave");
                    instanceService.RegisterInstance(false);
                    services.AddHostedService<SelfRecoveryBackgroundTask>();
                }                
            }
            else
            {
                Console.WriteLine("This instance will run as a Master");
                instanceService.RegisterInstance(true);
                services.AddHostedService<JobRunnerBackgroundTask>();
            }
        }

        private void OnShutdown(IInstanceService instanceService, IJobService jobService)
        {
            _logger.Log(LogLevel.Warning, "Application shutdown initiated");

            instanceService.MarkInstanceDown(true);

            //mark currently running jobs from this instance as stopped
            jobService.MarkStopped(instanceService.GetCurrentInstanceId());

            _logger.Log(LogLevel.Warning, "Instance marked as down");
        }
    }
}
