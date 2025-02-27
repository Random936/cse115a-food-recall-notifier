# CSE115A Food Recalls Notifier - Web Crawler
# Author: Size Liu

from datetime import datetime
import requests
import urllib
import json

from crawlers.webcrawler import WebCrawler

BASE_URL = "https://api.fda.gov/food/enforcement.json"

class FDAWebCrawler(WebCrawler):
    def __init__(self, api_key, limit=100):
        self.api_key = api_key
        self.limit = limit
        self.skip = 0

    def get_after(self, date):
        assert isinstance(date, datetime)

        while True:
            lower_date = date.strftime("%Y%m%d")
            upper_date = datetime.now().strftime("%Y%m%d")
            params = {
                'api_key': self.api_key,
                'limit': self.limit,
                'skip': self.skip,
                'sort': 'report_date:asc',
                'search': f'report_date[{lower_date}+TO+{upper_date}]'
            }

            try:
                payload_str = urllib.parse.urlencode(params, safe=':+[]')
                print(f"Making request: {BASE_URL}?{payload_str}")

                response = requests.get(BASE_URL, params=payload_str, stream=True)
                response.raise_for_status()  # Check if the request was successful
                data = response.json()
                results = data.get('results', [])

                if not results:
                    break

                for result in results:
                    yield result

                self.skip += self.limit

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
