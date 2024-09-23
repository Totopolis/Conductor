using Conductor.Server;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.PreBuild();
    var app = builder.Build();
    await app.PostBuild();
    app.Run();
}
catch
{
    // TODO: serilog to console
}
