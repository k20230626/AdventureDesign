using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using UraniumUI;

#if WINDOWS
using Microsoft.UI.Xaml.Media;
#endif

namespace ADHuffmanCode;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.UsePageResolver(true)
			.UseAutodependencies()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialSymbol");
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddMaterialIconFonts();
			});
		
#if WINDOWS

		builder.ConfigureLifecycleEvents(events => {
			events.AddWindows(windowEvent => {
				windowEvent.OnWindowCreated(window => {
					window.Title = "ADHuffmanCode";
#if WINDOWS10_0_17763_0_OR_GREATER
					window.SystemBackdrop = new DesktopAcrylicBackdrop();
#endif
					
				});
			});
		});
#endif
				
		return builder.Build();
	}
}
