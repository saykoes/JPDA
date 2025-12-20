using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JPDA.Data;
using JPDA.Views;
using Microsoft.EntityFrameworkCore;

namespace JPDA.ViewModels;

public partial class DictionaryViewModel : ViewModelBase
{
    
    public ObservableCollection<WordView> DictCollection { get; } = [];

    [ObservableProperty] private string _writtenWord = (App.IsRedirect) ? App.KanjiInputText : "";

    [ObservableProperty] private Vector _scrollOffset = new Vector(0,0);
    
    [ObservableProperty] private int _currentPageOffset = 1;
    [ObservableProperty] private int _allPageOffset = 2;
    [ObservableProperty] private bool _pageControlVisible = false;
    [ObservableProperty] private bool _pageControlBackVisible = false;
    [ObservableProperty] private bool _pageControlForwardVisible = false;
    
    [ObservableProperty] private bool _isLoading = false;   // Loading animation
    
    private void PageControlUpdate()
    {
        PageControlVisible = (AllPageOffset > 1);
        PageControlBackVisible = (CurrentPageOffset > 1);
        PageControlForwardVisible = (CurrentPageOffset < AllPageOffset);
    }

    [RelayCommand]
    public async Task SearchButtonCommand()
    {
        IsLoading = true;
        
        // Resetting values
        CurrentPageOffset = 1;
        AllPageOffset = 1;
        PageControlUpdate();
        DictCollection.Clear();
        
        // Counting all words
        await using (var context = new ExpressionContext())
        {
            var term = WrittenWord.ToLower();
            var entriesCount = await Task.Run(async () =>
            {
                return await context.Entries
                    .Where(e => e.Senses.Any(s => s.Glosses.Any(g =>
                        g.Content!.ToLower().StartsWith(term))))
                    .CountAsync(); // Use Async version
            });
            
            AllPageOffset = (entriesCount / 25) + 1;
            PageControlUpdate();
        }

        // Searching words
        await LookUpWord(); 
        IsLoading = false;
            
    }
    
    [RelayCommand]
    public Task NextButtonCommand() => UpdatePage(1);

    [RelayCommand]
    public Task PrevButtonCommand() => UpdatePage(-1);

    private async Task UpdatePage(int offsetDelta)
    {
        IsLoading = true;
    
        CurrentPageOffset += offsetDelta;
        DictCollection.Clear();

        await LookUpWord();
        IsLoading = false;
    }
    
