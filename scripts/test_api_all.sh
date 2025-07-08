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

# Función para validar respuesta y código HTTP
validate_response_http() {
  local url="$1"
  local method="$2"
  local token="$3"
  local data="$4"
  local expected_code="$5"
  local msg="$6"
  local content_type=${7:-"application/json"}

  if [[ "$method" == "GET" ]]; then
    response=$(curl -s -w "\n%{http_code}" -X GET "$url" -H "Authorization: Bearer $token")
  elif [[ "$method" == "POST" ]]; then
    response=$(curl -s -w "\n%{http_code}" -X POST "$url" -H "Authorization: Bearer $token" -H "Content-Type: $content_type" -d "$data")
  elif [[ "$method" == "PUT" ]]; then
    response=$(curl -s -w "\n%{http_code}" -X PUT "$url" -H "Authorization: Bearer $token" -H "Content-Type: $content_type" -d "$data")
  elif [[ "$method" == "DELETE" ]]; then
    response=$(curl -s -w "\n%{http_code}" -X DELETE "$url" -H "Authorization: Bearer $token")
  elif [[ "$method" == "PATCH" ]]; then
    response=$(curl -s -w "\n%{http_code}" -X PATCH "$url" -H "Authorization: Bearer $token" -H "Content-Type: $content_type" -d "$data")
  else
    echo "[ERROR] Método HTTP no soportado: $method"
    return
  fi
  http_code=$(echo "$response" | tail -n1)
  body=$(echo "$response" | sed '$d')
  if [[ "$http_code" == "$expected_code" ]]; then
    echo "[OK] $msg ($http_code)"
  else
    echo "[ERROR] $msg (esperado $expected_code, recibido $http_code): $body"
  fi
}

# --- USERS ---
USERS_URL="$BASE_URL/users"
# Obtener token de admin o usuario de prueba
LOGIN_EMAIL="${1:-$ADMIN_EMAIL}"
LOGIN_PASS="${2:-$ADMIN_PASS}"
TOKEN=$(curl -s -X POST "$USERS_URL/login" -H "Content-Type: application/json" -d '{"email":"'$LOGIN_EMAIL'","password":"'$LOGIN_PASS'"}')
USER_TOKEN=$(echo "$TOKEN" | jq -r .data.token)
echo "TOKEN obtenido: $USER_TOKEN"

# USERS (esperado: 200 para admin, 403 para usuario sin permiso)
validate_response_http "$USERS_URL?page=1&pageSize=2" "GET" "$USER_TOKEN" "" "200" "Listar usuarios"
validate_response_http "$USERS_URL/1" "GET" "$USER_TOKEN" "" "200" "Obtener usuario por ID"
validate_response_http "$USERS_URL/by-email/$USER_EMAIL" "GET" "$USER_TOKEN" "" "200" "Buscar usuario por email"
validate_response_http "$USERS_URL" "POST" "$USER_TOKEN" '{"firstName":"Test","lastName":"User","email":"testuser@example.com","password":"test123","roleId":3}' "200" "Crear usuario de prueba"
validate_response_http "$USERS_URL/3" "PUT" "$USER_TOKEN" '{"firstName":"TestUpdated","lastName":"UserUpdated","email":"testuser@example.com","roleId":3}' "200" "Actualizar usuario de prueba"
validate_response_http "$USERS_URL/3" "DELETE" "$USER_TOKEN" "" "200" "Eliminar usuario de prueba"
validate_response_http "$USERS_URL/2/change-password" "POST" "$USER_TOKEN" '{"currentPassword":"user123","newPassword":"user456"}' "200" "Cambiar contraseña de usuario regular"
USER_LOGIN=$(curl -s -X POST "$USERS_URL/login" -H "Content-Type: application/json" -d '{"email":"'$USER_EMAIL'","password":"user456"}')
USER_TOKEN2=$(echo "$USER_LOGIN" | jq -r .data.token)
validate_response_http "$USERS_URL/2/change-password" "POST" "$USER_TOKEN2" '{"currentPassword":"user456","newPassword":"user123"}' "200" "Restaurar contraseña original del usuario regular"
validate_response_http "$USERS_URL/forgot-password" "POST" "" '{"email":"'$USER_EMAIL'"}' "200" "Probar endpoint forgot-password"
validate_response_http "$USERS_URL/reset-password" "POST" "" '{"token":"dummy-token","newPassword":"irrelevant"}' "200" "Probar endpoint reset-password (token dummy)"

