using System;
using System.Collections.Generic;
using Discord;
using Fergun.Interactive.Selection;

namespace LeagueStatusBot.Helpers;

public class ButtonSelection<T> : BaseSelection<ButtonOption<T>>
    {
        public ButtonSelection(ButtonSelectionBuilder<T> builder)
            : base(builder)
        {
        }

        // This method needs to be overriden to build our own component the way we want.
        public override ComponentBuilder GetOrAddComponents(bool disableAll, ComponentBuilder builder = null)
        {
            builder ??= new ComponentBuilder();
            foreach (var option in Options)
            {
                var emote = EmoteConverter?.Invoke(option);
                string label = StringConverter?.Invoke(option);
                if (emote is null && label is null)
                {
                    throw new InvalidOperationException($"Neither {nameof(EmoteConverter)} nor {nameof(StringConverter)} returned a valid emote or string.");
                }

                var button = new ButtonBuilder()
                    .WithCustomId(emote?.ToString() ?? label)
                    .WithStyle(option.Style) // Use the style of the option
                    .WithEmote(emote)
                    .WithDisabled(option.disable);

                if (label is not null)
                    button.Label = label;

                builder.WithButton(button);
            }

            return builder;
        }
    }