# COINK · API de registro de usuarios

Prueba técnica para la posición de **Desarrollador Back-End Senior**.

API REST en **C# / .NET 8 (LTS)** que registra usuarios validando la cadena
**país → departamento → municipio**, con **PostgreSQL**, todas las consultas
resueltas mediante **procedimientos almacenados**, contenerizada con **Docker** y
documentada con **Swagger UI**.

---

## Ejecutar

Requisito único: **Docker Desktop**. No hace falta instalar .NET ni PostgreSQL.

```bash
# 1. Crear el archivo de variables de entorno
copy .env.example .env        # Windows
cp .env.example .env          # Linux / macOS

# 2. Levantar todo
docker compose up --build
```

Al terminar:

| Qué | Dónde |
|---|---|
| Documentación interactiva | http://localhost:8080/swagger |
| API | http://localhost:8080/api/usuarios |
| PostgreSQL | `localhost:5432` |

Los puertos salen del archivo `.env` (`API_PUERTO` y `POSTGRES_PUERTO`). Si los
cambia porque ya tiene algo escuchando ahí, las direcciones cambian igual: por
ejemplo, con `API_PUERTO=8090` la documentación queda en
`http://localhost:8090/swagger`. Dentro de Docker los contenedores siempre se
hablan por 8080 y 5432, así que no hay nada más que tocar.

La base de datos se crea, se estructura y se puebla sola la primera vez: los
scripts de `database/` se ejecutan en orden al inicializar el contenedor.

Para detener: `Ctrl+C` y luego `docker compose down`.
Para empezar **desde cero** (borra los datos y vuelve a ejecutar los scripts):

```bash
docker compose down -v
docker compose up --build
```

---

## Ejecutar la API desde el IDE (opcional)

Útil para depurar. La base de datos sigue en Docker; solo la API corre en la máquina.

```bash
# 1. Levantar únicamente PostgreSQL
docker compose up -d base-datos

# 2. Guardar la cadena de conexión FUERA del repositorio (una sola vez).
#    Use el puerto de POSTGRES_PUERTO y la contraseña de su archivo .env.
#    Puerto 5432 por defecto.
#    Dentro de Docker la API se conecta a base-datos:5432 .
cd src/Coink.Usuarios.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=SU_POSTGRES_PUERTO;Database=coink_db;Username=coink_app;Password=SU_CLAVE_DEL_ENV;Timeout=10;Command Timeout=15"
```

Luego, en Visual Studio, seleccione el perfil **API local** y presione F5:
se abre `http://localhost:5080/swagger`.

Desde la terminal, el equivalente es `dotnet run --project src/Coink.Usuarios.Api`.

> El puerto local es el **5080** y no el 8080 a propósito: así puede tener el
> entorno de Docker levantado al mismo tiempo sin que choquen.
>
> La cadena de conexión se guarda en el almacén de secretos del usuario
> (`%APPDATA%\Microsoft\UserSecrets`), no en el proyecto: `launchSettings.json`
> se versiona y no debe contener credenciales.

---

## Probar en un minuto

```bash
# Registro válido -> 201 Created
curl -i -X POST http://localhost:8080/api/usuarios ^
  -H "Content-Type: application/json" ^
  -d "{\"nombre\":\"Xiomara Zapata\",\"telefono\":\"+573001234567\",\"paisId\":1,\"departamentoId\":1,\"municipioId\":1,\"direccion\":\"Calle 10 # 43-12 Apto 301\"}"

# Consultar lo registrado
curl -i http://localhost:8080/api/usuarios/1
```

