namespace SimulationAndModel.Features.BasicExample;

public partial class BasicExamplePage : ContentPage
{
	public BasicExamplePage(BasicExampleViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}