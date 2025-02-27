using System.Globalization;
namespace Food_Recall_Notif.Model
{
    public class Food_Item
    {
        public required string city { get; set; }
        public required string state { get; set; }
        public required string product_description { get; set; }
        public required string report_date { get; set; }
        public required string recall_number { get; set; }
        public required string product_type { get; set; }
        public string CityState => $"{city}, {state}";
        public string Date
        {
            get
            {
                if (DateTime.TryParse(report_date, out DateTime parsedDate))
                {
                    return parsedDate.ToString("MMMM dd, yyyy"); // Example: "June 20, 2012"
                }
                return "Invalid Date"; // Fallback if parsing fails
            }
        }

    }
    public class UPC_Item
    {
        public required string country { get; set; }
        public required string city { get; set; }
        public required string address_1 { get; set; }
        public required string address_2 { get; set; }
        public required string reason_for_recall { get; set; }
        public required string product_quantity { get; set; }
        public required string code_info { get; set; }
        public required string center_classification_date { get; set; }
        public required string distribution_pattern { get; set; }
        public required string state { get; set; }
        public required string product_description { get; set; }
        public required string report_date { get; set; }
        public required string classification { get; set; }
        public Dictionary<string, object> openfda { get; set; } = new Dictionary<string, object>();
        public required string recalling_firm { get; set; }
        public required string recall_number { get; set; }
        public required string initial_firm_notification { get; set; }
        public required string product_type { get; set; }
        public required string event_id { get; set; }
        public string? termination_date { get; set; }
        public string? more_code_info { get; set; }
        public required string recall_initiation_date { get; set; }
        public required string postal_code { get; set; }
        public required string voluntary_mandated { get; set; }
        public required string status { get; set; }
        public string CityState => $"{city}, {state}";
        public string Date
        {
            get
            {
                if (DateTime.TryParse(report_date, out DateTime parsedDate))
                {
                    return parsedDate.ToString("MMMM dd, yyyy"); // Example: "June 20, 2012"
                }
                return "Invalid Date"; // Fallback if parsing fails
            }
        }
    }


    public class ServerStatus
    {
        public required string name { get; set; }
        public required string version { get; set; }
        public required List<UPC_Item> newest { get; set; } // Change from string to List<RecallItem>
        public required string last_modified { get; set; }
    }
}
