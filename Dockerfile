FROM mcr.microsoft.com/dotnet/runtime:3.1

COPY bin/Release/netcoreapp3.1/publish/ App/

WORKDIR /App

ENV TMUPS_IP "127.0.0.1"
ENV TMUPS_ID "Machine ID"
ENV INFLUXDB_IP "127.0.0.1"
ENV INFLUXDB_PORT "8086"
ENV INFLUXDB_DATABASE "database"
ENV INFLUXDB_USERNAME "username"
ENV INFLUXDB_PASSWORD "password"

ENTRYPOINT ["dotnet", "Tmups2InfluxDB.dll"]
