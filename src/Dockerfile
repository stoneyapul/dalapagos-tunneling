FROM --platform=$BUILDPLATFORM  mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./Dalapagos.Tunneling.sln .
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.sln

# copy everything else and build app
COPY Dalapagos.Tunneling.Api/. .
COPY Dalapagos.Tunneling.Core/. .
COPY Dalapagos.Tunneling.Monitor.HF/. .
COPY Dalapagos.Tunneling.Repository.EF/. .
COPY Dalapagos.Tunneling.Rport/. .
COPY Dalapagos.Tunneling.Secrets.KeyVault/. .
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dalapagos.Tunneling.Api.dll"]
