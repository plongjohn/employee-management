using EmployeeManagement.Core.Data;
using EmployeeManagement.Core.Repositories;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Desktop.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EmployeeManagement.Desktop;

internal static class Program
{
    private const string LocalSettingsFile = "appsettings.Local.json";

    // Log files sit next to the executable. The path is fixed in code
    // because a relative path in the configuration would depend on the working directory,
    // which differs between Visual Studio, "dotnet run" and a double-click.
    private static readonly string LogFilePath =
        Path.Combine(AppContext.BaseDirectory, "logs", "employee-management-.log");

    private const int RetainedLogFiles = 14;
    private const string LogOutputTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Logs failures that happen before the configuration is loaded, e.g. a broken settings file.
        Log.Logger = WriteToLogFile(new LoggerConfiguration()).CreateBootstrapLogger();
        var log = Log.ForContext(typeof(Program));

        try
        {
            using var host = CreateHost();
            host.Start();
            log.Information("Application started");

            Application.Run(host.Services.GetRequiredService<MainForm>());

            host.StopAsync().GetAwaiter().GetResult();
            log.Information("Application stopped");
        }
        catch (Exception ex)
        {
            log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Configuration.AddJsonFile(LocalSettingsFile, optional: true, reloadOnChange: false);

        builder.Services.AddSerilog((_, logger) =>
            WriteToLogFile(logger.ReadFrom.Configuration(builder.Configuration)));

        AddCoreServices(builder.Services, builder.Configuration);
        builder.Services.AddTransient<MainForm>();

        return builder.Build();
    }

    private static void AddCoreServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
        services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
        services.AddSingleton<EmployeeValidator>();
        services.AddSingleton<IEmployeeService, EmployeeService>();
        services.AddSingleton<IDepartmentService, DepartmentService>();
    }

    private static LoggerConfiguration WriteToLogFile(LoggerConfiguration logger) =>
        logger.WriteTo.File(
            LogFilePath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: RetainedLogFiles,
            outputTemplate: LogOutputTemplate);
}
