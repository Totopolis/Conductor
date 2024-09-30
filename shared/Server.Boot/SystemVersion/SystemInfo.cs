using Microsoft.Extensions.Hosting;
using Server.Abstractions;

namespace Server.Boot.SystemVersion;

/*
Add to MyApp.Server.csproj file this code:
 	<ItemGroup>
		<AssemblyAttribute Include="Server.Boot.SystemVersion.ReleaseDate">
			<_Parameter1>$([System.DateTime]::UtcNow.ToString("O"))</_Parameter1>
		</AssemblyAttribute>
	</ItemGroup>
*/
internal sealed class SystemInfo : ISystemInfo
{
    private readonly IHostEnvironment _environment;

    public SystemInfo(IHostEnvironment hostEnvironment)
    {
        _environment = hostEnvironment;
        StartDateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Source code build time.
    /// </summary>
    public DateTime BuildDateTime => ReleaseDateAttribute.GetReleaseDate();

    /// <summary>
    /// When system started.
    /// </summary>
    public DateTime StartDateTime { get; init; }

    public bool IsDevelopment => _environment.IsDevelopment();
}
