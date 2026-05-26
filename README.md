# MyTodosBackend - Clean Architecture Todo API

A production-ready **ASP.NET Core 8** backend API for a Todo application built with **Clean Architecture** principles. This is the backend companion to the [my-todo-app](https://github.com/yunisKazimli1/my-todo-app) React + TypeScript frontend.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Setup & Installation](#setup--installation)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Design Decisions](#design-decisions)
- [Development](#development)

---

## 🎯 Overview

This project implements a **Todo Management API** that handles task creation, updates, deletion, filtering, and sorting. The application enforces business rules like minimum title length and due date validation while maintaining a clean, scalable architecture suitable for production environments.

**Key Statistics:**
- Clean Architecture with 4 separate projects (Domain, Application, Infrastructure, API)
- ~1,300 lines of C# code
- Full input validation with FluentValidation
- Centralized exception handling middleware
- SQLite database with Entity Framework Core
- CORS enabled for frontend integration

---

## ✨ Features

### Task Management
✅ **Create Tasks** - Add todos with title (min 10 characters)  
✅ **View Tasks** - Retrieve all todos with advanced filtering and sorting  
✅ **Complete Tasks** - Mark todos as done  
✅ **Delete Tasks** - Remove todos from database  
✅ **Set Due Dates** - Assign and update task deadlines  

### Advanced Features
✅ **Filtering** - All, Active, Completed, Overdue  
✅ **Sorting** - A-Z, Z-A, Due Date (earliest/latest first)  
✅ **Pagination** - Configurable page size with total count  
✅ **Overdue Detection** - Automatic red flag for past due dates  
✅ **Input Validation** - FluentValidation for data integrity  
✅ **Error Handling** - Centralized exception middleware with proper HTTP status codes  
✅ **CORS Support** - Pre-configured for frontend development  

---

## 📁 Project Structure

```
MyTodosBackend/
├── MyTodosBackend.Api/                 # Presentation Layer
│   ├── Controllers/
│   │   └── TodoController.cs           # REST endpoints
│   ├── Middleware/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── Extensions/
│   ├── ApiServiceRegistration.cs       # DI setup for API layer
│   ├── Program.cs                      # Application entry point
│   └── appsettings.json                # Configuration
│
├── MyTodosBackend.Application/         # Business Logic Layer
│   ├── Implementations/
│   │   └── TodoManager.cs              # Core business logic
│   ├── Interfaces/
│   │   └── ITodoManager.cs             # Service abstraction
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── AddTodoDto.cs
│   │   ├── GetTodoDto.cs
│   │   └── UpdateTodoDateDto.cs
│   ├── Validators/                     # FluentValidation rules
│   │   ├── AddTodoDtoValidator.cs
│   │   ├── GetTodosQueryValidator.cs
│   │   └── UpdateTodoDateDtoValidator.cs
│   ├── Queries/                        # Query models
│   │   └── GetTodosQuery.cs
│   ├── CustomMapper/                   # Manual DTO mapping
│   │   └── TodoMapper.cs
│   ├── Utility/                        # Helper classes
│   │   ├── CustomExceptions/
│   │   └── Responses/
│   └── ApplicationServiceRegistration.cs
│
├── MyTodosBackend.Domain/              # Business Logic (Core)
│   ├── Entities/
│   │   └── Todo.cs                     # Domain entity
│   └── Enums/
│       ├── TodoFilterEnum.cs
│       └── TodoSortingEnum.cs
│
├── MyTodosBackend.Infrastructure/      # Data Access Layer
│   ├── Context/
│   │   └── AppDbContext.cs             # EF Core DbContext
│   ├── Migrations/                     # Database migrations
│   └── InfrastructureServiceRegistration.cs
│
├── MyTodosBackend.Tests/               # Unit & Integration Tests
└── MyTodosBackend.sln
```

---

## 🏗️ Architecture

### Clean Architecture Layers

This project follows **Clean Architecture** principles with strict separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│  API Layer (Presentation)                               │
│  - REST Controllers                                      │
│  - Request/Response handling                            │
│  - Exception middleware                                 │
└─────────────────┬───────────────────────────────────────┘
                  │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Application Layer (Business Logic)                      │
│  - TodoManager (use case implementation)                │
│  - DTOs, Queries, Validators                            │
│  - CustomMapper (data transformation)                   │
└─────────────────┬───────────────────────────────────────┘
                  │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Domain Layer (Business Rules)                           │
│  - Todo entity                                          │
│  - Enums (TodoFilterEnum, TodoSortingEnum)              │
│  - No external dependencies                             │
└─────────────────┬───────────────────────────────────────┘
                  │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Infrastructure Layer (Data Access)                      │
│  - AppDbContext (EF Core)                               │
│  - Database configuration                               │
└─────────────────────────────────────────────────────────┘
```

### Key Principles

- **Dependency Inversion** - High-level modules depend on abstractions (interfaces)
- **Separation of Concerns** - Each layer has a single responsibility
- **Testability** - Components can be tested in isolation
- **Independence from Frameworks** - Business logic doesn't depend on EF Core, ASP.NET details
- **Maintainability** - Clear structure makes it easy to understand and modify

---

## 🛠️ Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | ASP.NET Core | 8.0 |
| **Language** | C# | 12 |
| **ORM** | Entity Framework Core | 8.0 |
| **Database** | SQLite | Latest |
| **Validation** | FluentValidation | 11.x |
| **Testing** | xUnit / Moq | Latest |
| **API Documentation** | Swagger/OpenAPI | Built-in |

---

## 🚀 Setup & Installation

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Git** - Version control
- **Visual Studio 2022** or **VS Code** with C# extension (optional)

### Step 1: Clone Repository

```bash
git clone https://github.com/yunisKazimli1/MyTodosBackend.git
cd MyTodosBackend
```

### Step 2: Install Dependencies

```bash
dotnet restore
```

### Step 3: Setup Database

Create and apply database migrations:

```bash
# Install EF Core CLI (if not already installed)
dotnet tool install --global dotnet-ef

# Apply migrations to create database
dotnet ef database update --project MyTodosBackend.Infrastructure --startup-project MyTodosBackend.Api
```

This will create `todos.db` SQLite database file in the project root.

### Step 4: Run Application

```bash
cd MyTodosBackend.Api
dotnet run
```

The API will start at:
- **HTTPS**: `https://localhost:7176`
- **HTTP**: `http://localhost:5000`

### Step 5: Access Swagger UI

Open browser and navigate to:
```
https://localhost:7176/swagger
```

You'll see interactive API documentation with all endpoints.

---

## 📡 API Endpoints

### Base URL
```
https://localhost:7176/api/todo
```

### 1. Get All Todos (with filtering, sorting, pagination)

```http
GET /api/todo?page=1&pageSize=10&filterBy=All&sortBy=Az

Query Parameters:
  - page: int (default: 1) - Page number for pagination
  - pageSize: int (default: 10) - Items per page (5, 10, 20, 50, 100)
  - filterBy: string (All|Active|Completed|Overdue)
  - sortBy: string (Az|Za|DueDateEarliestFirst|DueDateLatestFirst)
```

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Complete project documentation",
      "dueDate": "2024-12-31",
      "isCompleted": false,
      "isOverdue": false
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42
}
```

### 2. Get Todo by ID

```http
GET /api/todo/{id}
```

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Complete project documentation",
  "dueDate": "2024-12-31",
  "isCompleted": false,
  "isOverdue": false
}
```

### 3. Create Todo

```http
POST /api/todo
Content-Type: application/json

{
  "title": "Complete project documentation"
}
```

**Validation Rules:**
- Title must not be empty
- Title must be at least 10 characters

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Complete project documentation",
  "dueDate": null,
  "isCompleted": false,
  "isOverdue": false
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "'Title' must be at least 10 characters long.",
  "traceId": "0HN2QVDUFBGV8:00000001"
}
```

### 4. Mark Todo as Complete

```http
PATCH /api/todo/{id}/complete
```

**Response (204 No Content):** - No body returned

### 5. Update Todo Due Date

```http
PATCH /api/todo/{id}/updateDate?dueDate=2024-12-31
```

**Query Parameters:**
- dueDate: string (format: YYYY-MM-DD) - New due date

**Response (204 No Content):** - No body returned

### 6. Delete Todo

```http
DELETE /api/todo/{id}
```

**Response (204 No Content):** - No body returned

### Error Responses

**404 Not Found:**
```json
{
  "message": "Item with ID '550e8400-e29b-41d4-a716-446655440000' was not found.",
  "traceId": "0HN2QVDUFBGV8:00000001"
}
```

**400 Bad Request (Validation):**
```json
{
  "message": "Due date cannot be in the past.",
  "traceId": "0HN2QVDUFBGV8:00000001"
}
```

**500 Internal Server Error:**
```json
{
  "message": "An unexpected error occurred",
  "traceId": "0HN2QVDUFBGV8:00000001"
}
```

---

## 💾 Database

### Entity: Todo

```sql
CREATE TABLE Todos (
    Id GUID PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    DueDate DATE NULL,
    IsCompleted BOOLEAN DEFAULT 0,
    CreatedAt DATETIME NOT NULL
);
```

### Entity Relationships

```
Todo (aggregate root)
├── Id: Guid (primary key)
├── Title: string (required, max 200 chars)
├── DueDate: DateOnly? (optional)
├── IsCompleted: bool (default: false)
└── CreatedAt: DateTime (UTC)
```

### Computed Properties

- **IsOverdue** - Calculated at DTO level: `!IsCompleted && DueDate != null && DueDate < Today`

### Why SQLite?

- ✅ Zero configuration for development
- ✅ Lightweight and portable
- ✅ Perfect for small-to-medium projects
- ✅ Easy migration to production databases (SQL Server, PostgreSQL)
- ✅ No server setup required

For production, consider upgrading to:
- **SQL Server** - Enterprise-grade, full ACID compliance
- **PostgreSQL** - Open-source, high performance
- **Azure SQL Database** - Managed cloud database

---

## 🎨 Design Decisions

### 1. **Custom Mapper Instead of AutoMapper**

**Decision**: Implemented manual `TodoMapper` instead of AutoMapper library

**Rationale:**
```
✅ Project Scope: Only ONE DTO mapping scenario (Todo → GetTodoDto)
✅ Simplicity: Custom mapper is 20 lines vs 100+ AutoMapper config
✅ Zero Overhead: No reflection cost, no extra package dependency
✅ Clarity: Direct mapping logic is immediately visible and easy to debug

