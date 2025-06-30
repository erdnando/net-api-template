# 🔧 Guía de Configuración Local

## ⚡ Setup Rápido para Desarrollo

### 1. **Configurar Variables de Entorno**

```bash
# Copiar el template
cp .env.example .env

# Editar con tus valores reales
nano .env  # o tu editor preferido
```

### 2. **Configurar Base de Datos**

```bash
# Opción A: Usar Docker (Recomendado)
docker-compose up -d mysql

# Opción B: MySQL local
# Instalar MySQL y crear la base de datos:
# CREATE DATABASE ReactTemplateDb;
# CREATE DATABASE ReactTemplateDb_Dev;
```

### 3. **Configurar Email (SMTP)**

Para testing local, puedes usar:

```env
# Modo simulación (NO envía emails reales)
SmtpSettings__SimulateMode=true

# Modo real (requiere configuración SMTP válida)
SmtpSettings__SimulateMode=false
SmtpSettings__Host=smtp.gmail.com
SmtpSettings__Username=tu_email@gmail.com
SmtpSettings__Password=tu_app_password_gmail
```

### 4. **Ejecutar la Aplicación**

```bash
# Restaurar paquetes
dotnet restore

# Aplicar migraciones
dotnet ef database update

# Ejecutar
dotnet run
```

## 🔒 **Flujo de Seguridad**

### ✅ **Lo que SÍ se sube a GitHub:**
- `.env.example` (template sin secretos)
- `appsettings.template.json` (template sin secretos)
- `appsettings.json` (con placeholders)
- Código fuente
- Documentación

### ❌ **Lo que NO se sube a GitHub:**
- `.env` (con secretos reales)
- `appsettings.Development.json` (si tiene secretos)
- Logs con información sensible
- Archivos de base de datos

## 👥 **Para Nuevos Desarrolladores**

Cuando alguien clona el repositorio:

```bash
# 1. Clonar
git clone tu-repositorio
cd netapi-template

# 2. Configurar entorno
cp .env.example .env
# Editar .env con valores reales

# 3. Configurar base de datos
cp appsettings.template.json appsettings.Development.json
# Editar appsettings.Development.json si necesario

# 4. Ejecutar
dotnet restore
dotnet ef database update
dotnet run
```

## 🚀 **Para Despliegue**

### **Desarrollo Local:**
- Usar `.env` + `appsettings.Development.json`

### **Staging/Testing:**
- Variables de entorno del servidor
- Azure App Configuration

### **Producción:**
- Azure Key Vault
- Variables de entorno seguras del servidor
- CI/CD con GitHub Secrets

## 🔍 **Verificación**

Antes de hacer commit:

```bash
# Verificar que .env no está staged
git status | grep .env

# Debe mostrar solo:
# ?? .env.example

# Ejecutar verificación de seguridad
./security-check.sh
```

## ⚠️ **Troubleshooting**

### Error: "Database connection string is required"
- Verificar que `.env` existe y tiene `ConnectionStrings__DefaultConnection`

### Error: "JWT SecretKey is required"  
- Verificar que `.env` tiene `JwtSettings__SecretKey`

### La API no envía emails
- Verificar configuración SMTP en `.env`
- Para testing, usar `SmtpSettings__SimulateMode=true`
