FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Salin seluruh project
COPY . ./

# Restore dependencies
RUN dotnet restore

# Publish
RUN dotnet publish -c Release --no-restore -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "impactbackend.dll"]