❌ AutoMapper Would Be Overkill For:
   - Single mapping scenario
   - Small codebase
   - Clear, straightforward transformation

✅ When AutoMapper IS Needed:
   - 10+ different DTOs
   - Complex nested mappings
   - Team standardization requirement
   - Large enterprise project
```

**Production Consideration**: If this application scales and requires 10+ DTOs, migrate to AutoMapper with minimal refactoring.

---

### 2. **Direct DbContext Access in Services (No Repository Pattern)**

**Decision**: `TodoManager` directly uses `AppDbContext` instead of implementing Repository pattern

**Rationale:**
```
✅ Project Scope: Services use ONLY 3 operations
   - DbSet.FindAsync(id)
   - DbContext.SaveChangesAsync()
   - DbSet.Add/Remove (basic operations)

✅ Benefits of Direct Access:
   - Simplified dependency injection
   - One less abstraction layer
   - Faster development
   - Easier to understand data flow
   - LINQ queries remain efficient

❌ Repository Pattern Would Be Overkill Because:
   - Extra interface layer for simple CRUD
   - Added complexity without benefit
   - More boilerplate code

✅ When Repository Pattern IS Needed:
   - 5+ different data sources (DB, API, cache, file system)
   - Complex querying logic requiring abstraction
   - Need to swap database implementations
   - Team standardization requirement
   - Large enterprise project with multiple developers
