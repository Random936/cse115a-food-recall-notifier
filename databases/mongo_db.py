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
        newest = list(self.recalls.find({}, sort=[('timestamp', pymongo.DESCENDING)]).limit(n))
        for n in newest:
            del n["_id"]

        return newest

    def last_modified(self):
        newest = self.recalls.find_one({}, sort=[('timestamp', pymongo.DESCENDING)])
        return 0 if newest is None else newest['date_scraped']

    def query(self, key):
        item = self.recalls.find_one({"upc": key})
        if item is not None:
            del item["_id"]

        return item

    def search(self, field, term, offset, count):
        assert isinstance(offset, int) and isinstance(count, int)
        assert offset >= 0 and count > 0

        query = {field: {"$regex": term} }
        projection = {
            "_id": 0,
            "upc": 1,
            "date": 1,
            "brand": 1,
            "product_type": 1,
            "description": 1
        }

        print(query)
        cursor = self.recalls.find(query, projection)

        if offset > 0:
            cursor.skip(offset)

        if count > 0:
            cursor.limit(count)

        results = list(cursor)
        print(results)
        return results

    def update(self, webcrawler, parsers):
        assert isinstance(webcrawler, WebCrawler)
        assert all([isinstance(p, WebParser) for p in parsers])

        results = None
        if self.last_modified() == 0:
            results = webcrawler.get_all()
        else:
            results = webcrawler.get_from(datetime.fromisoformat(self.last_modified()))

        for result in results:
            for parser in parsers:
                result = parser.parse(result)

            self.add(result)

    def add(self, value):
        self.recalls.insert_one(value)
