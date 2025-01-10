using CollageSystemPC.Methods;
using CollageSystemPC.Methods.actions;

namespace CollageSystemPC.Pages;

public partial class Login : ContentPage
{
    private DataBase database = DataBase.selectedDatabase;
    private MineSQLite _sqlite = new MineSQLite();

    public Login(){
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _sqlite.deleteSession();

    }

    private async void LoginBtnClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("خطا", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        string username = UsernameEntry.Text.ToLower();
        string password = PasswordEntry.Text;
        var IfUserExist = await database.UserLoginChecker(username, password,null);
        if (IfUserExist == null)
        {
            await DisplayAlert("خطاء", "هناك خطاء في اسم المستخدم أو كلمة المرور", "حسنا");
            return;
        }
        UserSession.UserId = IfUserExist.AdminId;
        UserSession.Password = IfUserExist.Password;
        UserSession.AdminType = IfUserExist.AdminType;
        UserSession.SessionYesNo = false;
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new ManagementPage());
        }
    }
}