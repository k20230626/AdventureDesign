using adventuredesign8puzzle.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SkiaSharp;

namespace adventuredesign8puzzle.ViewModels;


public partial class MainViewModel : BaseViewModel
{
    private readonly Avd8puzzleService puzzleService;
    private readonly BitMapService bitMapService;
    
    [ObservableProperty]
    private int _size = 3;

    [ObservableProperty] 
    private ImageSource _puzzleImageSource = "";

    [ObservableProperty]
    private string _imageName = "";

    
    [ObservableProperty]
    private Dictionary<int, SKData> _bitmapTable = new();

    [ObservableProperty] 
    private bool _isChecked = false;
    private SKBitmap ImageBitMap { get; set; }
    
    
    //TODO: 이벤트도 좀더 이름을 명확하게 해야함
    public event Action<int[]> PuzzleContentChanged;
    public event Action<int[],bool> CheckboxChanged;
    public event Action<(int, int)> PuzzleTileMoved;
    public MainViewModel(Avd8puzzleService puzzleService, BitMapService bitMapService)
    {
        this.puzzleService = puzzleService;
        this.bitMapService = bitMapService;
    }

    [RelayCommand]
    public async Task PickImage()
    {
        var option = PickOptions.Images;
        
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result == null)
                return;
            if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                var stream = await result.OpenReadAsync();

                PuzzleImageSource = ImageSource.FromFile(result.FullPath);
                ImageBitMap = SKBitmap.Decode(stream);
                BitmapTable = bitMapService.DivideNxN(ImageBitMap, Size);
                
                ImageName = result.FileName.Split(".").First();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    
    
    [RelayCommand]
    private void SetSize()
    {
        var puzzle = puzzleService.GenNPuzzle(Size);
        Debug.WriteLine(puzzleService.ToString());
        if (ImageBitMap is not null)
        {
            BitmapTable = bitMapService.DivideNxN(ImageBitMap, Size);
        }  
        PuzzleContentChanged?.Invoke(puzzle);

    }

    [RelayCommand]
    private void ShowNumber() {
        CheckboxChanged?.Invoke(puzzleService.GetPuzzle(),IsChecked);
    }

    [RelayCommand]
    private void UpdatePuzzle() 
    {
        
    }
    

    [RelayCommand]
    private void ShufflePuzzle()
    {
        var puzzle = puzzleService.ShufflePuzzle();
        Debug.WriteLine(puzzleService.ToString());
        PuzzleContentChanged?.Invoke(puzzle);
    }

    [RelayCommand]
    private void MovePuzzleTile(int tileIndex)
    {
        
        //TODO: label
        var puzzleTile = puzzleService.MovePuzzleTile(tileIndex);
        var swapedIndex = puzzleTile[tileIndex];
        if(tileIndex == swapedIndex) //안바뀌었다면 이벤트를 발생 안시킴
            return;
        PuzzleTileMoved?.Invoke((tileIndex, swapedIndex));
        CheckboxChanged?.Invoke(puzzleService.GetPuzzle(),IsChecked);
        Debug.WriteLine(puzzleService.ToString());
    }

    
}
