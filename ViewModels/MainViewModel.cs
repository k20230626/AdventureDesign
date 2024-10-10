using adventuredesign8puzzle.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SkiaSharp;

namespace adventuredesign8puzzle.ViewModels;


public partial class MainViewModel : BaseViewModel
{
    private readonly Avd8puzzleService puzzleService;
    private readonly BitMapService bitMapService;

    /// <summary>
    /// 두번 이동하는 현상, 첫번째는 부동소수점이 포함된 값을 정수형으로 바꾸면서 2번 호출됨
    /// 
    /// </summary
    [ObservableProperty]
    public int _size = 3;

    [ObservableProperty] 
    private ImageSource _puzzleImageSource = "";

    [ObservableProperty]
    private string _imageName = "";

    
    private Dictionary<int, Stream> _bitmapTable = new();

    public Dictionary<int, Stream> BitmapTable
    {
        get { return _bitmapTable; }
        set
        {
            SetProperty(ref _bitmapTable, value);
            PuzzleContentChanged?.Invoke(PuzzleContent);
        }
    }
    
    private int[,] _puzzleContent = { };

    public int[,] PuzzleContent
    {
        get { return _puzzleContent; }
        set
        {
            SetProperty(ref _puzzleContent, value);
            PuzzleContentChanged?.Invoke(value);
        }
    }
    
    private Stream ImageStream { get; set; }
    
    
    public event Action<int[,]> PuzzleContentChanged;

    public MainViewModel(Avd8puzzleService puzzleService, BitMapService bitMapService)
    {
        this.puzzleService = puzzleService;
        this.bitMapService = bitMapService;
    }

    [RelayCommand]
    public async Task PickImage()
    {
        var option = PickOptions.Images;
        
        if(ImageStream is not null)
            ImageStream.Dispose();
        
        try
        {
            var result = await FilePicker.Default.PickAsync(option);
            if (result == null)
                return;
            if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                var stream = await result.OpenReadAsync();

                PuzzleImageSource = ImageSource.FromFile(result.FullPath);
                ImageStream = stream;
                BitmapTable = bitMapService.DivideNxN(SKBitmap.Decode(ImageStream), Size);
                
                
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
        PuzzleContent = puzzleService.SetSize(Size);
        if (ImageStream is not null && ImageStream.CanRead)
        {
            BitmapTable = bitMapService.DivideNxN(SKBitmap.Decode(ImageStream), Size);
        }

        PuzzleContentChanged?.Invoke(PuzzleContent);
    }

    [RelayCommand]
    private void UpdatePuzzle() 
    {
        
    }
    

    [RelayCommand]
    private void SufflePuzzle()
    {
        PuzzleContent = puzzleService.ShufflePuzzle();
    }

    [RelayCommand]
    private void MoveTile((int,int) a)
    {
        
    }

    private IToast makeToast(string message)
    {
        return Toast.Make(message,ToastDuration.Short);
    }
    
}
