# 🔒 Guía de Seguridad - Protección de Secretos

## ⚠️ IMPORTANTE: Antes de subir a GitHub

### 1. Variables de Entorno (.env)

**NUNCA subas archivos con secretos reales**. Usa variables de entorno:

```bash
# Crear archivo .env (ya existe)
touch .env
```

**Contenido del .env:**
```env
# Base de datos
DB_PASSWORD=tu_password_seguro
DB_HOST=localhost
DB_PORT=3306
DB_NAME=ReactTemplateDb
DB_USER=root

# JWT
JWT_SECRET_KEY=tu_jwt_secret_super_seguro_aqui

# SMTP (Email)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=tu_email@gmail.com
SMTP_PASSWORD=tu_app_password_gmail
SMTP_FROM_EMAIL=noreply@tudominio.com
SMTP_FROM_NAME=Tu Sistema

# Configuraciones
PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES=30
PASSWORD_RESET_MAX_REQUESTS_PER_DAY=3
```

### 2. Configurar .gitignore

Asegúrate de que `.gitignore` incluya:

```gitignore
# Secretos y configuraciones sensibles
.env
.env.local
.env.production
.env.staging
appsettings.json
appsettings.Development.json
appsettings.Production.json

# Logs y archivos temporales
logs/
*.log

# Archivos de build
bin/
obj/
```

### 3. Protección de Logs 🔒

**Los archivos de logs están completamente protegidos:**

```gitignore
# Protección automática en .gitignore
[Ll]og/        # Carpetas log/Log
[Ll]ogs/       # Carpetas logs/Logs  
*.log          # Archivos .log individuales
```

**Uso seguro de logs:**
- ✅ Usar carpeta `logs/` para todos los archivos de log
- ✅ Los logs se rotan automáticamente (configurado en Program.cs)
- ❌ NUNCA loguear passwords, tokens o datos sensibles completos
- ❌ NUNCA commitear archivos de log al repositorio

**Verificación de protección:**
```bash
# Verificar que logs está ignorado
git check-ignore logs/

# Debe mostrar: logs/
```

### 4. Sistema de Templates de Configuración 📝

**Este proyecto usa un sistema seguro de templates:**

**✅ Archivos que SÍ se suben a GitHub:**
```
appsettings.template.json              ← Template principal con variables ${VAR}
appsettings.Development.template.json  ← Template para desarrollo
.env.example                          ← Ejemplo de variables de entorno
```

**❌ Archivos que NUNCA se suben:**
```
appsettings.json                      ← Configuración real (generada localmente)
appsettings.Development.json          ← Configuración de desarrollo real
.env                                  ← Variables de entorno reales
```

**🔧 Configuración Local:**

1. **Crear archivos de configuración reales:**
```bash
# Copiar template y rellenar con valores reales
cp appsettings.template.json appsettings.json
cp appsettings.Development.template.json appsettings.Development.json

# Editar los archivos copiados con datos reales
```

2. **Configurar variables de entorno:**
```bash
# Copiar ejemplo y rellenar
cp .env.example .env
# Editar .env con valores reales
```

**🎯 Ventajas del Sistema de Templates:**
- ✅ **Seguridad**: No hay riesgo de subir secretos por error
- ✅ **Colaboración**: Nuevos desarrolladores saben qué configurar
- ✅ **Mantenimiento**: Cambios de estructura se reflejan en templates
- ✅ **Documentación**: Templates sirven como documentación viva

### 5. Archivos de Template

Crea archivos de ejemplo SIN secretos reales:

**appsettings.template.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Uid=${DB_USER};Pwd=${DB_PASSWORD};"
  },
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "NetApiTemplate",
    "Audience": "NetApiTemplate",
    "ExpirationHours": 24
  },
  "SmtpSettings": {
    "SimulateMode": true,
    "Host": "${SMTP_HOST}",
    "Port": ${SMTP_PORT},
    "Username": "${SMTP_USERNAME}",
    "Password": "${SMTP_PASSWORD}",
    "FromEmail": "${SMTP_FROM_EMAIL}",
    "FromName": "${SMTP_FROM_NAME}"
  }
}
```

### 6. Azure Key Vault (Producción)

Para producción en Azure:

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
    var keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";
    
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential());
}
```

### 7. GitHub Secrets (CI/CD)

En GitHub Repository → Settings → Secrets and variables → Actions:

- `DB_PASSWORD`
- `JWT_SECRET_KEY`
- `SMTP_PASSWORD`
- etc.

### 8. Docker Secrets

Para contenedores:

```dockerfile
# NO incluir secretos en el Dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
# Los secretos se pasan como variables de entorno al ejecutar
```

```bash
# Ejecutar con secretos
docker run -e DB_PASSWORD=secret -e JWT_SECRET_KEY=secret tu-app
```

## ✅ Lista de Verificación Pre-GitHub

- [ ] Archivo `.env` en `.gitignore`
- [ ] Remover secretos de `appsettings.json`
- [ ] Crear `appsettings.template.json`
- [ ] Documentar variables de entorno necesarias
- [ ] Verificar que no hay API keys hardcodeadas
- [ ] Revisar logs para no exponer secretos
- [ ] Configurar GitHub Secrets si usas CI/CD

## 🚨 Qué NO hacer

❌ **NUNCA hagas esto:**
```json
{
  "Password": "mi_password_real_123",
  "ApiKey": "sk-1234567890abcdef",
  "ConnectionString": "Server=prod.com;Pwd=real_password;"
}
```

✅ **HAZ esto en su lugar:**
```json
{
  "Password": "${DB_PASSWORD}",
  "ApiKey": "${API_KEY}",
  "ConnectionString": "Server=${DB_HOST};Pwd=${DB_PASSWORD};"
}
```

## 🔍 Herramientas de Detección

- **git-secrets**: Detecta secretos antes del commit
- **TruffleHog**: Escanea repositorios en busca de secretos
- **GitHub Secret Scanning**: Automático en repos públicos

## 📝 Ejemplo de Setup para Desarrolladores

```bash
# 1. Clonar repo
git clone tu-repo

# 2. Copiar template
cp appsettings.template.json appsettings.json

# 3. Configurar .env
cp .env.example .env
# Editar .env con valores reales

# 4. Ejecutar
dotnet run
```