using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DSharpPlus.BetterPagination.Extensions;

internal static class DiscordMessageBuilderExtensions
{
    internal static DiscordMessageBuilder WithPaginationArgs(
        this DiscordMessageBuilder builder,
        Page page,
        DiscordActionRowComponent? additionalComponents)
    {
        builder.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
        {
            foreach (var component in page.Components)
                builder.AddActionRowComponent(component);
        }

        builder.WithContent(page.Content);
        
        if (additionalComponents != null)
            builder.AddActionRowComponent(additionalComponents);
        
        return builder;
    }
}