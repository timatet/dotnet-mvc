FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5000

WORKDIR /src
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "run"]