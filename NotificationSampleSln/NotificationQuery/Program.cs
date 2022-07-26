using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationDispatcher
{
    class Program
    {
        static int messageCount = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine($"Press the spacebar to send a message to each tag in {string.Join(", ", DispatcherConstants.SubscriptionTags)}");
            WriteSeparator();
        
                await GetRegistrations();
              
        }
        
        private async static Task  GetRegistrations()
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(DispatcherConstants.FullAccessConnectionString, DispatcherConstants.NotificationHubName);

            int registrationsCount=0;
            int tagCount=0;

            try
            {
//                 hub.un("2CF6D452A79ED9F3BF5FE587D04252969C820CC5316EB99711D22431411CF91F");

                var registrations = await hub.GetAllRegistrationsAsync(0);

                var continuationToken = registrations.ContinuationToken;
    var registrationDescriptionsList = new List<RegistrationDescription>(registrations);

    while (!string.IsNullOrWhiteSpace(continuationToken))
    {
        var otherRegistrations = await hub.GetAllRegistrationsAsync(continuationToken, 0);
        registrationDescriptionsList.AddRange(otherRegistrations);
        continuationToken = otherRegistrations.ContinuationToken;
    }


                foreach (var registration in registrationDescriptionsList)
                {
                    registrationsCount++;

                    Console.WriteLine("PNS: " + registration.PnsHandle);
                    //registration.
                    var tags = registration.Tags;

                    Console.WriteLine("Expiration: " + registration.ExpirationTime);
                    Console.WriteLine("ID: " + registration.RegistrationId);

                    foreach(var tag in tags)
                    {
                        Console.WriteLine("Tag: " + tag);
                        tagCount++;

                      /*  if (tag == "144290")
                        {
                            await hub.DeleteRegistrationAsync(registration.RegistrationId);
                             Console.WriteLine("Deleting");
                        }*/
                    }

             /*   if (registrationsCount == 100)
                {
                   await hub.DeleteRegistrationAsync(registration.RegistrationId);
                Console.WriteLine("Deleting");
                }
*/
                    Console.WriteLine();
                    Console.WriteLine();
                }
                    Console.WriteLine("Registration Count: "+registrationsCount );
                    Console.WriteLine("Tag Count: "+tagCount );

            }
            catch (Exception e)
            {

            }
        }
        private static async Task SendTemplateNotificationsAsync()
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(DispatcherConstants.FullAccessConnectionString, DispatcherConstants.NotificationHubName);
            Dictionary<string, string> templateParameters = new Dictionary<string, string>();

            messageCount++;

            // Send a template notification to each tag. This will go to any devices that
            // have subscribed to this tag with a template that includes "messageParam"
            // as a parameter
            foreach (var tag in DispatcherConstants.SubscriptionTags)
            {
                templateParameters["messageParam"] = $"Netcash Notification #{messageCount} to {tag}";
                //                     templateParameters["messageParam"] = $"Netcash Notification to user 11";

                try
                {
                    await hub.SendTemplateNotificationAsync(templateParameters, tag);
                    Console.WriteLine($"Sent message to {tag} subscribers.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send template notification: {ex.Message}");
                }
            }

            Console.WriteLine($"Sent messages to {DispatcherConstants.SubscriptionTags.Length} tags.");
            WriteSeparator();
        }

        private static void WriteSeparator()
        {
            Console.WriteLine("==========================================================================");
        }
    }
}
