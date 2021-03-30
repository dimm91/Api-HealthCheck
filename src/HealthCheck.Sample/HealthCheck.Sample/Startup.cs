using HealthCheck.Sample.Data;
using HealthCheck.Sample.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace HealthCheck.Sample
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthCheck.Sample", Version = "v1" });
            });

            // Add the healthcheck middleware
            // You can add as many health check as your app may need
            // Some can be connection with your Database, external services, resource statuses, etc.
            services.AddHealthChecks()
                .AddCheck<EmailServiceProviderHealthCheck>("Email service provider HealthCheck")

                // This will check the connection with your database if it's successful, if is not (case if it doesn't exist)
                // Then it will return an unhealthy state
                // Required package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
                .AddDbContextCheck<AppDbContext>("Daabase HealtCheck");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration["ConnectionStrings:DefaultConnection"]);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCheck.Sample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // FYI: If at least one Health check is unhealthy the endpoint will return as Unhealthy
                endpoints.MapHealthChecks("Health", new HealthCheckOptions()
                {
                    //Here you can specify the status code the response will return per each 'HealthStatus'
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    
                    
                });
                endpoints.MapControllers();
            });
        }
    }
}
