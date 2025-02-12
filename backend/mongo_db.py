import pymongo
from database import Database

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
        return 0 if newest is None else newest

    def query(self, key):
        raise NotImplementedError

    def search(self, searcher, term):
        raise NotImplementedError

    def update(self):
        raise NotImplementedError

    def add(self, value):
        raise NotImplementedError
