# Utiliza la imagen base de SDK de .NET 8.0 para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el proyecto y restaura los paquetes
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código y compila la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de construcción final utilizando una imagen ASP.NET Core más ligera
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia los archivos publicados de la etapa de construcción anterior
COPY --from=build /app/out .

# Configura el puerto y el entorno
ENV ASPNETCORE_URLS http://*:44319
ENV ASPNETCORE_ENVIRONMENT=Development

# Exponer el puerto en el que la aplicación se ejecutará
EXPOSE 44319

# Comando para ejecutar la aplicación cuando se inicie el contenedor
ENTRYPOINT ["dotnet", "MyServiceApi.dll"]
