﻿using CollageSystemPC.Methods;
using SQLite;

namespace CollageSystemPC.Pages;

public partial class Login : ContentPage
{
    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    public readonly SQLiteAsyncConnection _database;
    public Login()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page
        _database = new SQLiteAsyncConnection(dbPath);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DeleteSession();
    }
    //delete session if user logout from the account


    private async Task DeleteSession()
    {
        var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
        if (session != null) { await _database.DeleteAsync(session); }
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
        var IfUserExist = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
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