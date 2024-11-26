using DSharpPlus.BetterPagination.Helpers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DSharpPlus.BetterPagination.Extensions;

public static class SlashCommandContextExtensions
{
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