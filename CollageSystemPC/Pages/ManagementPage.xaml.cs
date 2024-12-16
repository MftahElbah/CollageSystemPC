using SQLite;
using System.Collections.ObjectModel;
using CollageSystemPC.Methods;
using Syncfusion.Maui.ListView;
using Microsoft.UI.Xaml.Controls;
using System;


namespace CollageSystemPC.Pages;

public partial class ManagementPage : ContentPage
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
    private ObservableCollection<TeacherViewModel> TeacherTableGetter;
    public ObservableCollection<TeacherViewModel> TeacherTableSetter
    {
        get => TeacherTableGetter;
        set
        {
            TeacherTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }

    }
    private ObservableCollection<SubViewModel> SubGetter;
    public ObservableCollection<SubViewModel> SubSetter
    {
        get => SubGetter;
        set
        {
            SubGetter = value;
            OnPropertyChanged();
        }
    }
    public readonly SQLiteAsyncConnection _database;
    public DatabaseHelper dbh;
    public int sid;
    public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    
    public ManagementPage()
	{
        InitializeComponent();
        _database = new SQLiteAsyncConnection(dbPath);
        dbh = new DatabaseHelper(dbPath);
        StdTableGetter = new ObservableCollection<StdViewModel>();
        TeacherTableGetter = new ObservableCollection<TeacherViewModel>();
        SubGetter = new ObservableCollection<SubViewModel>();
        BindingContext = this;
        PageShowStatus(1);
        

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStd();
        await LoadTeacher();
        await LoadSub();
        STDR.IsChecked = true;
    }
    private async Task LoadStd()
    {
        var std = await dbh.GetStdTableViewAsync();
        StdTableSetter = new ObservableCollection<StdViewModel>(std);
    }
    private async Task LoadSub()
    {
        var sub = await dbh.GetSubViewModelsAsync();
        SubSetter = new ObservableCollection<SubViewModel>(sub);
        
    }
    private async Task LoadTeacher()
    {
        var teacher = await dbh.GetTeacherTableViewAsync();
        TeacherTableSetter = new ObservableCollection<TeacherViewModel>(teacher);
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
        TitleLbl.Text = "تعديل حساب";
        TeacherTableDataGrid.SelectedRow = null;
    }
    private void SubTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (SubTableDataGrid.SelectedRow == null)
        {
            return;
        }
        var DataRow = SubTableDataGrid.SelectedRow;
        
        sid = int.Parse(DataRow?.GetType().GetProperty("SubId")?.GetValue(DataRow)?.ToString());
        SubNameLbl.Text = $"اسم المادة: {DataRow?.GetType().GetProperty("SubName")?.GetValue(DataRow)?.ToString()}";
        TeacherNameLbl.Text = $"اسم الأستاذ: {DataRow?.GetType().GetProperty("SubTeacher")?.GetValue(DataRow)?.ToString()}";
        SubPopupWindow.IsVisible = true;
    }
    private async void DeleteSubClick(object sender, EventArgs e){
        var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == sid);
        await _database.DeleteAsync(Sub);
        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        SubPopupWindow.IsVisible = false;
        LoadSub();
    }
    private async void CancelSubClick(object sender, EventArgs e){
        SubPopupWindow.IsVisible = false;
    }

    private void TeacherTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {

        if (TeacherTableDataGrid.SelectedRow == null)
        {
            return;
        }
        TR.IsChecked = true;
        StdPopupWindow.IsVisible = true;
        var DataRow = TeacherTableDataGrid.SelectedRow;
        IdEntry.Text = DataRow?.GetType().GetProperty("TeacherId")?.GetValue(DataRow)?.ToString();
        IdEntry.IsEnabled = false;
        NameEntry.Text = DataRow?.GetType().GetProperty("TeacherName")?.GetValue(DataRow)?.ToString();
        UsernameEntry.Text = DataRow?.GetType().GetProperty("TeacherUsername")?.GetValue(DataRow)?.ToString();
        string activeSwitch= DataRow?.GetType().GetProperty("IsActive")?.GetValue(DataRow)?.ToString().ToLower();

        if (activeSwitch == "true")
            ActiveSwitch.IsOn=true;
        else 
            ActiveSwitch.IsOn=false;

        UpdateBtn.IsVisible = true;
        SaveBtn.IsVisible = false;
        ActiveSwitch.IsVisible=true;
        TitleLbl.Text = "تعديل حساب";
        StdPopupWindow.IsVisible = true;
        StdTableDataGrid.SelectedRow = null;
    }

    /*private void UNEChanged(object sender, TextChangedEventArgs e)
    {
        // Use regex to allow only English letters
        string filteredText = new string(e.NewTextValue.Where(char.IsLetter).ToArray());

        // If the text contains invalid characters, revert to the filtered text
        if (e.NewTextValue != filteredText)
        {
            UsernameEntry.Text = filteredText;
        }
    }*/
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
            await LoadTeacher();
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
        await LoadTeacher();
        ClearEntrys();
    }


    private void AddClicked(object sender, EventArgs e){
        STDR.IsChecked = true;
        IdEntry.IsEnabled = true;
        SaveBtn.IsVisible = true;
        UpdateBtn.IsVisible = false;
        ActiveSwitch.IsVisible = false;
        StdPopupWindow.IsVisible = true;
        TitleLbl.Text = "إضافة حساب";
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

    private void STDTableClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void TeachersTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    private void SubTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(3);
    }

    public void PageShowStatus(int Status)
    {
        //to show posts
        if (Status == 1)
        {
            STDTableShower.TextColor = Color.FromArgb("#DCDCDC");
            STDTableShower.Background = Color.FromArgb("#2374AB");
            StdTableDataGrid.IsVisible = true;

            TeachersTableShower.TextColor = Color.FromArgb("#1A1A1A");
            TeachersTableShower.Background = Colors.Transparent;
            TeacherTableDataGrid.IsVisible = false;

            SubTableShower.TextColor = Color.FromArgb("#1A1A1A");
            SubTableShower.Background = Colors.Transparent;
            SubTableDataGrid.IsVisible = false;

        }
        //to show Degrees Table
        if (Status == 2)
        {
            STDTableShower.TextColor = Color.FromArgb("#1A1A1A");
            STDTableShower.Background = Colors.Transparent;
            StdTableDataGrid.IsVisible = false;

            TeachersTableShower.TextColor = Color.FromArgb("#DCDCDC");
            TeachersTableShower.Background = Color.FromArgb("#2374AB");
            TeacherTableDataGrid.IsVisible = true;

            SubTableShower.TextColor = Color.FromArgb("#1A1A1A");
            SubTableShower.Background = Colors.Transparent;
            SubTableDataGrid.IsVisible = false;


        }
        //to show To show books
        if (Status == 3)
        {
            STDTableShower.TextColor = Color.FromArgb("#1A1A1A");
            STDTableShower.Background = Colors.Transparent;
            StdTableDataGrid.IsVisible = false;

            TeachersTableShower.TextColor = Color.FromArgb("#1A1A1A");
            TeachersTableShower.Background = Colors.Transparent;
            TeacherTableDataGrid.IsVisible = false;

            SubTableShower.TextColor = Color.FromArgb("#DCDCDC");
            SubTableShower.Background = Color.FromArgb("#2374AB");
            SubTableDataGrid.IsVisible = true;


        }
    }
}

//when i click on exist data if id entry not enable u can update without updateing password , that means he is editing