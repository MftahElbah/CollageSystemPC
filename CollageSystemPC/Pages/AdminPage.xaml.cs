﻿
using CollageSystemPC.Methods.actions;
using System.Collections.ObjectModel;
using TP.Methods;

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

    public int AdminId;
    public string sid;
    public string usernamechecker;
    public int SearchTypeNum;
    public string SearchWord;
    private MineSQLite _sqlite = new MineSQLite();

    public AdminPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

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
        var TeacherData = await _sqlite.GetUserData(1);

        // Clear existing data and repopulate StdTableSetter
        TeacherTableSetter.Clear();
        TeacherTableSetter = new ObservableCollection<UsersAccountTable>(TeacherData);
        

        // Set initial ItemsSource to StdTableSetter
        TeacherTableDataGrid.ItemsSource = TeacherTableSetter;

    }
    private async Task LoadSub()
    {

        var SubData = await _sqlite.GetSubData();

        // Clear existing data and repopulate StdTableSetter
        
        SubSetter.Clear();
        SubSetter = new ObservableCollection<SubTable>(SubData);

        // Set initial ItemsSource to StdTableSetter
        SubTableDataGrid.ItemsSource = SubSetter;

    }
    private async Task LoadAdmin()
    {
        var AdminData = await _sqlite.GetAdminData();

        // Clear existing data and repopulate StdTableSetter
        AdminSetter.Clear();
        AdminSetter = new ObservableCollection<AdminAccountTable>(AdminData);

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
    private void BackClicked(object sender, EventArgs e){
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
                var SearchedUserData = await _sqlite.GetUserDataByName(SearchBarEntry.Text.ToLower(), 1);
                TeacherTableSetter = new ObservableCollection<UsersAccountTable>(SearchedUserData);
                TeacherTableDataGrid.ItemsSource = TeacherTableSetter;
                break;
            case 2:
                var SearchedSubData = await _sqlite.GetSubDataByName(SearchBarEntry.Text.ToLower());
                SubSetter = new ObservableCollection<SubTable>(SearchedSubData);
                SubTableDataGrid.ItemsSource = SubSetter;
                break;
            case 3:
                var SearchedAdminData = await _sqlite.GetAdminDataByName(SearchBarEntry.Text.ToLower());
                AdminSetter = new ObservableCollection<AdminAccountTable>(SearchedAdminData);
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
        var existingId = await _sqlite.CheckIfIdExist(id);
        if (existingId != null)
        {
            await DisplayAlert("خطاء", "رقم الدراسي المكتوب موجود بالفعل", "حسنا");
            return;
        }

        // Check if the Username already exists
        string us = UsernameEntry.Text.ToLower();
        var existingUsername = await _sqlite.CheckIfUsernameExist(us);
        if (existingUsername != null)
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
        await _sqlite.InsertUser(id, us, NameEntry.Text, PasswordEntry.Text, 1);

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
        var existingUsername = await _sqlite.CheckIfAdminUsernameExist(us);
        if (existingUsername != null)
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

        await _sqlite.InsertAdmin(us, AdminNameEntry.Text, AdminPasswordEntry.Text, AdminType);
        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadTeacher();
        AdminPopupWindow.IsVisible = false;

    }

    private async void UpdateBtnClicked(object sender, EventArgs e) {
        // Check if Name or Username fields are empty
        if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
            return;
        }
        if (usernamechecker != UsernameEntry.Text)
        {
            var existingUsername = await _sqlite.CheckIfUsernameExist(UsernameEntry.Text.ToLower());
            if (existingUsername != null)
            {
                await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
                return;
            }
        }
        if (!string.IsNullOrEmpty(PasswordEntry.Text)){
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

        }
        await _sqlite.UpdateUser(int.Parse(IdEntry.Text), UsernameEntry.Text, NameEntry.Text, PasswordEntry.Text, 1, ActiveSwitch.IsToggled);
        await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadTeacher();
        AcountPopupWindow.IsVisible = false;
    }
    private async void AdminUpdateBtnClicked(object sender, EventArgs e)
    {
        // Check if Name or Username fields are empty
        if (string.IsNullOrEmpty(AdminNameEntry.Text) || string.IsNullOrEmpty(AdminUsernameEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
            return;
        }

        // Check if Username is being changed and already exists
        if (usernamechecker != AdminUsernameEntry.Text)
        {
            string us = AdminUsernameEntry.Text.ToLower();
            var existingUsername = await _sqlite.CheckIfAdminUsernameExist(us);
            if (existingUsername != null)
            {
                await DisplayAlert("خطاء", "اسم المستخدم المكتوب موجود بالفعل", "حسنا");
                return;
            }
        }

        // Validate password if provided
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
        }

        // Determine AdminType based on radio button selection
        bool AdminType = AdminRadio.IsChecked ? true : false;

        // Update Admin using the provided method
        await _sqlite.UpdateAdmin(AdminId, AdminUsernameEntry.Text, AdminNameEntry.Text, AdminPasswordEntry.Text, AdminType);

            await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");

            // Clear input fields and reload data
            ClearEntrys();
            await LoadAdmin();
            AdminPopupWindow.IsVisible = false;
        
    }


    private async void DelAccBtnClicked(object sender, EventArgs e){
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذا الحساب؟", "نعم", "لا");
        if (!conf)
        { return; }

        await _sqlite.DeleteUser(int.Parse(IdEntry.Text));

        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        AcountPopupWindow.IsVisible = false;
        ClearEntrys();
        await LoadTeacher();
    }
    private async void DelAdminBtnClicked(object sender, EventArgs e) {
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذا المسؤول؟", "نعم", "لا");
        if (!conf)
        { return; }

        await _sqlite.DeleteAdmin(AdminId);

        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        AdminPopupWindow.IsVisible = false;
        ClearEntrys();
        await LoadAdmin();
    }
    private async void DeleteSubClick(object sender, EventArgs e){
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذه المادة؟", "نعم", "لا");
        if (!conf)
        { return; }

        await _sqlite.DeleteSub(int.Parse(sid));
        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        SubPopupWindow.IsVisible = false;
        await LoadSub();
    }


    private void CloseBtnClicked(object sender, EventArgs e) { AcountPopupWindow.IsVisible = false; }
    private void CancelSubClick(object sender, EventArgs e) { SubPopupWindow.IsVisible = false; }
    private void AdminCloseBtnClicked(object sender, EventArgs e) { AdminPopupWindow.IsVisible = false; }


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