(En Windows, `^` continúa la línea; en Linux y macOS use `\`.)

### Identificadores cargados por el script semilla

| País | | Departamento | | Municipio |
|---|---|---|---|---|
| 1 Colombia | | 1 Antioquia · 2 Atlántico · 3 Bogotá D.C. · 4 Cundinamarca · 5 Valle del Cauca | | 1 Medellín · 5 Barranquilla · 7 Bogotá D.C. · 11 Cali |
| 2 Perú | | 6 Cusco · 7 Lima | | 13 Cusco · 15 Lima |

También puede consultarlos con `GET /api/catalogos/paises`.

### Escenarios que muestran las validaciones

| Qué probar | Datos | Respuesta |
|---|---|---|
| Registro correcto | país 1, departamento 1, municipio 1 | **201** |
| Teléfono mal formado | `"telefono": "3001234567"` | **400** |
| País inexistente | `"paisId": 999` | **422** |
| Departamento de otro país | país 1 + departamento 6 (Cusco) | **422** |
| Municipio de otro departamento | departamento 1 + municipio 7 (Bogotá) | **422** |
| Teléfono repetido | el mismo del primer registro | **409** |

---

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/usuarios` | Registra un usuario |
| `GET` | `/api/usuarios/{id}` | Consulta un usuario con su ubicación resuelta |
| `GET` | `/api/catalogos/paises` | Países |
| `GET` | `/api/catalogos/paises/{paisId}/departamentos` | Departamentos de un país |
| `GET` | `/api/catalogos/departamentos/{departamentoId}/municipios` | Municipios de un departamento |

Todos los errores se devuelven en formato `ProblemDetails` con un
código estable y un `traceId` para correlacionar con los registros.

---

## Estructura

```
Clients-module/
├── database/            Scripts SQL: base, tablas, procedimientos y datos semilla
├── src/
│   ├── Coink.Usuarios.Api               Controladores, errores, Swagger
│   ├── Coink.Usuarios.Application       DTOs, validación, servicio, contratos
│   └── Coink.Usuarios.Infrastructure    Acceso a datos con Npgsql
├── tests/               Pruebas unitarias
├── docker-compose.yml
├── Dockerfile
├── .env.example
└── README.md        
```

---

## Ejecutar las pruebas

Con Docker, sin instalar nada:

```bash
docker build --target pruebas --progress=plain .
```

Con el SDK de .NET 8 instalado:

```bash
dotnet test
```

La etapa `pruebas` no forma parte de la imagen que levanta `docker compose`: se
construye solo si se pide. Una prueba en rojo debe avisar, no impedir que la API
arranque.

### Pruebas de extremo a extremo

Verifican la solución realmente levantada —API, PostgreSQL y procedimiento
almacenado— y se omiten salvo que se pidan:

```powershell
docker compose up -d
$env:COINK_PRUEBAS_INTEGRACION = "1"
$env:COINK_URL_API = "http://localhost:8080"   # o el puerto de su .env
dotnet test
```

---

## Base de datos

| Objeto | Tipo | Para qué |
|---|---|---|
| `sp_registrar_usuario` | `PROCEDURE` | Valida país → departamento → municipio e inserta, en una sola transacción |
| `fn_obtener_usuario` | `FUNCTION` | Detalle del usuario con la ubicación resuelta |
| `fn_listar_paises` · `fn_listar_departamentos` · `fn_listar_municipios` | `FUNCTION` | Catálogos |

Los scripts están en `database/` y se pueden ejecutar también a mano:

```bash
psql -U postgres -f database/00_crear_base_datos.sql
psql -U postgres -d coink_db -f database/01_tablas.sql
psql -U postgres -d coink_db -f database/02_procedimientos.sql
psql -U postgres -d coink_db -f database/03_datos_semilla.sql
```

---

## Si algo falla

| Síntoma | Causa y solución |
|---|---|
| `POSTGRES_PASSWORD is required` | Falta el archivo `.env`. Copie `.env.example` |
| `port is already allocated` | Ya hay algo en el 8080 o el 5432. Cambie `API_PUERTO` o `POSTGRES_PUERTO` en `.env` |
| La API responde 500 al registrar | La base existe pero sin los objetos: el volumen se creó antes de tener los scripts. `docker compose down -v` y vuelva a levantar |
| La API responde **503** `SERVICIO_NO_DISPONIBLE` | No alcanza a PostgreSQL. Ejecutándola desde el IDE, lo más común es que el puerto de la cadena de conexión no sea el de `POSTGRES_PUERTO`. Verifique con `docker compose ps` que la base esté *healthy* y revise el secreto con `dotnet user-secrets list` |
| Cambié un script de `database/` y no se aplicó | Los scripts solo corren al crear el volumen. `docker compose down -v` |
