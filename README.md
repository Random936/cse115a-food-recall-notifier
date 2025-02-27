Open the Maui.sln file in visual studio 2022, and then you can simulate. 

Currently receives new format as of 2/26/2025.
Format:
{
    "status": "Ongoing",
    "city": "Farmers Branch",
    "state": "TX",
    "country": "United States",
    "classification": "Class I",
    "openfda": {},
    "product_type": "Food",
    "event_id": "95888",
    "recalling_firm": "GBC Food Services, LLC dba Yummi Sushi",
    "address_1": "14043 Distribution Way",
    "address_2": "",
    "postal_code": "75234-3438",
    "voluntary_mandated": "Voluntary: Firm initiated",
    "initial_firm_notification": "E-Mail",
    "distribution_pattern": "CO",
    "recall_number": "F-0438-2025",
    "product_description": "Cucumber w/ Ranch Snack Cup\t850065403144\tNet Wt: 9.5oz (269g)",
    "product_quantity": "6088 units (total)",
    "reason_for_recall": "Potential Salmonella",
    "recall_initiation_date": "20241129",
    "center_classification_date": "20250129",
    "report_date": "20250205",
    "code_info": "Purchase Date: 11/22 to 11/29/2024",
    "more_code_info": ""
}


Gets from dedicated website, and then deserializes JSON in the format of into a class called ServerStatus containing name, version, newest, and last_modified. The newest being the list of objects in the class of 
Recall Item, containing status, product_description, reason_for_recall, and report_date. 

Saves a JSON file at default AppDataDirectory named "server_status.json".

Once the app is opened, it takes the file, and compares it to the new data that is used from ClientAPI, basically a "get" but for the website.
Then it takes the product_descriptions not in the old file, and uses it to make the notification content.

