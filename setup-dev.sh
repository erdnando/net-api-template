#!/bin/bash
# 🚀 Script de Setup Automático para Desarrollo Local

echo "🚀 Configurando NetAPI Template para desarrollo local..."
echo "=================================================="

# Colores
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Verificar si .env existe
if [ ! -f ".env" ]; then
    echo -e "${YELLOW}⚠️  Archivo .env no encontrado${NC}"
    if [ -f ".env.example" ]; then
        echo "🔧 Copiando .env.example a .env..."
        cp .env.example .env
        echo -e "${GREEN}✅ Archivo .env creado${NC}"
        echo -e "${YELLOW}📝 IMPORTANTE: Edita .env con tus valores reales${NC}"
    else
        echo -e "${RED}❌ Error: .env.example no encontrado${NC}"
        exit 1
    fi
else
    echo -e "${GREEN}✅ Archivo .env ya existe${NC}"
fi

# Verificar si appsettings.Development.json existe
if [ ! -f "appsettings.Development.json" ]; then
    echo "🔧 Creando appsettings.Development.json..."
    cat > appsettings.Development.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "DetailedErrors": true,
  "SmtpSettings": {
    "SimulateMode": true
  }
}
EOF
    echo -e "${GREEN}✅ appsettings.Development.json creado${NC}"
else
    echo -e "${GREEN}✅ appsettings.Development.json ya existe${NC}"
fi

# Verificar herramientas necesarias
echo ""
echo "🔍 Verificando herramientas necesarias..."

# Verificar .NET
if command -v dotnet &> /dev/null; then
    dotnet_version=$(dotnet --version)
    echo -e "${GREEN}✅ .NET SDK: $dotnet_version${NC}"
else
    echo -e "${RED}❌ .NET SDK no encontrado${NC}"
    echo "   Instala desde: https://dotnet.microsoft.com/download"
fi

# Verificar EF Tools
if dotnet tool list -g | grep -q "dotnet-ef"; then
    ef_version=$(dotnet tool list -g | grep "dotnet-ef" | awk '{print $2}')
    echo -e "${GREEN}✅ Entity Framework Tools: $ef_version${NC}"
else
    echo -e "${YELLOW}⚠️  Entity Framework Tools no encontrado${NC}"
    echo "🔧 Instalando EF Tools..."
    dotnet tool install --global dotnet-ef
fi

# Restaurar paquetes
echo ""
echo "📦 Restaurando paquetes NuGet..."
if dotnet restore; then
    echo -e "${GREEN}✅ Paquetes restaurados${NC}"
else
    echo -e "${RED}❌ Error al restaurar paquetes${NC}"
    exit 1
fi

# Verificar conexión a base de datos
echo ""
echo "🗄️  Verificando conexión a base de datos..."

# Extraer connection string del .env
if [ -f ".env" ]; then
    connection_string=$(grep "ConnectionStrings__DefaultConnection" .env | cut -d'=' -f2-)
    if [ ! -z "$connection_string" ]; then
        echo "   Connection String configurado: ✅"
        
        # Aplicar migraciones
        echo "🔄 Aplicando migraciones..."
        if dotnet ef database update; then
            echo -e "${GREEN}✅ Base de datos actualizada${NC}"
        else
            echo -e "${YELLOW}⚠️  Error al aplicar migraciones (puede ser normal si la DB no está disponible)${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  Connection String no configurado en .env${NC}"
    fi
fi

# Verificar configuración final
echo ""
echo "🔍 Verificación final de configuración..."

# Verificar archivos críticos
critical_files=(".env" "appsettings.json" "Program.cs")
for file in "${critical_files[@]}"; do
    if [ -f "$file" ]; then
        echo -e "${GREEN}✅ $file${NC}"
    else
        echo -e "${RED}❌ $file no encontrado${NC}"
    fi
done

echo ""
echo "=================================================="
echo -e "${GREEN}🎉 ¡Setup completado!${NC}"
echo ""
echo "📋 Próximos pasos:"
echo "1. Editar .env con tus valores reales"
echo "2. Configurar base de datos MySQL"
echo "3. Ejecutar: dotnet run"
echo "4. Acceder a: https://localhost:7001/swagger"
echo ""
echo "📚 Documentación adicional:"
echo "- SETUP_LOCAL.md - Configuración detallada"
echo "- README_SECURITY.md - Guía de seguridad"
echo "- MYSQL_SETUP.md - Configuración de base de datos"
echo ""
echo -e "${YELLOW}⚠️  Recuerda: NUNCA subir .env a GitHub${NC}"
