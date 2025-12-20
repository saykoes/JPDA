using Avalonia.Controls;
namespace JPDA.Views;
using JPDA.ViewModels;
public partial class MainWindow : Window
{
    // MainWindow isn't initialized on mobile
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}