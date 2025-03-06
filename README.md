# CSE115A_Web_scraper_photo

This the complete version of the web scraper for **CSE115A Food Recall Notifier**

**This folder contains:**  
**FDA_Crawler.py**: Based on the information provided by the FDA API, the crawler is designed to crawl basic information except images and company statements.  
**FDA_Crawler_Photo.py**: Crawl from the official FDA recall website that includes product information and company statements (information not included in the API crawler).  
**merge.py**: Match and merge the results of the two crawlers.  
**.env**: Contains the FDA API KEYs required for FDA APIs.  

**Other files:**  
**food_recalls.json**: This is where the data is after you execute FDA_Crawler.py.    
**food_recall_announcement_photo.json**:This is where the data is after you execute FDA_Crawler_Photo.py.    
**food-enforcement-0001-of-0001.json**: This is a dataset downloaded from the FDA's official website (its data is theoretically the same as the data after the execution of FDA_Crawler.py), and can be used as an alternative if an error occurs in the API.(last updated:03/06/2025)    
**merged_food_recalls.json**: This is the merged dataset after executing merge.py and should be the final dataset.    


