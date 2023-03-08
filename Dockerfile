########################
# Configuration settings
########################

ARG API_NAME="ZonneConnect"
ARG \
   API_PROJECT="./src/$API_NAME/$API_NAME.csproj" \
   API_DLL="./$API_NAME.dll" \
   NET_VERSION="6.0" \
   # options: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
   # minimal keeps your pipeline readable while inforing you what's going on
   VERBOSITY="minimal" \
   APP_DIR="/app"

#############################################
# Build image, uses NET_VERSION, API_PROJECT, 
# VERBOSITY, APP_DIR
#############################################

FROM mcr.microsoft.com/dotnet/sdk:$NET_VERSION AS build

ARG API_PROJECT VERBOSITY APP_DIR
ENV \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=1 \
   DOTNET_NOLOGO=0

WORKDIR /build

# Let's restore the solution, nuget and project files and do a restore
# in cachable layers. This will speed up the build process greatly

# copy global files to restore
# TODO: Do we need to take the .editorconfig with us during build?
COPY ./*.sln ./

# copy src files to restore
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# copy test files to restore
RUN echo "" \
   && echo "-------------" \
   && echo "1/4 RESTORING" \
   && echo "-------------" \
   && echo "" \
   && dotnet restore --verbosity "$VERBOSITY" \
   && echo "" 

# copy dirs that are only needed for building and testing
COPY src ./src

# Note on build: don't use --no-restore, sometimes certain packages cannot be
# restored by the dotnet restore. The build will add them, as it has more context (!?)
# example: Package System.Text.Json, version 6.0.0 was not found

RUN echo "" \
   && echo "------------" \
   && echo "2/4 BUILDING" \
   && echo "------------" \
   && echo "" \
   && dotnet build --configuration Release --verbosity "$VERBOSITY" -nowarn:NETSDK1004 \
   && echo "" \
   && echo "--------------" \
   && echo "3/4 PUBLISHING" \
   && echo "--------------" \
   && echo "" \
   && dotnet publish "$API_PROJECT" --configuration Release --output "$APP_DIR" --no-restore -nowarn:NETSDK1004 \
   && echo "" \
   && echo "------------" \
   && echo "4/4 FINISHED" \
   && echo "------------" \
   && echo ""

LABEL cicd="zonneconnect"

############################################
# Runtime image, uses API_DLL, APP_DIR
############################################

FROM mcr.microsoft.com/dotnet/runtime:$NET_VERSION-alpine as runtime

ARG API_DLL APP_DIR

# create a new user and change directory ownership
RUN addgroup --gid 1000 mygroup && \
    adduser --disabled-password \
    --home "$APP_DIR" \
    --gecos '' \
    --uid 1000 \
    --grp 1000 \
    dotnetuser && \

# impersonate into the new user
USER dotnetuser

ENV \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=1

WORKDIR $APP_DIR
COPY --from=build $APP_DIR .

ENV PROGRAM="$API_DLL"
ENTRYPOINT dotnet "$PROGRAM" current --poll

LABEL cicd="zonneconnect"


