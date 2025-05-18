echo "⌛ Waiting for Vault..."
until curl -s http://vault:8200/v1/sys/health > /dev/null; do
  sleep 1
done

echo "🔐 Bootstrapping Vault..."

curl -s --header "X-Vault-Token: root" \
  --request POST \
  --data '{
    "data": {
      "Secret": "HelloSecureRandomKeyHere1234567890",
      "Issuer": "AuthService",
      "Audience": "HCMApp",
      "ExpiryMinutes": "60"
    }
  }' http://vault:8200/v1/secret/data/hcm/jwt

curl -s --header "X-Vault-Token: root" \
  --request POST \
  --data '{
    "data": {
      "AuthDb": "Host=db;Port=5432;Database=AuthDb;Username=postgres;Password=postgres",
       "PeopleDb": "Host=db;Port=5432;Database=PeopleDb;Username=postgres;Password=postgres"
    }
  }' http://vault:8200/v1/secret/data/hcm/db

echo "✅ Vault bootstrap complete."
