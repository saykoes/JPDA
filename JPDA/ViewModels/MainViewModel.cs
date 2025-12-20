using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JPDA.Data;
using JPDA.Views;

namespace JPDA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isMenuPaneOpen = false; 

    [ObservableProperty] private Uri _langResourcePath = new Uri("avares://JPDA/Assets/Lang/en.axaml");

    [ObservableProperty] private int _menuButtonWidth = 80;
    [ObservableProperty] private int _menuIconWidth = 40;
    [ObservableProperty] private int _menuIconHeight = 70; 
    
    [ObservableProperty] private string _menuButtonAlignment = MainView.MenuButtonAlignStr;
    
    [RelayCommand]
    private void TriggerPane()
    {
        IsMenuPaneOpen = !IsMenuPaneOpen;
    }
    
    private static string? GetUiString(string str)
    {
        object? uiSettingsStr = "Undefined";
        Application.Current?.Resources.MergedDictionaries[0].TryGetResource(str, null, out uiSettingsStr);
        return (string?)uiSettingsStr;
    }
    
    public static ObservableCollection<ListItemTemplate> MenuItems { get; } =
    [
        new ListItemTemplate(typeof(KanjiViewModel), GetUiString("ui_menu_kanji_input"), GetIcon("EditRegular")),
        new ListItemTemplate(typeof(DictionaryViewModel), GetUiString("ui_menu_dictionary"), GetIcon("BookRegular")),
        new ListItemTemplate(typeof(SettingsViewModel), GetUiString("ui_settings"), GetIcon("SettingsRegular"))
    ];
    
    [ObservableProperty] private ListItemTemplate? _selectedListItem ;  // Current menu selection
    // Redefining generated property for our needs (changing menu selection changes currentPage)
    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
        IsMenuPaneOpen = false;
    }
    
    [ObservableProperty] private ViewModelBase? _currentPage = (ViewModelBase)Activator.CreateInstance(MenuItems[App.CurrentScreenId].ModelType)!;
    
    public void LangMenuReset()
    {
        MenuItems.Clear();
        MenuItems.Add(new ListItemTemplate(typeof(KanjiViewModel),GetUiString("ui_menu_kanji_input"), GetIcon("EditRegular")));
        MenuItems.Add(new ListItemTemplate(typeof(DictionaryViewModel),GetUiString("ui_menu_dictionary"), GetIcon("BookRegular")));
        MenuItems.Add(new ListItemTemplate(typeof(SettingsViewModel),GetUiString("ui_settings"), GetIcon("SettingsRegular")));
    }
    
    public static StreamGeometry? GetIcon(string str)
    {
        Application.Current!.TryFindResource(str, out var res);
        return (StreamGeometry?)res;
    }
}

public class ListItemTemplate(Type type, string? title, StreamGeometry? icon)
{
    public string? Label { get; set; } = title;
    public Type ModelType { get; set; } = type;
    public StreamGeometry? ListItemIcon { get; } = icon;
}

