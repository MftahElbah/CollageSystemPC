using SQLite;
using System.Collections.ObjectModel;
using CollageSystemPC.Methods;
using Syncfusion.Maui.ProgressBar;
using Windows.System;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;
using System;

namespace CollageSystemPC.Pages;

public partial class StdManagement : ContentPage
{
    private ObservableCollection<StdViewModel> StdTableGetter;
    public ObservableCollection<StdViewModel> StdTableSetter
    {
        get => StdTableGetter;
        set
        {
            StdTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }
    public readonly SQLiteAsyncConnection _database;
    public DatabaseHelper dbh;
    public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    
    public StdManagement()
	{
        InitializeComponent();
        _database = new SQLiteAsyncConnection(dbPath);
        dbh = new DatabaseHelper(dbPath);
        StdTableGetter = new ObservableCollection<StdViewModel>();
        BindingContext = this;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStd();
    }
    private async Task LoadStd()
    {
        var std = await dbh.GetStdTableViewAsync();
        if (std == null)
        {
            await DisplayAlert("no", "no data", "niga");
            return;
        }
        StdTableSetter = new ObservableCollection<StdViewModel>(std);
    }

    private void StdTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {

        if (StdTableDataGrid.SelectedRow == null)
        {
            return;
        }
        STDR.IsChecked = true;
        StdPopupWindow.IsVisible = true;
        var DataRow = StdTableDataGrid.SelectedRow;
        IdEntry.Text = DataRow?.GetType().GetProperty("StdId")?.GetValue(DataRow)?.ToString();
        IdEntry.IsEnabled = false;
        NameEntry.Text = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        UsernameEntry.Text = DataRow?.GetType().GetProperty("StdUsername")?.GetValue(DataRow)?.ToString();
        string activeSwitch= DataRow?.GetType().GetProperty("IsActive")?.GetValue(DataRow)?.ToString().ToLower();

        if (activeSwitch == "true")
            ActiveSwitch.IsOn=true;
        else 
            ActiveSwitch.IsOn=false;

        UpdateBtn.IsVisible = true;
        SaveBtn.IsVisible = false;
        ActiveSwitch.IsVisible=true;
        StdPopupWindow.IsVisible = true;
        StdTableDataGrid.SelectedRow = null;
    }

    private void UNEChanged(object sender, TextChangedEventArgs e)
    {
        // Use regex to allow only English letters
        string filteredText = new string(e.NewTextValue.Where(char.IsLetter).ToArray());

        // If the text contains invalid characters, revert to the filtered text
        if (e.NewTextValue != filteredText)
        {
            UsernameEntry.Text = filteredText;
        }
    }
    private async void UpdateBtnClicked(object sender, EventArgs e)
    {
        int id = int.Parse(IdEntry.Text);
        var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == id);
        if (user != null)
        {
            if (String.IsNullOrEmpty(NameEntry.Text) || String.IsNullOrEmpty(UsernameEntry.Text))
            {
                await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
                return;
            }

            int UTInt;
            if (TR.IsChecked)
            {
                UTInt = 1;
            }
            else
            {
                UTInt = 2;
            }
            user.Name = NameEntry.Text;
            user.Username = UsernameEntry.Text;
            user.UserType = UTInt;
            user.IsActive = ActiveSwitch.IsOn.Value;
            if (!string.IsNullOrEmpty(PasswordEntry.Text))
            {
                if (PasswordEntry.Text.Length < 8)
                {
                    await DisplayAlert("خطاء", "يجب ان يكون كلمة السر يتكون من 8 حروف على الأقل", "حسنا");
                    return;
                }
                if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
                {
                    await DisplayAlert("خطاء", "كلمة السر غير متشابهة", "حسنا");
                    return;
                }
                user.Password = PasswordEntry.Text;
            }
            await _database.UpdateAsync(user);
            await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");
            ClearEntrys();
            await LoadStd();
            StdPopupWindow.IsVisible = false;
        }
    }
    private async void SaveBtnClicked(object sender, EventArgs e)
    {
        
        if (String.IsNullOrEmpty(IdEntry.Text) || String.IsNullOrEmpty(NameEntry.Text) || String.IsNullOrEmpty(UsernameEntry.Text) || String.IsNullOrEmpty(PasswordEntry.Text) || String.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }
        int id = int.Parse(IdEntry.Text);
        var userid = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == id);
        if (userid != null)
        {
            await DisplayAlert("خطاء", "رقم الدراسي المكتوب موجود بالفعل", "حسنا");
            return;
        }

        string us = UsernameEntry.Text.ToLower();
        var username = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == us);
        if (username != null)
        {
            await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
            return;
        }
        if (PasswordEntry.Text.Length < 8)
        {
            await DisplayAlert("خطاء", "يجب ان يكون كلمة السر يتكون من 8 حروف على الأقل", "حسنا");
            return;
        }
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("خطاء", "كلمة السر غير متشابهة", "حسنا");
            return;
        }

        int UTInt;
        if (TR.IsChecked)
        {
            UTInt = 1;
        }
        else
        {
            UTInt = 2;
        }

        var newuser = new UsersAccountTable
        {
            UserId = int.Parse(IdEntry.Text),
            Name = NameEntry.Text,
            Username = UsernameEntry.Text.ToLower(),
            Password = PasswordEntry.Text,
            UserType = UTInt,
            IsActive = true,  
        };
        await _database.InsertAsync(newuser);
        await Task.Delay(1000);
        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");
        StdPopupWindow.IsVisible = false;
        await LoadStd();
        ClearEntrys();
    }


    private void AddClicked(object sender, EventArgs e){
        TR.IsChecked = true;
        STDR.IsChecked = true;
        IdEntry.IsEnabled = true;
        SaveBtn.IsVisible = true;
        UpdateBtn.IsVisible = false;
        ActiveSwitch.IsVisible = false;
        StdPopupWindow.IsVisible = true;
    }
    private void BackBtnClicked(object sender, EventArgs e){        
            StdPopupWindow.IsVisible=false;
            ClearEntrys();           
    }

    private void ClearEntrys(){
        STDR.IsChecked = true;
        IdEntry.Text = "";
        NameEntry.Text = "";
        UsernameEntry.Text = "";
        PasswordEntry.Text = "";
        ConfirmPasswordEntry.Text = "";
    }
   
}

//when i click on exist data if id entry not enable u can update without updateing password , that means he is editing