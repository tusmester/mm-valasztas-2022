#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LakossagStat.WebApp/LakossagStat.WebApp.csproj", "LakossagStat.WebApp/"]
COPY ["LakossagStat.Data/LakossagStat.Data.csproj", "LakossagStat.Data/"]
RUN dotnet restore "LakossagStat.WebApp/LakossagStat.WebApp.csproj"
COPY . .
WORKDIR "/src/LakossagStat.WebApp"
RUN dotnet build "LakossagStat.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LakossagStat.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LakossagStat.WebApp.dll"]