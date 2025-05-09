namespace SimulationAndModel.Features.FourExercise;

public partial class FourExercisePage : ContentPage
{
	public FourExercisePage(FourExerciseViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}