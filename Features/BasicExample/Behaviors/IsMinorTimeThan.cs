using Syncfusion.Maui.Toolkit.TextInputLayout;

namespace SimulationAndModel.Features.BasicExample.Behaviors;

public class IsMinorTimeThan : Behavior<SfTextInputLayout>
{
    public TimePicker? GreaterTime { get; set; }
    private TimePicker? _timePicker;
    private SfTextInputLayout? _sfTextInputLayout;
    protected override void OnAttachedTo(SfTextInputLayout bindable)
    {
        _sfTextInputLayout = bindable;
        if (bindable.Children == null || bindable.Children.Count == 0)
            return;

        _timePicker = bindable.Children[0] as TimePicker;

        _timePicker!.TimeSelected += OnTimeSelected;

        base.OnAttachedTo(bindable);       
    }

    protected override void OnDetachingFrom(SfTextInputLayout bindable)
    {
        bindable.ErrorText = string.Empty;
        bindable.HasError = false;
        // _timePicker!.TimeSelected -= OnTimeSelected;

        base.OnDetachingFrom(bindable);
    }

    private void OnTimeSelected(object? sender, TimeChangedEventArgs eventArgs)
    {
        if (GreaterTime == null)
            return;
        if (eventArgs?.NewTime == TimeSpan.Zero)
            return;

        _sfTextInputLayout!.ErrorText = string.Empty;
        _sfTextInputLayout!.HasError = false;

        if (eventArgs?.NewTime >= GreaterTime.Time)
        {
            _sfTextInputLayout.ErrorText = "Hora final no puede ser mayor o igual que el inicial";
            _sfTextInputLayout.HasError = true;
        }
    }
}
