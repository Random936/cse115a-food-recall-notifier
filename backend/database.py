import json

class Database:
    def query(self, key):
        raise NotImplementedError

    def search(self, searcher, term):
        raise NotImplementedError

    def update(self):
        raise NotImplementedError

    def add(self, key, value):
        raise NotImplementedError


class StaticDB(Database):
    def __init__(self, csv_path):
        self.path = csv_path
        self.update()

    def update(self):
        with open(self.path, 'r') as f:
            self.db = json.load(f)

    def query(self, upc):
        print(self.db)
        return self.db.get(str(upc))
