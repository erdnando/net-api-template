# 🔥 .NET Core 9 Backend API - Documentación

## 🚀 **Inicio Rápido**
- [Configuración Local](./setup/SETUP_LOCAL.md)
- [Configuración General](./setup/CONFIGURATION.md)
- [Setup MySQL](./setup/MYSQL_SETUP.md)

## 🔐 **Seguridad**
- [README de Seguridad](./security/README_SECURITY.md)
- [Validación de Seguridad](./security/SECURITY_VALIDATION_REPORT.md)
- [Verificación de Logs](./security/LOGS_SECURITY_VERIFICATION.md)

## 🎯 **Funcionalidades**
- [Password Reset Implementation](./features/PASSWORD_RESET_IMPLEMENTATION.md)
- [Utils System Summary](./features/UTILS_SYSTEM_SUMMARY.md)

## 🔌 **API Documentation**
- [Utils API Documentation](./api/UTILS_API_DOCUMENTATION.md)

## 🚀 **Deployment**
- [Guía de Deployment](./deployment/DEPLOYMENT.md)

## 📋 **Reportes y Resoluciones**
- [Agente Inteligente con Telemetría](./reports/TELEMETRY_ML_REALTIME_ANALYSIS.md) (Análisis completo del agente inteligente y componentes de soporte)
- [Arquitectura del Agente Inteligente](./deployment/TELEMETRY_ARCHITECTURE.md) (Diagrama visual y alternativas)
- [Azure Functions Migration Analysis](./reports/AZURE_FUNCTIONS_MIGRATION_ANALYSIS.md)
- [Issues Resolved](./reports/ISSUES_RESOLVED.md)
- [Investigation Results](./reports/INVESTIGATION_RESULTS.md)
- [Frontend URL Fix Report](./reports/FRONTEND_URL_FIX_REPORT.md)
- [AppSettings Cleanup Report](./reports/APPSETTINGS_CLEANUP_REPORT.md)
- [Test Cleanup Report](./reports/TEST_CLEANUP_REPORT.md)
- [Frontend Reset Password Prompt](./reports/FRONTEND_RESET_PASSWORD_PROMPT.md)

## 📂 **Estructura del Proyecto Backend**
```
netapi-template/
├── docs/                     # 📚 Documentación
│   ├── api/                 # 🔌 API Documentation
│   ├── features/            # 🎯 Funcionalidades
│   ├── setup/               # 🚀 Configuración
│   ├── security/            # 🔐 Documentación de seguridad
│   ├── deployment/          # 🚀 Deployment y DevOps
│   ├── reports/             # 📋 Reportes e investigaciones
│   └── archive/             # 🗄️ Documentos obsoletos
├── Controllers/             # 🎮 API Controllers
├── Services/                # 🔧 Business Logic
├── Models/                  # 📋 Data Models
├── DTOs/                    # 📦 Data Transfer Objects
└── Data/                    # 🗄️ Database Context
```

## 🔥 **Backend (.NET Core 9)**
- **Puerto**: 5000
- **Framework**: .NET Core 9
- **Base de datos**: MySQL con Entity Framework Core
- **Autenticación**: JWT Tokens
- **API Documentation**: Swagger/OpenAPI
- **Testing**: xUnit

## 🛠️ **Scripts de Desarrollo**
```bash
# Desde la raíz del proyecto
./start-backend.sh          # Iniciar solo el backend
./start-fullstack.sh        # Iniciar backend + frontend
```

## 📝 **Convenciones de Documentación Backend**

### **Nomenclatura específica para .NET:**
- `*_API_*.md` - Documentación de API y endpoints
- `*_IMPLEMENTATION.md` - Implementaciones de funcionalidades
- `*_REPORT.md` - Reportes de bugs, fixes, investigaciones
- `*_SECURITY.md` - Documentación de seguridad
- `SETUP_*.md` - Guías de configuración
- `DEPLOYMENT.md` - Guías de deployment

### **Categorización:**
- **API**: Endpoints, controllers, DTOs
- **Features**: Implementaciones de funcionalidades
- **Setup**: Configuración, instalación, ambiente local
- **Security**: Autenticación, autorización, validaciones
- **Deployment**: CI/CD, producción, docker
- **Reports**: Bug reports, investigaciones, fixes

## 🔄 **Mantenimiento**
- Usar `./cleanup-backend-docs.sh` para organizar
- Mover reportes antiguos a `/archive`
- Mantener API documentation actualizada
- Revisar seguridad mensualmente

---
**Framework**: .NET Core 9  
**Última actualización**: ${new Date().toLocaleDateString('es-ES')}
