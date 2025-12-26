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
    
    public void ResetMainView()
    {
        MainViewWindow = new MainView(); 
        if(App.CurrentScreenId == 2)
            MainViewWindow = new MainView();    // for settings animation (it doesn't display it first time)
    }
}