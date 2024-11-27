using DSharpPlus.BetterPagination.Helpers;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DSharpPlus.BetterPagination.Handlers;

internal class PaginationHandler
{
    internal DiscordButtonComponent BackButton => ButtonHelpers.CreateBackButton(_backButtonId, _currentPageNumber);
    internal DiscordButtonComponent PageLabel => ButtonHelpers.CreatePageLabel(nameof(PageLabel), _currentPageNumber, PageCount);
    internal DiscordButtonComponent ForwardButton => ButtonHelpers.CreateForwardButton(_forwardButtonId, _currentPageNumber, PageCount);
 
    private int PageCount => _pages.Count;
    
    private int _currentPageNumber = 1;
    private readonly IReadOnlyList<Page> _pages;
    private readonly string _backButtonId;
    private readonly string _forwardButtonId;
    
    public PaginationHandler(IReadOnlyList<Page> pages)
    {
        _pages = pages;
        _backButtonId = Guid.NewGuid().ToString();
        _forwardButtonId = Guid.NewGuid().ToString();
    }


    internal Page GetCurrentPage() => _pages[_currentPageNumber - 1];

    internal Page GetNextPage()
    {
        _currentPageNumber++;
        
        return GetCurrentPage();
    }

    internal Page GetPreviousPage()
    {
        _currentPageNumber--;
        
        return GetCurrentPage();
    }
    
    internal IReadOnlyList<DiscordButtonComponent> GetPaginationButtons() => [BackButton, PageLabel, ForwardButton];
}