#!/bin/bash

# Test script to investigate the pending issues

echo "=============================================="
echo "🔍 Testing UtilsController Issues"
echo "=============================================="

# Get admin token
echo "1. Getting admin token..."
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@sistema.com","password": "admin123"}' | jq -r '.token')

if [ "$TOKEN" == "null" ] || [ -z "$TOKEN" ]; then
    echo "❌ Failed to get admin token"
    exit 1
fi

echo "✅ Got admin token: ${TOKEN:0:20}..."

echo ""
echo "2. Testing System Health endpoint..."
curl -s -X GET http://localhost:5000/api/Utils/system-health \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo ""
echo "3. Creating password reset attempt..."
curl -s -X POST http://localhost:5000/api/auth/forgot-password \
  -H "Content-Type: application/json" \
  -d '{"email": "erdnando@gmail.com"}' | jq '.'

echo ""
echo "4. Testing Password Reset Stats endpoint..."
curl -s -X GET http://localhost:5000/api/Utils/password-reset-stats \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo ""
echo "5. Testing user existence check..."
curl -s -X GET "http://localhost:5000/api/Utils/user-exists?email=erdnando@gmail.com" \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo ""
echo "6. Testing cleanup expired tokens..."
curl -s -X POST http://localhost:5000/api/Utils/cleanup-expired-tokens \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo ""
echo "=============================================="
echo "🔍 Database Investigation"
echo "=============================================="

echo "7. Testing direct database queries..."
