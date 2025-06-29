# 🛠️ Sistema de Utils/Administración - Documentación

## 📋 **Resumen**

Se ha implementado un sistema completo de utilidades administrativas con el **controller `UtilsController`** que permite a los administradores gestionar aspectos del sistema, especialmente el **reset de intentos de contraseña**.

---

## 🏗️ **Arquitectura Implementada**

### **Capas Creadas:**

1. **DTOs** (`DTOs/UtilsDTOs.cs`)
   - `UtilsResetPasswordAttemptsRequestDto`
   - `UtilsResetPasswordAttemptsResponseDto` 
   - `UtilsPasswordResetStatsDto`
   - `CleanupExpiredTokensResponseDto`
   - `SystemConfigDto`
   - `SystemHealthDto`
   - Y más DTOs para extensibilidad

2. **Interfaz del Servicio** (`Services/Interfaces/IUtilsService.cs`)
   - Contrato para todas las operaciones de utilidad
   - Métodos para gestión de reset, auditoría, configuración

3. **Implementación del Servicio** (`Services/UtilsService.cs`)
   - Lógica de negocio para todas las operaciones
   - Manejo de errores y logging
   - Validaciones de seguridad

4. **Controller** (`Controllers/UtilsController.cs`)
   - Endpoints REST para el frontend
   - Autenticación y autorización
   - Documentación Swagger

5. **Registro en DI** (`Program.cs`)
   - Servicio registrado en el contenedor de dependencias

---

## 🔑 **Funcionalidades Principales**

### **1. 🔄 Reset de Intentos de Contraseña**
- **Endpoint**: `POST /api/utils/reset-password-attempts`
- **Función**: Elimina tokens de reset de las últimas 24h para un usuario
- **Acceso**: Solo administradores

### **2. 📊 Estadísticas de Reset**
- **Endpoint**: `GET /api/utils/password-reset-stats`
- **Función**: Muestra intentos por usuario, límites alcanzados, etc.
- **Acceso**: Solo administradores

### **3. 🧹 Limpieza de Tokens Expirados**
- **Endpoint**: `POST /api/utils/cleanup-expired-tokens`
- **Función**: Elimina tokens que ya expiraron
- **Acceso**: Solo administradores

### **4. 🔧 Información del Sistema**
- **Configuración**: `GET /api/utils/system-config`
- **Salud**: `GET /api/utils/system-health`
- **Validaciones**: `GET /api/utils/user-exists`

---

## 🚀 **Cómo Usar desde el Frontend**

### **Ejemplo JavaScript/React:**

```javascript
// Resetear intentos de un usuario
async function resetUserAttempts(email) {
  const response = await fetch('/api/utils/reset-password-attempts', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({ email })
  });
  
  const result = await response.json();
  return result;
}

// Obtener estadísticas
async function getStats() {
  const response = await fetch('/api/utils/password-reset-stats', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  const stats = await response.json();
  return stats;
}
```

---

## 🔒 **Seguridad Implementada**

1. **Autenticación JWT**: Todos los endpoints requieren token válido
2. **Autorización por Roles**: Solo usuarios con rol "admin" pueden acceder
3. **Logging de Auditoría**: Todas las acciones se registran con IP y usuario
4. **Validación de Entrada**: DTOs validan formato de email y datos
5. **Rate Limiting**: Endpoints sujetos a límites de velocidad

---

## 📝 **Configuración Actual**

Basado en tu archivo `.env`, la configuración actual es:

```properties
# Límite de intentos por día
PasswordResetSettings__MaxResetRequestsPerDay=3

# Tiempo de expiración de tokens (minutos)
PasswordResetSettings__TokenExpirationMinutes=30
```

---

## 🛡️ **Cómo Funciona el Reset de Intentos**

1. **Usuario hace varios intentos** → Tokens se crean en `PasswordResetTokens`
2. **Llega al límite** → No puede solicitar más resets por 24h
3. **Admin usa el endpoint** → Se eliminan tokens de las últimas 24h
4. **Usuario puede volver a intentar** → Contador se reinicia

---

## 🔄 **Extensibilidad Futura**

El controller está diseñado para ser fácilmente extensible. Puedes agregar:

- **Gestión de usuarios en lote**
- **Configuración dinámica del sistema**  
- **Exportar logs de auditoría**
- **Estadísticas de uso de API**
- **Monitoreo de rendimiento**
- **Backup/restore de datos**

Solo necesitas:
1. Agregar DTOs en `UtilsDTOs.cs`
2. Definir métodos en `IUtilsService.cs`
3. Implementar en `UtilsService.cs`
4. Crear endpoints en `UtilsController.cs`

---

## ✂️ **Scripts de Mantenimiento**

También se crearon scripts SQL y bash para mantenimiento directo:

- **SQL**: `scripts/reset-password-maintenance.sql`
- **Bash**: `scripts/reset-password-attempts.sh`

---

## 📚 **Documentación Completa**

- **Frontend**: `UTILS_API_DOCUMENTATION.md`
- **Swagger**: Disponible en `/swagger` cuando ejecutes la API

---

## 🎯 **Configuración en Variables de Entorno**

El número máximo de resets se configura en:

1. **Archivo `.env`**: `PasswordResetSettings__MaxResetRequestsPerDay=3`
2. **appsettings**: `"PasswordResetSettings:MaxResetRequestsPerDay"`
3. **Variables del SO**: Para producción

¡El sistema está **listo para usar**! 🚀
