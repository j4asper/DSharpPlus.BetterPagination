using DSharpPlus.BetterPagination.Helpers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DSharpPlus.BetterPagination.Extensions;

public static class SlashCommandContextExtensions
{
    /// <summary>
    /// Sends a paginated message with forward and back buttons that allow users to navigate through pages of content.
    /// The method supports an optional set of additional components (e.g., buttons) and restricts usage to the invoking user if required.
    /// </summary>
    /// <param name="context">The context of the slash command invocation, containing user information and interaction context.</param>
    /// <param name="pages">A read-only list of <see cref="Page"/> objects, each containing an embed to be displayed on the paginated message.</param>
    /// <param name="additionalComponents">Optional additional components (e.g., buttons, select menus) to be added to the message. Default is null.</param>
    /// <param name="asEphemeral">A flag indicating whether the paginated message should be sent as an ephemeral message. If set to true, the message will only be visible to the user who invoked the command. Default is false.</param>
    /// <param name="allowUsageByAnyone">A flag indicating whether any user can navigate the pagination, or restricts usage to the invoking user only. Default is false.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// The method creates forward and back buttons to allow users to navigate between pages. It waits for user interaction (button clicks) and updates the message accordingly.
    /// If no interaction occurs within the timeout period, the message is updated with a timeout response. 
    /// If the <paramref name="allowUsageByAnyone"/> flag is set to false, only the user who invoked the command can interact with the buttons.
    /// </remarks>
    public static async Task SendBetterPaginatedMessageAsync(
        this SlashCommandContext context,
        IReadOnlyList<Page> pages,
        IReadOnlyList<DiscordComponent>? additionalComponents = null,
        bool asEphemeral = false,
        bool allowUsageByAnyone = false)
    {
        var pageCount = pages.Count;
        
        var currentPage = 1;
        
        var components = CreatePaginationComponents(currentPage, pageCount);

        var message = await SendInitialResponseAsync(context, pages, components, additionalComponents, asEphemeral);

        var timedOut = false;

        while (!timedOut)
        {
            var response =
                await message.WaitForButtonAsync(c =>
                    c.Id == components[0].CustomId || c.Id == components[2].CustomId);

            if (response.TimedOut)
            {
                await SendTimeoutResponseAsync(message, pages, pageCount, components, additionalComponents);
                
                timedOut = true;
            }
            else
            {
                if (!allowUsageByAnyone && response.Result.User.Id != context.User.Id)
                    continue;

                if (response.Result.Id == components[0].CustomId)
                    currentPage--;

                if (response.Result.Id == components[2].CustomId)
                    currentPage++;

                var updatedComponents = CreatePaginationComponents(currentPage, pageCount, components[2].CustomId, components[0].CustomId);
                
                var interactionResponse = new DiscordInteractionResponseBuilder()
                    .AddComponents(updatedComponents);
                
                var page = pages[currentPage - 1];
                
                interactionResponse.AddEmbed(page.Embed);
                
                if (page.Components.Count > 0)
                    interactionResponse.AddComponents(page.Components);

                if (string.IsNullOrEmpty(page.Content))
                    interactionResponse.WithContent(page.Content);
                
                if (asEphemeral)
                    interactionResponse.AsEphemeral();

                if (additionalComponents != null)
                    interactionResponse.AddComponents(additionalComponents);

                await response.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                    interactionResponse);
            }
        }
    }

    private static List<DiscordComponent> CreatePaginationComponents(int currentPage, int pageCount, string? forwardButtonId = null, string? backButtonId = null)
    {
        forwardButtonId ??= Guid.NewGuid().ToString();
        backButtonId ??= Guid.NewGuid().ToString();
        
        var backBtn = ButtonHelpers.CreateBackButton(backButtonId, currentPage);
        var forwardBtn = ButtonHelpers.CreateForwardButton(forwardButtonId, currentPage, pageCount);
        var pageLabel = ButtonHelpers.CreatePageLabel(Guid.NewGuid().ToString(), currentPage, pageCount);

        return [backBtn, pageLabel, forwardBtn];
    }

    private static async Task<DiscordMessage> SendInitialResponseAsync(
        SlashCommandContext context,
        IReadOnlyList<Page> pages,
        IReadOnlyList<DiscordComponent> components,
        IReadOnlyList<DiscordComponent>? additionalComponents = null,
        bool asEphemeral = false)
    {
        var page = pages[0];
        
        var interactionResponse = new DiscordInteractionResponseBuilder()
            .AddComponents(components);
        
        interactionResponse.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
            interactionResponse.AddComponents(page.Components);

        if (string.IsNullOrEmpty(page.Content))
            interactionResponse.WithContent(page.Content);
        
        if (asEphemeral)
            interactionResponse.AsEphemeral();
        
        if (additionalComponents != null)
            interactionResponse.AddComponents(additionalComponents);

        await context.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            interactionResponse);

        var message = await context.Interaction.GetOriginalResponseAsync();
        
        return message;
    }

    private static async Task SendTimeoutResponseAsync(
        DiscordMessage message,
        IReadOnlyList<Page> pages,
        int currentPage,
        IReadOnlyList<DiscordComponent> components,
        IReadOnlyList<DiscordComponent>? additionalComponents = null)
    {
        var page = pages[currentPage - 1];
        
        var timedOutResponse = new DiscordMessageBuilder()
            .AddComponents(components);
        
        timedOutResponse.AddEmbed(page.Embed);

        if (page.Components.Count > 0)
            timedOutResponse.AddComponents(page.Components);

        if (string.IsNullOrEmpty(page.Content))
            timedOutResponse.WithContent(page.Content);
        
        if (additionalComponents != null)
            timedOutResponse.AddComponents(additionalComponents);

        await message.ModifyAsync(timedOutResponse);
    }
}