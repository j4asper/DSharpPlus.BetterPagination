using DSharpPlus.Entities;

namespace DSharpPlus.BetterPagination.Models;

public class BetterPage
{
    public string Content { get; set; }
    
    public DiscordEmbed? Embed { get; set; }
    
    public IReadOnlyList<DiscordActionRowComponent> Components { get; }
    
    public BetterPage(string content = "", DiscordEmbed? embed = null, IReadOnlyList<DiscordComponent>? components = null)
    {
        Content = content;
        Embed = embed;

        if (components is null or [])
        {
            Components = [];

            return;
        }

        if (components[0] is DiscordActionRowComponent arc)
        {
            if (components.Count > 4)
            {
                throw new ArgumentException("Pages can only contain four rows of components");
            }

            Components = [arc];
        }
        else
        {
            List<DiscordActionRowComponent> componentRows = [];
            List<DiscordComponent> currentRow = new(5);

            foreach (DiscordComponent component in components)
            {
                if (component is BaseDiscordSelectComponent)
                {
                    componentRows.Add(new([component]));

                    continue;
                }

                if (currentRow.Count == 5)
                {
                    componentRows.Add(new DiscordActionRowComponent(currentRow));
                    currentRow = new List<DiscordComponent>(5);
                }

                currentRow.Add(component);
            }

            if (currentRow.Count > 0)
            {
                componentRows.Add(new DiscordActionRowComponent(currentRow));
            }

            Components = componentRows;

        }
    }
}