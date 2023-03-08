#!/bin/sh

set -e

echo ""
printf "Zonneplan account email: "
read email

printf "PvOutput API key       : "
read api_key

printf "PvOutput System ID     : "
read system_id

echo ""
echo "Configuring the system..."

echo "PvOutput__ApiKey=$api_key" > .env
echo "PvOutput__SystemId=$system_id" >> .env

# configure by connecting

if [ ! -d ./data ]; then
	mkdir -p ./data
	chown -R 1000:1000 ./data
	chmod 750 ./data
fi

tag="zonneconnect-cli"
docker rm -f "$tag"
docker build . --tag "$tag"
docker run -it --name "$tag" --user dotnetuser -v "$(pwd)/data:/app/data" --entrypoint dotnet "$tag" /app/ZonneConnect.dll connect "$email"
docker rm "$tag"

echo ""
echo "Starting container..."

bash ./run.sh