# --- CATALOG ---
CATALOG_URL="$BASE_URL/catalog"
validate_response_http "$CATALOG_URL" "GET" "$USER_TOKEN" "" "200" "Listar catálogo"
validate_response_http "$CATALOG_URL/1" "GET" "$USER_TOKEN" "" "200" "Obtener ítem catálogo por ID"
validate_response_http "$CATALOG_URL/category/example" "GET" "$USER_TOKEN" "" "200" "Obtener catálogo por categoría"
validate_response_http "$CATALOG_URL/type/example" "GET" "$USER_TOKEN" "" "200" "Obtener catálogo por tipo"
validate_response_http "$CATALOG_URL/active" "GET" "$USER_TOKEN" "" "200" "Obtener catálogo activo"
validate_response_http "$CATALOG_URL/in-stock" "GET" "$USER_TOKEN" "" "200" "Obtener catálogo en stock"
validate_response_http "$CATALOG_URL" "POST" "$USER_TOKEN" '{"title":"TestItem","description":"Test desc","category":"example","price":10.5,"inStock":true,"active":true}' "200" "Crear ítem catálogo"
validate_response_http "$CATALOG_URL/1" "PUT" "$USER_TOKEN" '{"title":"UpdatedItem","description":"Updated desc","category":"example","price":12.0,"inStock":true,"active":true}' "200" "Actualizar ítem catálogo"
validate_response_http "$CATALOG_URL/1" "DELETE" "$USER_TOKEN" "" "200" "Eliminar ítem catálogo"
validate_response_http "$CATALOG_URL/1/status" "PATCH" "$USER_TOKEN" 'true' "200" "Patch status catálogo (activar/desactivar)"

# --- TASKS ---
TASKS_URL="$BASE_URL/tasks"
validate_response_http "$TASKS_URL" "GET" "$USER_TOKEN" "" "200" "Listar tareas"
validate_response_http "$TASKS_URL/1" "GET" "$USER_TOKEN" "" "200" "Obtener tarea por ID"
validate_response_http "$TASKS_URL/user/2" "GET" "$USER_TOKEN" "" "200" "Obtener tareas por usuario"
validate_response_http "$TASKS_URL/completed" "GET" "$USER_TOKEN" "" "200" "Obtener tareas completadas"
validate_response_http "$TASKS_URL/pending" "GET" "$USER_TOKEN" "" "200" "Obtener tareas pendientes"
validate_response_http "$TASKS_URL" "POST" "$USER_TOKEN" '{"title":"Test Task","description":"Test desc","userId":2,"completed":false}' "200" "Crear tarea"
validate_response_http "$TASKS_URL/1" "PUT" "$USER_TOKEN" '{"title":"Updated Task","description":"Updated desc","userId":2,"completed":true}' "200" "Actualizar tarea"
validate_response_http "$TASKS_URL/1" "DELETE" "$USER_TOKEN" "" "200" "Eliminar tarea"

# --- PERMISSIONS ---
PERM_URL="$BASE_URL/permissions"
validate_response_http "$PERM_URL/modules" "GET" "$USER_TOKEN" "" "200" "Listar módulos"
validate_response_http "$PERM_URL/modules/1" "GET" "$USER_TOKEN" "" "200" "Obtener módulo por ID"
validate_response_http "$PERM_URL/modules" "POST" "$USER_TOKEN" '{"name":"TestModule","code":"testmod","description":"desc"}' "200" "Crear módulo"
validate_response_http "$PERM_URL/modules/1" "PUT" "$USER_TOKEN" '{"name":"UpdatedModule","code":"testmod","description":"desc updated"}' "200" "Actualizar módulo"
validate_response_http "$PERM_URL/modules/1" "DELETE" "$USER_TOKEN" "" "200" "Eliminar módulo"
validate_response_http "$PERM_URL/users/2" "GET" "$USER_TOKEN" "" "200" "Obtener permisos por usuario"
validate_response_http "$PERM_URL/users/2" "PUT" "$USER_TOKEN" '[{"moduleId":1,"type":1}]' "200" "Actualizar permisos de usuario"
validate_response_http "$PERM_URL/users/2/modules/1" "DELETE" "$USER_TOKEN" "" "200" "Eliminar permiso usuario-módulo"
validate_response_http "$PERM_URL/users/2/modules/testmod/check" "GET" "$USER_TOKEN" "" "200" "Check permiso usuario-módulo"
validate_response_http "$PERM_URL/users/2/modules" "GET" "$USER_TOKEN" "" "200" "Obtener módulos de usuario"
validate_response_http "$PERM_URL/users/2/modules/1" "POST" "$USER_TOKEN" '{}' "200" "Agregar permiso usuario-módulo"

# --- ROLES ---
ROLES_URL="$BASE_URL/roles"
validate_response_http "$ROLES_URL" "GET" "$USER_TOKEN" "" "200" "Listar roles"
validate_response_http "$ROLES_URL/1" "GET" "$USER_TOKEN" "" "200" "Obtener rol por ID"
validate_response_http "$ROLES_URL" "POST" "$USER_TOKEN" '{"name":"TestRole","description":"desc"}' "200" "Crear rol"
validate_response_http "$ROLES_URL/1" "PUT" "$USER_TOKEN" '{"name":"UpdatedRole","description":"desc updated"}' "200" "Actualizar rol"
validate_response_http "$ROLES_URL/1" "DELETE" "$USER_TOKEN" "" "200" "Eliminar rol"

# --- MODULES ---
MODULES_URL="$BASE_URL/modules"
validate_response_http "$MODULES_URL" "GET" "$USER_TOKEN" "" "200" "Listar módulos (nuevo endpoint)"
validate_response_http "$MODULES_URL" "POST" "$USER_TOKEN" '{"name":"TestModule","code":"testmodule_$(date +%s)","path":"/test-module","icon":"TestIcon","adminOnly":false,"order":99}' "200" "Crear módulo de prueba (nuevo endpoint)"

echo "\nPruebas completas de todos los endpoints principales."
