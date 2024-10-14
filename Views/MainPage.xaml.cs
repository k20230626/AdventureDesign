using adventuredesign8puzzle.Extension;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Controls.Shapes;

using UraniumUI.Material.Controls;
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
        vm.ShowAlert += ShowAlert;
    }

    private async void ShowAlert((string title, string message) obj) {
        
         await DisplayAlert(obj.title,obj.message,"OK");
    }


    private void ContentPage_Loaded(object sender, EventArgs e) {
        vm.SetSizeCommand.Execute(null);
        vm.IsChecked = true;
    }

    private void MainPage_OnAppearing(object sender, EventArgs e) {
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
            ButtonImageUpdate();
            return;
        }

        _previousSize = vm.Size;
        //setup grid
        
        
        
        //TODO: 초기 size를 맥에선 못가져온느듯
        if (PuzzleContentGrid.WidthRequest > 0) {
            PuzzleContentGrid.WidthRequest = size * vm.Size;
        }
        PuzzleContentGrid.Children.Clear();
        PuzzleContentGrid.GenNxNGrid(vm.Size);
        
        for (int i = 0; i < vm.Size; i++) {
            for (int j = 0; j < vm.Size; j++) {
                var index = i * vm.Size + j;

                var puzzleTileView = GenPuzzleTile(size, index,ref puzzleContent);

                PuzzleContentGrid.Children.Add(puzzleTileView);
                PuzzleContentGrid.SetRow(puzzleTileView, i);
                PuzzleContentGrid.SetColumn(puzzleTileView, j);
            }
        }
    }

    private void ButtonImageUpdate() {
        foreach (var view in PuzzleContentGrid) {
            if (view is ButtonView buttonView) {
                Image image;
                if (buttonView.Content is Grid grid) {
                    image = grid.Children[1] as Image;

                    if (image is not null) {
                        image.Source = ImageSource.FromStream(() =>
                            //TODO: NUll 체크
                            GetImageStreamFromBitmapTable(int.Parse((grid.Children[0] as Label)?.Text)));
                    }
                }
                
            }
        }
    }

    private void ButtonLabelUpdate(int[] puzzle, bool isChecked) {
        int index = 0;
        foreach (var view in PuzzleContentGrid) {
            if (view is ButtonView buttonView) {
                Label label;
                if (buttonView.Content is Grid grid) {
                    label = grid.Children[0] as Label;
                    if (label is not null) {
                        label.Opacity = isChecked ? 1 : 0;
                        label.Text = puzzle[index++].ToString();
                    }
                }

            }
        }
    }

    
    //TODO:이게 한번더 생각해봐야할듯 이미지 부분에서 좀 꼬이는듯
    private void SwapTileImage((int, int,int[]) tiles) {
        //ButtonImageUpdate();
        //return;
        var (fromIndex, toIndex,puzzle) = tiles;
        int fromValue = puzzle[fromIndex];
        int toValue = puzzle[toIndex];
        
        
        var fromButtonContent = (PuzzleContentGrid.Children[fromIndex] as ButtonView)?.Content as Grid;
        var toButtonContent = (PuzzleContentGrid.Children[toIndex] as ButtonView)?.Content as Grid;

        if (toButtonContent != null && fromButtonContent != null) {
            var fromButton = fromButtonContent.Children[1] as Image;
            var toButton = toButtonContent.Children[1] as Image;
            
            if (fromButton != null && toButton != null) {
                (fromButton.Source, toButton.Source) = (toButton.Source, fromButton.Source);
            }
        }
    }
    //그리드 Children만드는 함수
    private ButtonView GenPuzzleTile(int size,int index,ref int[] puzzleContent) {
        var grid = new Grid() {
            WidthRequest = size,
            HeightRequest = size
        };

        var label = new Label() {
            Text = puzzleContent[index].ToString(),
            InputTransparent = true,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            ZIndex = 10,
            FontSize = 24,
            TextColor = Color.Parse("White"),
        };
        
        ButtonView buttonView = new ButtonView {
            WidthRequest = size,
            HeightRequest = size,
            StrokeShape = new Rectangle(),
            Padding = 0,
            BackgroundColor = Color.Parse("White"),
            PressedCommand = vm.MovePuzzleTileCommand,
            CommandParameter = index,
            StrokeThickness = 0
        };


        int tableIndex = puzzleContent[index];
        Image image = new Image {
            WidthRequest = size,
            HeightRequest = size,
            Source = ImageSource.FromStream(() =>
                GetImageStreamFromBitmapTable(tableIndex))
        };

        grid.Children.Add(label);
        grid.Children.Add(image);

        buttonView.Content = grid;
        return buttonView;
        
    }
    
    private Stream GetImageStreamFromBitmapTable(int value) {
        vm.BitmapTable.TryGetValue(value, out var stream);
        if (stream is null) {
            return null;
        }
        return stream.AsStream();
    }


}