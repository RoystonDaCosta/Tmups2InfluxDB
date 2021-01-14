FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY Tmups2InfluxDB.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/runtime:3.1

WORKDIR /app
COPY --from=build /app .

ENV TMUPS_IP "127.0.0.1"
ENV TMUPS_ID "Machine ID"
ENV INFLUXDB_IP "127.0.0.1"
ENV INFLUXDB_PORT "8086"
ENV INFLUXDB_DATABASE "database"
ENV INFLUXDB_USERNAME "username"
ENV INFLUXDB_PASSWORD "password"
ENV RETRY 10
ENV READING 5

ENTRYPOINT ["dotnet", "Tmups2InfluxDB.dll"]
