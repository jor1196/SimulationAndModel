namespace SimulationAndModel.Features.OneClassPage;

public partial class OneClassPage : ContentPage
{
	public OneClassPage(OneClassViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}