namespace CiscoWebexPresence.GUI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Core.Enrichers;
    using Serilog.Enrichers.AspNetCore.HttpContext;
    using DigitalGeniusTestAPI.Services.GorgiasApi;
    using DigitalGeniusTestAPI.Configuration;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Program class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static IConfiguration? configuration = null;

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                configuration = BuildConfiguration();
                CreateWebApplication(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Create an instance of the web application.
        /// </summary>
        /// <param name="args">An array of args.</param>
        /// <returns>An instance of <see cref="WebApplication"/>.</returns>
        public static WebApplication CreateWebApplication(string[] args)
        {
            try
            {
                Log.Logger = CreateLoggerForWebApp().CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log.Logger = CreateDefaultLogger().CreateLogger();
            }

            Log.Information("Starting CiscoWebexPresence web host...");
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddSerilog();
            ConfigureAllServices(builder.Services);
            return ConfigureApp(builder.Build());
        }

        /// <summary>
        /// Configures the web application.
        /// </summary>
        /// <param name="app">Web application to configure.</param>
        /// <returns>Configured web application.</returns>
        public static WebApplication ConfigureApp(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseSerilogLogContext(options =>
            {
                options.EnrichersForContextFactory = context => new[]
                {
                    new PropertyEnricher("TraceIdentifier", context.TraceIdentifier),
                };
            });
            
            return app;
        }

        /// <summary>
        /// Configures the necessary additions to the services of the web application i.e. new configuration for dependency injection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Configured service collection.</returns>
        public static IServiceCollection ConfigureAllServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add services to the container.
            IConfigurationSection apiOptionsSection = configuration?.GetSection(nameof(GorgiasApi)) ?? throw new Exception("Failed to get Gorgias API config.");

            services.AddSingleton<IGorgiasApiService, GorgiasApiService>()
            .AddOptions<GorgiasApi>().Bind(apiOptionsSection);
            services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull});

            services.AddHttpClient("GorgiasAPI", client =>
            {
                client.BaseAddress = apiOptionsSection.Get<GorgiasApi>().BaseUri;
            });

            // register http context accessor
            services.AddHttpContextAccessor();

            return services;
        }

        /// <summary>
        /// Builds the configuration.
        /// </summary>
        /// <returns>An instance of <see cref="IConfiguration"/>.</returns>
        public static IConfiguration BuildConfiguration()
        {
            string currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            IConfiguration newConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{currentEnvironment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            return newConfiguration;
        }

        /// <summary>
        /// Creates Logging Configuration for serilog.
        /// </summary>
        private static LoggerConfiguration CreateLoggerForWebApp()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration);
        }

        /// <summary>
        /// Configures the default logger.
        /// </summary>
        private static LoggerConfiguration CreateDefaultLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Async(asyncConfig =>
                {
                    string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceIdentifier}] [{ThreadId}] [{SourceContext}] {Message}{NewLine}{Exception}";
                    asyncConfig.Console(outputTemplate: outputTemplate);
                    asyncConfig.Debug(outputTemplate: outputTemplate);
                });
        }
    }
}
