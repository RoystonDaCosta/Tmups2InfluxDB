#Generate docker image

dotnet restore

dotnet publish -c Release

docker build -t tmups2influxdb -f Dockerfile .

##For with config

docker run -it --rm -e TMUPS_IP=127.0.0.1 -e TMUPS_ID=ID -e INFLUXDB_IP=127.0.0.1 -e INFLUXDB_DATABASE=base -e INFLUXDB_USERNAME=name -e INFLUXDB_PASSWORD=pass tmups2influxdb:latest 
