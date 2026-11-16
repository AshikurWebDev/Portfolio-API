# Portfolio API image. Built by deploy/docker-compose.yml (build context: this api/ folder).
#
# Two stages:
#   1. "build" uses the full .NET SDK to restore and publish a Release build.
#   2. The final image has only the ASP.NET Core runtime, so it is much smaller.
#
# The runtime image is Debian/Ubuntu based (not Alpine) because SkiaSharp's
# Linux native files (SkiaSharp.NativeAssets.Linux.NoDependencies) need glibc.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, using only the project files, so Docker can cache the NuGet
# packages and skip this step when only .cs files change.
COPY Directory.Build.props ./
COPY src/Portfolio.Domain/Portfolio.Domain.csproj src/Portfolio.Domain/
COPY src/Portfolio.Application/Portfolio.Application.csproj src/Portfolio.Application/
COPY src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj src/Portfolio.Infrastructure/
COPY src/Portfolio.Api/Portfolio.Api.csproj src/Portfolio.Api/
RUN dotnet restore src/Portfolio.Api/Portfolio.Api.csproj

COPY src/ src/
RUN dotnet publish src/Portfolio.Api/Portfolio.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Listen on 8080 inside the container. Only Caddy (same Docker network) talks to it.
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Npgsql looks for the Kerberos (GSSAPI) library at startup and logs an error when it is missing.
# The API does not use Kerberos, but installing the small library keeps production logs clean.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

# The aspnet image ships a non-root "app" user; run as that, not root.
USER app
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
