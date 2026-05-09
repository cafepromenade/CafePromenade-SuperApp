FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY src/CafePromenade.Core/CafePromenade.Core.csproj src/CafePromenade.Core/
COPY src/CafePromenade.Web/CafePromenade.Web.csproj src/CafePromenade.Web/
COPY src/CafePromenade.CLI/CafePromenade.CLI.csproj src/CafePromenade.CLI/
COPY src/CafePromenade.TUI/CafePromenade.TUI.csproj src/CafePromenade.TUI/
COPY CafePromenade-SuperApp.sln .

RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/CafePromenade.Web/CafePromenade.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y git curl && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt-get update \
    && apt-get install -y gh \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /app/repos

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5180
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5180

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5180/health || exit 1

ENTRYPOINT ["dotnet", "CafePromenade.Web.dll"]
