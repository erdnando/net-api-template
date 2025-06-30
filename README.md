# � .NET Core 9 Backend API

Secure REST API desarrollada con .NET Core 9, MySQL, JWT Authentication y mejores prácticas de seguridad.

## ⚡ **Inicio Rápido**

```bash
# Desde la raíz del proyecto full-stack
./start-backend.sh

# O desde este directorio
dotnet run
```

**🌐 URLs:**
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## 📚 **Documentación Completa**

Toda la documentación está organizada en [`docs/`](./docs/README.md):

- 🚀 [**Setup y Configuración**](./docs/setup/) - Configuración local, MySQL, variables de entorno
- 🔐 [**Seguridad**](./docs/security/) - Autenticación, validaciones, logs de seguridad  
- 🎯 [**Funcionalidades**](./docs/features/) - Password reset, Utils system
- 🔌 [**API Documentation**](./docs/api/) - Endpoints, DTOs, especificaciones
- 🚀 [**Deployment**](./docs/deployment/) - Guías de deploy y CI/CD
- 📋 [**Reports**](./docs/reports/) - Bug fixes, investigaciones, mejoras

## 🛠️ **Tech Stack**

- **Framework**: .NET Core 9
- **Base de datos**: MySQL + Entity Framework Core
- **Autenticación**: JWT Tokens
- **API Docs**: Swagger/OpenAPI
- **Testing**: xUnit
- **Logging**: Serilog
- **Validation**: FluentValidation
## 📂 **Estructura del Proyecto**

```
netapi-template/
├── docs/                     # 📚 Documentación organizada
├── Controllers/             # 🎮 API Controllers
├── Services/                # 🔧 Business Logic Services
├── Models/                  # 📋 Entity Models
├── DTOs/                    # 📦 Data Transfer Objects
├── Data/                    # 🗄️ Database Context
├── Middleware/              # 🔧 Custom Middleware
├── Configurations/          # ⚙️ App Configuration
└── Tests/                   # 🧪 Unit & Integration Tests
```

## 🔐 **Seguridad Implementada**

- ✅ **JWT Authentication** con roles y permisos
- ✅ **Input Validation** y sanitización
- ✅ **SQL Injection Protection** via EF Core
- ✅ **CORS** configurado para desarrollo
- ✅ **Rate Limiting** en endpoints críticos
- ✅ **Security Headers** (HSTS, CSP, etc.)
- ✅ **Logging de seguridad** y auditoría

## 🧪 **Testing**

```bash
# Ejecutar todos los tests
dotnet test

# Con coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 **Scripts Disponibles**

```bash
# Desarrollo (desde raíz del proyecto)
./start-backend.sh          # Inicia solo el backend
./start-fullstack.sh        # Inicia backend + frontend

# Organización
./cleanup-backend-docs.sh   # Organiza documentación
```

## 🔧 **Configuración Rápida**

### **Variables de Entorno (.env):**
```bash
DB_CONNECTION_STRING="Server=localhost;Database=netapi;User=root;Password=tu_password;"
JWT_SECRET="tu-jwt-secret-super-seguro"
JWT_ISSUER="NetApiTemplate"
CORS_ORIGINS="http://localhost:3000"
```

### **Base de Datos:**
```bash
# Crear migración
dotnet ef migrations add NombreMigracion

# Aplicar migración
dotnet ef database update
```

## 🤝 **Contribución**

1. Consulta la [documentación completa](./docs/README.md)
2. Revisa [configuración de seguridad](./docs/security/)
3. Sigue las [mejores prácticas](./docs/setup/)

---
💡 **Para información detallada**, consulta [`docs/README.md`](./docs/README.md)
- **Security Headers**: HSTS, Content-Type-Options, Frame-Options, etc.
- **Rate Limiting**: Protección contra ataques de fuerza bruta
- **Password Security**: Hashing seguro con Salt
- **Password Reset Flow**: Con tokens seguros y expiración
- **Security Logging**: Auditoría de eventos de seguridad
- **CORS Policy**: Configuración restrictiva para producción
- **Secret Management**: Variables de entorno y templates seguros

### 🔧 Configuración Avanzada
- **Azure Key Vault**: Para secretos en producción
- **GitHub Secrets**: Para CI/CD pipelines
- **Docker Support**: Configuración sin secretos hardcodeados

## 🔐 **Usuarios de Prueba**

```
Admin: admin@sistema.com / admin123
Usuario: erdnando@gmail.com / user123
```

## 📧 **Reset de Contraseña**

El sistema incluye flujo completo de reset de contraseña:

1. **Solicitar reset:** `POST /api/Users/forgot-password`
2. **Reset con token:** `POST /api/Users/reset-password`
3. **Soporte para email real:** Configurar SMTP en variables de entorno

Ver `PASSWORD_RESET_IMPLEMENTATION.md` para detalles técnicos.

## 📁 Estructura del Proyecto

```
netapi-template/
├── 📁 Configuration/          # Configuraciones de la aplicación
│   └── MappingProfile.cs      # Perfiles de AutoMapper
├── 📁 Controllers/            # Controladores de la API
│   ├── BaseController.cs      # Controlador base con funcionalidades comunes
│   └── UsersController.cs     # Controlador de usuarios y autenticación
├── 📁 Data/                   # Contexto de base de datos
│   └── ApplicationDbContext.cs # DbContext principal
├── 📁 DTOs/                   # Data Transfer Objects
│   ├── CommonDtos.cs          # DTOs comunes (ApiResponse, PaginatedResponse)
│   └── UserDtos.cs            # DTOs específicos para usuarios
├── 📁 Extensions/             # Métodos de extensión
│   └── ServiceCollectionExtensions.cs # Extensiones para DI
├── 📁 Helpers/                # Clases de utilidad
│   └── JsonHelper.cs          # Utilidades para JSON
├── 📁 Middleware/             # Middleware personalizado
│   └── GlobalExceptionMiddleware.cs # Manejo global de excepciones
├── 📁 Models/                 # Modelos de entidad
│   ├── BaseEntity.cs          # Entidad base con propiedades comunes
│   └── User.cs                # Modelo de usuario
├── 📁 Repositories/           # Patrón Repository
│   ├── 📁 Interfaces/         # Interfaces de repositorios
│   │   ├── IRepository.cs     # Repositorio genérico
│   │   ├── IUnitOfWork.cs     # Unit of Work
│   │   └── IUserRepository.cs # Repositorio de usuarios
│   ├── Repository.cs          # Implementación del repositorio genérico
│   ├── UnitOfWork.cs          # Implementación del Unit of Work
│   └── UserRepository.cs      # Implementación del repositorio de usuarios
└── 📁 Services/               # Lógica de negocio
    ├── 📁 Interfaces/         # Interfaces de servicios
    │   └── IUserService.cs    # Interfaz del servicio de usuarios
    └── UserService.cs         # Implementación del servicio de usuarios
