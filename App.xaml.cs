using adventuredesign8puzzle.Services;

namespace adventuredesign8puzzle;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage(new MainViewModel(new Avd8PuzzleService(), new BitMapService()));
	}
}
