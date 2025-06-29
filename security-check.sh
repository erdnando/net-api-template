#!/bin/bash
# 🔒 Script de Verificación de Seguridad Pre-GitHub

echo "🔍 Verificando seguridad antes de subir a GitHub..."
echo "=================================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Contadores
errors=0
warnings=0

# Función para mostrar errores
show_error() {
    echo -e "${RED}❌ ERROR: $1${NC}"
    ((errors++))
}

# Función para mostrar advertencias
show_warning() {
    echo -e "${YELLOW}⚠️  ADVERTENCIA: $1${NC}"
    ((warnings++))
}

# Función para mostrar éxito
show_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

echo "1. Verificando archivo .gitignore..."
if [ -f ".gitignore" ]; then
    if grep -q "\.env" .gitignore && grep -q "appsettings\.json" .gitignore; then
        show_success "Archivo .gitignore configurado correctamente"
    else
        show_error "Archivo .gitignore no protege secretos adecuadamente"
    fi
else
    show_error "Archivo .gitignore no encontrado"
fi

echo ""
echo "2. Verificando archivos de secretos..."

# Verificar que archivos sensibles no estén staged
if git diff --cached --name-only | grep -E "\.(env|json)$|appsettings" > /dev/null; then
    show_error "Archivos con posibles secretos están staged para commit"
    git diff --cached --name-only | grep -E "\.(env|json)$|appsettings"
fi

# Verificar existencia de archivos template
if [ -f "appsettings.template.json" ]; then
    show_success "Archivo template de configuración encontrado"
else
    show_warning "No se encontró appsettings.template.json"
fi

if [ -f ".env.example" ]; then
    show_success "Archivo ejemplo de variables de entorno encontrado"
else
    show_warning "No se encontró .env.example"
fi

echo ""
echo "3. Buscando secretos hardcodeados en código..."

# Buscar patterns de secretos comunes
secret_patterns=(
    "password.*=.*['\"][^'\"]{8,}"
    "secret.*=.*['\"][^'\"]{16,}"
    "key.*=.*['\"][^'\"]{16,}"
    "smtp.*password.*=.*['\"][^'\"]{8,}"
    "connectionstring.*=.*password"
)

found_secrets=false
for pattern in "${secret_patterns[@]}"; do
    if grep -r -i -E "$pattern" --include="*.cs" --include="*.json" . > /dev/null 2>&1; then
        if [ "$found_secrets" = false ]; then
            echo -e "${RED}⚠️  Posibles secretos encontrados:${NC}"
            found_secrets=true
        fi
        grep -r -i -E "$pattern" --include="*.cs" --include="*.json" . | head -3
        ((warnings++))
    fi
done

if [ "$found_secrets" = false ]; then
    show_success "No se encontraron secretos hardcodeados obvios"
fi

echo ""
echo "4. Verificando archivos de configuración..."

config_files=("appsettings.json" "appsettings.Development.json" ".env")
for file in "${config_files[@]}"; do
    if [ -f "$file" ]; then
        if [ "$file" = ".env" ] || grep -q "password.*:" "$file" 2>/dev/null; then
            show_error "Archivo $file contiene posibles secretos"
        fi
    fi
done

echo ""
echo "5. Verificando commits previos..."
if git log --oneline -10 | grep -i -E "(password|secret|key|token)" > /dev/null; then
    show_warning "Commits recientes mencionan secretos - revisar historial"
fi

echo ""
echo "6. Verificando archivos grandes..."
large_files=$(find . -type f -size +10M ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null)
if [ ! -z "$large_files" ]; then
    show_warning "Archivos grandes encontrados (podrían contener datos sensibles):"
    echo "$large_files"
fi

echo ""
echo "=================================================="
echo "📊 RESUMEN DE VERIFICACIÓN"
echo "=================================================="

if [ $errors -gt 0 ]; then
    echo -e "${RED}❌ Errores encontrados: $errors${NC}"
    echo -e "${RED}⛔ NO SUBIR A GITHUB HASTA RESOLVER ERRORES${NC}"
elif [ $warnings -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Advertencias: $warnings${NC}"
    echo -e "${YELLOW}📋 Revisar advertencias antes de subir${NC}"
else
    echo -e "${GREEN}🎉 ¡Todo parece estar bien!${NC}"
    echo -e "${GREEN}✅ Seguro para subir a GitHub${NC}"
fi

echo ""
echo "🔧 RECOMENDACIONES FINALES:"
echo "- Usar 'git add' selectivamente, no 'git add .'"
echo "- Revisar cada archivo antes del commit con 'git diff --staged'"
echo "- Configurar GitHub Secrets para CI/CD"
echo "- Considerar usar Azure Key Vault para producción"

exit $errors
