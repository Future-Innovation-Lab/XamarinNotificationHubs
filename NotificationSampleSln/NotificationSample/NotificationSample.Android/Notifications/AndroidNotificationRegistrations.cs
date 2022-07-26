using NotificationHubSample.Interfaces;
using Xamarin.Forms;
using Firebase.Messaging;
using WindowsAzure.Messaging;

using Android.Util;
using System;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Firebase.Iid;
using Android.Gms.Extensions;

[assembly: Dependency(typeof(NotificationHubSample.Droid.Notifications.AndroidNotificationRegistrations))]

namespace NotificationHubSample.Droid.Notifications
{
    //XAMARIN FORMS VERSION
    public class AndroidNotificationRegistrations : INotificationRegistrations
    {
        public async Task SendRegistrationToServer()
        {
            await RefreshToken();


            var userTag = await SecureStorage.GetAsync("UserTag");

            var pushNotificationToken = await SecureStorage.GetAsync("PushNotificationToken");

            if (!string.IsNullOrEmpty(userTag) && !string.IsNullOrEmpty(pushNotificationToken))
            {

                await Task.Run(() =>
                {
                    try
                    {

                        NotificationHub hub = new NotificationHub(AppConstants.NotificationHubName, AppConstants.ListenConnectionString, Platform.CurrentActivity);

                        // register device with Azure Notification Hub using the token from FCM
                        Registration registration = hub.Register(pushNotificationToken, userTag);

                        // subscribe to the SubscriptionTags list with a simple template.
                        string pnsHandle = registration.PNSHandle;
                        TemplateRegistration templateReg = hub.RegisterTemplate(pnsHandle, "userTemplate", AppConstants.FCMTemplateBody, userTag);
                    }
                    catch (Exception e)
                    {
                        Log.Error(AppConstants.DebugTag, $"Error registering device: {e.Message}");
                    }
                });

            }
        }


        private async Task RefreshToken()
        {
            try
            {
                await Task.Run(async () =>
                {

                    NotificationHub hub = new NotificationHub(AppConstants.NotificationHubName, AppConstants.ListenConnectionString, Platform.CurrentActivity);

                    var oldToken = await SecureStorage.GetAsync("PushNotificationToken");

                    if (!string.IsNullOrEmpty(oldToken))
                        hub.UnregisterAll(oldToken);

                   // var instanceIdResult = await FirebaseInstanceId.Instance.GetInstanceId().AsAsync<IInstanceIdResult>();
                   // var token = instanceIdResult.Token;

                    var token = await FirebaseMessaging.Instance.GetToken();


                    await SecureStorage.SetAsync("PushNotificationToken", token.ToString());
                    hub.UnregisterAll(token.ToString());
                });
            }
            catch (Exception e)
            {
                Log.Error(AppConstants.DebugTag, $"Error refreshing token: {e.Message}");
            }
        }

    }
}