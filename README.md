# 🚀 NetAPI Template - .NET 9 Secure REST API

Una API REST segura desarrollada con .NET 9, MySQL, JWT Authentication y buenas prácticas de seguridad siguiendo OWASP Top 10.

## 🔒 **IMPORTANTE - Configuración de Seguridad**

**⚠️ Antes de ejecutar en desarrollo:**

1. **Configurar variables de entorno:**
   ```bash
   cp .env.example .env
   # Editar .env con valores reales
   ```

2. **Configurar appsettings:**
   ```bash
   cp appsettings.template.json appsettings.json
   # Actualizar con valores de producción
   ```

3. **Verificar seguridad antes de subir a GitHub:**
   ```bash
   ./security-check.sh
   ```

## ⚡ **Inicio Rápido**

### Prerequisitos
- .NET 9 SDK
- MySQL 8.0+
- Visual Studio Code o Visual Studio

### Instalación

1. **Clonar el repositorio:**
   ```bash
   git clone <tu-repo>
   cd netapi-template
   ```

2. **Configurar variables de entorno:**
   ```bash
   cp .env.example .env
   # Editar .env con tus valores reales
   ```

3. **Restaurar paquetes:**
   ```bash
   dotnet restore
   ```

4. **Ejecutar migraciones:**
   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la aplicación:**
   ```bash
   dotnet run
   ```

6. **Acceder a Swagger:**
   - Desarrollo: `https://localhost:7001/swagger`
   - Producción: `https://tu-dominio.com/swagger`

## 🛡️ **Características de Seguridad**

### ✅ Implementadas
- **Authentication & Authorization**: JWT con roles
- **Input Validation**: Data Annotations y validaciones personalizadas  
- **SQL Injection Protection**: Entity Framework Core
- **HTTPS Enforcement**: Configurado por defecto
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