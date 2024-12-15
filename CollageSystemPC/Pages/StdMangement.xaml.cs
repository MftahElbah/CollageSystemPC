using SQLite;
using System.Collections.ObjectModel;
using TP;

namespace CollageSystemPC.Pages;

public partial class StdMangement : ContentPage
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
    public StdMangement()
	{
		InitializeComponent();
        _database = new SQLiteAsyncConnection(dbPath);
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStd();
    }
    private async Task LoadStd()
    {
        var std = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 2).ToListAsync();
        if (std.Count == 0)
        {
            return;
        }
        StdTableSetter = new ObservableCollection<UsersAccountTable>(std);
    }

    private void StdTableSelectionChanged(object sender, Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
    {
        if (DegreeTableDataGrid.SelectedRow == null)
        {
            return;
        }

        /*var DataRow = DegreeTableDataGrid.SelectedRow;
        StdNameEntry.Text = DataRow?.GetType().GetProperty("StdName")?.GetValue(DataRow)?.ToString();
        DegreeEntry.Text = DataRow?.GetType().GetProperty("Deg")?.GetValue(DataRow)?.ToString();
        MidDegreeEntry.Text = DataRow?.GetType().GetProperty("MiddelDeg")?.GetValue(DataRow)?.ToString();

        PopupEditDegreeWindow.IsVisible = true;
        DegreeTableDataGrid.SelectedRow = null;*/
    }
}