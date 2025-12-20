using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JPDA.ViewModels;

namespace JPDA.Views;

public partial class KanjiView : UserControl
{
    public KanjiView()
    {
        InitializeComponent();
        this.DataContext = new KanjiViewModel();
    }

    // Handling click here because it's easier to get web launcher in View (because it's child of Visual class)
    private async void JardicButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!String.IsNullOrEmpty(KanjiInput.Text))
        {
            bool success = await TopLevel.GetTopLevel(this)!.Launcher.LaunchUriAsync(
                new Uri("https://www.jardic.ru/search/search_r.php?q=" + KanjiInput.Text + "&dic_jardic=1"));
        }
    }
}