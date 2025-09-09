# .NET SDK (build için)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# csproj dosyasını kopyala ve restore yap
COPY CryptoDashboard/CryptoDashboard.csproj ./CryptoDashboard/
RUN dotnet restore ./CryptoDashboard/CryptoDashboard.csproj

# Tüm dosyaları kopyala ve build et
COPY . .
WORKDIR /src/CryptoDashboard
RUN dotnet publish -c Release -o /app/publish

# Runtime imajı
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CryptoDashboard.dll"]
