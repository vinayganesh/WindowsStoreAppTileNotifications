using System;
using System.Diagnostics;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.TileContent;
using Windows.Networking.Connectivity;
using Windows.Storage;

namespace TileNotifications
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private static void ClearTile()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        private static void EnableNotificationQueue()
        {
            // Enable the notification queue.
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }

        private static async void PostNotificationOnTile()
        {
            try
            {
                //Enable flip tiles
                EnableNotificationQueue();

                //clear the notifications
                ClearTile();

                //Update1
                CreateNotification(string.Format("{0}{1:MM/dd/yy hh:mm:ss}  ", "Last Backup time:", DateTime.Now));

                //Update2
                if (!IsWifiAvailable())
                    CreateNotification("Waiting For Wifi. Tap to continue");

                //Update 3
                var files = await KnownFolders.MusicLibrary.GetFilesAsync();
                if(files.Count>0)
                    CreateNotification("Content ready for backup");
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static bool IsWifiAvailable()
        {
            var internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            
            if (internetConnectionProfile == null)
            {
                return false;
            }

            return internetConnectionProfile.IsWlanConnectionProfile;
        }

        private static void CreateNotification(string notificationString)
        {
            // Create a notification for the Square310x310 tile using one of the available templates for the size.
            ITileSquare310x310Text09 tileContent = TileContentFactory.CreateTileSquare310x310Text09();
            tileContent.TextHeadingWrap.Text = notificationString;

            // Create a notification for the Wide310x150 tile using one of the available templates for the size.
            ITileWide310x150Text03 wide310x150Content = TileContentFactory.CreateTileWide310x150Text03();
            wide310x150Content.TextHeadingWrap.Text = notificationString;

            // Create a notification for the Square150x150 tile using one of the available templates for the size.
            ITileSquare150x150Text04 square150x150Content = TileContentFactory.CreateTileSquare150x150Text04();
            square150x150Content.TextBodyWrap.Text = notificationString;

            // Attach the Square150x150 template to the Wide310x150 template.
            wide310x150Content.Square150x150Content = square150x150Content;

            // Attach the Wide310x150 template to the Square310x310 template.
            tileContent.Wide310x150Content = wide310x150Content;

            // Send the notification to the application? tile.
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostNotificationOnTile();
        }
    }
}
