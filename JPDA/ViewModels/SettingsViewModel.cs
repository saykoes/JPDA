using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JPDA.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private int _selectedLangId = App.LangId;
    
    [ObservableProperty] private bool _isTargetLangEng = App.TargetEng; 
    [ObservableProperty] private bool _isTargetLangRus = App.TargetRus; 
    
    [ObservableProperty] private int _menuButtonAlignId = App.MenuButtonAlign;
}