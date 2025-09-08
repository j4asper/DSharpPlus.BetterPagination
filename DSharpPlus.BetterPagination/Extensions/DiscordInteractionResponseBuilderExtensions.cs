using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace DSharpPlus.BetterPagination.Extensions;

internal static class DiscordInteractionResponseBuilderExtensions
{
    internal static DiscordInteractionResponseBuilder WithPageContent(this DiscordInteractionResponseBuilder builder, Page page)
    {
        builder.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
        {
            foreach (var component in page.Components)
            {
                builder.AddActionRowComponent(component);
            }
        }

        builder.WithContent(page.Content);
        
        return builder;
    }

    internal static DiscordInteractionResponseBuilder WithPaginationArgs(
        this DiscordInteractionResponseBuilder builder,
        DiscordActionRowComponent? additionalComponents,
        bool asEphemeral)
    {
        if (asEphemeral)
            builder.AsEphemeral();

        if (additionalComponents != null)
        {
            builder.AddActionRowComponent(additionalComponents);
        }
        
        return builder;
    }
}