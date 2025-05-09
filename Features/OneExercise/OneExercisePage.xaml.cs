namespace SimulationAndModel.Features.OneExercise;

public partial class OneExercisePage : ContentPage
{
	public OneExercisePage(OneExerciseViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}