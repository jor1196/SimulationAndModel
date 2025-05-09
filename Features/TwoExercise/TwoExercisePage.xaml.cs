namespace SimulationAndModel.Features.TwoExercise;

public partial class TwoExercisePage : ContentPage
{
	public TwoExercisePage(TwoExerciseViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}