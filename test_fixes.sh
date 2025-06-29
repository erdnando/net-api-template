#!/bin/bash

echo "Testing UtilsController fixes..."

# Get admin token
echo "Getting admin token..."
RESPONSE=$(curl -s -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d @admin_login.json)
TOKEN=$(echo $RESPONSE | jq -r '.token')

if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
    echo "❌ Failed to get admin token"
    echo "Response: $RESPONSE"
    exit 1
fi

echo "✅ Got admin token"

# Test system health
echo ""
echo "Testing system health..."
curl -s -X GET http://localhost:5000/api/Utils/system-health \
  -H "Authorization: Bearer $TOKEN" | jq '.'

# Test password reset stats
echo ""
echo "Testing password reset stats..."
curl -s -X GET http://localhost:5000/api/Utils/password-reset-stats \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo ""
echo "Testing completed!"
