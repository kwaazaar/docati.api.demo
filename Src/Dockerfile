# buster-slim fixed because of required font packages
FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR "/src/Docati.Api.Demo"
COPY ["Src/Docati.Api.Demo.csproj", "Docati.Api.Demo.csproj"]
RUN dotnet restore "Docati.Api.Demo.csproj"
COPY Src/ ./
RUN dotnet build -f net5.0 -r linux-x64 "Docati.Api.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -f net5.0 -r linux-x64 "Docati.Api.Demo.csproj" -c Release -o /app/publish


FROM base AS final

# Enable Contrib debian packages
RUN sed -i 's/ buster main/ buster main contrib/g' /etc/apt/sources.list
RUN sed -i 's+ buster/updates main+ buster/updates main contrib+g' /etc/apt/sources.list
RUN sed -i 's/ buster-updates main/ buster-updates main contrib/g' /etc/apt/sources.list

# Install MS core fonts (several free fonts by MS)
RUN echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections
RUN apt-get update && apt-get install fontconfig ttf-mscorefonts-installer -y
# Update fonts cache
RUN fc-cache -f -v
# List available fonts
RUN fc-list : file

# Install libgdiplus (for working with images)
RUN apt-get install -y --no-install-recommends libgdiplus libc6-dev

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docati.Api.Demo.dll"]