using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CustomVisionCompanion.Services
{
    public enum HistoryBehavior
    {
        Default,
        ClearHistory
    }

    public class NavigationService : INavigationService
    {
        private Dictionary<string, Type> pages { get; } = new Dictionary<string, Type>();

        public static Page CurrentPage { get; set; }

        public Page MainPage
        {
            get
            {
                var page = CurrentPage ?? Application.Current.MainPage;
                if (page is MasterDetailPage)
                {
                    page = (page as MasterDetailPage).Detail;
                }

                return page;
            }
            private set
            {
                var page = CurrentPage ?? Application.Current.MainPage;
                if (page is MasterDetailPage)
                {
                    (page as MasterDetailPage).Detail = value;
                }
                else
                {
                    Application.Current.MainPage = value;
                }
            }
        }

        public void Configure(string key, Type pageType) => pages.Add(key, pageType);

        public string CurrentPageKey
        {
            get
            {
                var currentPageKey = (MainPage?.Navigation?.NavigationStack.LastOrDefault()?.BindingContext)?.GetType().Name;
                return currentPageKey;
            }
        }

        public bool CanGoBack => MainPage.Navigation.NavigationStack.Count() > 1;

        public void GoBack() => GoBack(null);

        public void GoBack(int? clearStackLevel = null)
        {
            var navigation = MainPage.Navigation;

            if (clearStackLevel == null)
            {
                navigation.PopAsync();
            }
            else
            {
                // Goes back for the specified number of pages.
                var existingPages = navigation.NavigationStack.ToList();
                var startStackPoint = existingPages.Count - clearStackLevel.Value;
                for (var i = startStackPoint; i < existingPages.Count; i++)
                {
                    navigation.RemovePage(existingPages[i]);
                }
            }
        }

        public void NavigateTo(string pageKey) => NavigateTo(pageKey, null);

        public void NavigateTo(string pageKey, object parameter) => NavigateTo(pageKey, parameter, HistoryBehavior.Default);

        public void NavigateTo(string pageKey, HistoryBehavior historyBehavior, int? clearStackLevel = null) => NavigateTo(pageKey, null, historyBehavior, clearStackLevel);

        public void NavigateTo(string pageKey, object parameter, HistoryBehavior historyBehavior, int? clearStackLevel = null)
        {
            if (pages.TryGetValue(pageKey, out var pageType))
            {
                var displayPage = (Page)Activator.CreateInstance(pageType);
                var currentPage = MainPage.Navigation.NavigationStack.LastOrDefault();

                if (displayPage.GetType() == currentPage.GetType() && historyBehavior == HistoryBehavior.Default)
                {
                    // Navigation to the same page type. Skips.
                    return;
                }

                var navigation = MainPage.Navigation;

                if (historyBehavior == HistoryBehavior.ClearHistory)
                {
                    if (clearStackLevel == null)
                    {
                        displayPage.SetNavigationArgs(parameter);
                        //navigation.InsertPageBefore(displayPage, MainPage.Navigation.NavigationStack[0]);

                        // Since we want to clear history, removes all the other pages from the navigation stack.
                        //var existingPages = navigation.NavigationStack.ToList();
                        //for (var i = 1; i < existingPages.Count; i++)
                        //{
                        //    navigation.RemovePage(existingPages[i]);
                        //}
                        MainPage = new NavigationPage(displayPage);
                    }
                    else
                    {
                        navigation.PushAsync(displayPage, parameter, animated: true);

                        // Deletes the stack till to the specified depth.
                        var existingPages = navigation.NavigationStack.ToList();
                        var startStackPoint = existingPages.Count - 2;
                        for (var i = startStackPoint; i > startStackPoint - clearStackLevel; i--)
                        {
                            navigation.RemovePage(existingPages[i]);
                        }
                    }
                }
                else
                {
                    // Simply pushes the page to the navigation stack.
                    navigation.PushAsync(displayPage, parameter, animated: true);
                }

                // After navigation, if the MainPage is a Master/Detail page, automatically closes the menu.
                if ((CurrentPage ?? Application.Current.MainPage) is MasterDetailPage page)
                {
                    page.IsPresented = false;
                }
            }
            else
            {
                throw new ArgumentException(
                          $"No such page: {pageKey}. Did you forget to call NavigationService.Configure?",
                          nameof(pageKey));
            }
        }
    }
}
