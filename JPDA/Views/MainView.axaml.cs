using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Threading;

namespace JPDA.Views;
using JPDA.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
public partial class MainView : UserControl
{
    // Doing some stuff here because it will do it on app startup (and view resets)
    
    public static readonly string PathToSettings = Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
        "jpda_settings.xml");
    
    public static readonly XDocument Xdoc = GetSettings(); // Loading the xml settings document. 

    public static string MenuButtonAlignStr = 
        Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == "MenuButtonAlign")!.Value == "0" ? "Top" : "Bottom";

    public static readonly string DbPath = GetDbPath();
    
    private static readonly MainViewModel StaticMainVm =  new MainViewModel(); // Used in resetting views
    
    public MainView()
    {
        InitializeComponent();
        StaticMainVm.ChangePage();
        MainListBox.SelectedItem = StaticMainVm.SelectedListItem; // Used in resetting views
        this.DataContext = StaticMainVm;
        
        
        // Applying saved settings from xml to the app
        // App language
        if (int.TryParse(GetSetting("LangID"), out int langId))
        {
            App.LangId = langId;
            if (Application.Current is not null)
            {
                var langCode = App.LangId switch { 1 => "ru", 2 => "jp", _ => "en" };
                var uri = new Uri($"avares://JPDA/Assets/Lang/{langCode}.axaml");
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceInclude(uri) { Source = uri });
            }
            
        }

        // Target languages checkboxes
        bool.TryParse(GetSetting("IsTargetLangEng"), out App.TargetEng);
        bool.TryParse(GetSetting("IsTargetLangRus"), out App.TargetRus);
        
        // Menu Alignment and applying in ViewModel
        if (int.TryParse(GetSetting("MenuButtonAlign"), out int align)) {
            string buttonAlign = align switch
            {
                1 => "Bottom",
                _ => "Top" // Handles 0 and default
            };
            
            App.MenuButtonAlign = align;
            if (this.DataContext is MainViewModel mvm)
            {
                if (App.CurrentScreenId == 2)
                {
                    var temp = buttonAlign; // For some reason buttonAlign fails after Task.Delay(50) so we have to save it
                    Dispatcher.UIThread.Post(async void () =>
                    {
                        await Task.Delay(50); 
                        mvm.MenuButtonAlignment = MainView.MenuButtonAlignStr = temp;
                    });
                    mvm.MenuButtonAlignment = MainView.MenuButtonAlignStr = buttonAlign;
                    mvm.LangMenuReset(); // Change text in menu
                }
                
                // Set smaller menu button size for Desktop
                // (doing it here to use ViewModel in one place)
                if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    mvm.MenuButtonWidth = 45;
                    mvm.MenuIconWidth = 25;
                    mvm.MenuIconHeight = 35;
                }
            }
        }
    }

    private string? GetSetting(string key)
    {
        return MainView.Xdoc.Descendants().Where(p => p.HasElements == false).FirstOrDefault(e => e.Name.LocalName == key)?.Value;
    }

    private static XDocument GetSettings()
    {
        if (File.Exists(PathToSettings))
        {
            try { return XDocument.Load(PathToSettings); }
            catch { /* File is corrupted */ }
        }

        return XDocument.Parse(
            @"<xml>
            <LangID>0</LangID>
            <IsTargetLangEng>True</IsTargetLangEng>
            <IsTargetLangRus>False</IsTargetLangRus>
            <MenuButtonAlign>0</MenuButtonAlign>
        </xml>");
    }

    private static string GetDbPath()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "expression_rus_eng.db");

        if (!File.Exists(path))
        {
            using var assetStream = AssetLoader.Open(new Uri("avares://JPDA/Assets/expression_rus_eng.db"));
            using var destStream = File.Create(path);
            assetStream.CopyTo(destStream);
        }

        return path;
    }
    
}