    private async Task LookUpWord()
    {
        if (string.IsNullOrWhiteSpace(WrittenWord)) return;
    
        ScrollOffset = new Vector(0, 0);
        
        ObservableCollection<string> FirstCharToUpper(List<string?> input) =>
            new(input.Where(s => !string.IsNullOrEmpty(s)).Select(s => char.ToUpper(s![0]) + s[1..]));

        var searchTerm = WrittenWord.ToLower();
        
        // forming List for targetLanguages
        var targetLanguages = new List<int>();
        if (App.TargetEng) targetLanguages.Add(1);
        if (App.TargetRus) targetLanguages.Add(2);
        
        // Search in japanese (kanji)
        await using (var context = new ExpressionContext())
        {
            // Doing query on background thread (so that UI thread would not halt)
            var results = await Task.Run(async () =>
            {
                return await context.KEles
                    .AsNoTracking() // Optimization for read-only lookups
                    .Where(k => k.Keb == searchTerm)
                    .AsSplitQuery() // Optimization preventing result bloating
                    .Select(k => new
                    {
                        KanjiList = k.IdEntryNavigation!.KEles.Select(ke => ke.Keb).ToList(),
                        ReadingsForThisKanji = k.IdREles.Select(r => r.Reb).ToList(),
                        Glosses = k.IdEntryNavigation.Senses
                            .SelectMany(s => s.Glosses)
                            .Where(g => targetLanguages.Contains(g.IdLang))
                            .Select(g => g.Content)
                            .ToList()
                    })
                    .ToListAsync();
            });

            // Doing UI updates (DictCollection.Add()) on UI background thread
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var item in results)
                {
                    var kebGroup = string.Join("\n",
                        item.KanjiList.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct());
                    var rebGroup = string.Join(", ", item.ReadingsForThisKanji.Distinct());
                    var displayGlosses = item.Glosses.Distinct().ToList();

                    DictCollection.Add(new WordView(kebGroup, rebGroup, FirstCharToUpper(displayGlosses)));
                }
            }, DispatcherPriority.Background);

        }
        
        // Search in japanese kana phonetic system
        await using (var context = new ExpressionContext())
        {
            // Doing query on background thread (so that UI thread would not halt)
            var results = await Task.Run(async () =>
            {
                return await context.REles
                    .AsNoTracking() // Optimization for read-only lookups
                    .Where(r => r.Reb == searchTerm)
                    .AsSplitQuery() // Optimization preventing result bloating
                    .Select(r => new
                    {
                        KanjiList = r.IdKEles.Select(k => k.Keb).ToList(),
                        AllReadings = r.IdEntryNavigation!.REles.Select(re => re.Reb).ToList(),
                        Glosses = r.IdEntryNavigation.Senses
                            .SelectMany(s => s.Glosses)
                            .Where(g => targetLanguages.Contains(g.IdLang))
                            .Select(g => g.Content)
                            .ToList()
                    })
                    .ToListAsync();
            });

            // Doing UI updates (DictCollection.Add()) on UI background thread
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var item in results)
                {
                    var kebGroup = string.Join("\n",
                        item.KanjiList.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct());

                    // Fallback logic
                    var primary = !string.IsNullOrWhiteSpace(kebGroup) ? kebGroup : searchTerm;
                    var displayGlosses = FirstCharToUpper(item.Glosses.Distinct().ToList());

                    DictCollection.Add(new WordView(primary, searchTerm, displayGlosses));
                }
            }, DispatcherPriority.Background);

        }
        
        // Search in non-Japanese
        await using (var context = new ExpressionContext())
        {
            var pageSize = 25;
            // Doing query on background thread (so that UI thread would not halt)
            var results = await Task.Run(async () =>
            {
                return await context.Entries
                    .AsNoTracking() // Optimization for read-only lookups
                    .SelectMany(e => e.Senses, (entry, sense) => new { entry, sense }) // Flatten list to (entry,sense) pairs
                    .Where(x => x.sense.Glosses.Any(g => targetLanguages.Contains(g.IdLang) &&  // Looking only for our target languages
                                                         (EF.Functions.Like(g.Content, searchTerm + "%") ||         // "Word" || "Wording"
                                                          EF.Functions.Like(g.Content, "%) " + searchTerm + "%") || // "1) Word"
                                                          EF.Functions.Like(g.Content, "%, " + searchTerm + "%")))) // "1) something, Word" 
                    .Select(x => new
                    {
                        EntryId = x.entry.Id,   // For stable page system later
                        word = x.entry.REles.SelectMany(r => r.IdKEles).Select(k => k.Keb).FirstOrDefault(),
                        spelling = x.entry.REles.Select(r => r.Reb).FirstOrDefault(),
                        glosses = x.sense.Glosses.Where(g => targetLanguages.Contains(g.IdLang)).Select(g => g.Content).ToList(), // Selecting only our target languages
                        isExact = x.sense.Glosses.Any(g => g.Content == searchTerm) // For sorting by exact matches later
                    })
                    .OrderByDescending(x => x.isExact) // Exact mates first
                    .ThenBy(x => x.EntryId) // Sort by EntryID for stable page system
                    .Skip(Math.Max(0, (CurrentPageOffset - 1) * pageSize)) // Page system
                    .Take(pageSize)
                    .ToListAsync();
            });
            
            // Doing UI updates (DictCollection.Add()) on UI background thread
            Dispatcher.UIThread.Invoke(() => 
            {
                foreach (var e in results)
                {
                    var primary = !string.IsNullOrWhiteSpace(e.word) ? e.word : e.spelling;
                    DictCollection.Add(new WordView(primary, e.spelling, FirstCharToUpper(e.glosses)));
                }
            }, DispatcherPriority.Background);
        }
        
        PageControlUpdate();
    }
}