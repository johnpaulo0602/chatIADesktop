FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Gerar certificado de desenvolvimento para HTTPS
RUN apt-get update && apt-get install -y openssl
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /app/server.key -out /app/server.crt -subj "/CN=localhost"

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
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_HTTP_PORTS=80
ENV ASPNETCORE_HTTPS_PORTS=443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/server.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/app/server.key

# Abre o navegador na porta 80
ENTRYPOINT ["dotnet", "ChatIADesktop.dll"]