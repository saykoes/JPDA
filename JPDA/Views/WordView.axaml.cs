using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JPDA.ViewModels;

namespace JPDA.Views;

public partial class WordView : UserControl
{
    public WordView(string? uJpKanji, string? uJpKana, ObservableCollection<String> uWordDefs)
    {
        InitializeComponent();
        this.DataContext = new WordViewModel(uJpKanji, uJpKana, uWordDefs);
    }
}