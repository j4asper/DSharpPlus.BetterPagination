using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DSharpPlus.BetterPagination.Extensions;

internal static class DiscordInteractionResponseBuilderExtensions
{
    internal static DiscordInteractionResponseBuilder WithPageContent(this DiscordInteractionResponseBuilder builder, Page page)
    {
        builder.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
            builder.AddComponents(page.Components);

        builder.WithContent(page.Content);
        
        return builder;
    }

    internal static DiscordInteractionResponseBuilder WithPaginationArgs(
        this DiscordInteractionResponseBuilder builder,
        IReadOnlyList<DiscordComponent>? additionalComponents,
        bool asEphemeral)
    {
        if (asEphemeral)
            builder.AsEphemeral();

        if (additionalComponents != null)
            builder.AddComponents(additionalComponents);
        
        return builder;
    }
}