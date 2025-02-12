import pymongo
from database import Database
from collections import defaultdict

class MongoDB(Database):
    def __init__(self, host, port):
        assert isinstance(host, str)
        assert isinstance(port, int)

        self.client = pymongo.MongoClient(host, port)
        self.database = self.client.get_database("recall-notifier")
        self.recalls = self.database.get_collection("recalls")

    def state(self):
        return self.database.command("dbhash")["md5"]

    def last_modified(self):
        newest = self.recalls.find_one({}, sort=[('timestamp', pymongo.DESCENDING)])
        return 0 if newest is None else newest['date_scraped']

    def query(self, key):
        item = self.recalls.find_one({"upc": key})
        del item["_id"]
        return item

    def search(self, field, term, offset, count):
        assert offset >= 0 and count > 0
        query = {field: term}
        projection = {
            "_id": 0,
            "upc": 1,
            "date": 1,
            "brand": 1,
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

    def update(self):
        raise NotImplementedError

    def add(self, value):
        self.recalls.insert_one(value)
