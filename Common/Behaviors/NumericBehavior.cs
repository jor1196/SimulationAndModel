namespace SimulationAndModel.Common.Behaviors;

public partial class NumericBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.TextChanged += OnTextChanged;
        bindable.Completed += OnCompleted;
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.TextChanged -= OnTextChanged;
        bindable.Completed -= OnCompleted;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        ArgumentNullException.ThrowIfNull(entry, "It isn't entry");

        if (!string.IsNullOrEmpty(e.NewTextValue) && !int.TryParse(e.NewTextValue, out _))
        {
            entry.Text = e.OldTextValue;
        }
    }

    private void OnCompleted(object? sender, EventArgs e)
    {
        var entry = sender as Entry;
        ArgumentNullException.ThrowIfNull(entry, "It isn't entry");

        if (!string.IsNullOrEmpty(entry.Text) && !int.TryParse(entry.Text, out _))
        {
            entry.Text = string.Empty;
        }
    }
}
