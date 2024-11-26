# BetterPagination

This NuGet package provides an enhanced pagination experience for DSharpPlus, offering a better visual interface for navigating between paginated content.

[![NuGet Version](https://img.shields.io/nuget/v/J4asper.DSharpPlus.BetterPagination)](https://www.nuget.org/packages/J4asper.DSharpPlus.BetterPagination/)

<!-- TOC -->
  * [Installation](#installation)
    * [NuGet](#nuget)
    * [From Source](#from-source)
  * [Documentation](#documentation)
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

### Example Usage

Here’s an example of how to use the package to send a paginated message within a command:

```csharp
[Command("example")]
[Description("This is a paginated example command")]
public async ValueTask PaginatedExampleCommand(SlashCommandContext context)
{
    var embedPageOne = new DiscordEmbedBuilder()
        .WithDefaultColor()
        .WithContent("This is page 1");

    var embedPageTwo = new DiscordEmbedBuilder()
        .WithDefaultColor()
        .WithContent("This is page 2");

    List<Page> pages = new()
    {
        new Page { Embed = embedPageOne },
        new Page { Embed = embedPageTwo }
    };

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
