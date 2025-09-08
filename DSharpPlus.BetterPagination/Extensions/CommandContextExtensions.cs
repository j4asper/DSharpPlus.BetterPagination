using DSharpPlus.BetterPagination.Handlers;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DSharpPlus.BetterPagination.Extensions;

public static class CommandContextExtensions
{
    /// <summary>
    /// Sends a paginated message with forward and back buttons that allow users to navigate through pages of content.
    /// The method supports an optional set of additional components (e.g., buttons) and restricts usage to the invoking user if required.
    /// </summary>
    /// <param name="commandContext">The context of the command invocation, containing user information and interaction context.</param>
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
        this CommandContext commandContext,
        IReadOnlyList<Page> pages,
        IReadOnlyList<DiscordComponent>? additionalComponents = null,
        bool asEphemeral = false,
        bool allowUsageByAnyone = false)
    {
        var context = commandContext as SlashCommandContext
                      ?? throw new InvalidCastException("CommandContext is not a SlashCommandContext.");

        var paginationHandler = new PaginationHandler(pages);

        var initialResponse = await SendPaginatedMessageAsync(
            context,
            paginationHandler.GetCurrentPage(),
            paginationHandler.GetPaginationButtons(),
            additionalComponents,
            asEphemeral,
            isInitialMessage: true);

        var isTimedOut = false;
        
        while (!isTimedOut)
        {
            var interactionResponse = await initialResponse!.WaitForButtonAsync(c =>
                    paginationHandler.GetPaginationButtons().Components.Any(x => x.CustomId == c.Id));

            if (interactionResponse.TimedOut)
            {
                await SendPaginatedMessageAsync(
                    context,
                    paginationHandler.GetCurrentPage(),
                    paginationHandler.GetPaginationButtons(),
                    additionalComponents,
                    originalMessage: initialResponse,
                    isTimeoutMessage: true);

                isTimedOut = true;
            }
            else
            {
                if (!allowUsageByAnyone && interactionResponse.Result.User.Id != context.User.Id)
                    continue;

                Page? pageToShow = null;

                if (interactionResponse.Result.Id == paginationHandler.BackButton.CustomId)
                    pageToShow = paginationHandler.GetPreviousPage();

                if (interactionResponse.Result.Id == paginationHandler.ForwardButton.CustomId)
                    pageToShow = paginationHandler.GetNextPage();
                
                await SendPaginatedMessageAsync(
                    context,
                    pageToShow!,
                    paginationHandler.GetPaginationButtons(),
                    additionalComponents,
                    asEphemeral,
                    interaction: interactionResponse.Result.Interaction);
            }
        }
    }

    private static async Task<DiscordMessage?> SendPaginatedMessageAsync(
        SlashCommandContext context,
        Page currentPage,
        DiscordActionRowComponent paginationComponent,
        IReadOnlyList<DiscordComponent>? additionalComponents = null,
        bool asEphemeral = false,
        DiscordInteraction? interaction = null,
        bool isInitialMessage = false,
        bool isTimeoutMessage = false,
        DiscordMessage? originalMessage = null)
    {
        if (isTimeoutMessage && originalMessage is not null)
        {
            var response = new DiscordMessageBuilder()
                .WithPaginationArgs(currentPage, new DiscordActionRowComponent(additionalComponents ?? []));

            response.AddActionRowComponent(paginationComponent);
            
            await originalMessage.ModifyAsync(response);
        }
        else
        {
            var response = new DiscordInteractionResponseBuilder()
                .WithPageContent(currentPage)
                .AddActionRowComponent(paginationComponent)
                .WithPaginationArgs(new DiscordActionRowComponent(additionalComponents ?? []), asEphemeral);
            
            if (isInitialMessage)
            {
                await context.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, response);

                var message = await context.Interaction.GetOriginalResponseAsync();

                return message;
            }
            
            await interaction!.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, response);
        }
        
        return null;
    }
}