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
        bool allowUsageByAnyone = false)
    {
        var pageCount = pages.Count;
        var currentPage = 1;

        var pageEmbeds = pages.Select(x => x.Embed).ToArray();

        var forwardBtn = ButtonHelpers.CreateForwardButton(Guid.NewGuid().ToString(), currentPage, pageCount);

        var backBtn = ButtonHelpers.CreateBackButton(Guid.NewGuid().ToString(), currentPage);

        var pageLabel = ButtonHelpers.CreatePageLabel(Guid.NewGuid().ToString(), currentPage, pageCount);

        List<DiscordComponent> components = [backBtn, pageLabel, forwardBtn];

        var message = await SendInitialResponseAsync(context, pageEmbeds, components, additionalComponents);

        var timedOut = false;

        while (!timedOut)
        {
            var response =
                await message.WaitForButtonAsync(c => c.Id == forwardBtn.CustomId || c.Id == backBtn.CustomId);

            if (response.TimedOut)
            {
                await SendTimeoutResponseAsync(
                    message,
                    pageEmbeds,
                    currentPage,
                    pageCount,
                    backBtn.CustomId,
                    pageLabel.CustomId,
                    forwardBtn.CustomId,
                    additionalComponents);

                timedOut = true;
            }
            else
            {
                if (!allowUsageByAnyone)
                {
                    if (response.Result.User.Id != context.User.Id)
                        continue;
                }
                
                if (response.Result.Id == backBtn.CustomId)
                    currentPage--;

                if (response.Result.Id == forwardBtn.CustomId)
                    currentPage++;

                var interactionResponse = new DiscordInteractionResponseBuilder()
                    .AddEmbed(pageEmbeds[currentPage - 1])
                    .AddComponents(
                        ButtonHelpers.CreateBackButton(backBtn.CustomId, currentPage),
                        ButtonHelpers.CreatePageLabel(pageLabel.CustomId, currentPage, pageCount),
                        ButtonHelpers.CreateForwardButton(forwardBtn.CustomId, currentPage, pageCount));

                if (additionalComponents != null)
                    interactionResponse.AddComponents(additionalComponents);

                await response.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                    interactionResponse);
            }
        }
    }

    private static async Task<DiscordMessage> SendInitialResponseAsync(
        SlashCommandContext context,
        DiscordEmbed[] embeds,
        IReadOnlyList<DiscordComponent> components,
        IReadOnlyList<DiscordComponent>? additionalComponents = null)
    {
        var responseBuilder = new DiscordInteractionResponseBuilder()
            .AddEmbed(embeds[0])
            .AddComponents(components);

        if (additionalComponents != null)
            responseBuilder.AddComponents(additionalComponents);

        await context.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            responseBuilder);

        var message = await context.Interaction.GetOriginalResponseAsync();

        return message;
    }

    private static async Task SendTimeoutResponseAsync(
        DiscordMessage message,
        DiscordEmbed[] embeds,
        int currentPage,
        int pageCount,
        string backBtnId,
        string pageLabelId,
        string forwardBtnId,
        IReadOnlyList<DiscordComponent>? additionalComponents = null)
    {
        var timedOutResponse = new DiscordMessageBuilder()
            .AddEmbed(embeds[currentPage - 1])
            .AddComponents(
                ButtonHelpers.CreateBackButton(backBtnId, 1),
                ButtonHelpers.CreatePageLabel(pageLabelId, currentPage, pageCount),
                ButtonHelpers.CreateForwardButton(forwardBtnId, 1, 1));

        if (additionalComponents != null)
            timedOutResponse.AddComponents(additionalComponents);

        await message.ModifyAsync(timedOutResponse);
    }
}