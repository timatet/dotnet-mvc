FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -o /publish/ --no-restore

COPY node_modules/ /publish/node_modules/

FROM base AS final
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT ["dotnet", "dotnet-mvc.dll"]
