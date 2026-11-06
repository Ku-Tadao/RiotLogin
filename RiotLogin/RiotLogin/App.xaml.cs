using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using UiMessageBox = Wpf.Ui.Controls.MessageBox;
using UiMessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using MenuItem = System.Windows.Controls.MenuItem;
using DataGrid = System.Windows.Controls.DataGrid;

namespace RiotLogin
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationAccentColorManager.Apply(Color.FromRgb(220, 40, 60), ApplicationTheme.Dark); // Riot red
        }

        private void CopyField_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var grid = ((ContextMenu)item.Parent).PlacementTarget as DataGrid;
            if (grid?.SelectedItem is AccountProfile p)
                Copy((string)typeof(AccountProfile).GetProperty((string)item.Tag).GetValue(p));
        }

        public static void Copy(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) Clipboard.Clear();
                else Clipboard.SetText(text);
            }
            catch { /* clipboard can be momentarily locked */ }
        }

        // Non-blocking message in a window's InfoBar. Info/success messages fade out on their own.
        public static async void Notify(InfoBar bar, string message, InfoBarSeverity severity = InfoBarSeverity.Informational)
        {
            bar.Message = message;
            bar.Severity = severity;
            bar.IsOpen = true;
            if (severity == InfoBarSeverity.Informational || severity == InfoBarSeverity.Success)
            {
                await Task.Delay(4000);
                if (bar.Message == message) bar.IsOpen = false;
            }
        }

        public static async Task<bool> Confirm(string text)
        {
            var box = new UiMessageBox
            {
                Title = "RiotLogin",
                Content = text,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                Owner = Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            return await box.ShowDialogAsync() == UiMessageBoxResult.Primary;
        }
    }

    /// <summary>Colours a rank string ("Diamond IV 10LP") by its tier.</summary>
    public class RankBrushConverter : IValueConverter
    {
        private static readonly (string Tier, string Hex)[] Tiers =
        {
            ("Challenger", "#F0C75E"), ("Grandmaster", "#E8636B"), ("Master", "#B57CE0"),
            ("Diamond", "#6C9BEF"), ("Emerald", "#2EC27E"), ("Platinum", "#4FB3A9"),
            ("Gold", "#D8B45B"), ("Silver", "#A7B0BA"), ("Bronze", "#B07D62"), ("Iron", "#8C7F78"),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string ?? "";
            foreach (var (tier, hex) in Tiers)
                if (text.StartsWith(tier, StringComparison.OrdinalIgnoreCase))
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            return Application.Current.TryFindResource("TextFillColorTertiaryBrush"); // Unranked / empty
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>Colours the account status: OK green, banned red, restricted amber.</summary>
    public class StatusBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string ?? "";
            string hex = text == "OK" ? "#2EC27E" : text.StartsWith("Banned") ? "#E8636B" : text.Length > 0 ? "#E0A84F" : null;
            return hex == null ? Application.Current.TryFindResource("TextFillColorTertiaryBrush")
                               : new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
