# 🧙‍♀️ Morgana’s Trial – Round Table 2.0

This solution was developed for **Morgana’s Trial**, a .NET challenge focused on integrating **Umbraco CMS 15** with a secure, scalable **.NET Web API**. The goal was to expose selected Umbraco Management API features to external clients via a custom API layer.

---

## 🧱 Solution Structure

### 1. `UmbracoCMS` (Umbraco 15 CMS)
- **Framework:** .NET 9
- **CMS:** Umbraco 15
- **Database:** SQLite (included in the solution)
- **Purpose:** Hosts the CMS and Management API endpoints.

### 2. `UmbracoBridge` (.NET Web API)
- **Framework:** .NET 9
- **Purpose:** Exposes endpoints to interact with Umbraco’s Management API securely via client credentials.
- **Features:**
  - Validated endpoints.
  - Token-based authentication with caching.
  - OpenAPI / Swagger documentation.
  - Clean architecture: services, commands, queries, validators, contracts.
  - Unit testing using `xUnit` and `Moq`.

---

## 🌐 Exposed Endpoints

| Method | Route                               | Description                                      |
|--------|--------------------------------------|--------------------------------------------------|
| GET    | `/api/management/healthcheck`       | Gets Umbraco health status                      |
| POST   | `/api/management/documenttype`      | Creates a new Document Type in Umbraco         |
| DELETE | `/api/management/delete/{id}`       | Deletes a Document Type by ID                  |

### ✅ Validation Rules for POST `/documenttype`:
- `alias`, `name`, and `description` must **not be empty**.
- `icon` must **start with `icon-`**.

---

## 🧪 Unit Tests

Unit tests using `xUnit` and `Moq` cover:
- `ManagementController`: success and failure scenarios.
- `TokenService`: token acquisition and error handling.
- `HealthCheckService`: Umbraco connectivity errors.
- `DocumentTypeValidator`: for all rule cases (valid, missing fields, wrong icon format, etc.)

Run tests with:

```bash
dotnet test
