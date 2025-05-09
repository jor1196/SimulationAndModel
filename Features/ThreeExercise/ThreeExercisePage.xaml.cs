namespace SimulationAndModel.Features.ThreeExercise;

public partial class ThreeExercisePage : ContentPage
{
	public ThreeExercisePage(ThreeExerciseViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}