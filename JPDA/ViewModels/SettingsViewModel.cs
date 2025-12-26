using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JPDA.Views;

namespace JPDA.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private int _selectedLangId = App.LangId;

    partial void OnSelectedLangIdChanged(int value)
    {
        if (value != App.LangId)
        { 
            App.LangId = value;
            
            MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "LangID")!.Value = App.LangId.ToString();
            MainView.Xdoc.Save(MainView.PathToSettings);
            
            App.ResetViews(2); // Restart on mobile because for some reason list in MainViewModel doesn't update no matter what
        }
    }
    
    [ObservableProperty] private bool _isTargetLangEng = App.TargetEng;
    partial void OnIsTargetLangEngChanged(bool value)
    {
        App.TargetEng = value;
        
        MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "IsTargetLangEng")!.Value = App.TargetEng.ToString();
        MainView.Xdoc.Save(MainView.PathToSettings);
    }
    
    [ObservableProperty] private bool _isTargetLangRus = App.TargetRus;
    partial void OnIsTargetLangRusChanged(bool value)
    {
        App.TargetRus = value;
        
        MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "IsTargetLangRus")!.Value = App.TargetRus.ToString();
        MainView.Xdoc.Save(MainView.PathToSettings);
    }
    
    [ObservableProperty] private int _menuButtonAlignId = App.MenuButtonAlign;

    partial void OnMenuButtonAlignIdChanged(int value)
    {
        App.MenuButtonAlign = value;
        
        MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "MenuButtonAlign")!.Value = App.MenuButtonAlign.ToString();
        MainView.Xdoc.Save(MainView.PathToSettings);
        
        App.ResetViews(2);
    }
}