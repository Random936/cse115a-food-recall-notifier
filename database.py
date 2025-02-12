class Database:
    def state(self):
        raise NotImplementedError

    def last_modified(self):
        raise NotImplementedError

    def query(self, key):
        raise NotImplementedError

    def search(self, field, term, offset, count):
        raise NotImplementedError

    def update(self):
        raise NotImplementedError

    def add(self, value):
        raise NotImplementedError

