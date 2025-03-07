using Conductor.Server;

// TODO: use static log for boot's errors
var builder = WebApplication.CreateBuilder(args);

builder.PreBuild();

var app = builder.Build();

await app.PostBuild();

await app.RunAsync();
