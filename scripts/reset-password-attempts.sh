#!/bin/bash

# 🔄 Script para Reiniciar Intentos de Reset de Contraseña
# Uso: ./reset-password-attempts.sh [opcion] [parametros]

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuración de base de datos (carga desde .env si existe)
if [ -f .env ]; then
    export $(grep -v '^#' .env | xargs)
fi

DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-3306}
DB_NAME=${DB_NAME:-ReactTemplateDb}
DB_USER=${DB_USER:-root}
DB_PASSWORD=${DB_PASSWORD:-root_password}

# Función para mostrar ayuda
show_help() {
    echo -e "${BLUE}🔄 Script de Mantenimiento - Reset de Contraseñas${NC}"
    echo ""
    echo "Uso: $0 [opción] [parámetros]"
    echo ""
    echo "Opciones:"
    echo "  stats                    - Mostrar estadísticas de intentos"
    echo "  reset-user <email>       - Reiniciar intentos para un usuario"
    echo "  cleanup-expired          - Limpiar tokens expirados"
    echo "  reset-all               - Limpiar TODOS los tokens (peligroso)"
    echo "  help                    - Mostrar esta ayuda"
    echo ""
    echo "Ejemplos:"
    echo "  $0 stats"
    echo "  $0 reset-user erdnando@gmail.com"
    echo "  $0 cleanup-expired"
    echo ""
}

# Función para ejecutar consulta SQL
run_mysql_query() {
    local query="$1"
    mysql -h"$DB_HOST" -P"$DB_PORT" -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" -e "$query"
}

# Función para mostrar estadísticas
show_stats() {
    echo -e "${BLUE}📊 Estadísticas de Reset de Contraseñas (últimas 24h)${NC}"
    echo ""
    
    local query="
    SELECT 
        u.Email as 'Usuario',
        COUNT(prt.Id) as 'Intentos',
        MAX(prt.CreatedAt) as 'Último Intento'
    FROM Users u
    LEFT JOIN PasswordResetTokens prt ON u.Id = prt.UserId 
        AND prt.CreatedAt > DATE_SUB(NOW(), INTERVAL 1 DAY)
    GROUP BY u.Id, u.Email
    HAVING COUNT(prt.Id) > 0
    ORDER BY COUNT(prt.Id) DESC;"
    
    if ! run_mysql_query "$query"; then
        echo -e "${RED}❌ Error conectando a la base de datos${NC}"
        exit 1
    fi
}

# Función para reiniciar intentos de un usuario
reset_user_attempts() {
    local email="$1"
    
    if [ -z "$email" ]; then
        echo -e "${RED}❌ Error: Debes proporcionar un email${NC}"
        echo "Uso: $0 reset-user <email>"
        exit 1
    fi
    
    echo -e "${YELLOW}🔄 Reiniciando intentos para: $email${NC}"
    
    # Primero verificar si el usuario existe
    local check_query="SELECT COUNT(*) as count FROM Users WHERE Email = '$email';"
    local user_exists=$(mysql -h"$DB_HOST" -P"$DB_PORT" -u"$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" -N -e "$check_query")
    
    if [ "$user_exists" -eq 0 ]; then
        echo -e "${RED}❌ Usuario no encontrado: $email${NC}"
        exit 1
    fi
    
    # Eliminar tokens de las últimas 24 horas
    local reset_query="
    DELETE prt FROM PasswordResetTokens prt 
    INNER JOIN Users u ON prt.UserId = u.Id 
    WHERE u.Email = '$email' 
    AND prt.CreatedAt > DATE_SUB(NOW(), INTERVAL 1 DAY);"
    
    if run_mysql_query "$reset_query"; then
        echo -e "${GREEN}✅ Intentos reiniciados exitosamente para: $email${NC}"
        
        # Mostrar estadísticas actualizadas para ese usuario
        local stats_query="
        SELECT 
            COUNT(prt.Id) as 'Intentos Restantes'
        FROM Users u
        LEFT JOIN PasswordResetTokens prt ON u.Id = prt.UserId 
            AND prt.CreatedAt > DATE_SUB(NOW(), INTERVAL 1 DAY)
        WHERE u.Email = '$email';"
        
        echo -e "${BLUE}📊 Estado actual:${NC}"
        run_mysql_query "$stats_query"
    else
        echo -e "${RED}❌ Error reiniciando intentos${NC}"
        exit 1
    fi
}

# Función para limpiar tokens expirados
cleanup_expired() {
    echo -e "${YELLOW}🧹 Limpiando tokens expirados...${NC}"
    
    local cleanup_query="DELETE FROM PasswordResetTokens WHERE ExpiresAt < NOW();"
    
    if run_mysql_query "$cleanup_query"; then
        echo -e "${GREEN}✅ Tokens expirados eliminados exitosamente${NC}"
    else
        echo -e "${RED}❌ Error limpiando tokens expirados${NC}"
        exit 1
    fi
}

# Función para limpiar todos los tokens (peligroso)
reset_all() {
    echo -e "${RED}⚠️  ADVERTENCIA: Esto eliminará TODOS los tokens de reset${NC}"
    echo -e "${YELLOW}¿Estás seguro? (escribe 'CONFIRMAR' para continuar):${NC}"
    read -r confirmation
    
    if [ "$confirmation" != "CONFIRMAR" ]; then
        echo -e "${BLUE}❌ Operación cancelada${NC}"
        exit 0
    fi
    
    echo -e "${YELLOW}🗑️ Eliminando todos los tokens...${NC}"
    
    local reset_all_query="DELETE FROM PasswordResetTokens;"
    
    if run_mysql_query "$reset_all_query"; then
        echo -e "${GREEN}✅ Todos los tokens eliminados exitosamente${NC}"
    else
        echo -e "${RED}❌ Error eliminando tokens${NC}"
        exit 1
    fi
}

# Proceso principal
case "$1" in
    "stats"|"estadisticas")
        show_stats
        ;;
    "reset-user"|"reiniciar-usuario")
        reset_user_attempts "$2"
        ;;
    "cleanup-expired"|"limpiar-expirados")
        cleanup_expired
        ;;
    "reset-all"|"reiniciar-todo")
        reset_all
        ;;
    "help"|"ayuda"|"")
        show_help
        ;;
    *)
        echo -e "${RED}❌ Opción no válida: $1${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac
