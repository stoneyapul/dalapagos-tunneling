FROM --platform=$BUILDPLATFORM  mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Dalapagos.Tunneling.Api/*.csproj .
RUN dotnet restore -a $TARGETARCH

# copy everything else and build app
COPY Dalapagos.Tunneling.Api/. .
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dalapagos.Tunneling.Api.dll"]
