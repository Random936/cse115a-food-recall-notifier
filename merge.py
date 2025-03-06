# The main task for this code to merge the data from the FDA API crawler and the web crawler, and the current design is to use the data from the API as the base. 
# Then add information such as images and company announcements to the API. (Due to disorganzation of the info from FDA website)

import json
import re
from fuzzywuzzy import fuzz

# Get data from API and Image crawler result
with open('food_recalls.json', 'r', encoding='utf-8') as api_file:   # API
    api_recalls = json.load(api_file)
with open('food_recall_announcement_photo.json', 'r', encoding='utf-8') as web_file:  # Image/Annoucement
    web_recalls = json.load(web_file)


api_recalls = api_recalls[:500]    # take first 500 record (Change it base on your need)
web_recalls = web_recalls[:500]



# Pre-process company names to remove commercial suffixes (Inc., LLC, Ltd.) and standardize single quotes
def clean_company_name(name):
    name = re.sub(r"\b(Inc|LLC|Ltd|Co|Corporation|Company|Limited)\.?\b", "", name, flags=re.IGNORECASE)
    name = name.replace("’", "'").strip()  # Convert all single quotes
    return name.lower()  # Convert to lowercase

# Pre-process product descriptions by removing the “Product Description” prefix and keeping the main keywords.
def clean_product_description(desc):
    desc = desc.replace("Product Description", "").strip().lower()
    return re.sub(r"[^a-z0-9\s]", "", desc)  # Alphanumeric only

# Extract keywords from recall causes
def extract_recall_keywords(reason):
    reason = reason.lower()
    keywords = re.findall(r"\b[a-z]{4,}\b", reason)  # keywords with more than 4 letters
    return set(keywords)

# Convert web recalls into a dictionary 
web_dict = {}
for web_recall in web_recalls:
    product_details = web_recall.get("product_details", {})

    company_name = clean_company_name(product_details.get("Company Name", ""))
    product_desc = clean_product_description(product_details.get("Product Description", ""))
    recall_reason = extract_recall_keywords(product_details.get("Reason for Announcement", ""))   # Match the info base on company name, product description and recall reason

    key = f"{company_name}|{product_desc}"
    web_dict[key] = {
        "url": web_recall.get("url"),
        "company_announcement": web_recall.get("company_announcement", "No announcement found"),
        "product_images": web_recall.get("product_images", []),
        "recall_reason_keywords": recall_reason
    }

# Traversing API data -> Use fuzzy matching to match data
merged_results = []
for api_recall in api_recalls:
    company_name = clean_company_name(api_recall.get("recalling_firm", ""))
    product_desc = clean_product_description(api_recall.get("product_description", ""))
    recall_reason_keywords = extract_recall_keywords(api_recall.get("reason_for_recall", ""))

    best_match = None
    best_score = 0

    for web_key, web_data in web_dict.items():
        web_company, web_product = web_key.split("|")

        company_score = fuzz.ratio(company_name, web_company)  # company name
        product_score = fuzz.partial_ratio(product_desc, web_product) # product description
        common_keywords = recall_reason_keywords & web_data["recall_reason_keywords"]  # recall reason
        recall_score = len(common_keywords) * 10  # 10 points for each matched keywords

        # total match score
        total_score = company_score * 0.4 + product_score * 0.4 + recall_score * 0.2

        # Best match
        if total_score > best_score and total_score >= 70:  # Here I set as score as 70, you can modify this if you'd like to get higher accuracy
            best_match = web_data
            best_score = total_score

    # Merge the matched data
    if best_match:
        api_recall["company_announcement"] = best_match.get("company_announcement", "No announcement found")
        api_recall["product_images"] = best_match.get("product_images", [])
        api_recall["fda_announcement_url"] = best_match.get("url")

    # Keep the API result anyway (If not match)
    merged_results.append(api_recall)

# Merge the result
with open('merged_food_recalls.json', 'w', encoding='utf-8') as output_file:
    json.dump(merged_results, output_file, ensure_ascii=False, indent=4)

print(f"Merged {len(merged_results)} recall records into merged_food_recalls.json")


