FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Proiect.Web/Proiect.Web.csproj", "src/Proiect.Web/"]
COPY ["src/Proiect.Core/Proiect.Core.csproj", "src/Proiect.Core/"]
COPY ["src/Proiect.Infrastructure/Proiect.Infrastructure.csproj", "src/Proiect.Infrastructure/"]
RUN dotnet restore "src/Proiect.Web/Proiect.Web.csproj"
COPY . .
WORKDIR "/src/src/Proiect.Web"
RUN dotnet build "Proiect.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Proiect.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Proiect.Web.dll"]