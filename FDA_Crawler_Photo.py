import requests
from bs4 import BeautifulSoup
import json
import time
import re
from urllib.parse import urljoin

# FDA recall main page, which set to search in Food & Beverages
FDA_RECALLS_URL = "https://www.fda.gov/safety/recalls-market-withdrawals-safety-alerts?product_type=Food%20%26%20Beverages"
# AJAX URL
AJAX_URL = "https://www.fda.gov/datatables/views/ajax"

HEADERS = {
    "User-Agent": ("Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
                   "AppleWebKit/537.36 (KHTML, like Gecko) "
                   "Chrome/91.0.4472.124 Safari/537.36")
}

def flatten_list(lst):
# converts all elements in a nested list to strings
    result = []
    for item in lst:
        if isinstance(item, list):
            result.extend(flatten_list(item))
        else:
            result.append(str(item))
    return result

def fetch_food_recall_links_static(url):
# The first page is a static page
    recall_links = []
    try:
        response = requests.get(url, headers=HEADERS)
        response.raise_for_status()
    except requests.RequestException as e:
        print(f"Error in request: {e}")
        return recall_links

    soup = BeautifulSoup(response.text, 'html.parser')
    rows = soup.select("#datatable tbody tr")
    for row in rows:
        product_type_td = row.select_one("td.views-field-field-regulated-product-field")
        if product_type_td and "Food & Beverages" in product_type_td.get_text(strip=True):
            link_tag = row.select_one("a[href^='/safety/recalls-market-withdrawals-safety-alerts/']")
            if link_tag:
                href = link_tag["href"]
                full_url = urljoin("https://www.fda.gov", href)
                if full_url not in recall_links:
                    recall_links.append(full_url)
    return recall_links

def fetch_food_recall_links_ajax(page):
# The rest pages are loaded by AJAX
# Simulate AJAX request to get the link of recall announcement, page = AJAX page number, starting from 1 (the first page has been taken for static pages), and 10 data are returned per page.
    params = {
        "_drupal_ajax": 1,
        "_wrapper_format": "drupal_ajax",
        "pager_element": 0,
        "view_args": "",
        "view_base_path": "safety/recalls-market-withdrawals-safety-alerts/datatables-data",
        "view_display_id": "recall_datatable_block_1",
        "view_dom_id": "e0810721f0b8505216c9f7f83ce3d79c4f1db1d26234f5a2eb32b3696f2e00eb",
        "view_name": "recall_solr_index",
        "view_path": "/safety/recalls-market-withdrawals-safety-alerts",
        "draw": page + 1,
        "start": page * 10,  
        "length": 10,
        "search_api_fulltext": "",
        "field_regulated_product_field": "All",  
        "field_terminated_recall": "All"
    }
    recall_links = []
    try:
        response = requests.get(AJAX_URL, headers=HEADERS, params=params)
        response.raise_for_status()
    except requests.RequestException as e:
        print(f"AJAX Request Error (page {page}): {e}")
        return recall_links

    try:
        ajax_data = response.json()
    except json.JSONDecodeError as e:
        print(f"Error parsing AJAX response (page {page}): {e}")
        return recall_links

    # Parsing by list
    if isinstance(ajax_data, dict) and "data" in ajax_data and isinstance(ajax_data["data"], list):
        for row in ajax_data["data"]:
            if isinstance(row, list) and len(row) >= 4:
                # Column 3 (index 3) is the product type
                product_type = BeautifulSoup(row[3], 'html.parser').get_text(strip=True)
                if "Food" in product_type:  # For "Food"，"Food & Beverages"、"Foodborne Illness"、"Allergens" (Food & Beverages)
                    # Column 2 (index 1) contains links
                    soup_col = BeautifulSoup(row[1], 'html.parser')
                    link_tag = soup_col.find('a')
                    if link_tag and link_tag.get("href"):
                        full_url = urljoin("https://www.fda.gov", link_tag["href"])
                        if full_url not in recall_links:
                            recall_links.append(full_url)
    else:
        # Expand the data field into an HTML string and parse it.
        if isinstance(ajax_data, list):
            for command in ajax_data:
                if isinstance(command, dict) and "data" in command:
                    data_field = command["data"]
                    if isinstance(data_field, list):
                        flat_list = flatten_list(data_field)
                        html_fragment = "".join(flat_list)
                    elif isinstance(data_field, str):
                        html_fragment = data_field
                    else:
                        continue
                    fragment_soup = BeautifulSoup(html_fragment, 'html.parser')
                    rows = fragment_soup.select("tr")
                    for row in rows:
                        product_type_td = row.select_one("td.views-field-field-regulated-product-field")
                        if product_type_td and "Food & Beverages" in product_type_td.get_text(strip=True):
                            link_tag = row.select_one("a[href^='/safety/recalls-market-withdrawals-safety-alerts/']")
                            if link_tag:
                                href = link_tag["href"]
                                full_url = urljoin("https://www.fda.gov", href)
                                if full_url not in recall_links:
                                    recall_links.append(full_url)
        elif isinstance(ajax_data, dict) and "data" in ajax_data:
            data_field = ajax_data["data"]
            if isinstance(data_field, list):
                flat_list = flatten_list(data_field)
                html_fragment = "".join(flat_list)
            elif isinstance(data_field, str):
                html_fragment = data_field
            else:
                html_fragment = ""
            fragment_soup = BeautifulSoup(html_fragment, 'html.parser')
            rows = fragment_soup.select("tr")
            for row in rows:
                product_type_td = row.select_one("td.views-field-field-regulated-product-field")
                if product_type_td and "Food & Beverages" in product_type_td.get_text(strip=True):
                    link_tag = row.select_one("a[href^='/safety/recalls-market-withdrawals-safety-alerts/']")
                    if link_tag:
                        href = link_tag["href"]
                        full_url = urljoin("https://www.fda.gov", href)
                        if full_url not in recall_links:
                            recall_links.append(full_url)
        else:
            fragment_soup = BeautifulSoup(response.text, 'html.parser')
            rows = fragment_soup.select("tr")
            for row in rows:
                product_type_td = row.select_one("td.views-field-field-regulated-product-field")
                if product_type_td and "Food & Beverages" in product_type_td.get_text(strip=True):
                    link_tag = row.select_one("a[href^='/safety/recalls-market-withdrawals-safety-alerts/']")
                    if link_tag:
                        href = link_tag["href"]
                        full_url = urljoin("https://www.fda.gov", href)
                        if full_url not in recall_links:
                            recall_links.append(full_url)
    return recall_links

