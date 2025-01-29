import json
from flask import Flask
from database import Database, CsvDB

app = Flask(__name__)
database = CsvDB('/home/media/recalls-market-withdrawals-safety-alerts.csv')

assert isinstance(database, Database)

@app.route('/')
def root_path():
    return json.dumps({"name": "FRN-Server",
                       "version": "v0.1"})


@app.route('/query/<upc>')
def api_query(upc: int = 0):
    if not isinstance(upc, int) or not upc >= 0:
        return json.dumps({"error": "Invalid UPC provided"})

    product = database.get(upc)
    if product is None:
        return json.dumps({"error": "UPC not found"})

    return product.to_json()


if __name__ == '__main__':
    app.run()

