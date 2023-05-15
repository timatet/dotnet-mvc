FROM node:18.12.1 as build-npm

COPY package.json .
RUN npm install

FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /publish
FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime

WORKDIR /publish
COPY --from=build-env /publish .
COPY --from=build-npm /node_modules/ ./node_modules/

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5000

ARG RELEASE_VERSION=LATEST
ENV RELEASE_VERSION_ENVIRONMENT ${RELEASE_VERSION}

ENTRYPOINT ["dotnet", "dotnet-mvc.dll"]