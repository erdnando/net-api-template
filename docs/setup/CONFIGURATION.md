# 🔧 Guía de Configuración - Variables de Entorno

## 📋 **Métodos de Configuración**

Tu aplicación ahora soporta múltiples formas de configurar valores sensibles:

### **1. Archivo .env (Recomendado para Desarrollo)**

```bash
# Crear archivo .env para desarrollo local
cp .env.example .env
# Editar .env con tus valores reales
```

**Características:**
- ✅ Se carga automáticamente al iniciar la aplicación
- ✅ Ignorado por Git (protegido)
- ✅ Fácil de usar en desarrollo
- ❌ Solo para desarrollo local

### **2. Variables de Entorno del Sistema**

```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=ReactTemplateDb;Uid=root;Pwd=tu_password;"
export JwtSettings__SecretKey="tu_jwt_secret_aqui"

# Windows
set ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=ReactTemplateDb;Uid=root;Pwd=tu_password;
set JwtSettings__SecretKey=tu_jwt_secret_aqui
```

**Características:**
- ✅ Funciona en cualquier entorno
- ✅ Seguro para producción
- ✅ Compatible con Docker/Kubernetes
- ❌ Más complejo de gestionar

### **3. appsettings.{Environment}.json**

```bash
# Para desarrollo específico
cp appsettings.template.json appsettings.Development.json
# Editar con valores de desarrollo
```

**Características:**
- ✅ Específico por entorno
- ✅ Estructura clara
- ❌ Puede contener secretos si no se maneja bien
- ⚠️ Debe estar en .gitignore

## 🔐 **Formato de Variables de Entorno**

.NET usa el formato `Sección__Propiedad` para mapear a la configuración:

```bash
# appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "valor"
  },
  "JwtSettings": {
    "SecretKey": "valor"
  }
}

# Variables de entorno equivalentes
ConnectionStrings__DefaultConnection=valor
JwtSettings__SecretKey=valor
```

## 📝 **Variables Requeridas**

### **🗄️ Base de Datos**
```bash
ConnectionStrings__DefaultConnection=Server=host;Port=3306;Database=db;Uid=user;Pwd=password;
ConnectionStrings__DevelopmentConnection=Server=host;Port=3306;Database=db_dev;Uid=user;Pwd=password;
```

### **🔐 JWT Authentication**
```bash
# Generar con: openssl rand -base64 64
JwtSettings__SecretKey=tu_jwt_secret_de_al_menos_256_bits
```

### **📧 SMTP (Email)**
```bash
SmtpSettings__Host=smtp.gmail.com
SmtpSettings__Port=587
SmtpSettings__Username=tu_email@gmail.com
SmtpSettings__Password=tu_app_password_gmail
SmtpSettings__FromEmail=noreply@tudominio.com
SmtpSettings__FromName=Tu Sistema
SmtpSettings__SimulateMode=false  # true para simular, false para enviar real
```

### **⚙️ Configuraciones Opcionales**
```bash
PasswordResetSettings__TokenExpirationMinutes=30
PasswordResetSettings__MaxResetRequestsPerDay=3
```

## 🚀 **Configuración por Entorno**

### **Desarrollo Local**
1. Usar archivo `.env`
2. `SimulateMode=true` para emails
3. Base de datos local

### **Staging/Testing**
1. Variables de entorno del sistema
2. `SimulateMode=false` con SMTP real
3. Base de datos de pruebas

### **Producción**
1. Azure Key Vault o variables de entorno
2. `SimulateMode=false` con SMTP de producción
3. Base de datos de producción

## 🛠️ **Comandos Útiles**

### **Generar JWT Secret**
```bash
# Opción 1: OpenSSL
openssl rand -base64 64

# Opción 2: PowerShell
[System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString() + [System.Guid]::NewGuid().ToString()))

# Opción 3: Online
# https://generate-random.org/api-key-generator (64 caracteres)
```

### **Verificar Configuración**
```bash
# Verificar que las variables se cargan
dotnet run --environment Development

# Ver configuración actual (sin mostrar secretos)
dotnet run --environment Development --verbosity normal
```

### **Docker**
```bash
# Pasar variables al contenedor
docker run -e ConnectionStrings__DefaultConnection="tu_connection_string" \
           -e JwtSettings__SecretKey="tu_jwt_secret" \
           tu_app_image

# Usar archivo de variables
docker run --env-file .env tu_app_image
```

## 🔍 **Troubleshooting**

### **Problema: "JWT SecretKey is required"**
```bash
# Verificar que la variable existe
echo $JwtSettings__SecretKey

# O en el archivo .env
cat .env | grep JwtSettings__SecretKey
```

### **Problema: "Database connection string is required"**
```bash
# Verificar conexión a base de datos
echo $ConnectionStrings__DefaultConnection

# Probar conexión
mysql -h localhost -u root -p -e "SHOW DATABASES;"
```

### **Problema: Variables no se cargan**
1. Verificar que el archivo `.env` existe
2. Verificar que `DotNetEnv` está instalado
3. Verificar que `Env.Load()` está en `Program.cs`

## 🔒 **Seguridad**

### **✅ Hacer**
- Usar `.env` para desarrollo local
- Mantener `.env` en `.gitignore`
- Usar Azure Key Vault en producción
- Rotar secretos periódicamente

### **❌ No Hacer**
- Subir `.env` a Git
- Hardcodear secretos en código
- Usar secretos de desarrollo en producción
- Compartir secretos por email/chat

## 📚 **Referencias**

- [.NET Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Environment Variables](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/#environment-variables)
- [Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
- [Docker Environment Variables](https://docs.docker.com/compose/environment-variables/)
