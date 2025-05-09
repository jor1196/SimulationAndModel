namespace SimulationAndModel.Features.FiveExercise;

public partial class FiveExercisePage : ContentPage
{
	public FiveExercisePage(FiveExerciseViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}