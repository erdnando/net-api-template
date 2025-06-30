# 🚀 Guía de Despliegue Seguro

## 📋 **Lista de Verificación Pre-Despliegue**

### ✅ Desarrollo Local
- [ ] Configurar `.env` con valores reales
- [ ] Copiar `appsettings.template.json` a `appsettings.json`
- [ ] Ejecutar `./security-check.sh` 
- [ ] Verificar que tests pasan
- [ ] Confirmar login de usuarios de prueba
- [ ] Probar flujo de reset de contraseña

### ✅ Pre-GitHub
- [ ] Archivo `.env` eliminado o en `.gitignore`
- [ ] Secretos removidos de `appsettings.json`
- [ ] Script `./security-check.sh` pasa sin errores
- [ ] Archivos template creados (`.env.example`, `appsettings.template.json`)
- [ ] Documentación actualizada

### ✅ Producción
- [ ] Variables de entorno configuradas en servidor
- [ ] Base de datos MySQL configurada
- [ ] Certificados SSL/TLS configurados
- [ ] SMTP configurado para emails
- [ ] Logs configurados y monitoreados
- [ ] Rate limiting configurado

## 🔐 **Configuración por Ambiente**

### Desarrollo
```bash
# .env
DB_HOST=localhost
DB_PASSWORD=tu_password_dev
JWT_SECRET_KEY=clave_desarrollo_segura
SMTP_HOST=smtp.gmail.com
SMTP_PASSWORD=tu_app_password
```

### Staging
```bash
# Variables de entorno del servidor
export DB_HOST=staging-db.company.com
export DB_PASSWORD=staging_secure_password
export JWT_SECRET_KEY=staging_jwt_secret_key
export SMTP_HOST=smtp.company.com
```

### Producción
```bash
# Azure Key Vault o similar
DB_HOST=prod-db.company.com
DB_PASSWORD=<from-key-vault>
JWT_SECRET_KEY=<from-key-vault>
SMTP_PASSWORD=<from-key-vault>
```

## ☁️ **Despliegue en Azure**

### 1. Azure App Service
```bash
# CLI Azure
az webapp config appsettings set \
  --resource-group myResourceGroup \
  --name myAppName \
  --settings \
  ConnectionStrings__DefaultConnection="Server=myserver;Database=mydb;..." \
  JwtSettings__SecretKey="my-secure-jwt-secret"
```

### 2. Azure Key Vault
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

### 3. Docker en Azure Container Instances
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "netapi-template.dll"]
```

```bash
# Ejecutar con variables de entorno
az container create \
  --resource-group myResourceGroup \
  --name myapp \
  --image myregistry.azurecr.io/myapp:latest \
  --environment-variables \
  ConnectionStrings__DefaultConnection="Server=..." \
  JwtSettings__SecretKey="..."
```

## 🐳 **Docker Compose (Desarrollo)**

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=ReactTemplateDb;Uid=root;Pwd=${DB_PASSWORD}
      - JwtSettings__SecretKey=${JWT_SECRET_KEY}
    depends_on:
      - db
    env_file:
      - .env

  db:
    image: mysql:8.0
    environment:
      - MYSQL_ROOT_PASSWORD=${DB_PASSWORD}
      - MYSQL_DATABASE=ReactTemplateDb
    volumes:
      - mysql_data:/var/lib/mysql
    ports:
      - "3306:3306"

volumes:
  mysql_data:
```

## 🔧 **CI/CD Pipeline (GitHub Actions)**

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  security-check:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Run Security Check
      run: ./security-check.sh

  deploy:
    needs: security-check
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'my-app-name'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
```

## 🔍 **Monitoreo en Producción**

### Application Insights
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddCheck("smtp", () => 
        // Verificar conexión SMTP
        HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

### Logging
```csharp
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "YourApp.Security": "Information"
    }
  }
}
```

## 🚨 **Troubleshooting**

### Error: "JWT SecretKey is required"
```bash
# Verificar configuración
echo $JWT_SECRET_KEY
# O en Azure App Service
az webapp config appsettings list --name myapp --resource-group mygroup
```

### Error: "Database connection failed"
```bash
# Verificar conexión
mysql -h yourhost -u youruser -p
# Verificar variables de entorno
echo $DB_HOST $DB_USER
```

### Error: "SMTP configuration invalid"
```bash
# Verificar configuración SMTP
telnet smtp.gmail.com 587
# Verificar App Password para Gmail
```

## 📊 **Métricas de Seguridad**

### KPIs a Monitorear
- Intentos de login fallidos por IP
- Uso de endpoints de reset de contraseña
- Accesos a endpoints protegidos sin token
- Violaciones de rate limiting
- Errores de validación de entrada

### Alertas Recomendadas
- Más de 10 intentos de login fallidos desde una IP
- Más de 100 requests por minuto desde una IP
- Errores de base de datos críticos
- Fallas en servicios externos (SMTP)

---

**🔒 Recuerda: La seguridad es un proceso continuo, no un destino**
