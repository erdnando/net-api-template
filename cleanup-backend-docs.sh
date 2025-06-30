#!/bin/bash

# 🔥 Script para organizar documentación del Backend .NET Core

echo "🔥 Limpiando documentación del Backend..."

# Crear carpetas si no existen
mkdir -p docs/{api,features,setup,security,deployment,reports,archive,temp}

# Mover archivos de setup y configuración
echo "🚀 Organizando archivos de setup..."
find . -maxdepth 1 -name "SETUP_*.md" -exec mv {} docs/setup/ \;
find . -maxdepth 1 -name "CONFIGURATION*.md" -exec mv {} docs/setup/ \;
find . -maxdepth 1 -name "MYSQL_*.md" -exec mv {} docs/setup/ \;

# Mover archivos de API
echo "🔌 Organizando documentación de API..."
find . -maxdepth 1 -name "*API*.md" -exec mv {} docs/api/ \;
find . -maxdepth 1 -name "*ENDPOINT*.md" -exec mv {} docs/api/ \;

# Mover archivos de funcionalidades
echo "🎯 Organizando funcionalidades..."
find . -maxdepth 1 -name "*IMPLEMENTATION*.md" -exec mv {} docs/features/ \;
find . -maxdepth 1 -name "*SYSTEM_SUMMARY*.md" -exec mv {} docs/features/ \;
find . -maxdepth 1 -name "UTILS_*.md" -exec mv {} docs/features/ \;

# Mover archivos de seguridad
echo "🔐 Organizando documentación de seguridad..."
find . -maxdepth 1 -name "*SECURITY*.md" -exec mv {} docs/security/ \;
find . -maxdepth 1 -name "*LOGS*.md" -exec mv {} docs/security/ \;
find . -maxdepth 1 -name "README_SECURITY*.md" -exec mv {} docs/security/ \;

# Mover archivos de deployment
echo "🚀 Organizando deployment..."
find . -maxdepth 1 -name "DEPLOYMENT*.md" -exec mv {} docs/deployment/ \;

# Mover reportes e investigaciones
echo "📋 Organizando reportes..."
find . -maxdepth 1 -name "*REPORT*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*ISSUES*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*INVESTIGATION*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*FIX*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*FRONTEND*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*CLEANUP*.md" -exec mv {} docs/reports/ \;
find . -maxdepth 1 -name "*TEST*.md" -exec mv {} docs/reports/ \;

# Mover archivos obsoletos
echo "🗄️ Moviendo archivos obsoletos..."
find . -maxdepth 1 -name "*OLD*.md" -exec mv {} docs/archive/ \;
find . -maxdepth 1 -name "*DEPRECATED*.md" -exec mv {} docs/archive/ \;
find . -maxdepth 1 -name "*LEGACY*.md" -exec mv {} docs/archive/ \;

# Limpiar archivos temporales
echo "🧹 Limpiando archivos temporales..."
find docs/ -name "*.temp.md" -delete
find docs/ -name "*.draft.md" -delete
find docs/ -name "*.bak.md" -delete

echo "✅ Documentación del backend organizada!"
echo "📁 Estructura:"
find docs/ -type f -name "*.md" | sort

echo ""
echo "💡 Consejos para Backend:"
echo "  - API docs en 'docs/api/'"
echo "  - Features en 'docs/features/'"
echo "  - Seguridad en 'docs/security/'"
echo "  - Reportes en 'docs/reports/'"
echo "  - Setup en 'docs/setup/'"
