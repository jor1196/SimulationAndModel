namespace SimulationAndModel.Features.OneClass;

public partial class OneClassPage : ContentPage
{
	public OneClassPage(OneClassViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}