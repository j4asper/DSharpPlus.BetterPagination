using DSharpPlus.Entities;

namespace DSharpPlus.BetterPagination.Helpers;

internal static class ButtonHelpers
{
    internal static DiscordButtonComponent CreateBackButton(string id, int currentPage)
    {
        var isDisabled = currentPage == 1;
        
        return new DiscordButtonComponent(
            DiscordButtonStyle.Success,
            id,
            "<-",
            isDisabled);
    }
    
    internal static DiscordButtonComponent CreateForwardButton(string id, int currentPage, int pageCount)
    {
        var isDisabled = currentPage == pageCount;
        
        return new DiscordButtonComponent(
            DiscordButtonStyle.Success,
            id,
            "->",
            isDisabled);
    }
    
    internal static DiscordButtonComponent CreatePageLabel(string id, int currentPage, int pageCount)
    {
        return new DiscordButtonComponent(
            DiscordButtonStyle.Secondary,
            id,
            $"{currentPage}/{pageCount}",
            false);
    }
}