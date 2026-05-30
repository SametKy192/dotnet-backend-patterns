# Build stage — SDK image ile build al
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve bağımlılıkları yükle
COPY ["DockerDemo.Api.csproj", "./"]
RUN dotnet restore

# Kaynak kodu kopyala ve build al
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage — sadece runtime image kullan (daha küçük)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Build stage'den publish çıktısını kopyala
COPY --from=build /app/publish .

# Port 8080'i expose et
EXPOSE 8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "DockerDemo.Api.dll"]