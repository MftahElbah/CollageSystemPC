using CollageSystemPC.Methods;
using SQLite;
using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using TP.Methods;
using Windows.Devices.Radios;

namespace CollageSystemPC.Pages;

public partial class AdminPage : ContentPage
{
    private ObservableCollection<UsersAccountTable> TeacherTableGetter;
    public ObservableCollection<UsersAccountTable> TeacherTableSetter
    {
        get => TeacherTableGetter;
        set
        {
            TeacherTableGetter = value;
            OnPropertyChanged(); // Notify that SubTableView property has changed.
        }

    }
    private ObservableCollection<SubTable> SubGetter;
    public ObservableCollection<SubTable> SubSetter
    {
        get => SubGetter;
        set
        {
            SubGetter = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<AdminAccountTable> AdminGetter;
    public ObservableCollection<AdminAccountTable> AdminSetter
    {
        get => AdminGetter;
        set
        {
            AdminGetter = value;
            OnPropertyChanged();
        }
    }

    public readonly SQLiteAsyncConnection _database;
    public int AdminId;
    public string sid;
    public string usernamechecker;
    public int SearchTypeNum;
    public string SearchWord;
    public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

    public AdminPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        _database = new SQLiteAsyncConnection(dbPath);
        TeacherTableGetter = new ObservableCollection<UsersAccountTable>();
        SubGetter = new ObservableCollection<SubTable>();
        AdminGetter = new ObservableCollection<AdminAccountTable>();
        BindingContext = this;
        PageShowStatus(1);

        HideContentViewMethod.HideContentView(AcountPopupWindow);
        HideContentViewMethod.HideContentView(AdminPopupWindow);
        HideContentViewMethod.HideContentView(SubPopupWindow);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTeacher();
        await LoadAdmin();
        await LoadSub();
    }
    private async Task LoadTeacher()
    {
        var teacher = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 1).ToListAsync();

        // Clear existing data and repopulate StdTableSetter
        TeacherTableSetter.Clear();
        foreach (var T in teacher)
        {
            TeacherTableSetter.Add(T);
        }

        // Set initial ItemsSource to StdTableSetter
        TeacherTableDataGrid.ItemsSource = TeacherTableSetter;

    }
    private async Task LoadSub()
    {
        var sub = await _database.Table<SubTable>().ToListAsync();

        // Clear existing data and repopulate StdTableSetter
        SubSetter.Clear();
        foreach (var s in sub)
        {
            SubSetter.Add(s);
        }

        // Set initial ItemsSource to StdTableSetter
        SubTableDataGrid.ItemsSource = SubSetter;

    }
    private async Task LoadAdmin()
    {
        var admins = await _database.Table<AdminAccountTable>().ToListAsync();

        // Clear existing data and repopulate StdTableSetter
        AdminSetter.Clear();
        foreach (var admin in admins)
        {
            AdminSetter.Add(admin);
        }

        // Set initial ItemsSource to StdTableSetter
        AdminTableDataGrid.ItemsSource = AdminSetter;

    }

