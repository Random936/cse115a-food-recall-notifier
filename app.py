import os
import re
import json
from dotenv import load_dotenv
from flask import Flask, request, send_file
from databases.mongo_db import MongoDB
from config import MONGO_DB_HOST, MONGO_DB_PORT
from crawlers.fda_crawler import FDAWebCrawler
from parsers.upc_parser import UPCParser

load_dotenv()
app = Flask(__name__)

database = MongoDB(MONGO_DB_HOST, MONGO_DB_PORT)
database.update(FDAWebCrawler(os.getenv("FDA_API_KEY"), limit=1000), [UPCParser()])

def valid_recall_number(num):
    return re.fullmatch(r'[fF]-\d+-\d+', num)

@app.route('/')
def root_path():
    print(database.newest(5))
    return json.dumps({"name": "FRN-Server",
                       "version": "v0.1",
                       "newest": database.newest(5),
                       "last_modified": database.last_modified()})


@app.route('/query/<recall>')
def api_query(recall: str):
    if not valid_recall_number(recall):
        return json.dumps({"error": "Invalid recall number provided"})

    product = database.query(recall.upper())
    if product is None:
        return json.dumps({"error": "Recall number not found"})

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

    # Can be either asc or desc (sorts by date)
    sort = request.args.get('sort')

    if term == "all":
        term = ""

    results = database.search(field, term, offset, count, sort=sort)
    return json.dumps(results)


@app.route('/image/<recall>')
def api_image(recall: str):
    if not valid_recall_number(recall):
        return json.dumps({"error": "Invalid recall number provided"})

    product = database.query(recall.upper())
    if product is None:
        return json.dumps({"error": "Recall number not found"})

    return send_file('images/static.jpg')


if __name__ == '__main__':
    app.run()

