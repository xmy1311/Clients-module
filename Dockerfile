# =====================================================================
# API de registro de usuarios
# Imagen multietapa: se compila con el SDK y se ejecuta solo con el
# runtime, de modo que la imagen final no lleva compilador ni código
# fuente. Contexto de construcción: la raíz de este repositorio.
# =====================================================================

# ---------- Etapa 1: compilación ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Primero solo los archivos de proyecto: si no cambian, Docker reutiliza
# la capa de restauración y no vuelve a descargar los paquetes.
COPY Coink.Usuarios.sln ./
COPY src/Coink.Usuarios.Api/Coink.Usuarios.Api.csproj                     src/Coink.Usuarios.Api/
COPY src/Coink.Usuarios.Application/Coink.Usuarios.Application.csproj     src/Coink.Usuarios.Application/
COPY src/Coink.Usuarios.Infrastructure/Coink.Usuarios.Infrastructure.csproj src/Coink.Usuarios.Infrastructure/
COPY tests/Coink.Usuarios.UnitTests/Coink.Usuarios.UnitTests.csproj       tests/Coink.Usuarios.UnitTests/

RUN dotnet restore

# Ahora sí el resto del código.
COPY . .

RUN dotnet publish src/Coink.Usuarios.Api/Coink.Usuarios.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ---------- Etapa opcional: pruebas ----------
# No forma parte de la imagen final: solo se construye si se pide de forma
# explícita con --target. Así, una prueba en rojo nunca impide levantar la API.
#
#   docker build --target pruebas --progress=plain .
#
FROM build AS pruebas
RUN dotnet test tests/Coink.Usuarios.UnitTests/Coink.Usuarios.UnitTests.csproj \
    --configuration Release \
    --no-restore \
    --logger "console;verbosity=normal"

# ---------- Etapa 2: ejecución ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Usuario sin privilegios, provisto por la propia imagen oficial.
USER $APP_UID

ENTRYPOINT ["dotnet", "Coink.Usuarios.Api.dll"]
