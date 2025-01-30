using Plugin.LocalNotification;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1) {
                var request = new NotificationRequest
                {
                    NotificationId = 1131,
                    Title = "Did it work?",
                    Subtitle = "I hope it does",
                    Description = "Yes!",
                    BadgeNumber = 1,
                    //Schedule = new NotificationRequestSchedule //to keep it repeating
                    //{
                    //    NotifyTime = DateTime.Now.AddSeconds(1)
                    //    //NotifyRepeatInterval = TimeSpan.FromDays(1),
                    //}
                };
                LocalNotificationCenter.Current.Show(request);
                Console.WriteLine("Tapped Notification");


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

