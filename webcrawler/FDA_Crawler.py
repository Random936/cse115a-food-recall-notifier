# CSE115A Food Recalls Notifier - Web Crawler
# Author: Size Liu

from dotenv import load_dotenv
import os
import requests
import json
import re

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
        f.write('[')
        first_entry = True
        for recall in recalls:
            if not first_entry:
                f.write(',\n')
            json.dump(recall, f, ensure_ascii=False, indent=4)
            first_entry = False
        f.write(']')

def reformat_distribution_pattern(input):
    state_abbr_to_name = {
        "US": "Nationwide","nationwide":"Nationwide", 
        "AL": "Alabama", "AK": "Alaska", "AZ": "Arizona", "AR": "Arkansas", "CA": "California",
        "CO": "Colorado", "CT": "Connecticut", "DE": "Delaware", "FL": "Florida", "GA": "Georgia",
        "HI": "Hawaii", "ID": "Idaho", "IL": "Illinois", "IN": "Indiana", "IA": "Iowa", 
        "KS": "Kansas", "KY": "Kentucky", "LA": "Louisiana", "ME": "Maine", "MD": "Maryland", 
        "MA": "Massachusetts", "MI": "Michigan", "MN": "Minnesota", "MS": "Mississippi", 
        "MO": "Missouri", "MT": "Montana", "NE": "Nebraska", "NV": "Nevada", "NH": "New Hampshire", 
        "NJ": "New Jersey", "NM": "New Mexico", "NY": "New York", "NC": "North Carolina", 
        "ND": "North Dakota", "OH": "Ohio", "OK": "Oklahoma", "OR": "Oregon", "PA": "Pennsylvania", 
        "RI": "Rhode Island", "SC": "South Carolina", "SD": "South Dakota", "TN": "Tennessee", 
        "TX": "Texas", "UT": "Utah", "VT": "Vermont", "VA": "Virginia", "WA": "Washington", 
        "WV": "West Virginia", "WI": "Wisconsin", "WY": "Wyoming"
    }

    state_name_list = list(state_abbr_to_name.values())

    with open(input, "r") as file:
        data = json.load(file) 

    for record in data:
        found_states = []
        if "distribution_pattern" in record:
            distribution_pattern = record["distribution_pattern"]
            for abbr, name in state_abbr_to_name.items():
                if re.search(rf"{abbr}", distribution_pattern):
                    found_states.append(name)
                if re.search(rf"{name}", distribution_pattern, re.IGNORECASE):
                    found_states.append(name)

            found_states = list(set(found_states))
            record["distribution_pattern"] = found_states
    with open("food_recalls_updated.json", "w") as file:
        json.dump(data, file, indent=4)

def main():
    if not api_key:
        print("You need to set the FDA_API_KEY environment variable. (See readme.md for more details)")
        return

    # Output file
    output_file = 'food_recalls.json'

    # write to file
    save_recalls_to_json(api_key, output_file)
    print(f"Recall info has been save to {output_file}")
    reformat_distribution_pattern(output_file)

    

if __name__ == "__main__":
    main()
