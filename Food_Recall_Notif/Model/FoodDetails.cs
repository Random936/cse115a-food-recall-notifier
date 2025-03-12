using System.Text.RegularExpressions;
namespace Food_Recall_Notif.Model
{
    public class Food_Item
    {
        public required string report_date { get; set; }
        public required string recall_number { get; set; }
        public required string product_type { get; set; }
        public required string recalling_firm {get; set; }
        public required string distribution_pattern { get; set; }
        public List<string> ParsedStates
        {
            get
            {
                if (string.IsNullOrWhiteSpace(distribution_pattern))
                    return [];

                if (distribution_pattern.Trim().Equals("Nationwide", StringComparison.OrdinalIgnoreCase))
                    return ["Nationwide"];

                // Regular expression to match two-letter uppercase state abbreviations
                string statePattern = @"\b[A-Z]{2}\b";

                // Extract matches
                var matches = Regex.Matches(distribution_pattern, statePattern)
                                   .Select(m => m.Value)
                                   .ToList();

                // If no valid states were found, return the original distribution_pattern
                return matches.Any() ? matches : [distribution_pattern];
            }
        }

        public string Date
            {
                get
                {
                    if (report_date.Length == 8) // Ensure the string is in "YYYYMMDD" format
                    {
                        string year = report_date[..4];
                        string month = report_date.Substring(4, 2);
                        string day = report_date.Substring(6, 2);
                        
                        if (int.TryParse(year, out int y) &&
                            int.TryParse(month, out int m) &&
                            int.TryParse(day, out int d))
                        {
                            try
                            {
                                DateTime parsedDate = new(y, m, d);
                                return parsedDate.ToString("MMMM dd, yyyy"); // Example: "June 12, 2024"
                            }
                            catch
                            {
                                return "Invalid Date"; // If an invalid date is given
                            }
                        }
                    }
                    return "Invalid Date"; // If the format is incorrect
                }
            }
        public string ParsedStatesFormatted => ParsedStates.Any()
        ? $"States affected: {string.Join(", ", ParsedStates)}"
        : "States affected: None";

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
        public Dictionary<string, object> openfda { get; set; } = [];
        public required string recalling_firm { get; set; }
        public required string recall_number { get; set; }
        public required string initial_firm_notification { get; set; }
        public required string product_type { get; set; }
        public required string event_id { get; set; }
        public string? termination_date { get; set; } = "Not terminated yet";
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
                if (report_date.Length == 8) // Ensure the string is in "YYYYMMDD" format
                {
                    string year = report_date[..4];
                    string month = report_date.Substring(4, 2);
                    string day = report_date.Substring(6, 2);

                    if (int.TryParse(year, out int y) &&
                        int.TryParse(month, out int m) &&
                        int.TryParse(day, out int d))
                    {
                        try
                        {
                            DateTime parsedDate = new(y, m, d);
                            return parsedDate.ToString("MMMM dd, yyyy"); // Example: "June 12, 2024"
                        }
                        catch
                        {
                            return "Invalid Date"; // If an invalid date is given
                        }
                    }
                }
                return "Invalid Date"; // If the format is incorrect
            }

        }
        public List<string> ParsedStates
        {
            get
            {
                if (string.IsNullOrWhiteSpace(distribution_pattern))
                    return [];

                // Extract the part after "distributed to the following states :"
                string pattern = @"distributed to the following states\s*:\s*";
                string statesPart = Regex.Replace(distribution_pattern, pattern, "", RegexOptions.IgnoreCase);

                // Split by comma and trim spaces
                return statesPart
                    .Split(',')
                    .Select(s => s.Trim()) // Remove extra spaces
                    .Where(s => s.Length == 2) // Ensure valid state abbreviations
                    .ToList();
            }
        }
        public string ParsedStatesFormatted => ParsedStates.Any()
            ? $"States affected: {string.Join(", ", ParsedStates)}"
            : "States affected: None";
        public string FullAddress
        {
            get
            {
                return $"{address_1}, {city}, {state}, {postal_code}, {country}";
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
