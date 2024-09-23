using Conductor.Infrastructure;
using Conductor.Server;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddInfrastructureServices(builder.Configuration);

var consulSettings = builder.Configuration
    .GetSection(ConsulSettings.SectionName)
    .Get<ConsulSettings>()!;

builder.Configuration
    .AddConsul($"{consulSettings.ApplicationName}/{consulSettings.EnvironmentName}", opt =>
    {
        opt.ConsulConfigurationOptions = cco =>
        {
            cco.Address = new Uri(consulSettings.Uri);
            cco.Token = consulSettings.Token;
        };
    })
    .Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello world").WithOpenApi();

app.Run();
