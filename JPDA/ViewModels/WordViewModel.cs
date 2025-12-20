using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JPDA.Views;

namespace JPDA.ViewModels;

public partial class WordViewModel : ViewModelBase
{
    [ObservableProperty] private string _jpKanji = "漢字";
    [ObservableProperty] private string _jpKana = "かんじ";

    public ObservableCollection<String> WordDefs { get; set; }
    
    
    public WordViewModel(string? uJpKanji, string? uJpKana, ObservableCollection<String> uWordDefs)
    {
        JpKanji = uJpKanji;
        JpKana = uJpKana;
        WordDefs = uWordDefs;
    }
}