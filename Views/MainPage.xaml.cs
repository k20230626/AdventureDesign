using adventuredesign8puzzle.Extension;

namespace adventuredesign8puzzle.Views;

public partial class MainPage : ContentPage
{ 
	private MainViewModel vm => (MainViewModel)BindingContext;
    public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        var vm = (MainViewModel)BindingContext;
		vm.SetSizeCommand.Execute(null);
        vm.PuzzleContentChanged += makeGridPuzzle;

    }


    private void makeGridPuzzle(int[,] puzzleContent)
    {
        double width = PuzzleContentGrid.Width / vm.Size;
        double height = PuzzleContentGrid.Height / vm.Size;
        PuzzleContentGrid.GenNxNGrid(vm.Size);
        PuzzleContentGrid.Children.Clear();
        for(int i = 0; i < vm.Size; i++)
        {
            for(int j = 0; j < vm.Size; j++)
            {
                var button = new Button()
                {
                    Text = puzzleContent[i,j] == 0 ? "" : puzzleContent[i,j].ToString(),
                    WidthRequest = width,
                    HeightRequest = height,
                    FontSize = 32,
                    
                };

                button.Command = vm.MoveTileCommand;
                button.CommandParameter = (puzzleContent[i, j], 10 * i + j);
                PuzzleContentGrid.Children.Add(button);
                PuzzleContentGrid.SetRow(button, i);
                PuzzleContentGrid.SetColumn(button,j);

            }
        }
    }
}
