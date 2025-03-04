import re
from parsers.webparser import WebParser

class UPCParser(WebParser):
    def parse(self, record):
        if "code_info" not in record or not isinstance(record["code_info"], str):
            print(f"UPCParser.parse(): Code info not found or not a string for recall {record.get('recall_number')}")
            return record

        matches = re.findall(r"(?:[01]-?)?(?:[0-9]{5}-?[0-9]{5})(?:-?[0-9])?", record["code_info"])
        upc_numbers = [str(m) for m in matches]

        record["upc"] = upc_numbers
        return record

import json

if __name__ == "__main__":
    with open('test.json', 'r') as f:
        records = json.load(f)

    parser = UPCParser()
    for record in records:
        print("DEBUG", parser.parse(record).get("upc"), record.get("code_info"))
