import pymongo
from database import Database
from collections import defaultdict
from datetime import datetime
from crawlers.webcrawler import WebCrawler
from parsers.webparser import WebParser

class MongoDB(Database):
    def __init__(self, host, port):
        assert isinstance(host, str)
        assert isinstance(port, int)

        self.client = pymongo.MongoClient(host, port)
        self.database = self.client.get_database("recall-notifier")
        self.recalls = self.database.get_collection("recalls")

    def newest(self, n):
        newest = list(self.recalls.find({}, sort=[('report_date', pymongo.DESCENDING)]).limit(n))
        for n in newest:
            del n["_id"]

        return newest

    def last_modified(self):
        newest = self.recalls.find_one({"recall_initiation_date": {"$exists": True } }, sort=[('recall_initiation_date', pymongo.DESCENDING)])
        return 0 if newest is None else newest.get('recall_initiation_date', 0)

    def query(self, key):
        item = self.recalls.find_one({"recall_number": key})
        if item is not None:
            del item["_id"]

        return item

    def search(self, field, term, offset, count, sort=None):
        assert isinstance(offset, int) and isinstance(count, int)
        assert offset >= 0 and count > 0

        query = {field: {"$regex": term, "$options": "i"} }
        projection = {
            "_id": 0,
            "recall_number": 1,
            "upc": 1,
            "distribution_pattern": 1,
            "product_type": 1,
            "reason_for_recall": 1,
            "recalling_firm": 1,
            "product_description": 1,
            "report_date": 1
        }

        print(f"Recieved DB search with query: {query}")
        cursor = self.recalls.find(query, projection)

        if sort == "asc":
            cursor.sort("report_date", pymongo.ASCENDING)

        if sort == "desc":
            cursor.sort("report_date", pymongo.DESCENDING)

        if offset > 0:
            cursor.skip(offset)

        if count > 0:
            cursor.limit(count)

        results = list(cursor)
        return results

    def update(self, webcrawler, parsers):
        assert isinstance(webcrawler, WebCrawler)
        assert all([isinstance(p, WebParser) for p in parsers])

        results = None
        if self.last_modified() == 0:
            print("No database data detected. Populating database.")
            results = webcrawler.get_all()
        else:
            print(f"Updating database. Latest recall in database: {self.last_modified()}")
            date = datetime.strptime(self.last_modified(), "%Y%m%d")
            results = webcrawler.get_after(date)

        for result in results:
            for parser in parsers:
                result = parser.parse(result)

            self.add(result)

    def add(self, value):
        if "recall_number" not in value or self.query(value["recall_number"]) is not None:
            return

        self.recalls.insert_one(value)
