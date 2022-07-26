using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using AndroidX.Core.App;
using Firebase.Messaging;
using NotificationSample.Droid;
using System;
using System.Linq;
using WindowsAzure.Messaging;
using Xamarin.Essentials;
using Resource = NotificationSample.Droid.Resource;
using Prism.Ioc;
using Prism.Events;
using NotificationSample.Event;
using NotificationSample;
using NotificationSample.Views;
using Prism.Navigation;
using System.Threading.Tasks;

namespace NotificationHubSample.Droid
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        public async override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            string messageBody = string.Empty;

            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }

            else
            {
                messageBody = message.Data.Values.First();
            }

            SendLocalNotification(messageBody);
            await SendMessageToMainPage(messageBody);
        }

        public async override void OnNewToken(string token)
        {
           try
            {
                NotificationHub hub = new NotificationHub(AppConstants.NotificationHubName, AppConstants.ListenConnectionString, this);

                var oldToken = await SecureStorage.GetAsync("PushNotificationToken");

                if (!string.IsNullOrEmpty(oldToken))
                    hub.UnregisterAll(oldToken);

                await SecureStorage.SetAsync("PushNotificationToken", token);
                hub.UnregisterAll(token);


                var userTag = await SecureStorage.GetAsync("UserTag");

                // If there's a user and there's a token refresh re-register
                if (!string.IsNullOrEmpty(userTag))
                {
                    Registration registration = hub.Register(token, userTag);

                    string pnsHandle = registration.PNSHandle;
                    TemplateRegistration templateReg = hub.RegisterTemplate(pnsHandle, "userTemplate", AppConstants.FCMTemplateBody, userTag);
                }

            }
            catch (Exception e)
            {
                Log.Error(AppConstants.DebugTag, $"Error registering device: {e.Message}");
            }
           
        }

        void SendLocalNotification(string body)
        {
            Random rnd = new Random();
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", body);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, AppConstants.NotificationChannelName)
                .SetContentTitle("Samsung Future Lab Notification")

               .SetSmallIcon(Resource.Drawable.icon)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetShowWhen(true)
                .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationBuilder.SetChannelId(AppConstants.NotificationChannelName);
            }

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        private async Task  SendMessageToMainPage(string body)
        {
          var container = ContainerLocator.Container;
          var eventAggregator =  container.Resolve<IEventAggregator>();
           
            var nav = container.Resolve<INavigationService>();
           // await nav.NavigateAsync("PushNotificationPage");
            

            eventAggregator.GetEvent<NotificationEvent>().Publish(body);
         //   (App.Current.MainPage as MainPage)?.AddMessage(body);
        }
    }
}