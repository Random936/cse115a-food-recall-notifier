import os
import json
from dotenv import load_dotenv
from flask import Flask, request
from databases.mongo_db import MongoDB
from config import MONGO_DB_HOST, MONGO_DB_PORT
from crawlers.fda_crawler import FDAWebCrawler

load_dotenv()
app = Flask(__name__)

database = MongoDB(MONGO_DB_HOST, MONGO_DB_PORT)
database.update(FDAWebCrawler(os.getenv("FDA_API_KEY"), limit=1000), [])

@app.route('/')
def root_path():
    print(database.newest(5))
    return json.dumps({"name": "FRN-Server",
                       "version": "v0.1",
                       "newest": database.newest(5),
                       "last_modified": database.last_modified()})


@app.route('/query/<upc>')
def api_query(upc: str):
    if not upc.isdigit() or not int(upc) >= 0:
        return json.dumps({"error": "Invalid UPC provided"})

    product = database.query(upc)
    if product is None:
        return json.dumps({"error": "UPC not found"})

    return json.dumps(product)


@app.route('/search/<field>/<term>')
def api_search(field: str, term: str):
    if not isinstance(field, str) or not isinstance(term, str):
        return json.dumps({"error": "Invalid datatype received in REST API"})

    offset = request.args.get('offset')
    if offset is None:
        offset = 0
    else:
        offset = int(offset)

    count = request.args.get('count')
    if count is None:
        count = 10
    else:
        count = int(count)

    if term == "all":
        term = ""

    results = database.search(field, term, offset, count)
    return json.dumps(results)



if __name__ == '__main__':
    app.run()

