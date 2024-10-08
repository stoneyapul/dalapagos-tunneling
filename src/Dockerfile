FROM --platform=$BUILDPLATFORM  mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Dalapagos.Tunneling.Core/Dalapagos.Tunneling.Core.csproj ./Dalapagos.Tunneling.Core/
COPY Dalapagos.Tunneling.Monitor.HF/Dalapagos.Tunneling.Monitor.HF.csproj ./Dalapagos.Tunneling.Monitor.HF/
COPY Dalapagos.Tunneling.Repository.EF/Dalapagos.Tunneling.Repository.EF.csproj ./Dalapagos.Tunneling.Repository.EF/
COPY Dalapagos.Tunneling.Rport/Dalapagos.Tunneling.Rport.csproj ./Dalapagos.Tunneling.Rport/
COPY Dalapagos.Tunneling.Secrets.KeyVault/Dalapagos.Tunneling.Secrets.KeyVault.csproj ./Dalapagos.Tunneling.Secrets.KeyVault/
COPY Dalapagos.Tunneling.Api/Dalapagos.Tunneling.Api.csproj ./Dalapagos.Tunneling.Api/
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Core/Dalapagos.Tunneling.Core.csproj
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Monitor.HF/Dalapagos.Tunneling.Monitor.HF.csproj
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Repository.EF/Dalapagos.Tunneling.Repository.EF.csproj
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Rport/Dalapagos.Tunneling.Rport.csproj
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Secrets.KeyVault/Dalapagos.Tunneling.Secrets.KeyVault.csproj
RUN dotnet restore -a $TARGETARCH Dalapagos.Tunneling.Api/Dalapagos.Tunneling.Api.csproj

# copy everything else and build app
COPY Dalapagos.Tunneling.Api/. ./Dalapagos.Tunneling.Api
COPY Dalapagos.Tunneling.Core/. ./Dalapagos.Tunneling.Core
COPY Dalapagos.Tunneling.Monitor.HF/. ./Dalapagos.Tunneling.Monitor.HF
COPY Dalapagos.Tunneling.Repository.EF/. ./Dalapagos.Tunneling.Repository.EF
COPY Dalapagos.Tunneling.Rport/. ./Dalapagos.Tunneling.Rport
COPY Dalapagos.Tunneling.Secrets.KeyVault/. ./Dalapagos.Tunneling.Secrets.KeyVault
RUN dotnet publish -a $TARGETARCH --no-restore -o /app Dalapagos.Tunneling.Api/Dalapagos.Tunneling.Api.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dalapagos.Tunneling.Api.dll"]
