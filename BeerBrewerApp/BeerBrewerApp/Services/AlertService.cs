using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    public interface IAlertService
    {
        // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----
        Task ShowAlertAsync(string title, string message, string cancel = "OK");
        Task<bool> ShowConfirmationAsync(string title, string message, string confirm = "Yes", string cancel = "No");
        Task<string> ShowPopupAsync(string title, string message, string confirm = "Sumbit", string cancel = "Cancel");

        // ----- "Fire and forget" calls -----
        void ShowAlert(string title, string message, string cancel = "OK");
        /// <param name="callback">Action to perform afterwards.</param>
        void ShowConfirmation(string title, string message, Action<bool> callback, string confirm = "Yes", string cancel = "No");
        void ShowPopup(string title, string message, Action<string> callback, string confirm = "Sumbit", string cancel = "Cancel");
    }
    internal class AlertService : IAlertService
    {
        public Task ShowAlertAsync(string title, string message, string cancel = "OK")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public Task<bool> ShowConfirmationAsync(string title, string message, string confirm = "Yes", string cancel = "No")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, confirm, cancel);
        }

        public Task<string> ShowPopupAsync(string title, string message, string confirm = "Sumbit", string cancel = "Cancel")
        {
            return Application.Current.MainPage.DisplayPromptAsync(title, message, confirm, cancel);
        }

        // ----- "Fire and forget" calls -----
        public void ShowAlert(string title, string message, string cancel = "OK")
        {
            Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                await ShowAlertAsync(title, message, cancel)
            );
        }

        public void ShowConfirmation(string title, string message, Action<bool> callback,
                                     string confirm = "Yes", string cancel = "No")
        {
            Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            {
                bool answer = await ShowConfirmationAsync(title, message, confirm, cancel);
                callback(answer);
            });
        }

        public void ShowPopup(string title, string message, Action<string> callback, string confirm = "Sumbit", string cancel = "Cancel")
        {
            Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            {
                string answer = await ShowPopupAsync(title, message, confirm, cancel);
                callback(answer);
            });
        }
    }
}