```

**Architecture Principle**: 
> "Don't add abstraction layers preemptively. Add them when you need them. Entity Framework Core IS your repository pattern."

---

### 3. **Clean Architecture Instead of CQRS + MediatR**

**Decision**: Implemented layered Clean Architecture instead of CQRS (Command Query Responsibility Segregation) + MediatR

**Rationale:**
```
✅ Project Scope: Simple read/write patterns, NOT complex event sourcing
   - 5 simple endpoints
   - No need for separate read/write models
   - No event-driven architecture required

CLEAN ARCHITECTURE (Chosen):
✅ Simpler to understand (clear flow: Controller → Service → DB)
✅ Minimal dependencies (4 NuGet packages)
✅ Fast execution (no pipeline overhead)
✅ Clear separation of concerns
✅ Suitable for small-medium projects

CQRS + MediatR (Not Chosen):
❌ Adds complexity for "future scalability"
❌ Extra packages (MediatR, behaviors, handlers)
❌ Pipeline overhead on every request
❌ Steeper learning curve
❌ More code to maintain
✅ BUT: Worth it for:
   - Complex event-driven systems
   - Separate read/write optimization needs
   - Large teams with different read/write specialists
   - Event sourcing requirement
   - Microservices architecture
```

**Comparison:**

| Aspect | Clean Architecture | CQRS + MediatR |
|--------|-------------------|----------------|
| Complexity | Low | High |
| Lines of Code | ~1,300 | ~3,000+ |
| Learning Curve | Easy | Steep |
| Dependencies | 4 | 8+ |
| Best For | Small-Medium Projects | Large Enterprise Systems |
| Scalability | Good | Excellent |

**Upgrade Path**: If requirements evolve (event sourcing, microservices, complex workflows), can gradually introduce CQRS while keeping Clean Architecture foundation.

---

### 4. **Centralized Exception Handling Middleware**

**Implementation**: `ExceptionHandlingMiddleware` catches all exceptions globally

**Benefits:**
```
✅ Consistent error responses across all endpoints
✅ Centralized logging of errors
✅ Prevents accidental sensitive data leakage
✅ Proper HTTP status code mapping:
   - 404: ItemNotFoundException
   - 400: ValidationException
   - 500: All other errors
