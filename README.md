# GeoConnect Backend

API REST construida en .NET 8 para el backend de la aplicación móvil GeoConnect.

## Tecnologías
* **Framework:** .NET 9 (Web API)
* **Base de Datos:** SQL Server
* **ORM:** Entity Framework Core
* **Datos Espaciales:** NetTopologySuite
* **Autenticación:** JWT (JSON Web Tokens)

## Cómo ejecutar el proyecto localmente

1. Clonar el repositorio.
2. Configurar la cadena de conexión en `GeoConnectApi\appsettings.json`.
3. Abrir la Consola del Administrador de Paquetes y ejecutar:
   `Update-Database` (para aplicar las migraciones).
4. Ejecutar el proyecto (F5 en Visual Studio).

## Estructura de la Solución
* **GeoConnectApi:** Controladores y configuración principal.
* **Services:** Lógica de negocio y acceso a datos.
* **Models:** Entidades y Contexto de Base de Datos.
* **AutofillGooglePlacesID:** Service worker para hacer un chequeo frecuente sobre la integridad de los datos relacionados a los lugares en la DB.