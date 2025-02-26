# CSE115A Food Recalls Notifier - Web Crawler
# Author: Size Liu

from dotenv import load_dotenv
import os
import requests
import json
from databases.mongo_db import MongoDB
from config import MONGO_DB_HOST, MONGO_DB_PORT
from datetime import datetime, timezone

# load environment variables from .env file (see readme.md for more details)
load_dotenv()
api_key = os.getenv("FDA_API_KEY")

def WebCrawler(api_key, limit=100): # Fetch all food recalls from FDA API (Sorted from earliest to latest)
    base_url = "https://api.fda.gov/food/enforcement.json"
    skip = 0

    while True:
        params = {
            'api_key': api_key,
            'limit': limit,
            'skip': skip,
            'sort': 'report_date:desc' # Sort by report_date in ascending order (change desc to asc to sort in ascending order)
        }

        try:
            response = requests.get(base_url, params=params, stream=True)
            response.raise_for_status()  # Check if the request was successful
            data = response.json()
            results = data.get('results', [])
            if not results:
                break

            for result in results:
                result["date_scraped"] = datetime.now(tz=timezone.utc).isoformat(timespec='seconds')
                yield result

            skip += limit
        except requests.RequestException as e:
            print(f"There's error in request: {e}")
            break
        except ValueError:
            print("Unable to parse response to JSON.")
            break

def save_recalls_to_json(api_key, output_file):    # Save info to json file
    recalls = WebCrawler(api_key=api_key)

    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(recalls, f, ensure_ascii=False, indent=4)

def save_recalls_to_mongodb(api_key, output_file):    # Save info to json file
    database = MongoDB(MONGO_DB_HOST, MONGO_DB_PORT)
    recalls = WebCrawler(api_key=api_key)

    for recall in recalls:
        if "recall_number" not in recall:
            print("Recall record does not contain a recall number. Malformed.")
            continue
        if database.query(recall["recall_number"]) is not None:
            print("Recall record already in database.")
            continue

    assert "date_scraped" in entry
    database.add(entry)

def main():
    if not api_key:
        print("You need to set the FDA_API_KEY environment variable. (See readme.md for more details)")
        return

    # Output file
    output_file = 'food_recalls.json'

    # write to file
    save_recalls_to_json(api_key, output_file)
    print(f"Recall info has been save to {output_file}")

if __name__ == "__main__":
    main()