✅ Trace ID for debugging
✅ Prevents stack traces in production
```

---

### 5. **FluentValidation for Input Validation**

**Decision**: Used FluentValidation library instead of Data Annotations

**Rationale:**
```
✅ Benefits of FluentValidation:
   - Fluent, readable syntax
   - Complex validation rules
   - Reusable validator instances
   - Testable validation logic
   - Clear error messages
   - Async validation support

Example:
  RuleFor(x => x.Title)
    .NotEmpty()
    .MinimumLength(10)
    .MaximumLength(200)
    .Matches(@"^[a-zA-Z0-9\s\-\.]+$") // Custom regex
    .WithMessage("Invalid characters in title");
```

---

### 6. **DateOnly for Due Dates**

**Decision**: Used C# `DateOnly` type instead of `DateTime`

**Benefits:**
```
✅ Due dates don't need time component
✅ Clearer intent (date-only semantics)
✅ Eliminates timezone confusion
✅ Simpler database storage
✅ Cleaner API responses (2024-12-31 vs 2024-12-31T00:00:00Z)
```

---

### 7. **Filter & Sort as Query Parameters**

**Decision**: Implemented filtering and sorting as query parameters

**Rationale:**
```
✅ RESTful standard for filtering
✅ Cacheability (same URL = same cache)
✅ Browser-friendly (easy to test)
✅ Frontend-friendly (query string binding)
✅ Stateless design

Alternatives Considered:
❌ POST with filter body: Not RESTful
❌ Separate endpoints (/api/todo/completed, /api/todo/overdue):
   - Endpoint explosion
   - Code duplication
```

---

## 👨‍💻 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "ClassName=TodoManagerTests"
```

### Adding New Features

1. **Add Domain Entity** → `MyTodosBackend.Domain/Entities/`
2. **Add DTOs** → `MyTodosBackend.Application/DTOs/`
3. **Add Validator** → `MyTodosBackend.Application/Validators/`
4. **Add Mapper** → `MyTodosBackend.Application/CustomMapper/`
5. **Add Service Interface** → `MyTodosBackend.Application/Interfaces/`
6. **Add Service Implementation** → `MyTodosBackend.Application/Implementations/`
7. **Add Controller Endpoint** → `MyTodosBackend.Api/Controllers/`
8. **Update DbContext** → `MyTodosBackend.Infrastructure/Context/AppDbContext.cs`
9. **Create Migration** → `dotnet ef migrations add FeatureName`
10. **Write Tests** → `MyTodosBackend.Tests/`

### Code Style Guidelines

- **Naming**: PascalCase for classes/methods, camelCase for variables
- **Async**: Always use `async/await` for I/O operations
- **Validation**: Always validate input in validators, not in services
- **Exceptions**: Throw custom exceptions, catch in middleware
- **Comments**: Add XML documentation for public members

