using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
using SimulationAndModel.Features.FourExercise;
using SimulationAndModel.Features.OneExercise;
using SimulationAndModel.Features.ThreeExercise;
using SimulationAndModel.Features.TwoExercise;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SimulationAndModel;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddTransient<OneExerciseViewModel>();
		builder.Services.AddTransient<TwoExerciseViewModel>();
		builder.Services.AddTransient<ThreeExerciseViewModel>();
		builder.Services.AddTransient<FourExerciseViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
	}
}
