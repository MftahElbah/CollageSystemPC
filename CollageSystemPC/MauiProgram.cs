using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Handlers;
namespace CollageSystemPC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Cairo.ttf", "Cairo");
                    fonts.AddFont("Cairo-Bold.ttf", "CairoB");
                    fonts.AddFont("Cairo-Light.ttf", "CairoL");
                });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (handler.PlatformView is TextBox nativeEntry)
                {
                    // Remove the border
                    nativeEntry.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);

                    // Remove focus visual (e.g., glowing border or underline when focused)
                    nativeEntry.Style = new Microsoft.UI.Xaml.Style(typeof(TextBox))
                    {
                        Setters =
            {
                new Microsoft.UI.Xaml.Setter(TextBox.BorderThicknessProperty, new Microsoft.UI.Xaml.Thickness(0)),
                new Microsoft.UI.Xaml.Setter(TextBox.FocusVisualMarginProperty, new Microsoft.UI.Xaml.Thickness(0)),
                new Microsoft.UI.Xaml.Setter(TextBox.FocusVisualPrimaryThicknessProperty, new Microsoft.UI.Xaml.Thickness(0)),
                new Microsoft.UI.Xaml.Setter(TextBox.FocusVisualSecondaryThicknessProperty, new Microsoft.UI.Xaml.Thickness(0))
            }
                    };

                    // Change placeholder text color (optional)
                    if (!string.IsNullOrEmpty(nativeEntry.PlaceholderText))
                    {
                        nativeEntry.PlaceholderForeground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                                        Windows.UI.Color.FromArgb(255, 149, 149, 149));
                    }
                }
            });



            Microsoft.Maui.Handlers.RadioButtonHandler.Mapper.AppendToMapping(nameof(Microsoft.UI.Xaml.Controls.RadioButton), (handler, view) =>
            {
                if (handler.PlatformView is Microsoft.UI.Xaml.Controls.RadioButton nativeRadioButton)
                {
                    // Change the check color
                    nativeRadioButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 26, 26, 26));
                }
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
