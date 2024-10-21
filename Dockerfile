FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet tool install -g nbgv \
    && cd src \
    && dotnet publish "Conductor.Server/Conductor.Server.csproj" \
       -c Release \
       -o /app/publish \
    && rm -f /app/publish/*.pdb \
    && rm -f /app/publish/Conductor.Server

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_EnableDiagnostics=0
ENV COMPlus_EnableDiagnostics=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENTRYPOINT ["dotnet", "Conductor.Server.dll"]
