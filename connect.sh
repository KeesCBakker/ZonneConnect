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

# Get the UID and GID of the current user
export GID=$(id -g)
export UUID=$(id -g)

# configure by connecting
echo "Ensure data directory"
if [ ! -d "data" ]; then
  mkdir data
  chmod g+s data
fi

echo "Stopping container"
docker-compose down

tag="zonneconnect-cli"


# Clean up
docker rm -f "$tag"

# Build it
docker build . --tag "$tag" --build-arg UUID="${UUID}" --build-arg GID="${GID}"

# Add it
docker run -it --name "$tag" -v "$(pwd)/data:/app/data" --entrypoint dotnet "$tag" /app/ZonneConnect.dll connect "$email"

# Clean up
docker rm "$tag"

echo ""
echo "Starting container..."

cp ./data/token.json ./data/token-backup.json

bash ./run.sh
