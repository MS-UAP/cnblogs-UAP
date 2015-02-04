using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace CNBlogs
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        Frame rootFrame;

#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            this.rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 2;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;





#endif

                Init();

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();



#if WINDOWS_APP
            // Add commands to the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += (s, args) =>
            {
                Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                var preference = new SettingsCommand("Preference", loader.GetString("SettingPane_Item_Preference"), (handler) =>
                new PreferenceSettingsFlyout().Show());
                args.Request.ApplicationCommands.Add(preference);

                var about = new SettingsCommand("About", loader.GetString("SettingPane_Item_About"), (handler) =>
                new AboutSettingsFlyout().Show());
                args.Request.ApplicationCommands.Add(about);

                var privacy = new SettingsCommand("PrivacyStatements", loader.GetString("SettingPane_Item_Privacy"), (handler) =>
                new PrivacyStatementsSettingsFlyout().Show());
                args.Request.ApplicationCommands.Add(privacy);

            };
#endif
        }

        private async void Init()
        {
            await BackgroundTaskHelper.Register();
#if WINDOWS_APP
            Logger.LogAgent.GetInstance().Register("MS-UAP", "CNBlogs-Win8.1");
#endif
#if WINDOWS_PHONE_APP
            Logger.LogAgent.GetInstance().Register("MS-UAP", "CNBlogs-WP8.1");

#if DEBUG
            try
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                channel.PushNotificationReceived += OnPushNotification;

                // check if this uri changed since last time, update to server if yes

                if (!string.Equals(CNBlogSettings.Instance.NotificationUri, channel.Uri))
                {
                    CNBlogSettings.Instance.NotificationUri = channel.Uri;

                    var server = "http://172.22.117.114/wns/api/uri";

                    // get fav authors
                    var authors = await FollowHelper.GetFollowedAuthors();

                    var parameters = new List<KeyValuePair<string, string>>();

                    parameters.Add(new KeyValuePair<string, string>("uuid", Functions.GetUniqueDeviceId()));
                    parameters.Add(new KeyValuePair<string, string>("uri", Uri.EscapeUriString(channel.Uri)));

                    foreach (var author in authors)
                    {
                        var blogApp = !string.IsNullOrWhiteSpace(author.BlogApp) ? author.BlogApp : Functions.ParseBlogAppFromURL(author.Uri);

                        parameters.Add(new KeyValuePair<string, string>("favAuthors", blogApp));
                    }

                    var categories = await FollowHelper.GetFollowedCategories();

                    foreach (var category in categories)
                    {
                        parameters.Add(new KeyValuePair<string, string>("favCategories", category.Href));
                    }

                    var content = new HttpFormUrlEncodedContent(parameters);
                    var request = new HttpRequestMessage(HttpMethod.Post, new Uri(server));
                    request.Content = content;

                    var client = new HttpClient();

                    var response = await client.SendRequestAsync(request);
                }
            }
            catch (Exception ex)
            {
            }

#endif

#endif
        }
        PushNotificationChannel channel = null;
        string content = null;
        private void OnPushNotification(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            String notificationContent = String.Empty;

            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;

                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    break;

                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    break;

                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    break;
            }

            e.Cancel = true;
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}