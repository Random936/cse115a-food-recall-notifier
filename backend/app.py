import json
from flask import Flask
from database import Database, StaticDB

app = Flask(__name__)
database = StaticDB('/home/random/cse115a-food-recall-notifier/db.json')

assert isinstance(database, Database)

@app.route('/')
def root_path():
    return json.dumps({"name": "FRN-Server",
                       "version": "v0.1"})


@app.route('/query/<upc>')
def api_query(upc: str):
    if not upc.isdigit() or not int(upc) >= 0:
        return json.dumps({"error": "Invalid UPC provided"})

    product = database.query(upc)
    if product is None:
        return json.dumps({"error": "UPC not found"})

    return json.dumps(product)


if __name__ == '__main__':
    app.run()

