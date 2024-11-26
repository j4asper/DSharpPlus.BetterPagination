# BetterPagination

This NuGet package provides an enhanced pagination experience for [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus), offering a better visual interface for navigating between paginated content.

[![NuGet Version](https://img.shields.io/nuget/v/J4asper.DSharpPlus.BetterPagination)](https://www.nuget.org/packages/J4asper.DSharpPlus.BetterPagination/)

<!-- TOC -->
  * [Installation](#installation)
    * [NuGet](#nuget)
    * [From Source](#from-source)
  * [Documentation](#documentation)
    * [SendBetterPaginatedMessageAsync Parameters](#sendbetterpaginatedmessageasync-parameters)
    * [Example Usage](#example-usage)
<!-- TOC -->

## Installation

### NuGet

To install the package, open the terminal in your project and run the following command, replacing `1.0.0` with the latest version:

```console
dotnet add package J4asper.DSharpPlus.BetterPagination --version 1.0.0
```

Alternatively, you can search for `J4asper.DSharpPlus.BetterPagination` in your IDE’s NuGet Package Manager.

### From Source

To build the package from source, follow these steps:

1.  Clone the repository:

    ```console
    git clone https://github.com/j4asper/DSharpPlus.BetterPagination.git
    ```

2.  In your project’s `.csproj` file, add the following reference:

    ```xml
    <ProjectReference Include="../DSharpPlus.BetterPagination/DSharpPlus.BetterPagination/DSharpPlus.BetterPagination.csproj" />
    ```

## Documentation

This package adds an extension method, `SendBetterPaginatedMessageAsync`, to the [SlashCommandContext](https://dsharpplus.github.io/DSharpPlus/api/DSharpPlus.Commands.Processors.SlashCommands.SlashCommandContext.html) class, enabling easy pagination in your bot commands.

### SendBetterPaginatedMessageAsync Parameters

- `context` (`SlashCommandContext`):  
  The context of the slash command invocation, containing user information and the interaction context. This is required to send the initial response and track the user interaction.  
  [More info on `SlashCommandContext`](https://dsharpplus.github.io/DSharpPlus/api/DSharpPlus.Commands.Processors.SlashCommands.SlashCommandContext.html)

- `pages` (`IReadOnlyList<Page>`):  
  A read-only list of [`Page`](https://dsharpplus.github.io/DSharpPlus/api/DSharpPlus.Interactivity.Page.html) objects. Each `Page` object contains the embed and optional components (buttons, select menus, etc.) to be displayed on a specific page of the paginated message. The content of each page is encapsulated in the `Page` object.

- `additionalComponents` (`IReadOnlyList<DiscordComponent>?`, optional):  
  Optional additional components (e.g., buttons, select menus) that can be added to the message. This can be used to add custom interactive elements like menus, or other buttons. Default is `null`.

- `asEphemeral` (`bool`, optional):  
  A flag indicating whether the paginated message should be sent as an ephemeral message. If set to `true`, the message will only be visible to the user who invoked the command. Default is `false`.

- `allowUsageByAnyone` (`bool`, optional):  
  A flag indicating whether any user can interact with the pagination buttons, or if the interaction should be restricted to the invoking user only. If set to `false`, only the user who invoked the command will be able to interact with the buttons. Default is `false`.

### Example Usage

Here’s an example of how to use the package to send a paginated message within a command:

```csharp
[Command("example")]
[Description("This is a paginated example command")]
public async ValueTask PaginatedExampleCommand(SlashCommandContext context)
{
    var embedPageOne = new DiscordEmbedBuilder()
        .WithDescription("This is page 1");

    var embedPageTwo = new DiscordEmbedBuilder()
        .WithDescription("This is page 2");

    List<Page> pages =
    [
        new() { Embed = embedPageOne },
        new() { Embed = embedPageTwo }
    ];

    // Send the paginated message
    await context.SendBetterPaginatedMessageAsync(pages);
}
```

In this example:

-   Two pages are created using `DiscordEmbedBuilder`, each with different content.
-   These pages are added to a `List<Page>`, which is passed to the `SendBetterPaginatedMessageAsync` method to send a paginated message.
-   Users can navigate between pages using the forward and back buttons that are automatically added to the message.

Example of paginated message with 4 pages and an additional button.

![Paginated Example](https://raw.githubusercontent.com/j4asper/DSharpPlus.BetterPagination/refs/heads/main/.github/images/example.png)
