FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS builder
WORKDIR /app

COPY *.sln .
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done

RUN dotnet restore

COPY . .
RUN dotnet publish AzureKeyVaultEmulator/AzureKeyVaultEmulator.csproj -c Release -o publish --no-restore

########################################

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app

RUN apk add --no-cache icu-libs tzdata

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=builder /app/publish .

ENTRYPOINT ["dotnet", "AzureKeyVaultEmulator.dll"]
