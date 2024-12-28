using CollageSystemPC.Methods;
using SQLite;
using System.Collections.ObjectModel;
using TP.Methods;
namespace CollageSystemPC.Pages;

public partial class ManagementPage : ContentPage
{
    private ObservableCollection<UsersAccountTable> StdTableGetter;
    public ObservableCollection<UsersAccountTable> StdTableSetter
    {
        get => StdTableGetter;
        set
        {
            StdTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }
    }

    public readonly SQLiteAsyncConnection _database;
    public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
    
    public ManagementPage()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        _database = new SQLiteAsyncConnection(dbPath);
        StdTableGetter = new ObservableCollection<UsersAccountTable>();
        AdminPageBtn.IsVisible = UserSession.AdminType;

        HideContentViewMethod.HideContentView(SaveSession);
        HideContentViewMethod.HideContentView(AcountPopupWindow);

    }
    protected override async void OnAppearing(){
        base.OnAppearing();
        await LoadStd();
        await Task.Delay(1000);
        CheckSession();

    }
    private void CheckSession()
    {
        //Check if it's already saved session
        if (UserSession.SessionYesNo)
        {
            return;
        }
        //if he clicked Yes or No the message well not popup again
        SaveSession.IsVisible = true;
        UserSession.SessionYesNo = true;
    }
    private async void SaveSessionClicked(object sender, EventArgs e)
    {
        try
        {
            var session = new UserSessionTable
            {
                UserId = UserSession.UserId,
                Password = UserSession.Password,
            };
            await _database.InsertAsync(session);
            SaveSession.IsVisible = false;
        }
        catch (Exception ex)
        {
            // Log or display the error
            await DisplayAlert("error",$"Error saving session: {ex.Message}","yes");
        }
    }

    private void CancelSessionClicked(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false;
    }
    private async Task LoadStd()
    {
        var std = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 2).ToListAsync();

        // Clear existing data and repopulate StdTableSetter
        StdTableSetter.Clear();
        foreach (var student in std)
        {
            StdTableSetter.Add(student);
        }

        // Set initial ItemsSource to StdTableSetter
        StdTableDataGrid.ItemsSource = StdTableSetter;
    }
    private void AddClicked(object sender, EventArgs e){
        IdEntry.IsEnabled = true;
        SaveBtn.IsVisible = true;
        DelAccBtn.IsVisible = false;
        UpdateBtn.IsVisible = false;
        ActiveSwitch.IsVisible = false;
        albl.IsVisible = false;
        AcountPopupWindow.IsVisible = true;
        TitleLbl.Text = "إضافة حساب";
    }
    private async void AdminPageBtnClicked(object sender, EventArgs e) {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new AdminPage());
        }
    }
    private void LogoutClicked(object sender, EventArgs e) {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new Login());
        }
    }

    private async void SearchBarEntryChanged(object sender, TextChangedEventArgs e){

        if (string.IsNullOrEmpty(SearchBarEntry.Text)){
            await LoadStd();
            return;
        }

            var filteredStudents = await _database.Table<UsersAccountTable>()
            .Where(s => s.Name.Contains(SearchBarEntry.Text.ToLower()))
            .ToListAsync();

            StdTableSetter.Clear();
            // Clear and populate StdTableSetter with filtered results
            foreach (var student in filteredStudents)
            {
                StdTableSetter.Add(student);
            }

            // Switch DataGrid ItemsSource to StdTableSetter
            StdTableDataGrid.ItemsSource = StdTableSetter;
        
    }

    private async void StdTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        try
        {
            // Ensure there is a selected row
            var dataRow = StdTableDataGrid.SelectedRow;
            // Safely retrieve values from the data row
            IdEntry.Text = dataRow.GetType().GetProperty("UserId")?.GetValue(dataRow)?.ToString() ?? string.Empty;
            IdEntry.IsEnabled = false;

            NameEntry.Text = dataRow.GetType().GetProperty("Name")?.GetValue(dataRow)?.ToString() ?? string.Empty;

            UsernameEntry.Text = dataRow.GetType().GetProperty("Username")?.GetValue(dataRow)?.ToString() ?? string.Empty;

            string activeSwitch = dataRow.GetType().GetProperty("IsActive")?.GetValue(dataRow)?.ToString()?.ToLower() ?? "false";
            ActiveSwitch.IsToggled = activeSwitch == "true";

            // Set visibility and titles for the popup
            AcountPopupWindow.IsVisible = true;
            DelAccBtn.IsVisible = true;
            UpdateBtn.IsVisible = true;
            SaveBtn.IsVisible = false;
            ActiveSwitch.IsVisible = true;
            albl.IsVisible = true;
            TitleLbl.Text = "تعديل حساب";

            // Clear the selection after opening the popup
            StdTableDataGrid.SelectedRow = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
    
    private void BackBtnClicked(object sender, EventArgs e){        
            AcountPopupWindow.IsVisible=false;
            ClearEntrys();           
    }
    private async void SaveBtnClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(IdEntry.Text) || string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text) || string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }

        int id = int.Parse(IdEntry.Text);

        // Check if the UserId already exists
        var existingUser = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == id);
        if (existingUser != null)
        {
            await DisplayAlert("خطاء", "رقم الدراسي المكتوب موجود بالفعل", "حسنا");
            return;
        }

        // Check if the Username already exists
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

        // Create a new user and insert it into the database
        var newUser = new UsersAccountTable
        {
            UserId = id,
            Name = NameEntry.Text,
            Username = us,
            Password = PasswordEntry.Text,
            UserType = 2,
            IsActive = true,
        };
        await _database.InsertAsync(newUser);

        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadStd();
        AcountPopupWindow.IsVisible = false;
    }
    private async void UpdateBtnClicked(object sender, EventArgs e)
    {
        int id = int.Parse(IdEntry.Text);
        var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == id);

        if (user != null)
        {
            // Check if Name or Username fields are empty
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text))
            {
                await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
                return;
            }

            // Update UserType based on radio button selection
            user.Name = NameEntry.Text;
            user.Username = UsernameEntry.Text;
            user.IsActive = ActiveSwitch.IsToggled;

            // Handle password updates if provided
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

            // Update the database with the new user information
            await _database.UpdateAsync(user);
            await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");

            // Clear input fields and reload data
            ClearEntrys();
            await LoadStd();
            AcountPopupWindow.IsVisible = false;
        }
        else
        {
            await DisplayAlert("خطاء", "لم يتم العثور على المستخدم", "حسنا");
        }
    }
    private async void DeActiveStdClicked(object sender, EventArgs e) {
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من تعطيل حسابات جميع الطلبة؟", "نعم", "لا");
        if (!conf)
        { return; }
        var stdda = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 2).ToListAsync();
        foreach(var std in stdda)
        {
            std.IsActive = false;
        }
            await _database.UpdateAllAsync(stdda);
        await DisplayAlert("تعطيل", "تم العملية بنجاح", "حسنا");
        await LoadStd();

    }
    private async void DelAccBtnClicked(object sender, EventArgs e){
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذا الحساب؟", "نعم", "لا");
        if (!conf)
        { return; }
        
        int uid = int.Parse(IdEntry.Text);
        var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == uid);
        await _database.DeleteAsync(user);
        
        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        AcountPopupWindow.IsVisible = false;
        await LoadStd();
    }
    

    private void ClearEntrys(){
        IdEntry.Text = "";
        NameEntry.Text = "";
        UsernameEntry.Text = "";
        PasswordEntry.Text = "";
        ConfirmPasswordEntry.Text = "";
    }

}
                        