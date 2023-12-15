#!/bin/sh
export DOCKER_SCAN_SUGGEST=false

if [ ! -f "data/token.json" ] || [ ! -f ".env" ]; then
    echo "Please run ./connect.sh to configure the system."
    exit 1
fi

function echo_title {
  line=$(echo "$1" | sed -r 's/./-/g')
  printf "\n$line\n$1\n$line\n\n"
}

export GID=$(id -g)
export UUID=$(id -g)

echo_title "Pulling latest source"
git pull
git log --pretty=oneline -1

echo_title "Stopping container"
docker-compose down

chmod g+s data

echo_title "Starting container"
docker-compose up --build -d --remove-orphans

echo_title "Finished"
