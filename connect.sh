#!/bin/sh

set -e

echo ""
printf "Zonneplan account email     : "
read email

if [ ! -f ".env" ]; then

	printf "PvOutput API key            : "
	read api_key

	printf "PvOutput System ID          : "
	read system_id

	printf "Timezone (Europe/Amsterdam) : "
	read tz

	echo ""
	echo "Configuring the system..."

	echo "PvOutput__ApiKey=$api_key"		> .env
	echo "PvOutput__SystemId=$system_id"	>> .env
	echo "TZ=$tz"							>> .env

fi

# configure by connecting
mkdir -p ./data

echo "Stopping container"
docker-compose down

tag="zonneconnect-cli"
docker rm -f "$tag"
docker build . --tag "$tag"
docker run -it --name "$tag" -v "$(pwd)/data:/app/data" --entrypoint dotnet "$tag" /app/ZonneConnect.dll connect "$email"
docker rm "$tag"

echo ""
echo "Starting container..."

cp ./data/token.json ./data/token-backup.json

bash ./run.sh
