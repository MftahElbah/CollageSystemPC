using CollageSystemPC.Pages;
using CollageSystemPC.Methods;
using SQLite;
using CollageSystemPC.Methods.actions;
using System.Threading.Tasks;
using System;

namespace CollageSystemPC
{
    public partial class App : Application
    {
        DataBase database = DataBase.selectedDatabase;
        private MineSQLite _sqlite = new MineSQLite();
        public static async Task<bool> IsInternetAvailable()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://clients3.google.com/generate_204");
                    Console.WriteLine($"Internet check response: {response.StatusCode}");
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internet check failed: {ex.Message}");
                return false;
            }
        }

        public App()
        {
            InitializeComponent();

            //  DataBase.selectedDatabase = new MineSQLite();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYzMTM0MEAzMjM4MmUzMDJlMzBIUEF2a3E1ZzlTN3I3VXJDOHRKNDd3NlIyd0crTTd0TTBibml6Unl6SFl3PQ==");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new ContentPage());
        }

        protected override async void OnStart()
        {
            UserSession.internet = await IsInternetAvailable();
            if (UserSession.internet)
            {
                DataBase.selectedDatabase = new Firebase();
                UserSession.internet = true;
                Console.WriteLine("Using Firebase Database.");
            }
            else
            {
                DataBase.selectedDatabase = new MineSQLite();

                Console.WriteLine("Using SQLite Database.");
            }
            database = DataBase.selectedDatabase;

            await database.InitializeDatabaseAsync();
            await InitializeApp();
        }

        private async Task InitializeApp()
        {
            try
            {
                var session = await _sqlite.UserSessionChecker();
                
                if (session == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.Windows[0].Page = new NavigationPage(new Login());
                        });
                    }
                    return;
                }
                var trylogin = await database.UserLoginChecker(null,session.Password,session.UserId.ToString());
                if (trylogin == null)
                {
                    if (Application.Current?.Windows.Count > 0)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.Windows[0].Page = new NavigationPage(new Login());
                        });
                    }
                    return;
                }

                UserSession.UserId = trylogin.AdminId;
                UserSession.name = trylogin.Name;
                UserSession.AdminType = trylogin.AdminType;
                UserSession.Password = trylogin.Password;
                UserSession.SessionYesNo = true;

                if (Application.Current?.Windows.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.Windows[0].Page = new NavigationPage(new ManagementPage());
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during app initialization: {ex.Message}");
                if (Application.Current?.Windows.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.Windows[0].Page = new ContentPage
                        {
                            Content = new Label { Text = $"Error: {ex.Message}" }
                        };
                    });
                }
            }
        }
    }
}
