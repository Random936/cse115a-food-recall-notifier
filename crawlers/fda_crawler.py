# CSE115A Food Recalls Notifier - Web Crawler
# Author: Size Liu

from datetime import datetime
import requests
import json

from crawlers.webcrawler import WebCrawler

class FDAWebCrawler(WebCrawler):
    def __init__(api_key, limit=100):
        base_url = "https://api.fda.gov/food/enforcement.json"
        skip = 0

    def get_after(self, date):
        assert isinstance(date, datetime)

        while True:
            lower_date = date.strftime("%Y%m%d")
            upper_date = datetime.now().strftime("%Y%m%d")
            params = {
                'api_key': api_key,
                'limit': limit,
                'skip': skip,
                'sort': f'report_date:desc[{lower_date}+TO+{upper_date}]' # Sort by report_date in ascending order (change desc to asc to sort in ascending order)
            }

            try:
                response = requests.get(base_url, params=params, stream=True)
                response.raise_for_status()  # Check if the request was successful
                data = response.json()
                results = data.get('results', [])
                if not results:
                    break

                for result in results:
                    yield result
                    skip += limit

            except requests.RequestException as e:
                print(f"There's error in request: {e}")
                break
            except ValueError:
                print("Unable to parse response to JSON.")
                break

    def get_all(self):
        # Theoretically there shouldn't be recalls with an epoch timestamp before 0.
        # Either way not relevant now.
        return self.get_after(datetime.fromtimestamp(0))

#def save_recalls_to_mongodb(api_key, output_file):    # Save info to json file
#    database = MongoDB(MONGO_DB_HOST, MONGO_DB_PORT)
#    recalls = WebCrawler(api_key=api_key)
#
#    for recall in recalls:
#        if "recall_number" not in recall:
#            print("Recall record does not contain a recall number. Malformed.")
#            continue
#        if database.query(recall["recall_number"]) is not None:
#            print("Recall record already in database.")
#            continue
#
#    assert "date_scraped" in entry
#    database.add(entry)
