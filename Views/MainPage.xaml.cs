using adventuredesign8puzzle.Extension;
using CommunityToolkit.Maui.Alerts;
using SkiaSharp;

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
		vm.SetSizeCommand.Execute(null);
        vm.PuzzleContentChanged += DrawGridPuzzle;
    }
    


    private void DrawGridPuzzle(int[,] puzzleContent)
    {
        double width = PuzzleContentGrid.Width / vm.Size;
        double height = PuzzleContentGrid.Height / vm.Size;
        PuzzleContentGrid.GenNxNGrid(vm.Size);
        PuzzleContentGrid.Children.Clear();
        for(int i = 0; i < vm.Size; i++)
        {
            for(int j = 0; j < vm.Size; j++)
            {
                var button = new ImageButton()
                {
                    WidthRequest = width,
                    HeightRequest = height,
                    
                };
                button.Source = ImageSource.FromStream(() =>
                {
                    vm.BitmapTable.TryGetValue(puzzleContent[i, j], out var stream);
                    if (stream is null)
                    {
                        Toast.Make("stream is null").Show();
                        return null;
                    }
                        
                    if(!stream.CanRead)
                        Toast.Make("stream can't read").Show();
                    return stream;
                });
                
                

                button.Command = vm.MoveTileCommand;
                button.CommandParameter = (puzzleContent[i, j], vm.Size * i + j);
                PuzzleContentGrid.Children.Add(button);
                PuzzleContentGrid.SetRow(button, i);
                PuzzleContentGrid.SetColumn(button,j);

            }
        }
    }
}
