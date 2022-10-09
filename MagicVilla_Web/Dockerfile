FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MagicVilla_Web/MagicVilla_Web.csproj", "MagicVilla_Web/"]
RUN dotnet restore "MagicVilla_Web/MagicVilla_Web.csproj"
COPY . .
WORKDIR "/src/MagicVilla_Web"
RUN dotnet build "MagicVilla_Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagicVilla_Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicVilla_Web.dll"]
