using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DSharpPlus.BetterPagination.Extensions;

internal static class DiscordMessageBuilderExtensions
{
    internal static DiscordMessageBuilder WithPaginationArgs(
        this DiscordMessageBuilder builder,
        Page page,
        IReadOnlyList<DiscordComponent>? additionalComponents)
    {
        builder.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
            builder.AddComponents(page.Components);

        builder.WithContent(page.Content);
        
        if (additionalComponents != null)
            builder.AddComponents(additionalComponents);
        
        return builder;
    }
}