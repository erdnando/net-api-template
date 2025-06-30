# 🛠️ API Utils - Documentación para Frontend

Esta documentación describe cómo consumir los endpoints del controller `UtilsController` desde el frontend.

## 🔐 **Autenticación Requerida**

Todos los endpoints requieren:
- **JWT Token** en el header `Authorization: Bearer {token}`
- **Rol de Administrador** para acceder a las funciones

---

## 📡 **Endpoints Disponibles**

### 1. **🔄 Reiniciar Intentos de Reset de Contraseña**

**POST** `/api/utils/reset-password-attempts`

Reinicia el contador de intentos de reset para un usuario específico.

#### Request Body:
```json
{
  "email": "usuario@ejemplo.com"
}
```

#### Response Success (200):
```json
{
  "success": true,
  "message": "Intentos de reset reiniciados exitosamente. Se eliminaron 2 tokens.",
  "tokensRemoved": 2,
  "userEmail": "usuario@ejemplo.com",
  "processedAt": "2025-06-29T15:30:00Z"
}
```

#### Response Error (404):
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "tokensRemoved": 0,
  "userEmail": "usuario@ejemplo.com",
  "processedAt": "2025-06-29T15:30:00Z"
}
```

#### Ejemplo JavaScript:
```javascript
async function resetPasswordAttempts(userEmail) {
  try {
    const response = await fetch('/api/utils/reset-password-attempts', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify({ email: userEmail })
    });
    
    const result = await response.json();
    
    if (result.success) {
      alert(`✅ ${result.message}`);
    } else {
      alert(`❌ ${result.message}`);
    }
    
    return result;
  } catch (error) {
    console.error('Error:', error);
    alert('Error de conexión');
  }
}
```

---

### 2. **📊 Obtener Estadísticas de Reset**

**GET** `/api/utils/password-reset-stats`

Obtiene estadísticas detalladas de intentos de reset en las últimas 24 horas.

#### Response (200):
```json
{
  "userStats": [
    {
      "email": "usuario1@ejemplo.com",
      "attemptCount": 3,
      "lastAttempt": "2025-06-29T14:30:00Z",
      "isAtLimit": true,
      "hoursUntilReset": 2
    },
    {
      "email": "usuario2@ejemplo.com",
      "attemptCount": 1,
      "lastAttempt": "2025-06-29T13:00:00Z",
      "isAtLimit": false,
      "hoursUntilReset": 11
    }
  ],
  "totalUsers": 2,
  "totalAttempts": 4,
  "usersAtLimit": 1,
  "generatedAt": "2025-06-29T15:30:00Z"
}
```

#### Ejemplo JavaScript:
```javascript
async function getPasswordResetStats() {
  try {
    const response = await fetch('/api/utils/password-reset-stats', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    const stats = await response.json();
    
    // Mostrar en tabla o componente
    displayStatsTable(stats);
    
    return stats;
  } catch (error) {
    console.error('Error:', error);
  }
}

function displayStatsTable(stats) {
  console.log(`📊 Estadísticas:`);
  console.log(`Total usuarios: ${stats.totalUsers}`);
  console.log(`Total intentos: ${stats.totalAttempts}`);
  console.log(`Usuarios en límite: ${stats.usersAtLimit}`);
  
  stats.userStats.forEach(user => {
    console.log(`${user.email}: ${user.attemptCount} intentos ${user.isAtLimit ? '🔴 EN LÍMITE' : '✅'}`);
  });
}
```

---

### 3. **🧹 Limpiar Tokens Expirados**

**POST** `/api/utils/cleanup-expired-tokens`

Elimina todos los tokens de reset expirados del sistema.

#### Response (200):
```json
{
  "success": true,
  "message": "Se eliminaron 5 tokens expirados exitosamente.",
  "tokensRemoved": 5,
  "processedAt": "2025-06-29T15:30:00Z"
}
```

#### Ejemplo JavaScript:
```javascript
async function cleanupExpiredTokens() {
  try {
    const response = await fetch('/api/utils/cleanup-expired-tokens', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    const result = await response.json();
    alert(`🧹 ${result.message}`);
    
    return result;
  } catch (error) {
    console.error('Error:', error);
  }
}
```

---

### 4. **🔧 Obtener Configuración del Sistema**

**GET** `/api/utils/system-config`

Obtiene la configuración actual del sistema.

#### Response (200):
```json
{
  "maxResetRequestsPerDay": 3,
  "tokenExpirationMinutes": 30,
  "smtpSimulateMode": true,
  "frontendUrl": "http://localhost:3000"
}
```

---

### 5. **💚 Estado de Salud del Sistema**

**GET** `/api/utils/system-health`

Obtiene el estado de salud del sistema.

#### Response (200):
```json
{
  "databaseConnection": true,
  "emailService": true,
  "activeUsers": 15,
  "pendingResetTokens": 3,
  "expiredTokens": 2,
  "lastChecked": "2025-06-29T15:30:00Z"
}
```

---

### 6. **🔍 Verificar Existencia de Usuario**

**GET** `/api/utils/user-exists?email=usuario@ejemplo.com`

Verifica si un usuario existe en el sistema.

#### Response (200):
```json
{
  "success": true,
  "operation": "USER_EXISTS_CHECK",
  "message": "Usuario encontrado",
  "processedAt": "2025-06-29T15:30:00Z",
  "data": {
    "exists": true,
    "email": "usuario@ejemplo.com"
  }
}
```

---

## 🎨 **Componente React Ejemplo**

```jsx
import React, { useState, useEffect } from 'react';

const PasswordResetManager = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(false);
  const [emailToReset, setEmailToReset] = useState('');

  // Cargar estadísticas al montar
  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    setLoading(true);
    try {
      const response = await fetch('/api/utils/password-reset-stats', {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
      });
      const data = await response.json();
      setStats(data);
    } catch (error) {
      console.error('Error loading stats:', error);
    }
    setLoading(false);
  };

  const handleResetAttempts = async (e) => {
    e.preventDefault();
    if (!emailToReset) return;

    setLoading(true);
    try {
      const response = await fetch('/api/utils/reset-password-attempts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({ email: emailToReset })
      });
      
      const result = await response.json();
      
      if (result.success) {
        alert(`✅ ${result.message}`);
        setEmailToReset('');
        loadStats(); // Recargar estadísticas
      } else {
        alert(`❌ ${result.message}`);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error de conexión');
    }
    setLoading(false);
  };

  const handleCleanupExpired = async () => {
    if (!confirm('¿Limpiar todos los tokens expirados?')) return;

    try {
      const response = await fetch('/api/utils/cleanup-expired-tokens', {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
      });
      
      const result = await response.json();
      alert(`🧹 ${result.message}`);
      loadStats();
    } catch (error) {
      console.error('Error:', error);
    }
  };

  return (
    <div className="password-reset-manager">
      <h2>🔄 Gestión de Reset de Contraseñas</h2>
      
      {/* Formulario para resetear intentos */}
      <form onSubmit={handleResetAttempts} className="reset-form">
        <input
          type="email"
          placeholder="Email del usuario"
          value={emailToReset}
          onChange={(e) => setEmailToReset(e.target.value)}
          required
        />
        <button type="submit" disabled={loading}>
          {loading ? 'Procesando...' : 'Resetear Intentos'}
        </button>
      </form>

      {/* Botón de limpieza */}
      <button onClick={handleCleanupExpired} disabled={loading}>
        🧹 Limpiar Tokens Expirados
      </button>

      {/* Estadísticas */}
      {stats && (
        <div className="stats-section">
          <h3>📊 Estadísticas (últimas 24h)</h3>
          <div className="stats-summary">
            <p>Total usuarios: {stats.totalUsers}</p>
            <p>Total intentos: {stats.totalAttempts}</p>
            <p>Usuarios en límite: {stats.usersAtLimit}</p>
          </div>
          
          <table className="stats-table">
            <thead>
              <tr>
                <th>Usuario</th>
                <th>Intentos</th>
                <th>Último Intento</th>
                <th>Estado</th>
                <th>Acción</th>
              </tr>
            </thead>
            <tbody>
              {stats.userStats.map((user, index) => (
                <tr key={index}>
                  <td>{user.email}</td>
                  <td>{user.attemptCount}</td>
                  <td>{new Date(user.lastAttempt).toLocaleString()}</td>
                  <td>{user.isAtLimit ? '🔴 EN LÍMITE' : '✅ OK'}</td>
                  <td>
                    <button 
                      onClick={() => setEmailToReset(user.email)}
                      disabled={!user.isAtLimit}
                    >
                      Resetear
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default PasswordResetManager;
```

---

## 🔒 **Consideraciones de Seguridad**

1. **Solo Administradores**: Todos los endpoints validan que el usuario tenga permisos de administrador
2. **Logging de Auditoría**: Todas las acciones se registran en logs de seguridad
3. **Rate Limiting**: Los endpoints están sujetos a rate limiting
4. **Validación de Entrada**: Todos los datos de entrada son validados
5. **IP Tracking**: Se registra la IP del cliente para auditoría

---

## 🚀 **Próximas Funcionalidades**

El controller está diseñado para ser extensible. Futuras funcionalidades pueden incluir:
- Gestión de usuarios en lote
- Exportar logs de auditoría
- Configuración dinámica del sistema
- Estadísticas de uso de la API
- Monitoreo de rendimiento

---

## 📞 **Soporte**

Para dudas o problemas con la implementación, revisa:
1. Los logs de la aplicación en `logs/app.log`
2. Los códigos de respuesta HTTP
3. La documentación de Swagger en `/swagger`
