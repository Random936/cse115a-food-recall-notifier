using Plugin.LocalNotification;
using System.Diagnostics;
namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {

            InitializeComponent();
            ClientAPI client = new ClientAPI();
            Task<ServerStatus> Status = client.ClientAPIMain();
            Status.Wait();
    
            Debug.WriteLine(Status.Result.db_state);
            

        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            //get {"db_hash": "930eda84e917a5be65720605923ed5800d8d286967c7dd9bb71665831c5ba3f0", "date": 1738703106}, hash string if different from previous, then download data again.  
            //ex 
            //start = page
            //count = 10
            //domain to query notifier-api.randomctf.com http req
            count++;

            if (count == 1) {
                var request = new NotificationRequest
                {
                    NotificationId = 1131,
                    Title = "Pineapples 1/23/25",
                    Subtitle = "",
                    Description = "Pineapples has been recalled is for salmonella being found in products.",
                    BadgeNumber = 1,
                    //Schedule = new NotificationRequestSchedule //to keep it repeating
                    //{
                    //    NotifyTime = DateTime.Now.AddSeconds(1)
                    //    //NotifyRepeatInterval = TimeSpan.FromDays(1),
                    //}
                };
                LocalNotificationCenter.Current.Show(request);
                //Console.WriteLine("Tapped Notification");


                CounterBtn.Text = $"Clicked {count} tdsadsaime";
            }
            else { 
                var request = new NotificationRequest
                {
                    NotificationId = 1132,
                    Title = "Yes",
                    Subtitle = "YES",
                    Description = "YES!!!",
                    BadgeNumber = 2,
                    //Schedule = new NotificationRequestSchedule //to keep it repeating
                    //{
                    //    NotifyTime = DateTime.Now.AddSeconds(1)
                    //    //NotifyRepeatInterval = TimeSpan.FromDays(1),
                    //}
                };
            LocalNotificationCenter.Current.Show(request);
            CounterBtn.Text = $"Clicked {count} times";
                }
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}

