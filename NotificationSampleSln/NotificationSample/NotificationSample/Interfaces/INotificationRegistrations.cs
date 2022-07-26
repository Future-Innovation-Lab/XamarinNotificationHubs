using System.Threading.Tasks;

namespace NotificationHubSample.Interfaces
{
    public interface INotificationRegistrations
    {
        Task SendRegistrationToServer();
    }
}
