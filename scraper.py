import requests
import json
import sys
import re
from datetime import datetime, timezone
from databases.mongo_db import MongoDB
from config import MONGO_DB_HOST, MONGO_DB_PORT

res = requests.get('https://www.fda.gov/datatables/views/ajax?_drupal_ajax=1&_wrapper_format=drupal_ajax&view_name=recall_solr_index&view_display_id=recall_datatable_block_1&pager_element=0&draw=1&start=0&length=5000&columns[0][data]=0&columns[0][searchable]=true&columns[0][orderable]=true&search[value]=&search[regex]=false').json()

if "data" not in res:
    print("Invalid json returned from FDA API. No \"data\" key.", file=sys.stderr)
    exit(1)


database = MongoDB(MONGO_DB_HOST, MONGO_DB_PORT)

for raw_product in res["data"]:
    assert len(raw_product) >= 6

    page = raw_product[1].split('"')[1]
    res = requests.get("https://www.fda.gov" + page)
    if not res:
        print(f"Failed to request data for {page}", file=sys.stderr)
        continue

    upcs = re.findall(r'(?!<=[0-9])[0-9]{11,13}(?![0-9])', res.text)
    if len(upcs) == 0:
        print(f"Failed to find UPC for {page}", file=sys.stderr)
        continue

    # scraping states 
    # us states
    us_states = {
        "Alabama": "AL", "Alaska": "AK", "Arizona": "AZ", "Arkansas": "AR", "California": "CA",
        "Colorado": "CO", "Connecticut": "CT", "Delaware": "DE", "Florida": "FL", "Georgia": "GA",
        "Hawaii": "HI", "Idaho": "ID", "Illinois": "IL", "Indiana": "IN", "Iowa": "IA",
        "Kansas": "KS", "Kentucky": "KY", "Louisiana": "LA", "Maine": "ME", "Maryland": "MD",
        "Massachusetts": "MA", "Michigan": "MI", "Minnesota": "MN", "Mississippi": "MS",
        "Missouri": "MO", "Montana": "MT", "Nebraska": "NE", "Nevada": "NV", "New Hampshire": "NH",
        "New Jersey": "NJ", "New Mexico": "NM", "New York": "NY", "North Carolina": "NC",
        "North Dakota": "ND", "Ohio": "OH", "Oklahoma": "OK", "Oregon": "OR", "Pennsylvania": "PA",
        "Rhode Island": "RI", "South Carolina": "SC", "South Dakota": "SD", "Tennessee": "TN",
        "Texas": "TX", "Utah": "UT", "Vermont": "VT", "Virginia": "VA", "Washington": "WA",
        "West Virginia": "WV", "Wisconsin": "WI", "Wyoming": "WY"
    }

    affected_us_states = []

    for state, abbr in us_states.items():
        if re.findall(r'\b{state}\b|\b{abbr}[,.\b]|\b{abbr}\b and', res.text):
            affected_us_states.append(state)

    print(f"Found UPCs: {upcs} on page: {page}")
    for upc in set(upcs):
        if database.query(upc) is not None:
            continue

        entry = {
            "upc": upc,
            "date": datetime.fromisoformat(raw_product[0].split('"')[1]).isoformat(timespec='seconds'),
            "date_scraped": datetime.now(tz=timezone.utc).isoformat(timespec='seconds'),
            "brand": re.split("<|>", raw_product[1])[2],
            "page": page,
            "description": raw_product[2],
            "product_type": raw_product[3],
            "recall_reason": raw_product[4],
            "company": raw_product[5],
            "terminated": raw_product[6] == "Terminated",
            "affected_us_states": affected_us_states
        }
        print(f"Adding new record with UPC: {upc}\n{entry}")
        database.add(entry)
