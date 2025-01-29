import csv

class Database:
    def query(self, key):
        raise NotImplementedError

    def search(self, searcher, term):
        raise NotImplementedError

    def update(self):
        raise NotImplementedError

    def add(self, key, value):
        raise NotImplementedError


class CsvDB(Database):
    def __init__(self, csv_path):
        self.path = csv_path
        self.db = {}
        self.update()

    def update(self):
        with open(self.path, 'r') as f:
            reader = csv.DictReader(f)

            row = {
                "upc": 


           ['Date',
            'Brand-Names',
            'Product-Description',
            'Product-Types',
            'Recall-Reason-Description',
            'Company-Name',
            'Terminated Recall'] 
            
            print(reader.fieldnames)
            self.db 