```csharp
/// <summary>
/// Retrieves todos with advanced filtering, sorting, and pagination.
/// </summary>
/// <param name="query">The query parameters for filtering/sorting</param>
/// <returns>Paged result containing matching todos</returns>
public async Task<PagedResult<GetTodoDto>> GetTodos(GetTodosQuery query)
{
    // Implementation
}
```

### Debugging

**Visual Studio Debugging:**
```
1. Set breakpoint in code
2. Press F5 to start debugging
3. Click on variable to inspect value
4. Use Debug Console for expressions
```

**Command Line Debugging:**
```bash
# Build in Debug configuration
dotnet build --configuration Debug

# Run with debugging symbols
dotnet run --configuration Debug
```

### Database Migrations

```bash
# Create new migration
dotnet ef migrations add YourMigrationName --project MyTodosBackend.Infrastructure --startup-project MyTodosBackend.Api

# Apply pending migrations
dotnet ef database update --project MyTodosBackend.Infrastructure --startup-project MyTodosBackend.Api

# Revert to previous migration
dotnet ef database update PreviousMigrationName --project MyTodosBackend.Infrastructure --startup-project MyTodosBackend.Api
```

---

## 📊 Code Metrics

| Metric | Value |
|--------|-------|
| **Total Lines of Code** | ~1,300 |
| **Number of Projects** | 4 |
| **API Endpoints** | 6 |
| **DTOs** | 3 |
| **Validators** | 3 |
| **Custom Exceptions** | 2 |
| **Test Cases** | TBD |

---

## 🔐 Security Considerations

### ✅ Implemented

- Input validation with minimum length requirements
- SQL injection prevention (EF Core parameterized queries)
- CORS properly configured (specific origins only)
- Exception details hidden in production
- No sensitive data in error messages

### 🔄 Recommended Additions

- **Authentication** - Add JWT or OAuth2 for user identification
- **Authorization** - Implement role-based access (admin, user)
- **Audit Logging** - Log who did what and when
- **Rate Limiting** - Prevent API abuse
- **HTTPS Only** - Force HTTPS in production
- **HSTS Headers** - Prevent downgrade attacks
- **Input Sanitization** - Sanitize strings to prevent XSS

---

## 🐛 Known Limitations & Future Improvements

### Current Limitations

1. No user authentication (all operations are public)
2. No audit trail (who modified what)
3. Basic error handling (could add custom error codes)
4. No API versioning (all endpoints are v1 implied)
5. No logging to file (console only)

### Planned Improvements

- [ ] Add JWT authentication
- [ ] Implement audit logging
- [ ] Add OpenAPI/Swagger security definitions
- [ ] Add rate limiting middleware
- [ ] Add request/response logging
- [ ] Add distributed caching (Redis)
- [ ] Add health check endpoints
- [ ] Add API documentation (XML comments export)
- [ ] Performance optimization (indexing strategies)
- [ ] Add soft delete for todos (data retention)

---

## 📚 Additional Resources

- [Microsoft - Clean Architecture in ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Uncle Bob - Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [REST API Best Practices](https://restfulapi.net/)

---

## 📞 Support & Contact

- **Author**: Yunis Kazimli
- **GitHub**: [@yunisKazimli1](https://github.com/yunisKazimli1)
- **Frontend Repo**: [my-todo-app](https://github.com/yunisKazimli1/my-todo-app)
- **Report Issues**: Create an issue on GitHub

---

## 📄 License

This project is open source and available under the MIT License.

---

## ✅ Checklist for Production Deployment

- [ ] Update CORS allowed origins to production frontend URL
- [ ] Change database from SQLite to SQL Server/PostgreSQL
- [ ] Enable HTTPS enforcement
- [ ] Add authentication/authorization
- [ ] Implement logging to persistent storage
- [ ] Add health check endpoints
- [ ] Setup database backups
- [ ] Configure application insights/monitoring
- [ ] Enable request rate limiting
- [ ] Add API documentation (Swagger)
- [ ] Setup CI/CD pipeline
- [ ] Load test the API
- [ ] Security audit
- [ ] Update connection strings from secrets manager

---

**Last Updated**: May 2026  
**Status**: ✅ Production Ready (with noted security additions)
