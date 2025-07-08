# NetAPI Template

Este proyecto es una base robusta para el desarrollo de aplicaciones web modernas, combinando una API RESTful en .NET 9 y un frontend desacoplado en React. Incluye autenticación JWT, gestión de usuarios, módulos, permisos, utilidades administrativas, integración con email y buenas prácticas de seguridad y arquitectura.

## Documentación

Toda la documentación técnica y funcional se encuentra en la carpeta [`docs`](./docs/), organizada por temas y funcionalidades.

---

### 📚 Índice de Documentos por Categoría

#### 📂 api
- [UTILS_API_DOCUMENTATION.md](./docs/api/UTILS_API_DOCUMENTATION.md): Documentación de los endpoints del controller `UtilsController` para consumo desde el frontend.

#### 📂 features
- [UTILS_SYSTEM_SUMMARY.md](./docs/features/UTILS_SYSTEM_SUMMARY.md): Descripción general del sistema y utilidades administrativas implementadas.
- [PASSWORD_RESET_IMPLEMENTATION.md](./docs/features/PASSWORD_RESET_IMPLEMENTATION.md): Detalles técnicos y de seguridad del sistema de recuperación de contraseñas.

#### 📂 reports
- [TELEMETRY_ML_REALTIME_ANALYSIS.md](./docs/reports/TELEMETRY_ML_REALTIME_ANALYSIS.md): Análisis completo sobre la implementación de telemetría, alertas, reportes en tiempo real, ML y feeds en tiempo real.
- [AZURE_FUNCTIONS_MIGRATION_ANALYSIS.md](./docs/reports/AZURE_FUNCTIONS_MIGRATION_ANALYSIS.md): Análisis detallado sobre la viabilidad, pros/contras y recomendaciones para la migración de la API a Azure Functions.
- [APPSETTINGS_CLEANUP_REPORT.md](./docs/reports/APPSETTINGS_CLEANUP_REPORT.md): Reporte de limpieza y optimización de archivos de configuración.
- [TEST_CLEANUP_REPORT.md](./docs/reports/TEST_CLEANUP_REPORT.md): Acciones y resultados de la limpieza y mejora de pruebas automáticas.
- [INVESTIGATION_RESULTS.md](./docs/reports/INVESTIGATION_RESULTS.md): Hallazgos y análisis de problemas o mejoras realizadas.
- [FRONTEND_RESET_PASSWORD_PROMPT.md](./docs/reports/FRONTEND_RESET_PASSWORD_PROMPT.md): Detalles sobre la experiencia de usuario en el flujo de reset de contraseña.
- [FRONTEND_URL_FIX_REPORT.md](./docs/reports/FRONTEND_URL_FIX_REPORT.md): Reporte de corrección de URLs en el frontend.
- [ISSUES_RESOLVED.md](./docs/reports/ISSUES_RESOLVED.md): Listado de incidencias resueltas en el proyecto.

#### 📂 security
- [README_SECURITY.md](./docs/security/README_SECURITY.md): Guía de seguridad y protección de secretos.
- [SECURITY_VALIDATION_REPORT.md](./docs/security/SECURITY_VALIDATION_REPORT.md): Resultados y evidencias de pruebas de seguridad.
- [LOGS_SECURITY_VERIFICATION.md](./docs/security/LOGS_SECURITY_VERIFICATION.md): Detalles sobre el manejo y auditoría de logs de seguridad.

#### 📂 setup
- [SETUP_LOCAL.md](./docs/setup/SETUP_LOCAL.md): Guía rápida para configurar el entorno de desarrollo local.
- [CONFIGURATION.md](./docs/setup/CONFIGURATION.md): Detalles de configuración avanzada del sistema.
- [MYSQL_SETUP.md](./docs/setup/MYSQL_SETUP.md): Instrucciones para la configuración de MySQL en el proyecto.

#### 📂 deployment
- [DEPLOYMENT.md](./docs/deployment/DEPLOYMENT.md): Guía paso a paso para el despliegue seguro del sistema.

---

Para más detalles, revisa los subdirectorios dentro de `docs/` según el área de interés (features, security, reports, setup, etc.).