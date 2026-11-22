using PowerPosition.WindowsService;
using Serilog;
using PowerPosition.Server;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<ApplicationSettings>(
    builder.Configuration.GetSection("ApplicationSettings"));

builder.Services.AddSerilog((services, configuration) =>
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

// Register application classes
builder.Services.AddTransient<IProcessor, Processor>();
builder.Services.AddTransient<IService, Service>();

var host = builder.Build();
host.Run();
