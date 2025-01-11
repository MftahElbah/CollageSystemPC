namespace CollageSystemPC.Pages;

public partial class SnackbarPopup : ContentView
{
    public SnackbarPopup()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Shows the Snackbar with the specified type and message.
    /// </summary>
    /// <param name="type">Type of Snackbar:1 = "Message", 2 = "Error", 3 = "Internet"</param>
    /// <param name="message">The message to display</param>
    public async void ShowSnackbar(int type, string message)
    {
        // Set the message text
        SnackbarMessageLabel.Text = message;

        // Change background color based on type
        switch (type)
        {
            
            case 3:
                
                ConBoarder.BackgroundColor = Color.FromArgb("#1a1a1a"); // Black
                SnackbarMessageLabel.TextColor = Color.FromArgb("#EFEFEF");
                Message.IsVisible = true;
                return;

            case 2:
                
                ConBoarder.BackgroundColor = Color.FromArgb("#F02C2C"); // Red
                SnackbarMessageLabel.TextColor = Color.FromArgb("#efefef");
                break;

            case 1:
                ConBoarder.BackgroundColor = Color.FromArgb("#1a1a1a"); // Default Black
                SnackbarMessageLabel.TextColor = Color.FromArgb("#EFEFEF");
                
                break;
            }

        // Show the Snackbar
        Message.IsVisible = true;

        // Auto-hide after 3 seconds
        await Task.Delay(3000);
        Message.IsVisible = false;
    }
}
