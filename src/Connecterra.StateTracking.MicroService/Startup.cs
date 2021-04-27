using System;
using System.Linq;
using Connecterra.StateTracking.Common.Dispatcher;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.Common.Routing;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using Connecterra.StateTracking.MicroService.Projections;
using Connecterra.StateTracking.Persistent.Connection;
using Connecterra.StateTracking.Persistent.EventStore;
using Connecterra.StateTracking.Persistent.Projections;
using Connecterra.StateTracking.SnapshotStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Connecterra.StateTracking.MicroService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            //Service
            services.AddSingleton<IConfigurationLocator, ConfigurationLocator>();
            services.AddSingleton<IEventTypeResolver, EventTypeResolver>();

            //Database
            services.AddSingleton<IDbClient, CosmosDbClient>();
            services.AddScoped<IRepository<Cow>, CowRepository>();
            services.AddScoped<IRepository<Sensor>, SensorRepository>();
            services.AddScoped<IRepositorySnapshotDecorator<Cow>, CowRepositorySnapshotDecorator>();
            services.AddScoped<IRepositorySnapshotDecorator<Sensor>, SensorRepositorySnapshotDecorator>();

            //Snapshot-Event sourcing 
            services.AddScoped<ISnapshotStore, CosmosSnapshotStore>();

            //Events-Event sourcing 
            services.AddScoped<IEventStore, CosmosEventStore>();

            //Views-Event sourcing 
            services.AddSingleton<IViewRepository, CosmosViewRepository>();
            services.AddScoped<IView, CosmosView>();

            //Projections-Event sourcing 
            services.AddSingleton<IProjectionEngine, CosmosProjectionEngine>();
            services.AddTransient<CosmosProjection<NewSensorDeployedByYear>, NewSensorDeployedByYearProjection> ();
            services.AddTransient<CosmosProjection<PregnantCowDailyTotal>, PregnantCowDailyTotalProjection>();
            services.AddTransient<CosmosProjection<SensorDiedByMonth>, SensorDiedByMonthProjection>();

            //Routing-CQRS
            services.AddScoped<IRouter, Router>();
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            AddCommandQueryHandlers(services, typeof(ICommandHandler<>));
            AddCommandQueryHandlers(services, typeof(IQueryHandler<,>));

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "StateTracking API" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StateTracking API");
            });

            RegisterProjections(app, applicationLifetime);
        }
        private void AddCommandQueryHandlers(IServiceCollection services, Type handlerInterface)
        {
            var handlers = typeof(Startup).Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface)
            );

            foreach (var handler in handlers)
            {
                services.AddScoped(handler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface), handler);
            }
        }

        private void RegisterProjections(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            var projectionEngine = app.ApplicationServices.GetService(typeof(IProjectionEngine)) as IProjectionEngine;
           
            var newSensorDeployedByYear = app.ApplicationServices.GetService(typeof(CosmosProjection<NewSensorDeployedByYear>)) as CosmosProjection<NewSensorDeployedByYear>;
            projectionEngine.RegisterProjection(newSensorDeployedByYear);
            
            var pregnantCowDailyTotal = app.ApplicationServices.GetService(typeof(CosmosProjection<PregnantCowDailyTotal>)) as CosmosProjection<PregnantCowDailyTotal>;
            projectionEngine.RegisterProjection(pregnantCowDailyTotal);

            var sensorDiedByMonth = app.ApplicationServices.GetService(typeof(CosmosProjection<SensorDiedByMonth>)) as CosmosProjection<SensorDiedByMonth>;
            projectionEngine.RegisterProjection(sensorDiedByMonth);

            projectionEngine.StartAsync("StateTrackingInstance");
            applicationLifetime.ApplicationStopping.Register(() => OnShutdown(projectionEngine));
        }
        private void OnShutdown(IProjectionEngine projectionEngine)
        {
            projectionEngine.StopAsync();
        }
    }
}
