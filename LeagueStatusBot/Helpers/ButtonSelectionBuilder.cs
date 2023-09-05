using Fergun.Interactive;
using Fergun.Interactive.Selection;

namespace LeagueStatusBot.Helpers;

public class ButtonSelectionBuilder<T> : BaseSelectionBuilder<ButtonSelection<T>, ButtonOption<T>, ButtonSelectionBuilder<T>>
{
    public override InputType InputType => InputType.Buttons;

    public override ButtonSelection<T> Build() => new(this);
}