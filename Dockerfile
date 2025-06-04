FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ChatIADesktop.csproj", "."]
RUN dotnet restore "ChatIADesktop.csproj"
COPY . .
RUN dotnet build "ChatIADesktop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatIADesktop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar variáveis de ambiente para o desenvolvimento
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORTS=80

ENTRYPOINT ["dotnet", "ChatIADesktop.dll"]