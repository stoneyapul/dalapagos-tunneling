FROM --platform=$BUILDPLATFORM  mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY */*.csproj .
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Core.csproj
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Monitor.HF.csproj
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Repository.EF.csproj
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Rport.csproj
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Core.csproj
RUN dotnet restore -a $TARGETARCH ./Dalapagos.Tunneling.Secrets.KeyVault.csproj

# copy everything else and build app
COPY Dalapagos.Tunneling.Api/. .
COPY Dalapagos.Tunneling.Core/. .
COPY Dalapagos.Tunneling.Monitor.HF/. .
COPY Dalapagos.Tunneling.Repository.EF/. .
COPY Dalapagos.Tunneling.Rport/. .
COPY Dalapagos.Tunneling.Secrets.KeyVault/. .
RUN dotnet publish -a $TARGETARCH --no-restore -o /app Dalapagos.Tunneling.Api.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dalapagos.Tunneling.Api.dll"]
