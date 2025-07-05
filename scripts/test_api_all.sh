#!/bin/bash
# Script maestro para probar todos los endpoints principales del API (usuarios, catálogo, tareas, permisos, roles)
# Requiere: curl, jq

BASE_URL="http://localhost:5000/api"
ADMIN_EMAIL="admin@sistema.com"
ADMIN_PASS="admin123"
USER_EMAIL="erdnando@gmail.com"
USER_PASS="user123"

# Función para validar respuesta
validate_response() {
  local response="$1"
  local msg="$2"
  local success=$(echo "$response" | jq -r '.success // empty')
  if [[ "$success" == "false" || "$success" == "null" ]]; then
    echo "[ERROR] $msg: $(echo $response | jq -r '.message // .title // .errors')"
  else
    echo "[OK] $msg"
  fi
}

# --- USERS ---
USERS_URL="$BASE_URL/users"
# Obtener token de admin
TOKEN=$(curl -s -X POST "$USERS_URL/login" -H "Content-Type: application/json" -d '{"email":"'$ADMIN_EMAIL'","password":"'$ADMIN_PASS'"}')
ADMIN_TOKEN=$(echo "$TOKEN" | jq -r .data.token)
echo "TOKEN obtenido: $ADMIN_TOKEN"

resp=$(curl -s -X GET "$USERS_URL?page=1&pageSize=2" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Listar usuarios"
resp=$(curl -s -X GET "$USERS_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener usuario por ID"
resp=$(curl -s -X GET "$USERS_URL/by-email/$USER_EMAIL" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Buscar usuario por email"
resp=$(curl -s -X POST "$USERS_URL" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"firstName":"Test","lastName":"User","email":"testuser@example.com","password":"test123","roleId":3}')
validate_response "$resp" "Crear usuario de prueba"
resp=$(curl -s -X PUT "$USERS_URL/3" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"firstName":"TestUpdated","lastName":"UserUpdated","email":"testuser@example.com","roleId":3}')
validate_response "$resp" "Actualizar usuario de prueba"
resp=$(curl -s -X DELETE "$USERS_URL/3" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar usuario de prueba"
resp=$(curl -s -X POST "$USERS_URL/2/change-password" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"currentPassword":"user123","newPassword":"user456"}')
validate_response "$resp" "Cambiar contraseña de usuario regular"
USER_LOGIN=$(curl -s -X POST "$USERS_URL/login" -H "Content-Type: application/json" -d '{"email":"'$USER_EMAIL'","password":"user456"}')
USER_TOKEN=$(echo "$USER_LOGIN" | jq -r .data.token)
validate_response "$USER_LOGIN" "Login con usuario regular y nueva contraseña"
echo "USER_TOKEN obtenido: $USER_TOKEN"
resp=$(curl -s -X POST "$USERS_URL/2/change-password" -H "Authorization: Bearer $USER_TOKEN" -H "Content-Type: application/json" -d '{"currentPassword":"user456","newPassword":"user123"}')
validate_response "$resp" "Restaurar contraseña original del usuario regular"
resp=$(curl -s -X POST "$USERS_URL/forgot-password" -H "Content-Type: application/json" -d '{"email":"'$USER_EMAIL'"}')
validate_response "$resp" "Probar endpoint forgot-password"
resp=$(curl -s -X POST "$USERS_URL/reset-password" -H "Content-Type: application/json" -d '{"token":"dummy-token","newPassword":"irrelevant"}')
validate_response "$resp" "Probar endpoint reset-password (token dummy)"

echo "\n--- CATALOG ---"
CATALOG_URL="$BASE_URL/catalog"
resp=$(curl -s -X GET "$CATALOG_URL" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Listar catálogo"
resp=$(curl -s -X GET "$CATALOG_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener ítem catálogo por ID"
resp=$(curl -s -X GET "$CATALOG_URL/category/example" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener catálogo por categoría"
resp=$(curl -s -X GET "$CATALOG_URL/type/example" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener catálogo por tipo"
resp=$(curl -s -X GET "$CATALOG_URL/active" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener catálogo activo"
resp=$(curl -s -X GET "$CATALOG_URL/in-stock" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener catálogo en stock"
resp=$(curl -s -X POST "$CATALOG_URL" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"title":"TestItem","description":"Test desc","category":"example","price":10.5,"inStock":true,"active":true}')
validate_response "$resp" "Crear ítem catálogo"
resp=$(curl -s -X PUT "$CATALOG_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"title":"UpdatedItem","description":"Updated desc","category":"example","price":12.0,"inStock":true,"active":true}')
validate_response "$resp" "Actualizar ítem catálogo"
resp=$(curl -s -X DELETE "$CATALOG_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar ítem catálogo"
resp=$(curl -s -X PATCH "$CATALOG_URL/1/status" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d 'true')
validate_response "$resp" "Patch status catálogo (activar/desactivar)"

echo "\n--- TASKS ---"
TASKS_URL="$BASE_URL/tasks"
resp=$(curl -s -X GET "$TASKS_URL" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Listar tareas"
resp=$(curl -s -X GET "$TASKS_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener tarea por ID"
resp=$(curl -s -X GET "$TASKS_URL/user/2" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener tareas por usuario"
resp=$(curl -s -X GET "$TASKS_URL/completed" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener tareas completadas"
resp=$(curl -s -X GET "$TASKS_URL/pending" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener tareas pendientes"
resp=$(curl -s -X POST "$TASKS_URL" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"title":"Test Task","description":"Test desc","userId":2,"completed":false}')
validate_response "$resp" "Crear tarea"
resp=$(curl -s -X PUT "$TASKS_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"title":"Updated Task","description":"Updated desc","userId":2,"completed":true}')
validate_response "$resp" "Actualizar tarea"
resp=$(curl -s -X DELETE "$TASKS_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar tarea"

echo "\n--- PERMISSIONS ---"
PERM_URL="$BASE_URL/permissions"
resp=$(curl -s -X GET "$PERM_URL/modules" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Listar módulos"
resp=$(curl -s -X GET "$PERM_URL/modules/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener módulo por ID"
resp=$(curl -s -X POST "$PERM_URL/modules" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"TestModule","code":"testmod","description":"desc"}')
validate_response "$resp" "Crear módulo"
resp=$(curl -s -X PUT "$PERM_URL/modules/1" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"UpdatedModule","code":"testmod","description":"desc updated"}')
validate_response "$resp" "Actualizar módulo"
resp=$(curl -s -X DELETE "$PERM_URL/modules/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar módulo"
resp=$(curl -s -X GET "$PERM_URL/users/2" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener permisos por usuario"
resp=$(curl -s -X PUT "$PERM_URL/users/2" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '[{"moduleId":1,"type":1}]')
validate_response "$resp" "Actualizar permisos de usuario"
resp=$(curl -s -X DELETE "$PERM_URL/users/2/modules/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar permiso usuario-módulo"
resp=$(curl -s -X GET "$PERM_URL/users/2/modules/testmod/check" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Check permiso usuario-módulo"
resp=$(curl -s -X GET "$PERM_URL/users/2/modules" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener módulos de usuario"
resp=$(curl -s -X POST "$PERM_URL/users/2/modules/1" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{}')
validate_response "$resp" "Agregar permiso usuario-módulo"

echo "\n--- ROLES ---"
ROLES_URL="$BASE_URL/roles"
resp=$(curl -s -X GET "$ROLES_URL" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Listar roles"
resp=$(curl -s -X GET "$ROLES_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Obtener rol por ID"
resp=$(curl -s -X POST "$ROLES_URL" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"TestRole","description":"desc"}')
validate_response "$resp" "Crear rol"
resp=$(curl -s -X PUT "$ROLES_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"UpdatedRole","description":"desc updated"}')
validate_response "$resp" "Actualizar rol"
resp=$(curl -s -X DELETE "$ROLES_URL/1" -H "Authorization: Bearer $ADMIN_TOKEN")
validate_response "$resp" "Eliminar rol"

echo "\n--- MODULES ---"
MODULES_URL="$BASE_URL/modules"

# Listar módulos
resp=$(curl -s -X GET "$MODULES_URL" -H "Authorization: Bearer $ADMIN_TOKEN")
# Solo validar si la respuesta es un objeto, no un array
if echo "$resp" | jq -e 'type == "object"' >/dev/null 2>&1; then
  validate_response "$resp" "Listar módulos (nuevo endpoint)"
else
  echo "[OK] Listar módulos (nuevo endpoint) (respuesta es array, omitido)"
fi

# Crear módulo de prueba
MODULE_CODE="testmodule_$(date +%s)"
resp=$(curl -s -X POST "$MODULES_URL" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"TestModule","code":"'$MODULE_CODE'","path":"/test-module","icon":"TestIcon","adminOnly":false,"order":99}')
validate_response "$resp" "Crear módulo de prueba (nuevo endpoint)"
echo "Respuesta creación módulo: $resp" # DEBUG: mostrar respuesta real
MODULE_ID=$(echo "$resp" | jq -r '.data.id // empty')

# Actualizar módulo de prueba
if [[ -n "$MODULE_ID" && "$MODULE_ID" != "null" ]]; then
  resp=$(curl -s -X PUT "$MODULES_URL/$MODULE_ID" -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" -d '{"name":"TestModuleUpdated","path":"/test-module-upd","icon":"TestIconUpd","adminOnly":true,"order":100}')
  validate_response "$resp" "Actualizar módulo de prueba (nuevo endpoint)"

  # Eliminar módulo de prueba
  resp=$(curl -s -X DELETE "$MODULES_URL/$MODULE_ID" -H "Authorization: Bearer $ADMIN_TOKEN")
  validate_response "$resp" "Eliminar módulo de prueba (nuevo endpoint)"
else
  echo "[ERROR] No se pudo obtener el ID del módulo de prueba para actualizar/eliminar."
fi

echo "\nPruebas completas de todos los endpoints principales."