```

## 🛠️ Tecnologías y Patrones Implementados

### Arquitectura
- **Clean Architecture**: Separación clara de responsabilidades
- **Repository Pattern**: Abstracción de la capa de datos
- **Unit of Work Pattern**: Gestión de transacciones
- **Dependency Injection**: Inversión de control

### Tecnologías
- **.NET 9**: Framework principal
- **Entity Framework Core**: ORM para base de datos
- **AutoMapper**: Mapeo automático entre objetos
- **BCrypt**: Hashing de contraseñas
- **JWT**: Autenticación (preparado para implementar)

### Características
- **Global Exception Handling**: Manejo centralizado de errores
- **Soft Delete**: Eliminación lógica de registros
- **Audit Fields**: Campos de auditoría automáticos
- **Response Patterns**: Respuestas estandarizadas
- **Validation**: Validación de modelos
- **CORS**: Configurado para desarrollo

## 🚀 Cómo Usar

### 1. Instalación de Dependencias
Las dependencias ya están configuradas en el `.csproj`:
- Entity Framework Core
- AutoMapper
- BCrypt
- JWT Bearer Authentication

### 2. Configuración de Base de Datos
Actualmente usa InMemory Database para desarrollo. Para producción:

```csharp
// En ServiceCollectionExtensions.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
```

### 3. Migraciones (cuando uses una BD real)
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Endpoints Disponibles

#### Usuarios
- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `POST /api/users` - Crear usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario (soft delete)

#### Autenticación
- `POST /api/auth/login` - Iniciar sesión

### 5. Ejemplo de Uso

#### Crear Usuario
```json
POST /api/users
{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "password": "user123"
}
```

#### Login
```json
POST /api/auth/login
{
  "email": "juan@example.com",
  "password": "user123"
}
```

## 📋 Próximos Pasos Recomendados

1. **Implementar JWT completo**: Completar la generación y validación de tokens
2. **Configurar base de datos real**: Reemplazar InMemory por SQL Server/PostgreSQL
3. **Agregar validaciones**: Implementar FluentValidation
4. **Agregar logging**: Configurar Serilog
5. **Implementar paginación**: En los endpoints de listado
6. **Agregar tests**: Unit tests y integration tests
7. **Configurar Swagger**: Para documentación de API
8. **Implementar rate limiting**: Para seguridad
9. **Agregar health checks**: Para monitoreo

## 🏗️ Patrones de Desarrollo

### Agregar una Nueva Entidad

1. **Crear el modelo** en `Models/`
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

2. **Agregar DbSet** en `ApplicationDbContext`
```csharp
public DbSet<Product> Products { get; set; }
```

3. **Crear DTOs** en `DTOs/`
4. **Crear Repository** en `Repositories/`
5. **Crear Service** en `Services/`
6. **Crear Controller** en `Controllers/`
7. **Agregar mappings** en `MappingProfile`
8. **Registrar servicios** en `ServiceCollectionExtensions`

Esta estructura te proporciona una base sólida y escalable para desarrollar APIs REST en .NET siguiendo las mejores prácticas de la industria.







# Instalar herramientas EF si no las tienes
dotnet tool install --global dotnet-ef

# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar migración a la base de datos
dotnet ef database update

# Para revertir migraciones (si es necesario)
dotnet ef migrations remove

# Para ver el estado de las migraciones
dotnet ef migrations list

# ---------------------------------------------------
 Levantar MySQL con Docker (opcional)
docker-compose up -d

# Ejecutar migraciones
dotnet ef database update

# Ejecutar la API
dotnet run
dotnet run --urls http://localhost:5096

# La API estará disponible en: https://localhost:5001 o http://localhost:5000