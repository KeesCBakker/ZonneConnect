using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using ZonneConnect.CommandLine;
using ZonneConnect.PvOutput;
using ZonneConnect.Zonneplan;

static void ConfigureServices(IServiceCollection services)
{
    // build config
    var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    // settings
    services.Configure<PvOutputSettings>(configuration.GetSection("PvOutput"));

    // add commands:
    services.AddTransient<Command, CurrentCommand>();
    services.AddTransient<Command, ConnectCommand>();
    services.AddTransient<Command, MeCommand>();

    // add services:
    services.AddTransient<ZonneplanApi>();
    services.AddTransient<PvOutputClient>();
    services.AddHttpClient();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

// entry to run app
var commands = serviceProvider.GetServices<Command>();
var rootCommand = new RootCommand("Connects with the Zonneplan API and writes the output settings to PVOutput.");
commands.ToList().ForEach(command => rootCommand.AddCommand(command));

await rootCommand.InvokeAsync(args);
