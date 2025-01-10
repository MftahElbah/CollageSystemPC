using CollageSystemPC.Methods;
using CollageSystemPC.Methods.actions;
using System.Collections.ObjectModel;
using TP.Methods;
namespace CollageSystemPC.Pages;
using System.Globalization;
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

    private DataBase _database  = DataBase.selectedDatabase;
    private MineSQLite _sqlite = new MineSQLite();
    public ObservableCollection<SubjectPosts> Posts { get; set; }

    public int SubId = -1;
    public string LinkUrl;
    public string PostId;
    public bool showmessage;
    public bool[] Emptys = new bool[2];



    public ManagementPage()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Disable navigation bar for this page

        StdTableGetter = new ObservableCollection<UsersAccountTable>();
        Posts = new ObservableCollection<SubjectPosts>();

        AdminPageBtn.IsVisible = UserSession.AdminType;
        HideContentViewMethod.HideContentView(SaveSession , SaveSessionBorder);
        HideContentViewMethod.HideContentView(AcountPopupWindow, AcountPopupBorder);
        HideContentViewMethod.HideContentView(PasswordPopup, PasswordPopupBorder);
        HideContentViewMethod.HideContentView(PostPopupWindow, PostBorder);

    }
    protected override async void OnAppearing(){
        base.OnAppearing();
        await LoadStd();
        await Task.Delay(1000);
        await LoadPosts();
        PageShowStatus(1);
        CheckSession();
    }
    private async Task<bool> LoadPosts()
    {
        try
        {
            showmessage = false;
            Posts.Clear();

            // Fetch posts by Subject ID
            var data = await _database.getSubjectPostsBySubId(SubId);

            // Check if there are no posts
            if (data == null || data.Count == 0)
            {
                return true;       // Return false if no posts found
            }

            // Sort posts by PostDate in descending order
            var posts = data.OrderByDescending(b => b.PostDate).ToList();

            // Add posts to the observable collection
            foreach (var post in posts)
            {
                Posts.Add(post);
            }

            // Bind posts to the ListView
            Postslistview.ItemsSource = Posts;

            return false;  // Successfully loaded posts
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading posts: {ex.Message}");
            return true;  // Return false if an error occurs
        }
    }

    private async Task<bool> LoadStd()
    {
        try
        {
            // Fetch students with UserType = 2
            var StdData = await _database.GetUserData(2);

            // Clear existing data
            StdTableSetter.Clear();

            // Handle empty data
            if (StdData == null || StdData.Count == 0)
            {
                return true;      // Return false if no data
            }


            // Populate the ObservableCollection with fetched data
            foreach (var std in StdData)
            {
                StdTableSetter.Add(std);
            }

            // Bind data to DataGrid
            StdTableDataGrid.ItemsSource = StdTableSetter;

            return false;  // Successfully loaded data
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading students: {ex.Message}");
            return true;  // Error occurred
        }
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
        
            var session = new UserSessionTable
            {
                UserId = UserSession.UserId,
                Password = UserSession.Password,
            };
            await _sqlite.insertSession(session);
        SaveSession.IsVisible = false;
        
    }
    private void CancelSessionClicked(object sender, EventArgs e)
    {
        SaveSession.IsVisible = false;
    }
    private void AddPostBtnClicked(object sender, EventArgs e) {
        DeletePostBtn.IsVisible = false ;
        ClearEntrys();
        PostPopupWindow.IsVisible = true ;

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
    private async void LogoutClicked(object sender, EventArgs e) {
        await _sqlite.deleteSession();

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

        var SearchedStdData = await _database.GetUserDataByName(SearchBarEntry.Text.ToLower() , 2);
        StdTableSetter.Clear();
        StdTableSetter = new ObservableCollection<UsersAccountTable>(SearchedStdData);
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

    private void UsernameEntryChanged(object sender, TextChangedEventArgs e)
    {
        // Get the text from the Entry control
        string enteredText = e.NewTextValue;

        // Check if the entered text is in English by matching it with the English characters pattern
        if (IsTextInEnglish(enteredText))
        {
            // If the text is in English, do nothing or allow further processing
            Console.WriteLine("The text is in English");
        }
        else
        {
            // If the text is not in English, clear the text
            ((Entry)sender).Text = enteredText.Substring(0, enteredText.Length - 1);
            Console.WriteLine("The text was not in English, so it was cleared.");
        }
    }


    // Helper method to check if the text is in English
    private bool IsTextInEnglish(string text)
    {
        // Use a simple regex or check if the text contains only English letters and whitespace
        return text.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)) && text.All(c => c <= 127); // Check if all characters are in the ASCII range
    }

    private void IdEntryChanged(object sender, TextChangedEventArgs e)
    {
        // Get the entered text
        string enteredText = e.NewTextValue;

        // Check if the entered text is numeric
        if (!string.IsNullOrEmpty(enteredText) && !enteredText.All(char.IsDigit))
        {
            // If it's not numeric, clear the text
            ((Entry)sender).Text = enteredText.Substring(0, enteredText.Length - 1);
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
        var existingId = await _database.CheckIfIdExist(id);
        if (existingId != null)
        {
            await DisplayAlert("خطاء", "رقم الدراسي المكتوب موجود بالفعل", "حسنا");
            return;
        }

        // Check if the Username already exists
        string us = UsernameEntry.Text.ToLower();
        var existingUsername = await _database.CheckIfUsernameExist(us);
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
        await _database.InsertUser(id, us, NameEntry.Text, PasswordEntry.Text,2);

        await DisplayAlert("نجحت", "تم التسجيل بنجاح", "حسنا");

        // Clear input fields and reload data
        ClearEntrys();
        await LoadStd();
        PageShowStatus(1);
        AcountPopupWindow.IsVisible = false;
    }
    private async void UpdateBtnClicked(object sender, EventArgs e)
    {
        // Check if Name or Username fields are empty
        if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(UsernameEntry.Text))
        {
            await DisplayAlert("خطاء", "يجب الا يكون حقل الاسم و اسم المستخدم فارغين", "حسنا");
            return;
        }
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

        }
        await _database.UpdateUser(int.Parse(IdEntry.Text), UsernameEntry.Text.ToLower(), NameEntry.Text, PasswordEntry.Text, 2, ActiveSwitch.IsToggled);
        await DisplayAlert("نجحت", "تم التعديل بنجاح", "حسنا");

            // Clear input fields and reload data
            ClearEntrys();
            await LoadStd();
            AcountPopupWindow.IsVisible = false;

    }
    private void DeActiveStdClicked(object sender, EventArgs e) {
        PasswordPopup.IsVisible = true;

        
    }
    private void CancelDeActiveClicked(object sender, EventArgs e) {
        AgreePasswordEntry.Text = string.Empty;
        PasswordPopup.IsVisible = false;
    }
    private async void AgreeDeActiveClicked(object sender, EventArgs e) {

        if (AgreePasswordEntry.Text != UserSession.Password)
        {
            await DisplayAlert("متأكد؟", "خطا في كلمة السر", "نعم");
            return;
        }
        
        await _database.DeActiveAllSTD();
        await DisplayAlert("تعطيل", "تم العملية بنجاح", "حسنا");
        await LoadStd();
    }

    private async void DelAccBtnClicked(object sender, EventArgs e){
        bool conf = await DisplayAlert("متأكد؟", "هل انت متأكد من حذف هذا الحساب؟", "نعم", "لا");
        if (!conf)
        { return; }

        await _database.DeleteUser(int.Parse(IdEntry.Text) , 2);
        
        await DisplayAlert("حذفت", "تمت الحذف بنجاح", "حسنا");
        AcountPopupWindow.IsVisible = false;
        ClearEntrys();
        await LoadStd();
        PasswordPopup.IsVisible = false;
        PageShowStatus(1);
    }
    

    private void ClearEntrys(){
        IdEntry.Text = string.Empty;
        NameEntry.Text = string.Empty;
        UsernameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ConfirmPasswordEntry.Text = string.Empty;
        TitleEntry.Text = string.Empty;
        DesEditor.Text = string.Empty;
        LinkEntry.Text = string.Empty;
        
    }

    private void StudentTableShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(1);
    }
    private void PostsShowerClicked(object sender, EventArgs e)
    {
        PageShowStatus(2);
    }
    public async void PageShowStatus(int Status)
    {

        StudentTableShower.TextColor = Color.FromArgb("#1A1A1A");
        StudentTableShower.Background = Colors.Transparent;
        StdTableDataGrid.IsVisible = false;
        SearchBar.IsVisible = false;
        DeActiveStd.IsVisible = false;

        EmptyMessage.IsVisible=false;
        PostsShower.TextColor = Color.FromArgb("#1A1A1A");
        PostsShower.Background = Colors.Transparent;
        Postslistview.IsVisible = false;

        switch (Status)
        {


            case 1:
                StudentTableShower.TextColor = Color.FromArgb("#efefef");
                StudentTableShower.Background = Color.FromArgb("#1a1a1a");
                StdTableDataGrid.IsVisible = true;
                SearchBar.IsVisible = true;
                DeActiveStd.IsVisible = true;
                NoExistTitle.Text = "لا يوجد طلبة'";
                NoExistSubTitle.Text = "يمكنك اضافتهم عن طريق الزر الموجود بالاعلى";
                EmptyMessage.IsVisible = await LoadStd();

                break;
            case 2:
                PostsShower.TextColor = Color.FromArgb("#efefef");
                PostsShower.Background = Color.FromArgb("#1a1a1a");
                Postslistview.IsVisible = true;
                NoExistTitle.Text = "لا يوجد منشورات'";
                NoExistSubTitle.Text = "يمكنك اضافتهم عن طريق الزر الموجود بالاعلى";
                EmptyMessage.IsVisible = await LoadPosts();

                break;
            
        }

        //to show To show books

    }

    private void SelectionPostChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        ClearEntrys();
        DeletePostBtn.IsVisible = true;
        

        var SelectedPost = Postslistview.SelectedItem as SubjectPosts;

        PostId = SelectedPost.PostId.ToString();
        TitleEntry.Text = SelectedPost.PostTitle;
        DesEditor.Text = SelectedPost.PostDes;
        LinkEntry.Text = SelectedPost.PostFileLink;
        Postslistview.SelectedItem = null;
        /*LinkUrl = SelectedPost.PostFileLink;
        if (!string.IsNullOrEmpty(LinkUrl))
        {
            OpenLinkBtn.IsVisible = true;
        }*/

        PostPopupWindow.IsVisible = true;

    }
    private async void OpenLinkBtnClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(LinkEntry.Text))
        {
            return;
        }
        if (Uri.IsWellFormedUriString(LinkEntry.Text, UriKind.Absolute))
        {
            await Launcher.OpenAsync(LinkEntry.Text);
        }
    }
    private async void DeletePostClicked(object sender, EventArgs e)
    {

        int pid = int.Parse(PostId);
        bool confirm = await DisplayAlert("تأكيد الحذف", "هل أنت متأكد أنك تريد حذف هذا المنشور؟", "نعم", "لا");
        if (!confirm)
        {
			return;
        }
        // Initialize the YesNoContentView
        //var yesNoPopup = new YesNoContentView();

        // Add the popup to the current page's layout (assuming a Grid or StackLayout named 'MainLayout')
        //MainLayout.Children.Add(yesNoPopup);

        // Show the popup and wait for the user's response
        //bool isConfirmed = await yesNoPopup.ShowAsync();

        // Remove the popup after the response
        //MainLayout.Children.Remove(yesNoPopup);

        // If user clicked "No", exit the method
        /*if (!isConfirmed)
        {
            return;
        }*/
        // Perform delete operation
        await _database.deleteSubjectPost(pid);
        await DisplayAlert("تم الحذف", "تم حذف المنشور بنجاح", "حسنا");
        await LoadPosts();
        PostPopupWindow.IsVisible=false;
        PageShowStatus(2);
    }
    
    private async void SavePostClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(DesEditor.Text)) { 
            await DisplayAlert("خطا", "يجب ملئ العنوان والوصف", "حسنا");
            return;
        }
        if (!string.IsNullOrEmpty(LinkEntry.Text) && !Uri.IsWellFormedUriString(LinkEntry.Text, UriKind.Absolute))
        {
            await DisplayAlert("خطأ", "الرابط الذي أدخلته غير صحيح. يرجى إدخال رابط صالح.", "حسنا");
            return;
        }

        if (string.IsNullOrEmpty(PostId))
        {
            var post = new SubjectPosts
            {
                PostTitle = TitleEntry.Text,
                PostDes = DesEditor.Text,
                SubId = SubId,
                PostDate = DateTime.Now,
                PostFileLink = LinkEntry.Text,
            };
            await _database.insertSubjectPost(post);
            PageShowStatus(2);

            await DisplayAlert("تمت", "تم اضافة منشور", "حسنا");
        }
        else
        {
            int pid = int.Parse(PostId);
            var existingPost = await _database.getSubjectPost(pid);
            if (existingPost != null)
            {
                existingPost.PostTitle = TitleEntry.Text;
                existingPost.PostDes = DesEditor.Text;
                existingPost.PostFileLink = LinkEntry.Text;
                await _database.updateSubjectPost(existingPost);
                await DisplayAlert("تمت", "تم تعديل المنشور", "حسنا");
            }
        }
        ClearEntrys();
        await LoadPosts();
        PostPopupWindow.IsVisible = false;
    }
    private void CancelPostClicked(object sender, EventArgs e)
    {
        PostPopupWindow.IsVisible = false;
    }


}
                        