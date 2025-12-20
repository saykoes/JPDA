using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using JPDA.ViewModels;

namespace JPDA.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        this.DataContext = new SettingsViewModel();
    }

    private void SelectingItemsControl_OnSelectionChanged_Langs(object? sender, SelectionChangedEventArgs e)
    {
        //Console.WriteLine("Changed!");
        if (!(sender is ComboBox comboBox)) return;
        if (comboBox.SelectedIndex != App.LangId)
        { 
            App.LangId = comboBox.SelectedIndex;
            
            MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "LangID").Value = App.LangId.ToString();
            MainView.Xdoc.Save(MainView.PathToSettings);
            
            App.ResetViews(2); // Restart on mobile because for some reason list in MainViewModel doesn't update no matter what
        }
    }
    

    private void CheckBox_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Check is changed!");
        if (this.DataContext is SettingsViewModel vm)
        {
            App.TargetEng = vm.IsTargetLangEng;
            App.TargetRus = vm.IsTargetLangRus;
        }
        
        MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "IsTargetLangEng").Value = App.TargetEng.ToString();
        MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "IsTargetLangRus").Value = App.TargetRus.ToString();
        //Console.WriteLine($"Path! {MainView.PathToSettings}");
        MainView.Xdoc.Save(MainView.PathToSettings);
        
    }

    private void SelectingItemsControl_OnSelectionChanged_Button_Alignment(object? sender, SelectionChangedEventArgs e)
    {
        
        if (!(sender is ComboBox comboBox)) return;
        if (comboBox.SelectedIndex != App.MenuButtonAlign)
        {
            Console.WriteLine("Selection on Alignment is changed!");
            App.MenuButtonAlign = comboBox.SelectedIndex;


            MainView.Xdoc.Descendants().Where(p => p.HasElements == false)
                    .FirstOrDefault(e => e.Name.LocalName == "MenuButtonAlign").Value =
                App.MenuButtonAlign.ToString();
            MainView.Xdoc.Save(MainView.PathToSettings);
            App.ResetViews(2);
        }
    }
    
}