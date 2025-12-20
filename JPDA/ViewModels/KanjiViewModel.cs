using System;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JPDA.ViewModels;

public partial class KanjiViewModel : ViewModelBase
{
    [ObservableProperty] private string _inputText = "";

    [RelayCommand]
    public void RedirectToDictionary()
    {
        Console.WriteLine("Hello");
        if (String.IsNullOrEmpty(InputText)) return;
        Console.WriteLine("Not null!");
        App.IsRedirect = true;
        App.KanjiInputText = InputText;
        App.ResetViews(1);
    }
}