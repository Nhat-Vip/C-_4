# Dùng SDK .NET 8 để build app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY . .

# Restore packages & publish
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Stage runtime (chạy app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Mở cổng
EXPOSE 8080

# Khởi động app
ENTRYPOINT ["dotnet", "ASM_C#4.dll"]
