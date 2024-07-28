# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY /src/*.csproj ./src/
RUN dotnet restore

# copy everything else and build app
COPY /src/. ./src/
WORKDIR /src/Dalapagos.Tunneling.Api
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dalapagos.Tunneling.Api.dll"]