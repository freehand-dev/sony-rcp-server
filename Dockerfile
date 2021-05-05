FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# copy csproj and restore as distinct layer
COPY . .
RUN dotnet restore -r linux-arm

# build app layer
FROM build AS publish
RUN dotnet publish  -c release --self-contained false --no-restore --output /app/publish  "./sony-rcp-server/sony-vrcp-server.csproj"

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm32v7
ENV TZ=Europe/Kiev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN mkdir -p /opt/sony-rcp-server/bin
RUN mkdir -p /opt/sony-rcp-server/etc/sony-rcp-server
WORKDIR /opt/sony-rcp-server/bin
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sony-rcp-server.dll"]