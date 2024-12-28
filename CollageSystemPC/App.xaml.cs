using CollageSystemPC.Pages;
using CollageSystemPC.Methods;
using SQLite;
using CollageSystemPC.Methods.actions;

namespace CollageSystemPC
{
    public partial class App : Application
    {
        public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        public readonly SQLiteAsyncConnection _database;
        private MineSQLite _sqlite = new MineSQLite();


        public App()
        {
            InitializeComponent();
            _database = new SQLiteAsyncConnection(dbPath);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYzMTM0MEAzMjM4MmUzMDJlMzBIUEF2a3E1ZzlTN3I3VXJDOHRKNDd3NlIyd0crTTd0TTBibml6Unl6SFl3PQ==");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new ContentPage());
        }

        protected override async void OnStart()
        {
            var dbHelper = new DatabaseHelper(dbPath);
            await dbHelper.InitializeDatabaseAsync();
            await InitializeApp();
        }

        private async Task InitializeApp()
        {
            try
            {

                //var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
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


                UserSession.UserId = session.AdminId;
                UserSession.AdminType = session.AdminType;
                UserSession.Password = session.Password;
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
