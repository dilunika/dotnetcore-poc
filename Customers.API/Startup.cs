using System;
using AutoMapper;
using Customers.API.Data;
using Customers.API.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace Customers.API
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
            services
                .AddControllers(EnableNotAcceptableResponse())
                .AddNewtonsoftJson(ConfiguJsonSerializer())
                .ConfigureApiBehaviorOptions(GetApiBehaviourOptions());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            string connectionString = BuildDatabaseConnectionString();
            services.AddDbContext<CustomerPortalContext>(opt => opt.UseNpgsql(connectionString));

            services.AddScoped<ICustomerRepository, RelationalCustomerRepository>();
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
                app.UseExceptionHandler(DefaultServerErrorBuilder());
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        private string BuildDatabaseConnectionString()
        {
            string databaseHost = Configuration.GetValue<string>("DB_HOST");
            string databaseUsername = Configuration.GetValue<string>("DB_USERNAME");
            string databasePassword = Configuration.GetValue<string>("DB_PASSWORD");

            var connectionString = $"Host={databaseHost};Database=dh_customer_portal;Username={databaseUsername};Password={databasePassword}";

            return connectionString;
        }

        static Action<MvcOptions> EnableNotAcceptableResponse()
        {
            return setupAction => { setupAction.ReturnHttpNotAcceptable = true; };
        }

        static Action<ApiBehaviorOptions> GetApiBehaviourOptions()
        {
            return setupActions =>
            {
                setupActions.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    problemDetails.Detail = "See the erros field for details";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    if (ErrorsAreValodationRelated(context))
                    {
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occurred.";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occurred.";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            };
        }

        static Action<MvcNewtonsoftJsonOptions> ConfiguJsonSerializer()
        {
            return setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            };
        }

        private static bool ErrorsAreValodationRelated(ActionContext context)
        {
            var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

            return context.ModelState.ErrorCount > 0
                && (context is ControllerContext
                || actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count);
        }

        static Action<IApplicationBuilder> DefaultServerErrorBuilder()
        {
            return builder =>
            {
                builder.Run(async (context) =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                });
            };
        }
    }
}
