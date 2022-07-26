using NotificationHubSample.Interfaces;
using NotificationSample.Event;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace NotificationSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private DelegateCommand _registerCommand;
        private INotificationRegistrations _notificationRegistrations;
        private IPageDialogService _pageDialogService;
        
        public DelegateCommand RegisterCommand =>
            _registerCommand ?? (_registerCommand = new DelegateCommand(ExecuteRegisterCommand));


        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private async void ExecuteRegisterCommand()
        {
            var userTag = UserName;
            await SecureStorage.SetAsync("UserTag", userTag);

//            INotificationRegistrations notifications = DependencyService.Get<INotificationRegistrations>();
            await _notificationRegistrations.SendRegistrationToServer();


            await _pageDialogService.DisplayAlertAsync("User Tag Registered", "The user " + userTag + " has been registered.", "Close");
        }
        public MainPageViewModel(INavigationService navigationService, INotificationRegistrations notificationRegistrations, IPageDialogService pageDialogService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            Title = "Samsung Futurelab Push Example";

            eventAggregator.GetEvent<NotificationEvent>().Subscribe(ShowPushMessage, ThreadOption.UIThread);

            _notificationRegistrations = notificationRegistrations;
            _pageDialogService = pageDialogService;
        }


        private async  void ShowPushMessage(string message)
        {
            await _pageDialogService.DisplayAlertAsync("Push Notification", message, "cancel");
        }
    }
}
