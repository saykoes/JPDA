using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using JPDA.ViewModels;

namespace JPDA.Views;

public partial class DictionaryView : UserControl
{
    public DictionaryView()
    {
        InitializeComponent();
        this.DataContext = new DictionaryViewModel();

            if (this.DataContext is DictionaryViewModel vm)
            {
                if (App.IsRedirect)
                {
                    Dispatcher.UIThread.InvokeAsync(async () => { await vm.SearchButtonCommand(); },
                        DispatcherPriority.Background);
                    
                    App.IsRedirect = false;
                }
            }
        
    }
}