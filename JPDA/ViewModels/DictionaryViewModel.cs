using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
    [ObservableProperty] private Vector _scrollOffset = new Vector(0, 0);

    [ObservableProperty] private int _currentPageOffset = 1;
    [ObservableProperty] private int _allPageOffset = 1;

    [ObservableProperty] private bool _pageControlVisible = false;
    [ObservableProperty] private bool _pageControlBackVisible = false;
    [ObservableProperty] private bool _pageControlForwardVisible = false;

    [ObservableProperty] private bool _isLoading = false;
    
    // TokenSource  for canceling previous search when a new one starts
    private CancellationTokenSource? _searchCts;
    private void CancelPreviousSearch()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task SearchButtonCommand()
    {
        IsLoading = true;

        // Cancel anything currently running
        CancelPreviousSearch();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            // Reset values
            CurrentPageOffset = 1;
            AllPageOffset = 1;
            PageControlUpdate();

            await Dispatcher.UIThread.InvokeAsync(() => DictCollection.Clear(), DispatcherPriority.Background);

            // If empty - nothing to do
            if (string.IsNullOrWhiteSpace(WrittenWord))
                return;

            await LookUpWordCoreAsync(token, true);
        }
        catch (OperationCanceledException)
        {
            // Expected when user triggers another search/page action.
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void PageControlUpdate()
    {
        PageControlVisible = (AllPageOffset > 1);
        PageControlBackVisible = (CurrentPageOffset > 1);
        PageControlForwardVisible = (CurrentPageOffset < AllPageOffset);
    }
    
    [RelayCommand]
    public Task NextButtonCommand() => UpdatePageAsync(+1);

    [RelayCommand]
    public Task PrevButtonCommand() => UpdatePageAsync(-1);

    private async Task UpdatePageAsync(int offsetDelta)
    {
        IsLoading = true;

        // Cancel anything currently running
        CancelPreviousSearch();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            // Clamp page bounds
            var nextPage = CurrentPageOffset + offsetDelta;
            if (nextPage < 1) nextPage = 1;
            if (nextPage > AllPageOffset) nextPage = AllPageOffset;

            CurrentPageOffset = nextPage;
            PageControlUpdate();

            await Dispatcher.UIThread.InvokeAsync(() => DictCollection.Clear());

            await LookUpWordCoreAsync(token, false);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static ObservableCollection<string> FirstCharToUpper(List<string?> input)
    {
        return new ObservableCollection<string>(
            input
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => char.ToUpper(s![0]) + s[1..])
        );
    }

    private static List<int> GetTargetLanguages()
    {
        var targetLanguages = new List<int>(capacity: 2);
        if (App.TargetEng) targetLanguages.Add(1);
        if (App.TargetRus) targetLanguages.Add(2);
        return targetLanguages;
    }
    
    private const int PageSize = 25;

    // Protect against multiple LookUpWord executions at the same time
    private readonly SemaphoreSlim _searchGate = new(1, 1);
    
    private async Task LookUpWordCoreAsync(CancellationToken token, bool isNewSearch)
    {
        // Ensure only one LookUpWord is running at a time
        await _searchGate.WaitAsync(token);
        try
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(WrittenWord))
                return;

            var searchTerm = WrittenWord.ToLower();
            var targetLanguages = GetTargetLanguages();

            // Reset scroll to top on UI thread
            await Dispatcher.UIThread.InvokeAsync(() => ScrollOffset = new Vector(0, 0), DispatcherPriority.Background);

            await Task.Run(async () =>
            {
                // 1) Search by Japanese kanji (KEles)
                await using (var context = new ExpressionContext())
                {
                    var results = await context.KEles
                        .AsNoTracking()
                        .Where(k => k.Keb == searchTerm)
                        .AsSplitQuery()
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
                        .ToListAsync(token);

                    token.ThrowIfCancellationRequested();

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        foreach (var item in results)
                        {
                            var kebGroup = string.Join("\n",
                                item.KanjiList.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct());

                            var rebGroup = string.Join(", ",
                                item.ReadingsForThisKanji.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct());

                            var displayGlosses = item.Glosses.Distinct().ToList();

                            DictCollection.Add(new WordView(
                                kebGroup,
                                rebGroup,
                                FirstCharToUpper(displayGlosses)
                            ));
                        }
                    }, DispatcherPriority.Background);
                }

                if (DictCollection.Count != 0) return;

                // 2) Search by Japanese kana reading (REles)
                await using (var context = new ExpressionContext())
                {
                    var results = await context.REles
                        .AsNoTracking()
                        .Where(r => r.Reb == searchTerm)
                        .AsSplitQuery()
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
                        .ToListAsync(token);

                    token.ThrowIfCancellationRequested();

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        foreach (var item in results)
                        {
                            var kebGroup = string.Join("\n",
                                item.KanjiList.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct());

                            var primary = !string.IsNullOrWhiteSpace(kebGroup) ? kebGroup : searchTerm;
                            var displayGlosses = FirstCharToUpper(item.Glosses.Distinct().ToList());

                            DictCollection.Add(new WordView(primary, searchTerm, displayGlosses));
                        }
                    }, DispatcherPriority.Background);
                }

                if (DictCollection.Count != 0) return;

                // 3) Search in non-Japanese glosses (Entries/Senses/Glosses) with pagination

                if (isNewSearch)
                {
                    // Count entries for non-Japanese search to build page count.
                    int entriesCount;
                    await using (var context = new ExpressionContext())
                    {
                        entriesCount = await context.Entries
                            .AsNoTracking()
                            .Where(e => e.Senses.Any(s => s.Glosses.Any(g =>
                                targetLanguages.Contains(g.IdLang) &&
                                g.Content != null &&
                                EF.Functions.Like(g.Content.ToLower(), searchTerm + "%"))))
                            .CountAsync(token);
                    }

                    AllPageOffset = Math.Max(1, (entriesCount + PageSize - 1) / PageSize);
                    PageControlUpdate();
                }

                await using (var context = new ExpressionContext())
                {
                    var skip = Math.Max(0, (CurrentPageOffset - 1) * PageSize);

                    var results = await context.Entries
                        .AsNoTracking()
                        .SelectMany(e => e.Senses, (entry, sense) => new { entry, sense })
                        .Where(x => x.sense.Glosses.Any(g =>
                            targetLanguages.Contains(g.IdLang) &&
                            g.Content != null &&
                            (
                                EF.Functions.Like(g.Content, searchTerm + "%") ||
                                EF.Functions.Like(g.Content, "%) " + searchTerm + "%") ||
                                EF.Functions.Like(g.Content, "%, " + searchTerm + "%")
                            )))
                        .Select(x => new
                        {
                            EntryId = x.entry.Id,
                            word = x.entry.REles.SelectMany(r => r.IdKEles).Select(k => k.Keb).FirstOrDefault(),
                            spelling = x.entry.REles.Select(r => r.Reb).FirstOrDefault(),
                            glosses = x.sense.Glosses
                                .Where(g => targetLanguages.Contains(g.IdLang))
                                .Select(g => g.Content)
                                .ToList(),
                            isExact = x.sense.Glosses.Any(g => g.Content == searchTerm)
                        })
                        .OrderByDescending(x => x.isExact)
                        .ThenBy(x => x.EntryId)
                        .Skip(skip)
                        .Take(PageSize)
                        .ToListAsync(token);

                    token.ThrowIfCancellationRequested();

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        foreach (var e in results)
                        {
                            var primary = !string.IsNullOrWhiteSpace(e.word) ? e.word : e.spelling;
                            DictCollection.Add(new WordView(primary, e.spelling, FirstCharToUpper(e.glosses)));
                        }
                    }, DispatcherPriority.Background);
                }

                // Update page controls once at the end (current/all already set earlier)
                await Dispatcher.UIThread.InvokeAsync(PageControlUpdate);
            }, token);
        }
        finally
        {
            _searchGate.Release();
        }
    }
}