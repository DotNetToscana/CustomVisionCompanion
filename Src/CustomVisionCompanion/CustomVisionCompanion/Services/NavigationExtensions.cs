using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Settings;

namespace Xamarin.Forms
{
    /// <summary>
    /// Extensions for saving and restoring navigation states with arguments.
    /// https://github.com/aloisdeniel/Xam.Forms.NavigationExtensions
    /// </summary>
    public static class NavigationExtensions
    {
        #region Arguments access

        /// <summary>
        /// All the navigation arguments associated to pages.
        /// </summary>
        private static ConditionalWeakTable<Page, object> arguments = new ConditionalWeakTable<Page, object>();

        /// <summary>
        /// Gets the navigation arguments for a page.
        /// </summary>
        /// <returns>The navigation arguments.</returns>
        /// <param name="page">Page.</param>
        public static object GetNavigationArgs(this Page page)
        {
            object argument = null;
            arguments.TryGetValue(page, out argument);

            return argument;
        }

        /// <summary>
        /// Stores the navigation arguments for a page.
        /// </summary>
        /// <returns>The navigation arguments.</returns>
        /// <param name="page">Page.</param>
        /// <param name="args">Arguments.</param>
        public static void SetNavigationArgs(this Page page, object args)
        {
            arguments.Add(page, args);
        }

        #endregion

        #region State restoration

        /// <summary>
        /// Restores all stacks from saved state in local storage.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">Navigation.</param>
        /// <param name="name">The base identifier of the stack.</param>
        /// <param name="maximumRestoreSpan">The maximum time for navigation restoration.</param>
        public static async Task RestoreAsync(this INavigation navigation, TimeSpan maximumRestoreSpan)
        {
            try
            {
                var states = GetNavigationHistory();
                if (states != null)
                {
                    if (DateTime.Now - states.Date < maximumRestoreSpan)
                    {
                        var navigationPages = states.Navigation.Select(RestorePage).ToList();
                        var modalPages = states.Modal.Select(RestorePage).ToList();

                        if (navigationPages.Count > 1)
                        {
                            var initialPages = navigation.NavigationStack.ToList();

                            // 1. Restoring navigation stack
                            var lastNavigationPage = navigationPages.LastOrDefault();
                            navigationPages.RemoveAt(navigationPages.Count - 1);
                            await navigation.PushAsync(lastNavigationPage, false);
                            foreach (var page in navigationPages)
                                navigation.InsertPageBefore(page, lastNavigationPage);

                            // 2. Removing already present pages before restore
                            foreach (var page in initialPages)
                                navigation.RemovePage(page);
                        }

                        if (modalPages.Count > 0)
                        {
                            //3. Restoring modal stack
                            foreach (var page in modalPages)
                            {
                                await navigation.PushModalAsync(page, false);

                                // HACK: lack of InsertPageBefore for modal stack
                                if (Device.RuntimePlatform == Device.iOS)
                                    await Task.Delay(100);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Stores all the navigations stacks with the given name into local storage.
        /// </summary>
        /// <param name="navigation">The navigation service.</param>
        public static void Store(this INavigation navigation)
        {
            var states = new NavigationStates()
            {
                Date = DateTime.Now,
                Navigation = ConvertPages(navigation.NavigationStack),
                Modal = ConvertPages(navigation.ModalStack),
            };

            try
            {
                SaveNavigationHistory(states);
            }
            catch { }
        }

        /// <summary>
        /// Restores the page by instanciating the page and argument from stored state.
        /// </summary>
        /// <returns>The page.</returns>
        /// <param name="state">State.</param>
        private static Page RestorePage(PageState state)
        {
            var page = Activator.CreateInstance(state.PageType) as Page;
            var argument = state.Argument;
            page.SetNavigationArgs(argument);
            return page;
        }

        private static IEnumerable<PageState> ConvertPages(IEnumerable<Page> pages)
            => pages.Select((p) => new PageState()
            {
                PageType = p.GetType(),
                Argument = p.GetNavigationArgs(),
            });

        #endregion

        #region Navigation

        /// <summary>
        /// Navigates to the given page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="page">The destination page.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        public static Task PushAsync(this INavigation navigation, Page page, object args, bool animated)
        {
            page.SetNavigationArgs(args);
            return navigation.PushAsync(page, animated);
        }

        /// <summary>
        /// Navigates modaly to the given page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="page">The destination page.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        public static Task PushModalAsync(this INavigation navigation, Page page, object args, bool animated)
        {
            page.SetNavigationArgs(args);
            return navigation.PushModalAsync(page, animated);
        }

        /// <summary>
        /// Instanciates a page from its type(must have an empty constructor) and navigates to this page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="pageType">The destination page type.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        public static Task PushAsync(this INavigation navigation, Type pageType, object args = null, bool animated = true)
        {
            var page = Activator.CreateInstance(pageType) as Page;
            return navigation.PushAsync(page, args, animated);
        }

        /// <summary>
        /// Instanciates a page from its type(must have an empty constructor) and navigates modaly to this page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="pageType">The destination page type.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        public static Task PushModalAsync(this INavigation navigation, Type pageType, object args = null, bool animated = true)
        {
            var page = Activator.CreateInstance(pageType) as Page;
            return navigation.PushAsync(page, args, animated);
        }

        /// <summary>
        /// Instanciates a page from its type(must have an empty constructor) and navigates to this page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        /// <typeparam name="T">The destination page type.</typeparam>
        public static Task PushAsync<T>(this INavigation navigation, object args = null, bool animated = true) where T : Page
        {
            return navigation.PushAsync(typeof(T), args, animated);
        }

        /// <summary>
        /// Instanciates a page from its type(must have an empty constructor) and navigates modaly to this page with an argument that will be available from this page, but also stored with the navigation state.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="args">The navigation arguments.</param>
        /// <param name="animated">Indicates whether the navigation should be animated.</param>
        /// <typeparam name="T">The destination page type.</typeparam>
        public static Task PushModalAsync<T>(this INavigation navigation, object args = null, bool animated = true) where T : Page
        {
            return navigation.PushAsync(typeof(T), args, animated);
        }

        #endregion

        #region Settings

        private const string NavigationHistory = "NavigationHistory";

        private static NavigationStates GetNavigationHistory()
        {
            var history = CrossSettings.Current.GetValueOrDefault(NavigationHistory, null);
            if (history != null)
                return JsonConvert.DeserializeObject<NavigationStates>(history);

            return null;
        }

        private static void SaveNavigationHistory(NavigationStates states)
        {
            var history = JsonConvert.SerializeObject(states);
            CrossSettings.Current.AddOrUpdateValue(NavigationHistory, history);
        }

        #endregion

        private class NavigationStates
        {
            /// <summary>
            /// Gets or sets the store date.
            /// </summary>
            /// <value>The date.</value>
            public DateTime Date { get; set; }

            /// <summary>
            /// Gets or sets the all the stored navigation states (page and args).
            /// </summary>
            /// <value>The navigation stack.</value>
            public IEnumerable<PageState> Navigation { get; set; }

            /// <summary>
            /// Gets or sets the all the stored modal navigation states (page and args).
            /// </summary>
            /// <value>The modal stack.</value>
            public IEnumerable<PageState> Modal { get; set; }
        }

        private class PageState
        {
            /// <summary>
            /// Gets or sets the type of the page.
            /// </summary>
            /// <value>The type of the page.</value>
            public Type PageType { get; set; }

            /// <summary>
            /// Gets or sets the navigation argument.
            /// </summary>
            /// <value>The argument.</value>
            public object Argument { get; set; }
        }
    }
}