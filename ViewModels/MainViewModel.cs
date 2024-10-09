using adventuredesign8puzzle.Services;

namespace adventuredesign8puzzle.ViewModels;


public partial class MainViewModel : BaseViewModel
{
    private readonly avd8puzzleService _service;

    /// <summary>
    /// 두번 이동하는 현상, 첫번째는 부동소수점이 포함된 값을 정수형으로 바꾸면서 2번 호출됨
    /// 
    /// </summary>
    [ObservableProperty]
    public int _size = 3;

    [ObservableProperty] 
    private ImageSource _puzzleImageSource = "";

    [ObservableProperty]
    private string _imageName = "";

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



    public event Action<int[,]> PuzzleContentChanged;

    public MainViewModel(avd8puzzleService service)
    {
        this._service = service;
    }

    [RelayCommand]
    public async Task PickImage()
    {
        var option = PickOptions.Images;
        
        try
        {
            var result = await FilePicker.Default.PickAsync(option);
            if (result == null)
                return;
            if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                using (var stream = await result.OpenReadAsync())
                {
                    PuzzleImageSource = ImageSource.FromFile(result.FullPath);
                }

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
        PuzzleContent = _service.SetSize(Size);
    }
    

    [RelayCommand]
    private void SufflePuzzle()
    {
        PuzzleContent = _service.ShufflePuzzle();
    }

    [RelayCommand]
    private void MoveTile()
    {

    }

    
}
