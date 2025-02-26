# Test FDA API key
import os
import requests

API_KEY = os.getenv("FDA_API_KEY")
response = requests.get(
    "https://api.fda.gov/food/enforcement.json",
    params={"limit": 1},
    headers={"Authorization": f"Bearer {API_KEY}"}
)
print(response.status_code)  # Should be 200 if the key is valid