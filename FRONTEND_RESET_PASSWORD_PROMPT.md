# 🚀 PROMPT para GitHub Copilot - Implementación Frontend Reset Password

## 📋 **CONTEXTO DEL PROYECTO**

Implementar funcionalidad completa de "Olvidé mi contraseña" en una aplicación React + TypeScript que usa Material-UI. El **backend YA ESTÁ IMPLEMENTADO** con una arquitectura superior a estándares comunes.

## 🎯 **ARQUITECTURA DEL BACKEND (YA IMPLEMENTADA)**

### **Endpoints Disponibles:**
```typescript
// ✅ Ya implementados y funcionando
POST /api/Users/forgot-password
POST /api/Users/reset-password
```

### **DTOs del Backend:**
```typescript
// Request para solicitar reset
interface ForgotPasswordRequest {
  email: string;
}

// Response del forgot-password
interface ForgotPasswordResponse {
  success: boolean;
  message: string;
}

// Request para reset definitivo
interface ResetPasswordRequest {
  token: string;           // ⭐ NOTA: NO requiere email (más seguro)
  newPassword: string;
  confirmPassword: string;
}

// Response del reset-password
interface ResetPasswordResponse {
  success: boolean;
  message: string;
}
```

## 🔐 **¿POR QUÉ ESTA ARQUITECTURA ES SUPERIOR?**

### **❌ Enfoque Tradicional (Menos Seguro):**
```typescript
// Muchas implementaciones hacen esto:
interface ResetPasswordRequest {
  token: string;
  email: string;        // ← Requiere email otra vez
  newPassword: string;
}
```

**Problemas del enfoque tradicional:**
- 🚨 **Ataque Cross-Email**: Usar token de UserA con email de UserB
- 🚨 **Más superficie de ataque**: Frontend debe mantener estado del email
- 🚨 **Propenso a errores**: Usuario puede cambiar email en URL
- 🚨 **Menos UX**: Usuario debe recordar qué email usó

### **✅ Nuestro Enfoque (Más Seguro):**
```typescript
// Nuestro backend es más inteligente:
interface ResetPasswordRequest {
  token: string;           // Token ya contiene TODA la información
  newPassword: string;
  confirmPassword: string;
}
```

**Ventajas de nuestro enfoque:**
- 🛡️ **Más seguro**: Token criptográficamente vinculado al usuario
- 🎯 **Más simple**: Frontend no maneja estado de email
- 💡 **Mejor UX**: Usuario solo necesita el link del email
- 🔒 **Imposible cross-email attacks**: Token es autosuficiente

## 📝 **REQUERIMIENTOS DE IMPLEMENTACIÓN**

### **1. Estructura de Archivos a Crear:**
```
src/
├── services/
│   └── authService.ts              # ← Agregar métodos de reset
├── pages/
│   ├── ForgotPassword/
│   │   ├── index.ts               # ← Exportaciones
│   │   └── ForgotPassword.tsx     # ← Componente principal
│   └── ResetPassword/
│       ├── index.ts               # ← Exportaciones  
│       └── ResetPassword.tsx      # ← Componente para reset
├── hooks/
│   └── useResetPassword.ts        # ← Hook personalizado (opcional)
└── types/
    └── auth.ts                    # ← Tipos TypeScript
```

### **2. Rutas a Configurar:**
```typescript
// En tu router principal
<Routes>
  {/* Rutas públicas */}
  <Route path="/forgot-password" element={<ForgotPassword />} />
  <Route path="/reset-password" element={<ResetPassword />} />
  
  {/* Rutas protegidas existentes... */}
</Routes>
```

### **3. Flujo de Usuario:**
1. **Login** → Link "¿Olvidaste tu contraseña?"
2. **ForgotPassword** → Usuario ingresa email → Envío exitoso
3. **Email** → Usuario recibe link: `http://app.com/reset-password?token=abc123`
4. **ResetPassword** → Usuario ingresa nueva contraseña → Reset exitoso → Redirect a login

## 🛠️ **IMPLEMENTACIÓN REQUERIDA**

