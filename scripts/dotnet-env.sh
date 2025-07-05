#!/bin/bash
# Ejecuta comandos dotnet con las variables del .env cargadas en el entorno
# Uso: ./scripts/dotnet-env.sh ef database update

if [ ! -f .env ]; then
  echo "Archivo .env no encontrado en el directorio actual."
  exit 1
fi

# Exportar todas las variables del .env correctamente, incluyendo valores con espacios
set -a
source .env
set +a

dotnet "$@"
