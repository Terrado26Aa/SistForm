FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY SistForm-API/SistForm-API.csproj SistForm-API/
RUN dotnet restore SistForm-API/SistForm-API.csproj
COPY . .
RUN dotnet publish SistForm-API/SistForm-API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 5174
ENV ASPNETCORE_URLS=http://+:5174
ENTRYPOINT ["dotnet", "SistForm-API.dll"]