### **A. Actualizar AuthService**
```typescript
// En services/authService.ts - AGREGAR estos métodos:

export const authService = {
  // ... métodos existentes ...
  
  // Solicitar reset de contraseña
  async forgotPassword(email: string): Promise<ForgotPasswordResponse> {
    // Implementar llamada a POST /api/Users/forgot-password
  },
  
  // Realizar reset de contraseña
  async resetPassword(token: string, newPassword: string, confirmPassword: string): Promise<ResetPasswordResponse> {
    // Implementar llamada a POST /api/Users/reset-password
    // NOTA: Solo token, newPassword y confirmPassword - NO email
  }
};
```

### **B. Componente ForgotPassword**
```typescript
// En pages/ForgotPassword/ForgotPassword.tsx

// Requerimientos:
// ✅ Formulario con email usando Material-UI
// ✅ Validación de email
// ✅ Estados: idle, loading, success, error
// ✅ Mensaje de éxito: "Revisa tu email para continuar"
// ✅ Link para volver al login
// ✅ Manejo de errores (email no existe, rate limiting, etc.)
```

### **C. Componente ResetPassword**
```typescript
// En pages/ResetPassword/ResetPassword.tsx

// Requerimientos:
// ✅ Leer token desde URL query params
// ✅ Formulario con newPassword y confirmPassword
// ✅ Validación de contraseñas (min 8 chars, matching, etc.)
// ✅ Estados: idle, loading, success, error
// ✅ Redirect automático al login después de éxito
// ✅ Manejo de errores (token expirado, token inválido, etc.)
// ✅ Mostrar feedback visual del progreso
```

### **D. Integración con Login**
```typescript
// En tu componente Login existente - AGREGAR:

<Link 
  to="/forgot-password" 
  variant="body2"
  sx={{ textDecoration: 'none' }}
>
  ¿Olvidaste tu contraseña?
</Link>
```

## 🎨 **ESPECIFICACIONES DE UX/UI**

### **Diseño Material-UI:**
- ✅ Consistent con el diseño existente
- ✅ Responsive design
- ✅ Loading states con CircularProgress
- ✅ Error states con Alert severity="error"
- ✅ Success states con Alert severity="success"
- ✅ Form validation en tiempo real

### **Estados de Carga:**
```typescript
interface ResetPasswordState {
  loading: boolean;
  success: boolean;
  error: string | null;
  step: 'form' | 'success' | 'redirecting';
}
```

## 🔍 **VALIDACIONES REQUERIDAS**

### **ForgotPassword:**
- ✅ Email válido (regex)
- ✅ Email requerido
- ✅ Rate limiting feedback

### **ResetPassword:**
- ✅ Token presente en URL
- ✅ Contraseña mínimo 8 caracteres
- ✅ Contraseñas coinciden
- ✅ Token válido (feedback del servidor)

## 🧪 **TESTING CONSIDERACIONES**

- ✅ Token válido: `POST /api/Users/reset-password` con token real
- ✅ Token expirado: Manejar error 400
- ✅ Token inválido: Manejar error 400
- ✅ Email inexistente en forgot: Manejar según respuesta del servidor
- ✅ Rate limiting: Manejar error 429

## 📚 **EJEMPLOS DE RESPUESTAS DEL SERVIDOR**

```typescript
// Éxito en forgot-password
{
  "success": true,
  "message": "Se ha enviado un enlace de recuperación a tu email"
}

// Éxito en reset-password  
{
  "success": true,
  "message": "Contraseña actualizada correctamente"
}

// Error (token expirado)
{
  "success": false,
  "message": "El token ha expirado. Solicita un nuevo enlace de recuperación"
}
```

## 🎯 **RESULTADO ESPERADO**

Al finalizar deberías tener:
1. ✅ Flujo completo de reset funcional
2. ✅ UX pulida y consistente
3. ✅ Manejo robusto de errores
4. ✅ Integración perfecta con el sistema existente
5. ✅ Código mantenible y tipado

---

**💡 NOTA IMPORTANTE:** Este backend implementa un patrón de seguridad superior. El token es autosuficiente y no requiere email adicional, lo que elimina vectores de ataque comunes. Asegúrate de NO agregar el email al ResetPasswordRequest - esto reduciría la seguridad del sistema.
