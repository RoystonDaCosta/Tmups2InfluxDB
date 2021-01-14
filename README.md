#Generate docker image

docker build -t tmups2influxdb -f Dockerfile .

##For with config

docker run -it --rm -e TMUPS_IP=127.0.0.1 -e TMUPS_ID=ID -e INFLUXDB_IP=127.0.0.1 -e INFLUXDB_DATABASE=base -e INFLUXDB_USERNAME=name -e INFLUXDB_PASSWORD=pass tmups2influxdb:latest 

##Available config values and image defaults
TMUPS_IP "127.0.0.1"
TMUPS_ID "Machine ID"
INFLUXDB_IP "127.0.0.1"
INFLUXDB_PORT "8086"
INFLUXDB_DATABASE "database"
INFLUXDB_USERNAME "username"
INFLUXDB_PASSWORD "password"
RETRY 10
READING 5