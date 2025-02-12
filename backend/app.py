import json
from flask import Flask
from mongo_db import MongoDB

app = Flask(__name__)
database = MongoDB("localhost", 27017)
SEARCHERS = {
    "upc": None,
    "date": None,
    "brand": None,
    "product_type": None,
    "company": None,
    "description": None
}

@app.route('/')
def root_path():
    return json.dumps({"name": "FRN-Server",
                       "version": "v0.1",
                       "db_state": database.state(),
                       "last_modified": database.last_modified()})


@app.route('/query/<upc>')
def api_query(upc: str):
    if not upc.isdigit() or not int(upc) >= 0:
        return json.dumps({"error": "Invalid UPC provided"})

    product = database.query(upc)
    if product is None:
        return json.dumps({"error": "UPC not found"})

    return json.dumps(product)

@app.route('/query/<searcher>/<term>')
def api_search(searcher: str, term: str):
    if not isinstance(field, str) or not isinstance(term, str):
        return json.dumps({"error": "Invalid datatype received in REST API"})

    #results = database.search(
    raise NotImplementedError


if __name__ == '__main__':
    app.run()

