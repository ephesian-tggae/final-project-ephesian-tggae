FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MovieNest.Api.csproj .
RUN dotnet restore MovieNest.Api.csproj

COPY . .
RUN dotnet publish MovieNest.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN mkdir -p /var/data

COPY --from=build /app/publish .

EXPOSE 10000

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MovieNest.Api.dll"]
