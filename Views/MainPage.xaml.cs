using adventuredesign8puzzle.Extension;
using CommunityToolkit.Maui.Alerts;
using SkiaSharp;

namespace adventuredesign8puzzle.Views;

public partial class MainPage : ContentPage {
    private MainViewModel vm => (MainViewModel)BindingContext;
    private int _previousSize = 0;
    public MainPage(MainViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
        vm.CheckboxChanged += ButtonLabelUpdate;
        vm.PuzzleTileMoved += SwapTileImage;
        vm.PuzzleContentChanged += DrawGridPuzzle;

        
    }

    

    private void ContentPage_Loaded(object sender, EventArgs e) {
        vm.SetSizeCommand.Execute(null);
    }


    private void DrawGridPuzzle(int[] puzzleContent) {
        double width = PuzzleContentGrid.Width / vm.Size;
        double height = PuzzleContentGrid.Height / vm.Size;

        int size = (int)Math.Min(width, height);
        //TODO: 지금 여기 그리드를 계속 그리고 있는데 셔플할 경우에는 이 경우가 필요가 없음
        //TODO: 즉 그리드 그리는데 많은 리소스를 잡아 먹음

        if (_previousSize == vm.Size) {
            ButtonLabelUpdate(puzzleContent,vm.IsChecked);
            ButtonImageUpate();
            return;
        }

        _previousSize = vm.Size;
        PuzzleContentGrid.Children.Clear();
        PuzzleContentGrid.GenNxNGrid(vm.Size);
        
        for (int i = 0; i < vm.Size; i++) {
            for (int j = 0; j < vm.Size; j++) {
                var index = i * vm.Size + j;

                var grid = new Grid();
                
                var button = new ImageButton() {
                    WidthRequest = size,
                    HeightRequest = size,
                };

                var label = new Label();
                label.Text = puzzleContent[index].ToString();
                label.InputTransparent = true;
                label.ZIndex = 10;
                label.FontAttributes = FontAttributes.Bold;
                label.HorizontalTextAlignment = TextAlignment.Center;
                label.VerticalTextAlignment = TextAlignment.Center;
                label.FontSize = 24;
                label.TextColor = Color.Parse("White");

                
                button.Source = ImageSource.FromStream(() => {
                    vm.BitmapTable.TryGetValue(puzzleContent[index], out var stream);
                    if (stream is null) {
                        Debug.WriteLine("stream is null");
                        return null;
                    }

                    return stream.AsStream();
                });


                button.Command = vm.MovePuzzleTileCommand;
                button.CommandParameter = index;

                grid.Children.Add(label);
                grid.Children.Add(button);                
                
                
                PuzzleContentGrid.Children.Add(grid);
                PuzzleContentGrid.SetRow(grid,i);
                PuzzleContentGrid.SetColumn(grid,j);
            }
        }
    }

    private void ButtonImageUpate() {
        foreach (var view in PuzzleContentGrid) {
            if (view is Grid grid) {
                ImageButton btn = null;
                if (grid.Children[1] is ImageButton) {
                    btn = grid.Children[1] as ImageButton;
                }
                else {
                    foreach (var gridChild in grid.Children) {
                        if (gridChild is ImageButton) {
                            btn = gridChild as ImageButton;
                            break;
                        }
                        
                    }
                }

                if (btn is not null)
                    btn.Source = ImageSource.FromStream(() => {
                        vm.BitmapTable.TryGetValue(int.Parse(((Label)grid.Children[0]).Text), out var stream);
                        if (stream is null) {
                            Debug.WriteLine("stream is null");
                            return null;
                        }

                        return stream.AsStream();
                    });
            }
        }
    }

    private void ButtonLabelUpdate(int[] puzzle, bool isChecked) {
        int index = 0;
        foreach (var view in PuzzleContentGrid.Children) {
            if (view is not Grid)
                break;
            var grid = (Grid)view;
            if (grid.Children[0] is not null) {
                Label label = null;
                
                //Find Label
                if (grid.Children[0] is Label) {
                    label = (Label)grid.Children[0];
                }
                else {
                    foreach (var gridChild in grid.Children) {
                        if (gridChild is Label) {
                            label = gridChild as Label;
                            break;
                        }
                    }
                }

                if (label is not null) {
                    
                    label.Opacity = isChecked? 1 : 0;
                    label.Text = puzzle[index++].ToString();
                }

            }
        }
    }

    
    //TODO:이게 한번더 생각해봐야할듯 이미지 부분에서 좀 꼬이는듯
    private void SwapTileImage((int, int) tiles) {
        var (from, to) = tiles;
        var fromGrid = PuzzleContentGrid.Children[from] as Grid;
        var toGrid = PuzzleContentGrid.Children[to] as Grid;

        if (fromGrid != null && toGrid != null) {
            var fromButton = fromGrid.Children[1] as ImageButton;
            var toButton = toGrid.Children[1] as ImageButton;
            
            if (fromButton != null && toButton != null) {
                fromButton.Source = ImageSource.FromStream(() => vm.BitmapTable[to].AsStream());
                toButton.Source = ImageSource.FromStream(() => vm.BitmapTable[from].AsStream());
            }
        }
    }
    
    
}