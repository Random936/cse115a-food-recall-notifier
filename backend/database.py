import json
import hashlib
from pathlib import Path

class Database:
    def state(self):
        raise NotImplementedError

    def last_modified(self):
        raise NotImplementedError

    def query(self, key):
        raise NotImplementedError

    def search(self, searcher, term):
        raise NotImplementedError

    def update(self):
        raise NotImplementedError

    def add(self, key, value):
        raise NotImplementedError


class StaticDB(Database):
    def __init__(self, json_path):
        self.path = json_path
        self.db_lastmod = None
        self.update()

    def state(self):
        return self.db_hash

    def last_modified(self):
        return self.db_lastmod

    def update(self):

        pobj = Path(self.path)
        file_lastmod = int(pobj.lstat().st_mtime)

        # Already up to date
        if self.db_lastmod is not None and file_lastmod <= self.db_lastmod:
            return

        # Update DB
        with open(self.path, 'r') as f:
            self.db = sorted(json.load(f).items())

        self.db_lastmod = file_lastmod
        self.db_hash = hashlib.sha256(json.dumps(self.db).encode('utf-8')).hexdigest()


    def query(self, upc):
        print(self.db)
        return self.db.get(str(upc))
