FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /build
COPY src/ .
RUN dotnet restore Server.csproj
RUN dotnet build -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=builder /app/build .
COPY appsettings.json .

EXPOSE 22005

ENTRYPOINT ["dotnet", "Server.dll"]