def fetch_recall_details(url):
# Fetch the details of each recall announcement
    try:
        response = requests.get(url, headers=HEADERS)
        response.raise_for_status()
    except requests.RequestException as e:
        print(f"Request Error {url}: {e}")
        return None

    soup = BeautifulSoup(response.text, 'html.parser')

    # Get the company announcement
    company_announcement_section = soup.find('h2', id='recall-announcement')
    company_announcement = "No announcement found"
    if company_announcement_section:
        paragraphs = company_announcement_section.find_next_siblings('p')
        company_announcement = " ".join([p.get_text(strip=True) for p in paragraphs])

    # Get the product images
    product_images = []
    image_section = soup.find('div', id='recall-photos')
    if image_section:
        img_tags = image_section.find_all('img')
        for img in img_tags:
            img_src = img.get('src', '')
            if img_src.startswith('/files'):
                img_src = urljoin("https://www.fda.gov", img_src)
            product_images.append(img_src)

    # Get the product details
    details = {}
    description_list = soup.find('dl', class_='lcds-description-list--grid')
    if description_list:
        items = description_list.find_all(['dt', 'dd'])
        for i in range(0, len(items), 2):
            key = items[i].get_text(strip=True).replace(":", "")
            value = items[i+1].get_text(strip=True)
            details[key] = value

    return {
        'url': url,
        'company_announcement': company_announcement,
        'product_images': product_images,
        'product_details': details
    }

def main():
    all_links = set()

    # Crawl static page (fisrt page)
    static_links = fetch_food_recall_links_static(FDA_RECALLS_URL)
    all_links.update(static_links)
    print(f"There are {len(static_links)} links in static page.")

    # Crawl the AJAX from 2nd page（page start from 1）
    page = 1
    max_page = 20  # Change this to the desired maximum page number
    while page < max_page:
        ajax_links = fetch_food_recall_links_ajax(page)
        if not ajax_links:
            print(f"No data on page {page} or error occurred, stopping pagination.")
            break
        new_links = set(ajax_links) - all_links
        if not new_links:
            print(f"No new links on page {page} , stopping pagination.")
            break
        all_links.update(new_links)
        print(f"Page {page} AJAX crawled {len(new_links)} new links，total {len(all_links)} links.")
        page += 1
        time.sleep(2)

    recall_links = list(all_links)
    print(f"Total {len(recall_links)} food recall announcement links found.")

    all_recall_details = []
    for idx, link in enumerate(recall_links):
        print(f"Crawling: {idx+1}/{len(recall_links)}: {link}")
        details = fetch_recall_details(link)
        if details:
            all_recall_details.append(details)
        time.sleep(2)

    with open('food_recall_announcement_photo.json', 'w', encoding='utf-8') as f:
        json.dump(all_recall_details, f, ensure_ascii=False, indent=4)
    print("All recall details have been saved to food_recall_announcement_photo.json")

if __name__ == '__main__':
    main()