    private void AddTeacherClicked(object sender, EventArgs e){
        ClearEntrys();
        IdEntry.IsEnabled = true;
        SaveBtn.IsVisible = true;
        DelAccBtn.IsVisible = false;
        UpdateBtn.IsVisible = false;
        ActiveSwitch.IsVisible = false;
        albl.IsVisible = false;
        AcountPopupWindow.IsVisible = true;
        TitleLbl.Text = "إضافة معلم";
    }
    private void AddAdminClicked(object sender, EventArgs e){
        ClearEntrys();
        InserterRadio.IsChecked = true;
        AdminSaveBtn.IsVisible = true;
        DelAdminBtn.IsVisible = false;
        AdminUpdateBtn.IsVisible = false;
        AdminPopupWindow.IsVisible = true;
        ATitleLbl.Text = "إضافة مسؤول";

    }
    private async void BackClicked(object sender, EventArgs e){
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new ManagementPage());
        }
    }

    private void TeachersTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void SubTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    private void AdminTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(3);
    }
    public void PageShowStatus(int Status)
    {

        TeachersTableShower.TextColor = Color.FromArgb("#1A1A1A");
        TeachersTableShower.Background = Colors.Transparent;
        TeacherTableDataGrid.IsVisible = false;

        SubTableShower.TextColor = Color.FromArgb("#1A1A1A");
        SubTableShower.Background = Colors.Transparent;
        SubTableDataGrid.IsVisible = false;

        AdminTableShower.TextColor = Color.FromArgb("#1A1A1A");
        AdminTableShower.Background = Colors.Transparent;
        AdminTableDataGrid.IsVisible = false;

        SearchBarEntry.Placeholder = "";
        SearchTypeNum = Status;
        switch (Status)
        {
            

            case 1:
                TeachersTableShower.TextColor = Color.FromArgb("#efefef");
                TeachersTableShower.Background = Color.FromArgb("#1a1a1a");
                TeacherTableDataGrid.IsVisible = true;
                SearchBarEntry.Placeholder = "بحث بأسم المعلم";
                break;
            case 2:
                SubTableShower.TextColor = Color.FromArgb("#efefef");
                SubTableShower.Background = Color.FromArgb("#1a1a1a");
                SubTableDataGrid.IsVisible = true;
                SearchBarEntry.Placeholder = "بحث بأسم المادة";
                break;
            case 3:
                AdminTableShower.TextColor = Color.FromArgb("#efefef");
                AdminTableShower.Background = Color.FromArgb("#1a1a1a");
                AdminTableDataGrid.IsVisible = true;
                SearchBarEntry.Placeholder = "بحث بأسم المسؤول";
                break;
        }

        //to show To show books

    }

    private async void SearchBarEntryChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrEmpty(SearchBarEntry.Text))
        {
            await LoadTeacher();
            await LoadSub();
            await LoadAdmin();
            /*testlbl.Text = "0";*/
            return;
        }

        switch (SearchTypeNum)
        {

            case 1:
                var filteredTeachers = await _database.Table<UsersAccountTable>()
                .Where(s => s.Name.ToLower().Contains(SearchBarEntry.Text.ToLower()) && s.UserType == 1)
                .ToListAsync();

                TeacherTableSetter.Clear();
                foreach (var teacher in filteredTeachers)
                {
                    TeacherTableSetter.Add(teacher);
                }
                TeacherTableDataGrid.ItemsSource = TeacherTableSetter;
                break;
            case 2:
                var filteredSubjects = await _database.Table<SubTable>()
                .Where(s => s.SubName.ToLower().Contains(SearchBarEntry.Text.ToLower()))
                .ToListAsync();

                SubSetter.Clear();
                foreach (var sub in filteredSubjects)
                {
                    SubSetter.Add(sub);
                }
                SubTableDataGrid.ItemsSource = SubSetter;
                break;
            case 3:
                var filteredAdmin = await _database.Table<AdminAccountTable>()
                .Where(s => s.Name.ToLower().Contains(SearchBarEntry.Text.ToLower()))
                .ToListAsync();

                AdminSetter.Clear();
                foreach (var admin in filteredAdmin)
                {
                    AdminSetter.Add(admin);
                }
                AdminTableDataGrid.ItemsSource = AdminSetter;
                break;
        }
    }

    private async void TeacherTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e){
        try
        {
            ClearEntrys();
            var DataRow = TeacherTableDataGrid.SelectedRow;
            IdEntry.Text = DataRow?.GetType().GetProperty("UserId")?.GetValue(DataRow)?.ToString() ?? string.Empty;
            IdEntry.IsEnabled = false;
            NameEntry.Text = DataRow?.GetType().GetProperty("Name")?.GetValue(DataRow)?.ToString() ?? string.Empty;
            usernamechecker = DataRow?.GetType().GetProperty("Username")?.GetValue(DataRow)?.ToString() ?? string.Empty;
            UsernameEntry.Text = usernamechecker;
            string activeSwitch = DataRow?.GetType().GetProperty("IsActive")?.GetValue(DataRow)?.ToString().ToLower() ?? "false";
            ActiveSwitch.IsToggled = activeSwitch == "true";

            // Set visibility and titles for the popup
            UpdateBtn.IsVisible = true;
            DelAccBtn.IsVisible = true;
            SaveBtn.IsVisible = false;
            albl.IsVisible = true;

            ActiveSwitch.IsVisible = true;
            TitleLbl.Text = "تعديل حساب";
            AcountPopupWindow.IsVisible = true;
            TeacherTableDataGrid.SelectedRow = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
    private void SubTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e){
        var DataRow = SubTableDataGrid.SelectedRow;

        sid = DataRow?.GetType().GetProperty("SubId")?.GetValue(DataRow)?.ToString();
        SubNameLbl.Text = $"اسم المادة: {DataRow?.GetType().GetProperty("SubName")?.GetValue(DataRow)?.ToString()}";
        TeacherIdLbl.Text = $"رقم الاستاذ: {DataRow?.GetType().GetProperty("UserId")?.GetValue(DataRow)?.ToString()}";
        TeacherNameLbl.Text = $"اسم الأستاذ: {DataRow?.GetType().GetProperty("SubTeacherName")?.GetValue(DataRow)?.ToString()}";
        SubPopupWindow.IsVisible = true;
        SubTableDataGrid.SelectedRow = null;
    }
    private async void AdminTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        try
        {
            ClearEntrys();
            // Ensure there is a selected row
            var dataRow = AdminTableDataGrid.SelectedRow;

            // Safely retrieve values from the data row
            AdminId = int.Parse(dataRow.GetType().GetProperty("AdminId")?.GetValue(dataRow)?.ToString() ?? string.Empty);

            AdminNameEntry.Text = dataRow.GetType().GetProperty("Name")?.GetValue(dataRow)?.ToString() ?? string.Empty;
            usernamechecker = dataRow.GetType().GetProperty("Username")?.GetValue(dataRow)?.ToString() ?? string.Empty;
            AdminUsernameEntry.Text = usernamechecker;
            string AdminTypeS = dataRow.GetType().GetProperty("AdminType")?.GetValue(dataRow)?.ToString()?.Trim().ToLower() ?? string.Empty;

            AdminRadio.IsChecked = AdminTypeS == "true";



            // Set visibility and titles for the popup
            AdminPopupWindow.IsVisible = true;
            AdminUpdateBtn.IsVisible = true;
            AdminSaveBtn.IsVisible = false;
            DelAdminBtn.IsVisible = true;
            ATitleLbl.Text = "تعديل حساب المسؤول";

            // Clear the selection after opening the popup
            AdminTableDataGrid.SelectedRow = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void SaveBtnClicked(object sender, EventArgs e){
        
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
            UserType = 1,
            IsActive = true,
        };
        await _database.InsertAsync(newUser);

        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadTeacher();
        AcountPopupWindow.IsVisible = false;
      
    }
    private async void AdminSaveBtnClicked(object sender, EventArgs e){

        if ( string.IsNullOrEmpty(AdminNameEntry.Text) || string.IsNullOrEmpty(AdminUsernameEntry.Text) || string.IsNullOrEmpty(AdminPasswordEntry.Text) || string.IsNullOrEmpty(AdminConfirmPasswordEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب ملئ جميع الحقول", "حسنا");
            return;
        }

        // Check if the Username already exists
        string us = AdminUsernameEntry.Text.ToLower();
        var username = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.Username == us);
        if (username != null)
        {
            await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
            return;
        }

        if (AdminPasswordEntry.Text.Length < 8)
        {
            await DisplayAlert("خطاء", "يجب ان يكون كلمة السر يتكون من 8 حروف على الأقل", "حسنا");
            return;
        }

        if (AdminPasswordEntry.Text != AdminConfirmPasswordEntry.Text)
        {
            await DisplayAlert("خطاء", "كلمة السر غير متشابهة", "حسنا");
            return;
        }

        bool AdminType = AdminRadio.IsChecked ? true : false;


        // Create a new user and insert it into the database
        var newUser = new AdminAccountTable
        {
            Name = AdminNameEntry.Text,
            Username = us,
            Password = AdminPasswordEntry.Text,
            AdminType = AdminType,
            
        };
        await _database.InsertAsync(newUser);

        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadTeacher();
        AdminPopupWindow.IsVisible = false;

    }
    private async void UpdateBtnClicked(object sender, EventArgs e) {
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
            if (usernamechecker != UsernameEntry.Text)
            {
                // Check if the Username already exists
                string us = UsernameEntry.Text.ToLower();
                var username = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == us);
                if (username != null)
                {
                    await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
                    return;
                }
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
            await LoadTeacher();
            AcountPopupWindow.IsVisible = false;
        }
        else
        {
            await DisplayAlert("خطاء", "لم يتم العثور على المستخدم", "حسنا");
        }
    }
    private async void AdminUpdateBtnClicked(object sender, EventArgs e) {
        var admin = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.AdminId == AdminId);

        if (admin != null)
        {
            // Check if Name or Username fields are empty
            if (string.IsNullOrEmpty(AdminNameEntry.Text) || string.IsNullOrEmpty(AdminUsernameEntry.Text))
            {
                await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
                return;
            }

            if (usernamechecker != AdminUsernameEntry.Text)
            {
                // Check if the Username already exists
                string us = AdminUsernameEntry.Text.ToLower();
                var username = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.Username == us);
                if (username != null)
                {
                    await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
                    return;
                }
            }
            bool AdminType = AdminRadio.IsChecked ? true : false;


            // Update UserType based on radio button selection
            admin.Name = AdminNameEntry.Text;
            admin.Username = AdminUsernameEntry.Text;
            admin.AdminType = AdminType;
            // Handle password updates if provided
            if (!string.IsNullOrEmpty(AdminPasswordEntry.Text))
            {
                if (AdminPasswordEntry.Text.Length < 8)
                {
                    await DisplayAlert("خطاء", "يجب ان يكون كلمة السر يتكون من 8 حروف على الأقل", "حسنا");
                    return;
                }

                if (AdminPasswordEntry.Text != AdminConfirmPasswordEntry.Text)
                {
                    await DisplayAlert("خطاء", "كلمة السر غير متشابهة", "حسنا");
                    return;
                }

                admin.Password = AdminPasswordEntry.Text;
            }

            // Update the database with the new user information
            await _database.UpdateAsync(admin);
            await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");

            // Clear input fields and reload data
            ClearEntrys();
            await LoadAdmin();
            AdminPopupWindow.IsVisible = false;
        }
        else
        {
            await DisplayAlert("خطاء", "لم يتم العثور على المستخدم", "حسنا");
        }
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
        ClearEntrys();
        await LoadTeacher();
    }
    private async void DelAdminBtnClicked(object sender, EventArgs e) {
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذا المسؤول؟", "نعم", "لا");
        if (!conf)
        { return; }

        var user = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.AdminId == AdminId);
        await _database.DeleteAsync(user);

        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        AdminPopupWindow.IsVisible = false;
        ClearEntrys();
        await LoadAdmin();
    }
    private async void DeleteSubClick(object sender, EventArgs e){
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذه المادة؟", "نعم", "لا");
        if (!conf)
        { return; }
        int id = int.Parse(sid);
        var Sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == id);
        await _database.DeleteAsync(Sub);
        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        SubPopupWindow.IsVisible = false;
        await LoadSub();
    }
    private async void DeActiveStdClicked(object sender, EventArgs e)
    {
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من تعطيل حسابات جميع الطلبة؟", "نعم", "لا");
        if (!conf)
        { return; }
        var stdda = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 2).ToListAsync();
        foreach (var std in stdda)
        {
            std.IsActive = false;
        }
        await _database.UpdateAllAsync(stdda);
        await DisplayAlert("تعطيل", "تم العملية بنجاح", "حسنا");
        await LoadTeacher();

    }
    private async void CloseBtnClicked(object sender, EventArgs e) { AcountPopupWindow.IsVisible = false; }
    private async void CancelSubClick(object sender, EventArgs e) { SubPopupWindow.IsVisible = false; }
    private async void AdminCloseBtnClicked(object sender, EventArgs e) { AdminPopupWindow.IsVisible = false; }


    private void ClearEntrys()
    {
        IdEntry.Text = "";
        NameEntry.Text = "";
        UsernameEntry.Text = "";
        PasswordEntry.Text = "";
        ConfirmPasswordEntry.Text = "";
        AdminNameEntry.Text = "";
        AdminUsernameEntry.Text = "";
        AdminPasswordEntry.Text = "";
        AdminConfirmPasswordEntry.Text = "";
        InserterRadio.IsChecked = true;
    }
}