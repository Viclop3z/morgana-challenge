# 🧙‍♀️ Morgana’s Trial – Round Table 2.0

Esta solución fue desarrollada para **Morgana’s Trial**, un reto en .NET centrado en integrar **Umbraco CMS 15** con una **Web API .NET** segura y escalable. El objetivo fue exponer ciertas funcionalidades del Management API de Umbraco a clientes externos mediante una capa de API personalizada.

---

## 🧱 Estructura de la Solución
0
### 1. `UmbracoCMS` (Umbraco 15 CMS)

- **Framework:** .NET 9  
- **CMS:** Umbraco 15  
- **Base de Datos:** SQLite (incluida en la solución)  
- **Propósito:** Alojar el CMS y los endpoints del Management API.

### 2. `UmbracoBridge` (.NET Web API)

- **Framework:** .NET 9  
- **Propósito:** Exponer endpoints para interactuar con el Management API de Umbraco de forma segura mediante client credentials.  
- **Características:**
  - Endpoints validados.
  - Autenticación basada en token con caché.
  - Documentación OpenAPI / Swagger.
  - Arquitectura limpia: servicios, comandos, queries, validadores, contratos.
  - Tests unitarios con `xUnit` y `Moq`.

---

## 🌐 Endpoints Expuestos

| Método | Ruta                                | Descripción                                      |
|--------|-------------------------------------|--------------------------------------------------|
| GET    | `/api/management/healthcheck`       | Obtiene el estado de salud de Umbraco           |
| POST   | `/api/management/documenttype`      | Crea un nuevo Document Type en Umbraco          |
| DELETE | `/api/management/delete/{id}`       | Elimina un Document Type por ID                 |

### ✅ Reglas de Validación para POST `/documenttype`:

- `alias`, `name` y `description` **no deben estar vacíos**.
- `icon` debe **comenzar con `icon-`**.

---

## 🧪 Tests Unitarios

Los tests unitarios con `xUnit` y `Moq` cubren:

- `ManagementController`: escenarios exitosos y fallidos.
- `TokenService`: adquisición de token y manejo de errores.
- `HealthCheckService`: errores de conectividad con Umbraco.
- `DocumentTypeValidator`: todos los casos (válido, campos faltantes, formato incorrecto del icono, etc.)

Para correr los tests:

```bash
dotnet test


# ⚙️ Instrucciones de Configuración

## Prerrequisitos

- .NET 9 SDK
- Git
- Visual Studio 2022+ o Rider

## 🔧 Setup Local

Clonar el repositorio:

```bash
git clone https://github.com/Viclop3z/morgana-trial.git
cd morgana-trial


dotnet restore
dotnet build


dotnet run --project UmbracoCMS
dotnet run --project UmbracoBridge


## Documentación Swagger

- **Umbraco:** https://localhost:{PORT}/umbraco/swagger/index.html
- **Bridge API:** https://localhost:{PORT}/swagger/index.html

## Ejemplos de Peticiones

### ✅ POST `/api/management/documenttype`

**Request:**

```json
{
  "alias": "customAlias",
  "name": "My Document",
  "description": "Document for testing",
  "icon": "icon-notepad",
  "allowedAsRoot": true,
  "variesByCulture": false,
  "variesBySegment": false,
  "collection": null,
  "isElement": true
}

**Response:**

```json
{
  "id": "6bfba926-609f-44b6-89ea-97fe4f89baf9"
}

### ❌ POST Inválido

**Request:**

```json
{
  "alias": "",
  "name": "",
  "description": "",
  "icon": "notepad"
}

**Response:**

```json
{
  "errors": {
    "alias": "Alias must not be empty.",
    "name": "Name must not be empty.",
    "description": "Description must not be empty.",
    "icon": "Icon must start with 'icon-'."
  }
}

## ✅ Lista de Verificación

- UmbracoCMS se ejecuta con SQLite y expone `/umbraco/swagger`.
- UmbracoBridge consume el Management API de Umbraco.
- El token se solicita solo si no está en caché o está expirado.
- Validadores presentes para el payload del POST.
- Swagger habilitado y funcional para ambas APIs.

## 🛠️ appsettings.development.json para entorno de desarrollo

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Namespace": {
    "Name": "devv2"
  },
  "Services": {
    "BaseUrl": "http://localhost:57488/umbraco/management/api/v1",
    "Endpoints": [
      {
        "Name": "Token",
        "Path": "security/back-office/token"
      },
      {
        "Name": "HealthCheck",
        "Path": "health-check-group"
      },
      {
        "Name": "DocumentType",
        "Path": "document-type"
      },
      {
        "Name": "DeleteDocumentType",
        "Path": "document-type/{0}"
      }
    ]
  },
  "SecuritySettings": {
    "ClientId": "umbraco-back-office-morgana",
    "ClientSecret": "corrientes3482!!"
  }
}

Base url port should be the same as UmbracoCMS port.
```

## 📄 Logging UmbracoCMS

    user: victor.lopez@globant.com
    password: corrientes3482!!

## 📄 Running the Solution

dotnet build
dotnet run UmbracoMorgana.sln
