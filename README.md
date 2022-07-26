# XamarinNotificationHubs

## For Registering and Receiving Push Notifications use the Xamarin.Forms Project in NotificationSamples

### Change the AppConstants file with the Notification Hub Name and Listen Connecting string

 public static string NotificationHubName { get; set; } = "NotificationHubName";

 public static string ListenConnectionString { get; set; } = "ListenConnectionString";

Replace google-services.json file in Android project with file downloaded from Firebase portal


## For Sending Messages use the NotificationDispatcher project

### Change the DispatcherConstants file with the Notification Hub Name and Full Access Connecting string

 public static class DispatcherConstants
    {

       public static string[] SubscriptionTags { get; set; } = { "bobo" };

 
        public static string NotificationHubName { get; set; } = "NotificationHubName";

        public static string FullAccessConnectionString { get; set; } = "FullAccessConnectionString";
    }
    

## For Querying NotificatioQuery project

### Change the DispatcherConstants file with the Notification Hub Name and Full Access Connecting string

 public static class DispatcherConstants
    {

       public static string[] SubscriptionTags { get; set; } = { "bobo" };

 
        public static string NotificationHubName { get; set; } = "NotificationHubName";

        public static string FullAccessConnectionString { get; set; } = "FullAccessConnectionString";
    }
